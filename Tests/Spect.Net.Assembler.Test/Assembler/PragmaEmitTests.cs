using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.Assembler.Assembler;
// ReSharper disable PossibleInvalidOperationException

namespace Spect.Net.Assembler.Test.Assembler
{
    [TestClass]
    public class PragmaEmitTests : AssemblerTestBed
    {
        [TestMethod]
        public void OrgPragmaWorksWithExistingSegment()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                .org #6400
                ld a,b");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            output.Segments.Count.ShouldBe(1);
            output.Segments[0].StartAddress.ShouldBe((ushort)0x6400);
            output.Segments[0].Displacement.ShouldBeNull();
            output.Segments[0].EmittedCode.Count.ShouldBe(1);
        }

        [TestMethod]
        public void OrgPragmaWorksWithNewSegment()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                ld a,c
                .org #6400
                ld a,b
                ld a,c");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            output.Segments.Count.ShouldBe(2);
            output.Segments[0].StartAddress.ShouldBe((ushort)0x8000);
            output.Segments[0].Displacement.ShouldBeNull();
            output.Segments[0].EmittedCode.Count.ShouldBe(1);
            output.Segments[1].StartAddress.ShouldBe((ushort)0x6400);
            output.Segments[1].Displacement.ShouldBeNull();
            output.Segments[1].EmittedCode.Count.ShouldBe(2);
        }

        [TestMethod]
        public void SingleEntPragmaWorksAsExpected()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                .org #6400
                ld a,b
                .ent #6400");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            output.Segments.Count.ShouldBe(1);
            output.EntryAddress.ShouldBe((ushort)0x6400);
            output.Segments[0].Displacement.ShouldBeNull();
            output.Segments[0].EmittedCode.Count.ShouldBe(1);
        }

        [TestMethod]
        public void MultipleEntPragmaWorksAsExpected()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                .org #6400
                ld a,b
                .ent #6400
                ld a,c
                .ent #1234");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            output.Segments.Count.ShouldBe(1);
            output.EntryAddress.ShouldBe((ushort)0x1234);
            output.Segments[0].Displacement.ShouldBeNull();
            output.Segments[0].EmittedCode.Count.ShouldBe(2);
        }

        [TestMethod]
        public void SingleEntPragmaWorksWithCurrentAddress()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                .org #6400
                ld a,b
                .ent $");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            output.Segments.Count.ShouldBe(1);
            output.EntryAddress.ShouldBe((ushort)0x6401);
            output.Segments[0].Displacement.ShouldBeNull();
            output.Segments[0].EmittedCode.Count.ShouldBe(1);
        }

        [TestMethod]
        public void SingleXentPragmaWorksAsExpected()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                .org #6400
                ld a,b
                .xent #6400");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            output.Segments.Count.ShouldBe(1);
            output.ExportEntryAddress.ShouldBe((ushort)0x6400);
            output.Segments[0].Displacement.ShouldBeNull();
            output.Segments[0].EmittedCode.Count.ShouldBe(1);
        }

        [TestMethod]
        public void MultipleXentPragmaWorksAsExpected()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                .org #6400
                ld a,b
                .xent #6400
                ld a,c
                .xent #1234");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            output.Segments.Count.ShouldBe(1);
            output.ExportEntryAddress.ShouldBe((ushort)0x1234);
            output.Segments[0].Displacement.ShouldBeNull();
            output.Segments[0].EmittedCode.Count.ShouldBe(2);
        }

        [TestMethod]
        public void SingleXentPragmaWorksWithCurrentAddress()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                .org #6400
                ld a,b
                .xent $");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            output.Segments.Count.ShouldBe(1);
            output.ExportEntryAddress.ShouldBe((ushort)0x6401);
            output.Segments[0].Displacement.ShouldBeNull();
            output.Segments[0].EmittedCode.Count.ShouldBe(1);
        }

        [TestMethod]
        public void DispPragmaWorksWithNegativeDisplacement()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                .org #6400
                .disp -100
                ld a,b");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            output.Segments.Count.ShouldBe(1);
            output.Segments[0].StartAddress.ShouldBe((ushort)0x6400);
            output.Segments[0].Displacement.Value.ShouldBe((ushort)(0x10000-100));
            output.Segments[0].EmittedCode.Count.ShouldBe(1);
        }

        [TestMethod]
        public void DispPragmaWorksWithPositiveDisplacement()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                .org #6400
                .disp #1000
                ld a,b");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            output.Segments.Count.ShouldBe(1);
            output.Segments[0].StartAddress.ShouldBe((ushort)0x6400);
            output.Segments[0].Displacement.Value.ShouldBe((ushort)0x1000);
            output.Segments[0].EmittedCode.Count.ShouldBe(1);
        }

        [TestMethod]
        public void DispPragmaWorksWithZeroDisplacement()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                .org #6400
                .disp #1000
                ld a,b");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            output.Segments.Count.ShouldBe(1);
            output.Segments[0].StartAddress.ShouldBe((ushort)0x6400);
            output.Segments[0].Displacement.Value.ShouldBe((ushort)0x1000);
            output.Segments[0].EmittedCode.Count.ShouldBe(1);
        }

        [TestMethod]
        public void EquPragmaRaisesErrorWithNoLabel()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                .equ 100+100
                ld a,b");

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0082);
        }

        [TestMethod]
        public void EquPragmaWorksWithImmediateEvaluation()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                MySymbol .equ 100+100
                ld a,b");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            output.Segments.Count.ShouldBe(1);
            output.Symbols["MYSYMBOL"].Value.ShouldBe((ushort)200);
        }

        [TestMethod]
        public void EquPragmaWorksWithRedefinition()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                MySymbol ld a,1
                ld a,2
                MySymbol .equ 100+100
                ld a,b");

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0040);
        }

        [TestMethod]
        public void EquPragmaCreatesFixup()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                MySymbol .equ 100+OtherSymbol
                ld a,b
                OtherSymbol ld a,c");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            output.Segments.Count.ShouldBe(1);
            output.Fixups.Count.ShouldBe(1);
            var fixup = output.Fixups[0];
            fixup.Type.ShouldBe(FixupType.Equ);
            fixup.Expression.ShouldNotBeNull();
        }

        [TestMethod]
        public void VarPragmaWorksWithInitialDefinition()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                MySymbol .var 100+100
                ld a,b");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            output.Vars["MYSYMBOL"].Value.ShouldBe((ushort)200);
        }

        [TestMethod]
        public void VarPragmaWorksWithReassignment()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                MySymbol .var 100+100
                ld a,b
                MySymbol .var MySymbol*3");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            output.Vars["MYSYMBOL"].Value.ShouldBe((ushort)600);
        }

        [TestMethod]
        public void VarPragmaRaisesErrorWithNoLabel()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                .var 100+100
                ld a,b");

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0086);
        }

        [TestMethod]
        public void DefbPragmaWorksWithImmediateEvaluation()
        {
            // --- Arrange
            var compiler = new Z80Assembler();
            var expected = new byte[] {0x01, 0x45, 0xAE, 122};

            // --- Act
            var output = compiler.Compile(@"
                .defb #01, #2345, #AE, 122");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            output.Segments.Count.ShouldBe(1);
            var segment = output.Segments[0];
            segment.EmittedCode.Count.ShouldBe(expected.Length);
            for (var i = 0; i < expected.Length; i++)
            {
                segment.EmittedCode[i].ShouldBe(expected[i]);
            }
        }

        [TestMethod]
        public void DefwPragmaWorksWithImmediateEvaluation()
        {
            // --- Arrange
            var compiler = new Z80Assembler();
            var expected = new byte[] { 0x01, 0xA0, 0x45, 0x23, 0x12, 0xAE, 122, 0 };

            // --- Act
            var output = compiler.Compile(@"
                .defw #A001, #2345, #AE12, 122");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            output.Segments.Count.ShouldBe(1);
            var segment = output.Segments[0];
            segment.EmittedCode.Count.ShouldBe(expected.Length);
            for (var i = 0; i < expected.Length; i++)
            {
                segment.EmittedCode[i].ShouldBe(expected[i]);
            }
        }

        [TestMethod]
        public void DefwPragmaWorksWithFunctions()
        {
            // --- Arrange
            var compiler = new Z80Assembler();
            var expected = new byte[] { 0xE5, 0x03 };

            // --- Act
            var output = compiler.Compile(@"
                .defw 1000*sin(1.5)");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            output.Segments.Count.ShouldBe(1);
            var segment = output.Segments[0];
            segment.EmittedCode.Count.ShouldBe(expected.Length);
            for (var i = 0; i < expected.Length; i++)
            {
                segment.EmittedCode[i].ShouldBe(expected[i]);
            }
        }

        [TestMethod]
        public void DefwPragmaFailsFunctions()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                .DEFW 1000/sinx(1.5)");

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0200);
        }

        [TestMethod]
        public void DefmPragmaWorksAsExpected()
        {
            // --- Arrange
            var compiler = new Z80Assembler();
            var expected = new byte[] { 0x12, 0x10, 0x14, 0x61, 0x62, 0x63, 0x60 };

            // --- Act
            var output = compiler.Compile(".defm \"\\x12\\i\\Iabc\\P\"");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            output.Segments.Count.ShouldBe(1);
            var segment = output.Segments[0];
            segment.EmittedCode.Count.ShouldBe(expected.Length);
            for (var i = 0; i < expected.Length; i++)
            {
                segment.EmittedCode[i].ShouldBe(expected[i]);
            }
        }


        [TestMethod]
        public void SkipPragmaWorksWithoutFillValue()
        {
            // --- Arrange
            var compiler = new Z80Assembler();
            var expected = new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };

            // --- Act
            var output = compiler.Compile(@"
                .skip $+#05");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            output.Segments.Count.ShouldBe(1);
            var segment = output.Segments[0];
            segment.EmittedCode.Count.ShouldBe(expected.Length);
            for (var i = 0; i < expected.Length; i++)
            {
                segment.EmittedCode[i].ShouldBe(expected[i]);
            }
        }

        [TestMethod]
        public void SkipPragmaWorksWithFillValue()
        {
            // --- Arrange
            var compiler = new Z80Assembler();
            var expected = new byte[] { 0x3A, 0x3A, 0x3A, 0x3A, 0x3A };

            // --- Act
            var output = compiler.Compile(@"
                .skip $+#05, #3A");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            output.Segments.Count.ShouldBe(1);
            var segment = output.Segments[0];
            segment.EmittedCode.Count.ShouldBe(expected.Length);
            for (var i = 0; i < expected.Length; i++)
            {
                segment.EmittedCode[i].ShouldBe(expected[i]);
            }
        }

        [TestMethod]
        public void SkipPragmaFailsWithNegativeSkipValue()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                .skip $-#05");

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0081);
        }

        [TestMethod]
        public void SkipPragmaFailsWithNonImmediateSkipValue()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                .skip MySymbol+5");

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0201);
        }

        [TestMethod]
        public void SkipPragmaFailsWithNonImmediateFillValue()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                .skip $+5, MySymbol+3");

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0201);
        }

        [TestMethod]
        public void DefsPragmaWorksWithImmediateEvaluation()
        {
            // --- Arrange
            var compiler = new Z80Assembler();
            var expected = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

            // --- Act
            var output = compiler.Compile(@".defs 6");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            output.Segments.Count.ShouldBe(1);
            var segment = output.Segments[0];
            segment.EmittedCode.Count.ShouldBe(expected.Length);
            for (var i = 0; i < expected.Length; i++)
            {
                segment.EmittedCode[i].ShouldBe(expected[i]);
            }
        }

        [TestMethod]
        public void DefsPragmaWorksWithIndirectEvaluation()
        {
            // --- Arrange
            var compiler = new Z80Assembler();
            var expected = new byte[] { 0x00, 0x00, 0x00 };

            // --- Act
            var output = compiler.Compile(@"
                Count: .equ 3
                .defs Count");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            output.Segments.Count.ShouldBe(1);
            var segment = output.Segments[0];
            segment.EmittedCode.Count.ShouldBe(expected.Length);
            for (var i = 0; i < expected.Length; i++)
            {
                segment.EmittedCode[i].ShouldBe(expected[i]);
            }
        }

        [TestMethod]
        public void FillbPragmaWorksWithImmediateEvaluation()
        {
            CodeEmitWorks(@".fillb 3,#A5",
                0xA5, 0xA5, 0xA5);
        }

        [TestMethod]
        public void FillbPragmaWorksWithExpressions()
        {
            CodeEmitWorks(@"
                MySymbol: .equ #80A5
                Count:    .equ 3
                    .fillb Count,MySymbol",
                0xA5, 0xA5, 0xA5);
        }

        [TestMethod]
        public void FillwPragmaWorksWithImmediateEvaluation()
        {
            CodeEmitWorks(@".fillw 3,#80A5",
                0xA5, 0x80, 0xA5, 0x80, 0xA5, 0x80);
        }

        [TestMethod]
        public void FillwPragmaWorksWithExpressions()
        {
            CodeEmitWorks(@"
                MySymbol: .equ #80A5
                Count:    .equ 3
                    .fillw Count,MySymbol",
                0xA5, 0x80, 0xA5, 0x80, 0xA5, 0x80);
        }

        [TestMethod]
        public void NoModelPragmaWorks()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                .org #8000");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            output.ModelType.ShouldBeNull();
        }

        [TestMethod]
        public void ModelPragmaWorksWith48()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                .model Spectrum48");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            output.ModelType.ShouldBe(SpectrumModelType.Spectrum48);
        }

        [TestMethod]
        public void ModelPragmaWorksWith128()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                .model Spectrum128");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            output.ModelType.ShouldBe(SpectrumModelType.Spectrum128);
        }

        [TestMethod]
        public void ModelPragmaWorksWithP3()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                .model SpectrumP3");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            output.ModelType.ShouldBe(SpectrumModelType.SpectrumP3);
        }

        [TestMethod]
        public void ModelPragmaWorksWithNext()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                .model Next");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            output.ModelType.ShouldBe(SpectrumModelType.Next);
        }

        [TestMethod]
        public void ModelPragmaFailsWithUnkownModel()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                .model SpectrumQL");

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0089);
        }

        [TestMethod]
        public void ModelPragmaFailsWithRedefinition()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                .model Spectrum48
                .model Spectrum128");

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0088);
        }

    }
}