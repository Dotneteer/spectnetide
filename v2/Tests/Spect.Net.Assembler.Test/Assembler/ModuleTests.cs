using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.Assembler.Assembler;

// ReSharper disable StringLiteralTypo

namespace Spect.Net.Assembler.Test.Assembler
{
    [TestClass]
    public class ModuleTests: AssemblerTestBed
    {
        [TestMethod]
        public void EmptyModuleWorks()
        {
            CodeEmitWorks(@"
                .org #6000
                MyModule: .module
                    ld a,b
                .endmodule",
                0x78);
        }

        [TestMethod]
        public void ModuleWithNoNameFails()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                .org #6000
                .module
                ld a,b
                .endmodule
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0428);
        }

        [TestMethod]
        public void ModuleWithLocalNameFails()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                .org #6000
                `myModule: .module
                    ld a,b
                .endmodule
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0430);
        }

        [TestMethod]
        public void ModuleWithLocalIdFails()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                .org #6000
                .module `myModule
                    ld a,b
                .endmodule
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0430);
        }

        [TestMethod]
        public void ModuleIdOverridesLocalName()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                .org #6000
                myModule: .module moduleID
                    ld a,b
                .endmodule
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            output.NestedModules.ContainsKey("moduleID").ShouldBeTrue();
        }

        [TestMethod]
        public void ModuleWithDuplicatedNameFails1()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                .org #6000
                MyModule .module
                    ld a,b
                .endmodule
                ld b,c
                MyModule .module
                    ld a,b
                .endmodule
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(2);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0040);
            output.Errors[1].ErrorCode.ShouldBe(Errors.Z0429);
        }

        [TestMethod]
        public void ModuleWithDuplicatedNameFails2()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                .org #6000
                MyModule .module
                    ld a,b
                .endmodule
                ld b,c
                OtherModule .module MyModule
                    ld a,b
                .endmodule
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0429);
        }

        [TestMethod]
        public void ModuleWithDuplicatedNameFails3()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                .org #6000
                .module MyModule
                    ld a,b
                .endmodule
                ld b,c
                OtherModule .module MyModule
                    ld a,b
                .endmodule
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0429);
        }

        [TestMethod]
        public void ModuleWithDuplicatedNameFails4()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                .org #6000
                .module MyModule
                    ld a,b
                .endmodule
                ld b,c
                .module MyModule
                    ld a,b
                .endmodule
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0429);
        }

        [TestMethod]
        public void ModuleWithoutEndFails()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                .org #6000
                .module
                ld a,b
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0401);
        }

        [TestMethod]
        public void UnexpectedModuleEndFails()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                .org #6000
                .endmodule
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0405);
        }

        [TestMethod]
        public void LabelOnlyCodeEmissionWorks()
        {
            CodeEmitWorks(@"
                .org #6000
                LabelOnly: .module
                    ld a,b
                    ld bc,LabelOnly
                    .endmodule
                ",
                0x78, 0x01, 0x00, 0x60);
        }

        [TestMethod]
        public void LabelOnlyWithCommentCodeEmissionWorks()
        {
            CodeEmitWorks(@"
                .org #6000
                LabelOnly: ; Empty label
                    .module MyModule
                    ld a,b
                    ld bc,LabelOnly
                    .endmodule
                ",
                0x78, 0x01, 0x00, 0x60);
        }

        [TestMethod]
        public void MultiLabelOnlyCodeEmissionWorks()
        {
            CodeEmitWorks(@"
                .org #6000
                MyModule:
                    .module
                LabelOnly1:
                LabelOnly2:
                LabelOnly3:
                LabelOnly4:
                LabelOnly5:
                    ld a,b
                    ld bc,LabelOnly3
                    .endmodule
                ",
                0x78, 0x01, 0x00, 0x60);
        }

        [TestMethod]
        public void OrgWithHangingLabelWorks()
        {
            CodeEmitWorks(@"
                MyModule: .module
                LabelOnly:
                    .org #6000
                    ld a,b
                    ld bc,LabelOnly
                    .endmodule
                ",
                0x78, 0x01, 0x00, 0x60);
        }

        [TestMethod]
        public void EquWithHangingLabelWorks()
        {
            CodeEmitWorks(@"
                MyModule: .module
                LabelOnly:
                    .org #6000
                LabelOnly2:
                    .equ #4567
                    ld a,b
                    ld bc,LabelOnly2
                    .endmodule
                ",
                0x78, 0x01, 0x67, 0x45);
        }

        [TestMethod]
        public void VarWithHangingLabelWorks()
        {
            CodeEmitWorks(@"
                MyModule .module
                LabelOnly:
                    .org #6000
                LabelOnly2:
                    .var #4567
                    ld a,b
                    ld bc,LabelOnly2
                    .endmodule
                ",
                0x78, 0x01, 0x67, 0x45);
        }

        [TestMethod]
        public void OrphanHangingLabelWorks()
        {
            CodeEmitWorks(@"
                    .module MyModule
                    .org #6000
                    ld a,b
                    ld bc,LabelOnly
                LabelOnly:
                    .endmodule",
                0x78, 0x01, 0x04, 0x60);
        }

        [TestMethod]
        public void SingleTempLabelWithBackReferenceWorks()
        {
            CodeEmitWorks(@"
                Start:
                    .org #6000
                    .module MyModule
                    ld a,b
                `t1:
                    ld bc,`t1
                    .endmodule",
                0x78, 0x01, 0x01, 0x60);
        }

        [TestMethod]
        public void SingleTempLabelWithForwardReferenceWorks()
        {
            CodeEmitWorks(@"
                Start:
                    .module MyModule
                    .org #6000
                    ld a,b
                    ld bc,`t1
                `t1:
                    ld a,b
                    .endmodule
                ",
                0x78, 0x01, 0x04, 0x60, 0x78);
        }

        [TestMethod]
        public void StartAndEndLabelWithForwardReferenceWorks()
        {
            CodeEmitWorks(@"
                Start:
                    .module MyModule
                    .org #6000
                    ld a,b
                    ld bc,End
                End:
                    ld a,b
                    .endmodule
                ",
                0x78, 0x01, 0x04, 0x60, 0x78);
        }

        [TestMethod]
        public void TempLabelInDifferentScopesWork()
        {
            CodeEmitWorks(@"
                Start:
                    .module MyModule
                    .org #6000
                    ld a,b
                    ld bc,`t1
                `t1:
                    ld a,b
                Next: 
                    ld a,b
                    ld bc,`t1
                `t1:
                    ld a,b
                    .endmodule
                ",
                0x78, 0x01, 0x04, 0x60, 0x78, 0x78, 0x01, 0x09, 0x60, 0x78);
        }

        [TestMethod]
        public void ModuleTempLabelsAreIndependent()
        {
            CodeEmitWorks(@"
                    .org #6000
                Start:
                    .module MyModule
                    ld a,b
                    ld bc,`t1
                `t1:
                    ld a,b
                    .endmodule

                    .module MyModule2
                    ld a,b
                    ld bc,`t1
                `t1:
                    ld a,b
                    .endmodule
                ",
                0x78, 0x01, 0x04, 0x60, 0x78, 0x78, 0x01, 0x09, 0x60, 0x78);
        }

        [TestMethod]
        public void ModuleLabelsAreIndependent()
        {
            CodeEmitWorks(@"
                    .org #6000
                Start:
                    .module MyModule
                    ld a,b
                    ld bc,t1
                t1:
                    ld a,b
                    .endmodule

                    .module MyModule2
                    ld a,b
                    ld bc,t1
                t1:
                    ld a,b
                    .endmodule
                ",
                0x78, 0x01, 0x04, 0x60, 0x78, 0x78, 0x01, 0x09, 0x60, 0x78);
        }

        [TestMethod]
        public void InternalModuleLabelDoesNotWorkWithoutScope()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                    .org #6000
                Start:
                    .module MyModule
                    ld a,b
                    ld bc,t1
                t1:
                    ld a,b
                    .endmodule
                    ld hl,t1
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0201);
        }

        [TestMethod]
        public void NestedModulesWork()
        {
            CodeEmitWorks(@"
                    .org #6000
                Start:
                    .module MyModule
                    ld a,b
                    ld bc,t1
                t1:
                    ld a,b
                    .module Nested
                t1:
                    ld a,b
                    ld bc,t1
                    .endmodule
                    ld hl,t1
                    .endmodule
                ",
                0x78, 0x01, 0x04, 0x60, 0x78, 0x78, 0x01, 0x05, 0x60, 0x21, 0x04, 0x60);
        }

        [TestMethod]
        public void ModuleSeesSymbolsOutside1()
        {
            CodeEmitWorks(@"
                    .org #6000
                Start:
                    ld a,b
                    .module MyModule
                    nop
                    ld bc,Start
                    .endmodule
                ",
                0x78, 0x00, 0x01, 0x00, 0x60);
        }

        [TestMethod]
        public void ModuleSeesSymbolsOutside2()
        {
            CodeEmitWorks(@"
                    .org #6000
                Outside1:
                    ld a,b
                    .module MyModule
                    nop
                        .module MyModule2
                        ld bc,Outside1
                        nop
                        .endmodule
                    ld hl,OutSide1
                    .endmodule
                ",
                0x78, 0x00, 0x01, 0x00, 0x60, 0x00, 0x21, 0x00, 0x60);
        }

        [TestMethod]
        public void ModuleSeesSymbolsOutside3()
        {
            CodeEmitWorks(@"
                    .org #6000
                Outside1:
                    ld a,b
                    .module MyModule
                    nop
                Outside2:
                    nop
                        .module MyModule2
                        ld bc,Outside1
                        nop
                        .endmodule
                    ld hl,OutSide2
                    .endmodule
                ",
                0x78, 0x00, 0x00, 0x01, 0x00, 0x60, 0x00, 0x21, 0x02, 0x60);
        }

        [TestMethod]
        public void ModuleSeesSymbolsOutside4()
        {
            CodeEmitWorks(@"
                    .org #6000
                Outside:
                    ld a,b
                    .module MyModule
                    nop
                Outside:
                    nop
                        .module MyModule2
                        ld bc,Outside
                        nop
                        .endmodule
                    ld hl,OutSide
                    .endmodule
                ",
                0x78, 0x00, 0x00, 0x01, 0x02, 0x60, 0x00, 0x21, 0x02, 0x60);
        }

        [TestMethod]
        public void ModuleSeesSymbolsOutside5()
        {
            CodeEmitWorks(@"
                    .org #6000
                Outside:
                    ld a,b
                    .module MyModule
                    nop
                Outside:
                    nop
                        .module MyModule2
                        ld bc,Outside
                        nop
                        .endmodule
                    ld hl,OutSide
                    .endmodule
                    ld bc,Outside
                ",
                0x78, 0x00, 0x00, 0x01, 0x02, 0x60, 0x00, 0x21, 0x02, 0x60, 0x01, 0x00, 0x60);
        }

        [TestMethod]
        public void GlobalSymbolResolutionWorks()
        {
            CodeEmitWorks(@"
                    .org #6000
                MyStart:
                    ld a,b
                    ld bc,::MyStart
                ",
                0x78, 0x01, 0x00, 0x60);
        }

        [TestMethod]
        public void GlobalSymbolResolutionWorksWithinModule()
        {
            CodeEmitWorks(@"
                    .org #6000
                MyStart:
                    ld a,b
                    .module MyModule
                MyStart:
                    ld a,b
                    ld bc,MyStart
                    ld bc,::MyStart
                    .endmodule
                ",
                0x78, 0x78, 0x01, 0x01, 0x60, 0x01, 0x00, 0x60);
        }

        [TestMethod]
        public void GlobalSymbolResolutionWorksWithinModuleWithFixup()
        {
            CodeEmitWorks(@"
                    .org #6000
                Start:
                    ld a,b
                    .module MyModule
                    ld a,b
                    ld bc,MyId
                MyId:
                    ld bc,::MyId
                    .endmodule
                MyId:
                    nop
                ",
                0x78, 0x78, 0x01, 0x05, 0x60, 0x01, 0x08, 0x60, 0x00);
        }

        [TestMethod]
        public void ModuleResolutionWithinModuleWorks1()
        {
            CodeEmitWorks(@"
                    .org #6000
                Start:
                    ld a,b
                MyId:
                    nop
                    .module MyModule
                MyId:
                    ld a,b
                    ld bc,MyId
                    ld bc,::MyId
                    .endmodule
                    ld hl,MyModule.MyId
                ",
                0x78, 0x00, 0x78, 0x01, 0x02, 0x60, 0x01, 0x01, 0x60, 0x21, 0x02, 0x60);
        }

        [TestMethod]
        public void ModuleResolutionWithinModuleWorks2()
        {
            CodeEmitWorks(@"
                    .org #6000
                Start:
                    ld a,b
                MyId:
                    nop
                    .module MyModule
                MyId:
                    ld a,b
                    ld bc,MyId
                    ld bc,::MyId
                    .endmodule
                    ld hl,::MyModule.MyId
                ",
                0x78, 0x00, 0x78, 0x01, 0x02, 0x60, 0x01, 0x01, 0x60, 0x21, 0x02, 0x60);
        }

        [TestMethod]
        public void ModuleResolutionWithinModuleWithFixupWorks1()
        {
            CodeEmitWorks(@"
                    .org #6000
                Start:
                    ld a,b
                    .module MyModule
                    ld a,b
                    ld bc,MyId
                MyId:
                    ld bc,::MyId
                    .endmodule
                MyId:
                    ld hl,MyModule.MyId
                ",
                0x78, 0x78, 0x01, 0x05, 0x60, 0x01, 0x08, 0x60, 0x21, 0x05, 0x60);
        }

        [TestMethod]
        public void ModuleResolutionWithinModuleWithFixupWorks2()
        {
            CodeEmitWorks(@"
                    .org #6000
                Start:
                    ld a,b
                    .module MyModule
                    ld a,b
                    ld bc,MyId
                MyId:
                    ld bc,::MyId
                    .endmodule
                MyId:
                    ld hl,::MyModule.MyId
                ",
                0x78, 0x78, 0x01, 0x05, 0x60, 0x01, 0x08, 0x60, 0x21, 0x05, 0x60);
        }

        [TestMethod]
        public void ModuleResolutionWithinNestedModuleWorks1()
        {
            CodeEmitWorks(@"
                    .org #6000
                Start:
                    ld a,b
                MyId:
                    nop
                    .module MyModule
                MyId:
                    ld a,b
                    ld bc,MyId
                        .module NestedModule
                    Inner:    ld bc,::MyId
                        .endmodule
                        ld hl,NestedModule.Inner
                    .endmodule
                    ld hl,MyModule.MyId
                ",
                0x78, 0x00, 0x78, 0x01, 0x02, 0x60, 0x01, 0x01, 0x60, 0x21, 0x06, 0x60, 0x21, 0x02, 0x60);
        }

        [TestMethod]
        public void ModuleResolutionWithinNestedModuleWorks2()
        {
            CodeEmitWorks(@"
                    .org #6000
                Start:
                    ld a,b
                MyId:
                    nop
                    .module MyModule
                MyId:
                    ld a,b
                    ld bc,MyId
                        .module NestedModule
                    Inner:    ld bc,::MyId
                        .endmodule
                        ld hl,::MyModule.NestedModule.Inner
                    .endmodule
                    ld hl,::MyModule.MyId
                ",
                0x78, 0x00, 0x78, 0x01, 0x02, 0x60, 0x01, 0x01, 0x60, 0x21, 0x06, 0x60, 0x21, 0x02, 0x60);
        }

        [TestMethod]
        public void ModuleResolutionWithinNestedModuleWithFixupWorks1()
        {
            CodeEmitWorks(@"
                    .org #6000
                Start:
                    ld a,b
                    .module MyModule
                    ld hl,NestedModule.Inner
                        .module NestedModule
                        Inner: ld bc,MyId
                        MyId: nop   
                        .endmodule
                MyId:
                    ld bc,::MyId
                    .endmodule
                MyId:
                    ld hl,MyModule.MyId
                ",
                0x78, 0x21, 0x04, 0x60, 0x01, 0x07, 0x60, 0x00, 0x01, 0x0b, 0x60, 0x21, 0x08, 0x60);
        }

        [TestMethod]
        public void ModuleResolutionWithinNestedModuleWithFixupWorks2()
        {
            CodeEmitWorks(@"
                    .org #6000
                Start:
                    ld a,b
                    .module MyModule
                    ld hl,::MyModule.NestedModule.Inner
                        .module NestedModule
                        Inner: ld bc,MyId
                        MyId: nop   
                        .endmodule
                MyId:
                    ld bc,::MyModule.MyId
                    .endmodule
                MyId:
                    ld hl,MyModule.MyId
                ",
                0x78, 0x21, 0x04, 0x60, 0x01, 0x07, 0x60, 0x00, 0x01, 0x08, 0x60, 0x21, 0x08, 0x60);
        }

        [TestMethod]
        public void ModuleResolutionWithinNestedModuleWithFixupWorks3()
        {
            CodeEmitWorks(@"
                    .org #6000
                Start:
                    ld a,b
                    .module MyModule
                    ld hl,::MyModule.NestedModule.Inner
                        .module NestedModule
                        Inner: ld bc,MyId
                        MyId: nop   
                        .endmodule
                MyId:
                    ld bc,NestedModule.MyId
                    .endmodule
                MyId:
                    ld hl,MyModule.MyId
                ",
                0x78, 0x21, 0x04, 0x60, 0x01, 0x07, 0x60, 0x00, 0x01, 0x07, 0x60, 0x21, 0x08, 0x60);
        }

        [TestMethod]
        public void ModuleResolutionWithinNestedModuleWithFixupWorks4()
        {
            CodeEmitWorks(@"
                    .org #6000
                Start:
                    ld a,b
                    .module MyModule
                    ld hl,::MyModule.NestedModule.Inner
                        .module NestedModule
                        Inner: ld bc,MyId
                        MyId: nop   
                        .endmodule
                MyId:
                    ld bc,::MyModule.NestedModule.MyId
                    .endmodule
                MyId:
                    ld hl,MyModule.MyId
                ",
                0x78, 0x21, 0x04, 0x60, 0x01, 0x07, 0x60, 0x00, 0x01, 0x07, 0x60, 0x21, 0x08, 0x60);
        }

        [TestMethod]
        public void ModuleResolutionWithinNestedModuleWithFixupWorks5()
        {
            CodeEmitWorks(@"
                    .org #6000
                Start:
                    ld a,b
                    .module MyModule
                    ld hl,NestedModule.Inner
                        NestedModule: .module
                        Inner: ld bc,MyId
                        MyId: nop   
                        .endmodule
                MyId:
                    ld bc,::MyId
                    .endmodule
                MyId:
                    ld hl,MyModule.MyId
                ",
                0x78, 0x21, 0x04, 0x60, 0x01, 0x07, 0x60, 0x00, 0x01, 0x0b, 0x60, 0x21, 0x08, 0x60);
        }

        [TestMethod]
        public void ModuleResolutionWithinNestedModuleWithFixupWorks6()
        {
            CodeEmitWorks(@"
                    .org #6000
                Start:
                    ld a,b
                    .module MyModule
                    ld hl,::MyModule.NestedModule.Inner
                        NestedModule: .module
                        Inner: ld bc,MyId
                        MyId: nop   
                        .endmodule
                MyId:
                    ld bc,::MyModule.MyId
                    .endmodule
                MyId:
                    ld hl,MyModule.MyId
                ",
                0x78, 0x21, 0x04, 0x60, 0x01, 0x07, 0x60, 0x00, 0x01, 0x08, 0x60, 0x21, 0x08, 0x60);
        }

        [TestMethod]
        public void ModuleResolutionWithinNestedModuleWithFixupWorks7()
        {
            CodeEmitWorks(@"
                    .org #6000
                Start:
                    ld a,b
                    .module MyModule
                    ld hl,::MyModule.NestedModule.Inner
                        NestedModule: .module
                        Inner: ld bc,MyId
                        MyId: nop   
                        .endmodule
                MyId:
                    ld bc,NestedModule.MyId
                    .endmodule
                MyId:
                    ld hl,MyModule.MyId
                ",
                0x78, 0x21, 0x04, 0x60, 0x01, 0x07, 0x60, 0x00, 0x01, 0x07, 0x60, 0x21, 0x08, 0x60);
        }

        [TestMethod]
        public void ModuleResolutionWithinNestedModuleWithFixupWorks8()
        {
            CodeEmitWorks(@"
                    .org #6000
                Start:
                    ld a,b
                    .module MyModule
                    ld hl,::MyModule.NestedModule.Inner
                        NestedModule: .scope
                        Inner: ld bc,MyId
                        MyId: nop   
                        .endmodule
                MyId:
                    ld bc,::MyModule.NestedModule.MyId
                    .endmodule
                MyId:
                    ld hl,MyModule.MyId
                ",
                0x78, 0x21, 0x04, 0x60, 0x01, 0x07, 0x60, 0x00, 0x01, 0x07, 0x60, 0x21, 0x08, 0x60);
        }

        [TestMethod]
        public void ModuleLocalResolutionWithinModuleWorks()
        {
            CodeEmitWorks(@"
                    .org #6000
                Start:
                    ld a,b
                MyId:
                    nop
                    .module MyModule
                @MyId:
                    ld a,b
                    ld bc,@MyId
                    ld bc,::MyId
                    .endmodule
                ",
                0x78, 0x00, 0x78, 0x01, 0x02, 0x60, 0x01, 0x01, 0x60);
        }

        [TestMethod]
        public void ModuleLocalResolutionWithinModuleWithFixupWorks()
        {
            CodeEmitWorks(@"
                    .org #6000
                Start:
                    ld a,b
                    .module MyModule
                    ld a,b
                    ld bc,@MyId
                @MyId:
                    ld bc,::MyId
                    .endmodule
                MyId:
                    nop
                ",
                0x78, 0x78, 0x01, 0x05, 0x60, 0x01, 0x08, 0x60, 0x00);
        }

        [TestMethod]
        public void ModuleLocalResolutionWithinNestedModuleWorks1()
        {
            CodeEmitWorks(@"
                    .org #6000
                Start:
                    ld a,b
                MyId:
                    nop
                    .module MyModule
                @MyId:
                    ld a,b
                    ld bc,@MyId
                        .module NestedModule
                    Inner:    ld bc,::MyId
                        .endmodule
                        ld hl,NestedModule.Inner
                    .endmodule
                ",
                0x78, 0x00, 0x78, 0x01, 0x02, 0x60, 0x01, 0x01, 0x60, 0x21, 0x06, 0x60);
        }

        [TestMethod]
        public void ModuleLocalResolutionWithinNestedModuleWithFixupWorks1()
        {
            CodeEmitWorks(@"
                    .org #6000
                Start:
                    ld a,b
                    .module MyModule
                    ld hl,NestedModule.Inner
                        .module NestedModule
                        Inner: ld bc,@MyId
                        @MyId: nop   
                        .endmodule
                @MyId:
                    ld bc,::MyId
                    .endmodule
                MyId:
                    nop
                ",
                0x78, 0x21, 0x04, 0x60, 0x01, 0x07, 0x60, 0x00, 0x01, 0x0b, 0x60, 0x00);
        }

        [TestMethod]
        public void ModuleLocalResolutionWithinNestedModuleWithFixupWorks3()
        {
            CodeEmitWorks(@"
                    .org #6000
                Start:
                    ld a,b
                    .module MyModule
                    ld hl,::MyModule.NestedModule.Inner
                        .module NestedModule
                        Inner: ld bc,@MyId
                        @MyId: nop   
                        .endmodule
                @MyId:
                    .endmodule
                @MyId:
                    nop
                ",
                0x78, 0x21, 0x04, 0x60, 0x01, 0x07, 0x60, 0x00, 0x00);
        }

        [TestMethod]
        public void UnknownModuleNameIsCaught1()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                    .org #6000
                Start:
                    .module MyModule
                    ld a,b
                    ld bc,t1
                t1:
                    ld a,b
                    .endmodule
                    ld hl,OtherModule.t1
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0201);
        }

        [TestMethod]
        public void UnknownModuleNameIsCaught2()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                    .org #6000
                Start:
                    .module MyModule
                    ld a,b
                    ld bc,t1
                t1:
                    ld a,b
                    .endmodule
                    ld hl,::OtherModule.t1
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0201);
        }

        [TestMethod]
        public void ModuleLocalIsResolvedAtTheFirstLevel()
        {
            CodeEmitWorks(@"
                    .org #6000
                Start:
                    .module @MyModule
                    ld a,b
                    ld bc,t1
                t1:
                    ld a,b
                    .endmodule
                    ld hl,@MyModule.t1
                ",
                0x78, 0x01, 0x04, 0x60, 0x78, 0x21, 0x04, 0x60);
        }

        [TestMethod]
        public void LocalModuleNameIsHidden1()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                    .org #6000
                Start:
                    .module MyModule
                    ld a,b
                    .module @Nested
                    Inner: nop
                    .endmodule
                    ld bc,t1
                t1:
                    ld a,b
                    .endmodule
                    ld hl,MyModule.@Nested.Inner
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0201);
        }

        [TestMethod]
        public void LocalModuleNameIsHidden2()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                    .org #6000
                Start:
                    .module @MyModule
                    ld a,b
                    .module @Nested
                    Inner: nop
                    .endmodule
                    ld bc,t1
                t1:
                    ld a,b
                    .endmodule
                    ld hl,@MyModule.@Nested.Inner
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0201);
        }

        [TestMethod]
        public void LocalModuleNameIsHidden3()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                    .org #6000
                Start:
                    .module MyModule
                    ld a,b
                    .module Nested
                    @Inner: nop
                    .endmodule
                    ld bc,t1
                t1:
                    ld a,b
                    .endmodule
                    ld hl,MyModule.Nested.@Inner
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0201);
        }

        [TestMethod]
        public void NonLocalModuleNameIsVisible()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                    .org #6000
                Start:
                    .module MyModule
                    ld a,b
                    .module Nested
                    Inner: nop
                    .endmodule
                    ld bc,t1
                t1:
                    ld a,b
                    .endmodule
                    ld hl,MyModule.Nested.Inner
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
        }

        [TestMethod]
        public void ModuleStartCanBeAddressedWithLabel()
        {
            CodeEmitWorks(@"
                    .org #6000
                Start:
                    .module MyModule
                    ld a,b
                    MyNested: .module Nested
                    Inner: nop
                    .endmodule
                    ld bc,t1
                t1:
                    ld a,b
                    .endmodule
                    ld hl,Start
                ",
                0x78, 0x00, 0x01, 0x05, 0x60, 0x78, 0x21, 0x00, 0x60);
        }

        [TestMethod]
        public void NestedModuleStartCanBeAddressedWithLabel1()
        {
            CodeEmitWorks(@"
                    .org #6000
                Start:
                    .module MyModule
                    ld a,b
                    MyNested: .module Nested
                    Inner: nop
                    .endmodule
                    ld bc,t1
                t1:
                    ld a,b
                    .endmodule
                    ld hl,MyModule.MyNested
                ",
                0x78, 0x00, 0x01, 0x05, 0x60, 0x78, 0x21, 0x01, 0x60);
        }

        [TestMethod]
        public void NestedModuleStartCanBeAddressedWithLabel2()
        {
            CodeEmitWorks(@"
                    .org #6000
                Start:
                    .module MyModule
                    ld a,b
                    MyNested: .module
                    Inner: nop
                    .endmodule
                    ld bc,t1
                t1:
                    ld a,b
                    .endmodule
                    ld hl,MyModule.MyNested
                ",
                0x78, 0x00, 0x01, 0x05, 0x60, 0x78, 0x21, 0x01, 0x60);
        }
    }
}
