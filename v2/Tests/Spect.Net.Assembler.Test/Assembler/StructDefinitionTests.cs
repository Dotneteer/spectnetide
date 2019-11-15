using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.Assembler.Assembler;

// ReSharper disable StringLiteralTypo

namespace Spect.Net.Assembler.Test.Assembler
{
    [TestClass]
    public class StructDefinitionTests : AssemblerTestBed
    {
        [TestMethod]
        public void StructWithNoLabelFails()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                .struct
                .ends
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0432);
        }

        [TestMethod]
        public void StructWithLocalLabelFails()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                `local .struct
                .ends
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0433);
        }

        [TestMethod]
        public void StructWithEndLabelFails()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                MyStruct .struct
                MyEnd: .ends
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0436);
        }

        [TestMethod]
        public void StructWithHangingLabelFails()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                MyStruct
                    .struct
                MyEnd:
                    .ends
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0436);
        }

        [TestMethod]
        public void StructWithLabelWorks()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                MyStruct: .struct
                    .ends
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            output.Structs.ContainsKey("MyStruct").ShouldBeTrue();
            var def = output.Structs["MyStruct"];
            def.ShouldNotBeNull();
            def.Section.FirstLine.ShouldBe(0);
            def.Section.LastLine.ShouldBe(1);
            def.StructName.ShouldBe("MyStruct");
        }

        [TestMethod]
        public void StructWithExistingLabelFails()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                MyStruct: nop
                MyStruct: .struct
                    .ends
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0434);
        }

        [TestMethod]
        public void StructWithExistingLabelFailsCs1()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                MyStruct: nop
                mystruct: .struct
                    .ends
                ", new AssemblerOptions { UseCaseSensitiveSymbols = false });

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0434);
        }

        [TestMethod]
        public void StructWithExistingLabelFailsCs2()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                MyStruct: nop
                mystruct: .struct
                    .ends
                ", new AssemblerOptions { UseCaseSensitiveSymbols = true });

            // --- Assert
            output.ErrorCount.ShouldBe(0);
        }

        [TestMethod]
        public void StructWithExistingLabelFailsCs3()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                MyStruct: nop
                MyStruct: .struct
                    .ends
                ", new AssemblerOptions { UseCaseSensitiveSymbols = true });

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0434);
        }

        [TestMethod]
        public void StructWithHangingLabelWorks()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                MyStruct: 
                    .struct
                    .ends
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            output.Structs.ContainsKey("MyStruct").ShouldBeTrue();
            var def = output.Structs["MyStruct"];
            def.ShouldNotBeNull();
            def.Section.FirstLine.ShouldBe(1);
            def.Section.LastLine.ShouldBe(2);
            def.StructName.ShouldBe("MyStruct");
        }

        [TestMethod]
        public void StructWithNoEndFails()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                MyStruct: .struct
                    ld a,b
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0401);
        }

        [TestMethod]
        [DataRow(".ends")]
        [DataRow("ends")]
        [DataRow(".ENDS")]
        [DataRow("ENDS")]
        public void EndStructWithoutOpenTagFails(string source)
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(source);

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0405);
        }

        [TestMethod]
        [DataRow("ld a,b")]
        [DataRow("jp #1000")]
        public void StructWithInvalidInstructionFails(string stmt)
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile($@"
                MyStruct .struct
                  {stmt}
                .ends
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0435);
        }

        [TestMethod]
        [DataRow("; this is comment")]
        [DataRow("MyField")]
        [DataRow("MyField:")]
        [DataRow(".defb 0x80")]
        [DataRow(".defw 0x8078")]
        [DataRow(".defc \"Hello\"")]
        [DataRow(".defm \"Hello\"")]
        [DataRow(".defn \"Hello\"")]
        [DataRow(".defh \"e345\"")]
        [DataRow(".defs 100")]
        [DataRow(".fillb 10,#ff")]
        [DataRow(".fillw 10,#ffe3")]
        [DataRow(".defgx \"....OOOO\"")]
        [DataRow(".defg \"....OOOO\"")]
        public void StructWithValidPragmaWorks(string stmt)
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile($@"
                MyStruct .struct
                  {stmt}
                  .defb 0x80
                .ends
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
        }

        [TestMethod]
        public void StructDefinitionWithDefbWorks()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                MyStruct: 
                    .struct
                        .defb 0x13, 0x15
                    .ends
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            output.Structs.ContainsKey("MyStruct").ShouldBeTrue();
            var def = output.Structs["MyStruct"];
            def.ShouldNotBeNull();
            def.Size.ShouldBe(2);
        }

        [TestMethod]
        public void StructDefinitionFixupWithDefbWorks()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                MyStruct: 
                    .struct
                        .defb 0x13, Symb1, 0x15, Sym1*Symb2
                    .ends
                Symb1: .equ 1
                Symb2: .equ 2
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            output.Structs.ContainsKey("MyStruct").ShouldBeTrue();
            var def = output.Structs["MyStruct"];
            def.ShouldNotBeNull();
            def.Size.ShouldBe(4);
        }

        [TestMethod]
        public void StructDefinitionWithDefwWorks()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                MyStruct: 
                    .struct
                        .defw 0x13A5, 0x15A6
                    .ends
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            output.Structs.ContainsKey("MyStruct").ShouldBeTrue();
            var def = output.Structs["MyStruct"];
            def.ShouldNotBeNull();
            def.Size.ShouldBe(4);
        }

        [TestMethod]
        public void StructDefinitionFixupWithDefwWorks()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                MyStruct: 
                    .struct
                        .defw 0x13A5, Symb1, 0x15A6, Sym1*Symb2
                    .ends
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            output.Structs.ContainsKey("MyStruct").ShouldBeTrue();
            var def = output.Structs["MyStruct"];
            def.ShouldNotBeNull();
            def.Size.ShouldBe(8);
        }

        [TestMethod]
        public void StructDefinitionWithDefmWorks()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                MyStruct: 
                    .struct
                        .defm ""ABCD""
                    .ends
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            output.Structs.ContainsKey("MyStruct").ShouldBeTrue();
            var def = output.Structs["MyStruct"];
            def.ShouldNotBeNull();
            def.Size.ShouldBe(4);
        }

        [TestMethod]
        public void StructDefinitionWithDefnWorks()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                MyStruct: 
                    .struct
                        .defn ""ABCD""
                    .ends
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            output.Structs.ContainsKey("MyStruct").ShouldBeTrue();
            var def = output.Structs["MyStruct"];
            def.ShouldNotBeNull();
            def.Size.ShouldBe(5);
        }

        [TestMethod]
        public void StructDefinitionWithDefcWorks()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                MyStruct: 
                    .struct
                        .defc ""ABCD""
                    .ends
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            output.Structs.ContainsKey("MyStruct").ShouldBeTrue();
            var def = output.Structs["MyStruct"];
            def.ShouldNotBeNull();
            def.Size.ShouldBe(4);
        }

        [TestMethod]
        public void StructDefinitionWithDefhWorks()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                MyStruct: 
                    .struct
                        .defh ""12AB23CD""
                    .ends
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            output.Structs.ContainsKey("MyStruct").ShouldBeTrue();
            var def = output.Structs["MyStruct"];
            def.ShouldNotBeNull();
            def.Size.ShouldBe(4);
        }

        [TestMethod]
        public void StructDefinitionWithDefsWorks()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                MyStruct: 
                    .struct
                        .defs 3
                    .ends
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            output.Structs.ContainsKey("MyStruct").ShouldBeTrue();
            var def = output.Structs["MyStruct"];
            def.ShouldNotBeNull();
            def.Size.ShouldBe(3);
        }

        [TestMethod]
        public void StructDefinitionWithFillbWorks()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                MyStruct: 
                    .struct
                        .fillb 3, #A5
                    .ends
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            output.Structs.ContainsKey("MyStruct").ShouldBeTrue();
            var def = output.Structs["MyStruct"];
            def.ShouldNotBeNull();
            def.Size.ShouldBe(3);
        }

        [TestMethod]
        public void StructDefinitionWithFillwWorks()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                MyStruct: 
                    .struct
                        .fillw 2, #A547
                    .ends
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            output.Structs.ContainsKey("MyStruct").ShouldBeTrue();
            var def = output.Structs["MyStruct"];
            def.ShouldNotBeNull();
            def.Size.ShouldBe(4);
        }

        [TestMethod]
        public void StructDefinitionWithDefgWorks()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                MyStruct: 
                    .struct
                        .defg ----OOOO OOOO----
                    .ends
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            output.Structs.ContainsKey("MyStruct").ShouldBeTrue();
            var def = output.Structs["MyStruct"];
            def.ShouldNotBeNull();
            def.Size.ShouldBe(2);
        }

        [TestMethod]
        public void StructDefinitionWithDefgxWorks()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                MyStruct: 
                    .struct
                        .defgx ""----OOOO OOOO----""
                    .ends
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            output.Structs.ContainsKey("MyStruct").ShouldBeTrue();
            var def = output.Structs["MyStruct"];
            def.ShouldNotBeNull();
            def.Size.ShouldBe(2);
        }

        [TestMethod]
        public void StructDefinitionWithMultiplePragmasWorks()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                MyStruct: 
                    .struct
                        .defb 0x12, 0x13
                        ; This is a comment
                        .defgx ""----OOOO OOOO----""
                        .fillw 2, #12A3
                        .defw #FEDC
                    .ends
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            output.Structs.ContainsKey("MyStruct").ShouldBeTrue();
            var def = output.Structs["MyStruct"];
            def.ShouldNotBeNull();
            def.Size.ShouldBe(10);
        }

        [TestMethod]
        public void StructDefinitionWithFieldsWorks1()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                MyStruct: 
                    .struct
                        field1: .defb 0x12, 0x13
                        field2: ; This is a comment
                                .defgx ""----OOOO OOOO----""
                        field4: .fillw 2, #12A3
                        .defw #FEDC
                    .ends
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            output.Structs.ContainsKey("MyStruct").ShouldBeTrue();
            var def = output.Structs["MyStruct"];
            def.ShouldNotBeNull();
            def.Fields.Count.ShouldBe(3);
            def.Fields["field1"].Offset.ShouldBe((ushort)0);
            def.Fields["field2"].Offset.ShouldBe((ushort)2);
            def.Fields["field4"].Offset.ShouldBe((ushort)4);
            output.Symbols["MyStruct"].Value.AsWord().ShouldBe((ushort)10);
        }

        [TestMethod]
        public void StructDefinitionWithFieldsWorks2()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                MyStruct: 
                    .struct
                        field1: .defb 0x12, 0x13
                        field2: ; This is a comment
                        field3: .defgx ""----OOOO OOOO----""
                        field4: .fillw 2, #12A3
                        .defw #FEDC
                    .ends
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            output.Structs.ContainsKey("MyStruct").ShouldBeTrue();
            var def = output.Structs["MyStruct"];
            def.ShouldNotBeNull();
            def.Fields.Count.ShouldBe(4);
            def.Fields["field1"].Offset.ShouldBe((ushort)0);
            def.Fields["field2"].Offset.ShouldBe((ushort)2);
            def.Fields["field3"].Offset.ShouldBe((ushort)2);
            def.Fields["field4"].Offset.ShouldBe((ushort)4);
            output.Symbols["MyStruct"].Value.AsWord().ShouldBe((ushort)10);
        }

        [TestMethod]
        public void StructDefinitionWithDuplicatedFieldFails1()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                MyStruct: 
                    .struct
                        field1: .defb 0x12, 0x13
                        field2: ; This is a comment
                                .defgx ""----OOOO OOOO----""
                        field1: .fillw 2, #12A3
                        .defw #FEDC
                    .ends
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0438);
        }

        [TestMethod]
        public void StructDefinitionWithDuplicatedFieldFailsCs1()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                MyStruct: 
                    .struct
                        field1: .defb 0x12, 0x13
                        field2: ; This is a comment
                                .defgx ""----OOOO OOOO----""
                        Field1: .fillw 2, #12A3
                        .defw #FEDC
                    .ends
                ",
                new AssemblerOptions { UseCaseSensitiveSymbols = false });

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0438);
        }

        [TestMethod]
        public void StructDefinitionWithDuplicatedFieldFailsCs2()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                MyStruct: 
                    .struct
                        field1: .defb 0x12, 0x13
                        field2: ; This is a comment
                                .defgx ""----OOOO OOOO----""
                        field1: .fillw 2, #12A3
                        .defw #FEDC
                    .ends
                ",
                new AssemblerOptions { UseCaseSensitiveSymbols = true });

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0438);
        }

        [TestMethod]
        public void StructDefinitionWithDuplicatedFieldFailsCs3()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                MyStruct: 
                    .struct
                        field1: .defb 0x12, 0x13
                        field2: ; This is a comment
                                .defgx ""----OOOO OOOO----""
                        Field1: .fillw 2, #12A3
                        .defw #FEDC
                    .ends
                ",
                new AssemblerOptions { UseCaseSensitiveSymbols = true });

            // --- Assert
            output.ErrorCount.ShouldBe(0);
        }

        [TestMethod]
        public void StructDefinitionWithDuplicatedFieldFails2()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                MyStruct: 
                    .struct
                        field1 .defb 0x12, 0x13
                        field2 ; This is a comment
                        field2 .defgx ""----OOOO OOOO----""
                        field1 .fillw 2, #12A3
                        .defw #FEDC
                    .ends
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(2);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0438);
            output.Errors[1].ErrorCode.ShouldBe(Errors.Z0438);
        }

        [TestMethod]
        public void StructLabelShowsStructLength()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                MyStruct: 
                    .struct
                        .defb 0x12, 0x13
                        ; This is a comment
                        .defgx ""----OOOO OOOO----""
                        .fillw 2, #12A3
                        .defw #FEDC
                    .ends
                .defs MyStruct*10
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            output.Segments[0].EmittedCode.Count.ShouldBe(100);
        }

        [TestMethod]
        public void StructFieldsCanBeResolved()
        {
            // --- Arrange
            const string SOURCE = @"
                MyStruct: 
                    .struct
                        field1: .defb 0x12, 0x13
                        field2: ; This is a comment
                        field3: .defgx ""----OOOO OOOO----""
                        field4: .fillw 2, #12A3
                        .defw #FEDC
                    .ends
                .defb MyStruct.field1
                .defb MyStruct.field2
                .defb MyStruct.field3
                .defb MyStruct.field4
                ";

            // --- Act/Assert
            CodeEmitWorks(SOURCE, 0x00, 0x02, 0x02, 0x04);
        }

        [TestMethod]
        public void StructFieldsCanBeResolvedWithGlobalModule()
        {
            // --- Arrange
            const string SOURCE = @"
                MyStruct: 
                    .struct
                        field1: .defb 0x12, 0x13
                        field2: ; This is a comment
                        field3: .defgx ""----OOOO OOOO----""
                        field4: .fillw 2, #12A3
                        .defw #FEDC
                    .ends
                .defb ::MyStruct.field1
                .defb ::MyStruct.field2
                .defb ::MyStruct.field3
                .defb ::MyStruct.field4
                ";

            // --- Act/Assert
            CodeEmitWorks(SOURCE, 0x00, 0x02, 0x02, 0x04);
        }

        [TestMethod]
        public void StructFieldsCanBeResolvedOutOfModule()
        {
            // --- Arrange
            const string SOURCE = @"
                MyModule: .module
                MyStruct: 
                    .struct
                        field1: .defb 0x12, 0x13
                        field2: ; This is a comment
                        field3: .defgx ""----OOOO OOOO----""
                        field4: .fillw 2, #12A3
                        .defw #FEDC
                    .ends
                .endmodule
                .defb MyModule.MyStruct.field1
                .defb MyModule.MyStruct.field2
                .defb MyModule.MyStruct.field3
                .defb MyModule.MyStruct.field4
                ";

            // --- Act/Assert
            CodeEmitWorks(SOURCE, 0x00, 0x02, 0x02, 0x04);
        }

        [TestMethod]
        public void StructFieldsCanBeResolvedOutOfModuleFromGlobal()
        {
            // --- Arrange
            const string SOURCE = @"
                MyModule: .module
                MyStruct: 
                    .struct
                        field1: .defb 0x12, 0x13
                        field2: ; This is a comment
                        field3: .defgx ""----OOOO OOOO----""
                        field4: .fillw 2, #12A3
                        .defw #FEDC
                    .ends
                .endmodule
                .defb ::MyModule.MyStruct.field1
                .defb ::MyModule.MyStruct.field2
                .defb ::MyModule.MyStruct.field3
                .defb ::MyModule.MyStruct.field4
                ";

            // --- Act/Assert
            CodeEmitWorks(SOURCE, 0x00, 0x02, 0x02, 0x04);
        }

        [TestMethod]
        public void StructFieldsCanBeResolvedWithinModule()
        {
            // --- Arrange
            const string SOURCE = @"
                MyModule: .module
                MyStruct: 
                    .struct
                        field1: .defb 0x12, 0x13
                        field2: ; This is a comment
                        field3: .defgx ""----OOOO OOOO----""
                        field4: .fillw 2, #12A3
                        .defw #FEDC
                    .ends
                .defb MyStruct.field1
                .defb MyStruct.field2
                .defb MyStruct.field3
                .defb MyStruct.field4
                .endmodule
                ";

            // --- Act/Assert
            CodeEmitWorks(SOURCE, 0x00, 0x02, 0x02, 0x04);
        }

        [TestMethod]
        public void StructCanBeResolvedWithinModule()
        {
            // --- Arrange
            const string SOURCE = @"
                MyModule: .module
                MyStruct: 
                    .struct
                        field1: .defb 0x12, 0x13
                        field2: ; This is a comment
                        field3: .defgx ""----OOOO OOOO----""
                        field4: .fillw 2, #12A3
                        .defw #FEDC
                    .ends
                .defb ::MyModule.MyStruct
                .endmodule
                ";

            // --- Act/Assert
            CodeEmitWorks(SOURCE, 0x0A);
        }
    }
}