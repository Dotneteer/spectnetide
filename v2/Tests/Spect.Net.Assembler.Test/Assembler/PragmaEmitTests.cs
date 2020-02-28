using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.Assembler.Assembler;
// ReSharper disable PossibleInvalidOperationException
// ReSharper disable StringLiteralTypo
// ReSharper disable IdentifierTypo

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
        public void OrgPragmaWorksWithLocalLabel()
        {
            // --- Act
            CodeEmitWorks(@"
                `local .org #6000
                ld bc,`local",
                0x01, 0x00, 0x60);
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
                .disp 0
                ld a,b");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            output.Segments.Count.ShouldBe(1);
            output.Segments[0].StartAddress.ShouldBe((ushort)0x6400);
            output.Segments[0].Displacement.Value.ShouldBe((ushort)0x000);
            output.Segments[0].EmittedCode.Count.ShouldBe(1);
        }

        [TestMethod]
        public void XorgPragmaWorksWithNegativeValue()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                .org #6400
                .xorg -100
                ld a,b");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            output.Segments.Count.ShouldBe(1);
            output.Segments[0].StartAddress.ShouldBe((ushort)0x6400);
            output.Segments[0].XorgValue.Value.ShouldBe((ushort)(0x10000 - 100));
            output.Segments[0].EmittedCode.Count.ShouldBe(1);
        }

        [TestMethod]
        public void XorgPragmaWorksWithPositiveValue()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                .org #6400
                .xorg #1000
                ld a,b");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            output.Segments.Count.ShouldBe(1);
            output.Segments[0].StartAddress.ShouldBe((ushort)0x6400);
            output.Segments[0].XorgValue.Value.ShouldBe((ushort)0x1000);
            output.Segments[0].EmittedCode.Count.ShouldBe(1);
        }

        [TestMethod]
        public void XorgPragmaWorksWithZeroValue()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                .org #6400
                .xorg 0
                ld a,b");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            output.Segments.Count.ShouldBe(1);
            output.Segments[0].StartAddress.ShouldBe((ushort)0x6400);
            output.Segments[0].XorgValue.Value.ShouldBe((ushort)0x0000);
            output.Segments[0].EmittedCode.Count.ShouldBe(1);
        }

        [TestMethod]
        public void MultipleXorgPragmaInASegmentRaisesError()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                .org #6400
                .xorg 0
                ld a,b
                .xorg #1000
                ld a,b");

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0431);
        }

        [TestMethod]
        public void MultipleXorgPragmaInSeparateSegmentsWork()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                .org #6400
                .xorg 0
                ld a,b
                .org #6600
                .xorg #1000
                ld a,b");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
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
        public void EquPragmaWorksWithLocalLabel()
        {
            // --- Act
            CodeEmitWorks(@"
                `local .equ #10+#10
                ld a,`local",
                0x3E, 0x20);
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
            output.Symbols["MYSYMBOL"].Value.Value.ShouldBe((ushort)200);
        }

        [TestMethod]
        public void EquPragmaWorksWithImmediateEvaluation2()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                MySymbol: .equ 100+100
                ld a,b");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            output.Segments.Count.ShouldBe(1);
            output.Symbols["MYSYMBOL"].Value.Value.ShouldBe((ushort)200);
        }

        [TestMethod]
        public void EquPragmaWorksWithImmediateEvaluation3()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                MySymbol
                    .equ 100+100
                ld a,b");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            output.Segments.Count.ShouldBe(1);
            output.Symbols["MYSYMBOL"].Value.Value.ShouldBe((ushort)200);
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
        [DataRow(".var")]
        [DataRow(".VAR")]
        [DataRow("var")]
        [DataRow("VAR")]
        [DataRow("=")]
        [DataRow(":=")]
        public void VarPragmaWorksWithInitialDefinition(string source)
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                MySymbol " + source + @" 100+100
                ld a,b");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            output.Symbols["MYSYMBOL"].Value.Value.ShouldBe((ushort)200);
        }

        [TestMethod]
        [DataRow(".var")]
        [DataRow(".VAR")]
        [DataRow("var")]
        [DataRow("VAR")]
        [DataRow("=")]
        [DataRow(":=")]
        public void VarPragmaWorksWithReassignment(string source)
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                MySymbol " + source + @" 100+100
                ld a,b
                MySymbol " + source + @" MySymbol*3");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            output.Symbols["MYSYMBOL"].Value.Value.ShouldBe((ushort)600);
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
        public void VarPragmaWorksWithLocalLabel()
        {
            // --- Act
            CodeEmitWorks(@"
                `local .var #68
                ld a,`local",
                0x3e, 0x68);
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
        public void DefbPragmaFailsWithString()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                .defb ""Hello""");

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0305);
        }

        [TestMethod]
        public void DefbPragmaWorksInFlexiblemode()
        {
            // --- Arrange
            var compiler = new Z80Assembler();
            var expected = new byte[] { 0x12, 0x10, 0x14, 0x61, 0x62, 0x63, 0x60 };

            // --- Act
            var output = compiler.Compile(".zxbasic\n.defb \"\\x12\\i\\Iabc\\P\"");

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
        public void DefwPragmaFailsWithString()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                .defw ""Hello""");

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0305);
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
        public void DefmPragmaFailsWithNonString()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                .defm 123");

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0091);
        }

        [TestMethod]
        public void DefmPragmaWorksInFlexibleMode()
        {
            // --- Arrange
            var compiler = new Z80Assembler();
            var expected = new byte[] { 123 };

            // --- Act
            var output = compiler.Compile(".zxbasic\n.defm 123");

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
        public void DefnPragmaWorksAsExpected()
        {
            // --- Arrange
            var compiler = new Z80Assembler();
            var expected = new byte[] { 0x12, 0x10, 0x14, 0x61, 0x62, 0x63, 0x60, 0x00 };

            // --- Act
            var output = compiler.Compile(".defn \"\\x12\\i\\Iabc\\P\"");

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
        public void DefnPragmaFailsWithNonString()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                .defn 123");

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0091);
        }

        [TestMethod]
        public void DefnPragmaWorksInFlexibleMode()
        {
            // --- Arrange
            var compiler = new Z80Assembler();
            var expected = new byte[] { 123, 0 };

            // --- Act
            var output = compiler.Compile(".zxbasic\n.defn 123");

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
        public void DefcPragmaWorksAsExpected()
        {
            // --- Arrange
            var compiler = new Z80Assembler();
            var expected = new byte[] { 0x12, 0x10, 0x14, 0x61, 0x62, 0x63, 0xE0 };

            // --- Act
            var output = compiler.Compile(".defc \"\\x12\\i\\Iabc\\P\"");

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
        public void DefcPragmaFailsWithNonString()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                .defc 123");

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0091);
        }

        [TestMethod]
        public void DefcPragmaWorksInFlexibleMode()
        {
            // --- Arrange
            var compiler = new Z80Assembler();
            var expected = new byte[] { 0xA2 };

            // --- Act
            var output = compiler.Compile(".zxbasic\n.defc 0x22");

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
        public void DefhPragmaWorksAsExpected()
        {
            // --- Arrange
            var compiler = new Z80Assembler();
            var expected = new byte[] { 0x01, 0x05, 0xC1, 0xaf, 0x27, 0xd3 };

            // --- Act
            var output = compiler.Compile(".defh \"0105C1af27d3\"");

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
        public void DefhPragmaWorksWithEmptyString()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(".defh \"\"");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            output.Segments.Count.ShouldBe(1);
            var segment = output.Segments[0];
            segment.EmittedCode.Count.ShouldBe(0);
        }

        [TestMethod]
        public void DefhPragmaFailsWithNonString()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                .defh 123");

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0093);
        }

        [TestMethod]
        public void DefhPragmaFailsWithOddLengthString()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(".defh \"0105C1af27d\"");

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0094);
        }

        [TestMethod]
        public void DefhPragmaFailsWithBobHexaDigitString()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(".defh \"0105C1Xaf27d\"");

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0094);
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
        public void DefsPragmaWorksWithImmediateEvaluation1()
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
        public void DefsPragmaWorksWithImmediateEvaluation2()
        {
            // --- Arrange
            var compiler = new Z80Assembler();
            var expected = new byte[] { 0x2a, 0x2a, 0x2a, 0x2a, 0x2a, 0x2a };

            // --- Act
            var output = compiler.Compile(@".defs 6, 0x2a");

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
        public void DefsPragmaWorksWithIndirectEvaluation1()
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
        public void DefsPragmaWorksWithIndirectEvaluation2()
        {
            // --- Arrange
            var compiler = new Z80Assembler();
            var expected = new byte[] { 0x04, 0x04, 0x04 };

            // --- Act
            var output = compiler.Compile(@"
                Count: .equ 3
                Value: .equ 4
                .defs Count, Value");

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

        [TestMethod]
        public void AlignPragmaWorksWithNoExpression()
        {
            // --- Arrange
            var compiler = new Z80Assembler();
            // --- Assert

            // --- Act
            var output = compiler.Compile(@"
                ld a,b
                .align
                ld a,b");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            output.Segments.Count.ShouldBe(1);
            var bytes = output.Segments[0].EmittedCode;
            bytes.Count.ShouldBe(0x101);
            bytes[0].ShouldBe((byte)0x78);
            for (var i = 1; i < 0x100; i++)
            {
                bytes[i].ShouldBe((byte)0x00);
            }
            bytes[0x100].ShouldBe((byte)0x78);
        }

        [TestMethod]
        [DataRow("ld a,b", 0x100, "ld a,b", new byte[] {0x78}, new byte[] {0x78})]
        [DataRow("ld a,b", 0x02, "ld a,b", new byte[] { 0x78 }, new byte[] { 0x78 })]
        [DataRow("ld a,b", 0x04, "ld a,b", new byte[] { 0x78 }, new byte[] { 0x78 })]
        [DataRow("", 0x08, "ld a,b", new byte[0], new byte[] { 0x78 })]
        [DataRow("ld a,b", 0x10, "", new byte[] { 0x78 }, new byte[0])]
        public void AlignPragmaWorksWithExpression(string entrySource, int align, string exitSource, byte[] head, byte[] tail)
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(entrySource + "\n .align " + align + "\n" + exitSource);

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            output.Segments.Count.ShouldBe(1);
            var bytes = output.Segments[0].EmittedCode;
            var alignedLength = head.Length == 0 ? 0x00 : align;
            bytes.Count.ShouldBe(alignedLength + tail.Length);
            for (var i = 0; i < head.Length; i++)
            {
                bytes[i].ShouldBe(head[i]);
            }
            for (var i = head.Length; i < alignedLength; i++)
            {
                bytes[i].ShouldBe((byte)0x00);
            }
            for (var i = 0; i < tail.Length; i++)
            {
                bytes[alignedLength + i].ShouldBe(tail[i]);
            }
        }

        [TestMethod]
        [DataRow("trace #100", "256")]
        [DataRow("tracehex #100", "0100")]
        [DataRow("trace \"Hello\", #100, #200 ", "Hello256512")]
        [DataRow("trace 3.14/2", "1.57")]
        [DataRow("tracehex #1000*#1000", "01000000")]
        [DataRow("tracehex \"Hello\"", "48656C6C6F")]
        public void TracePragmaGeneratesOutput(string source, string message)
        {
            // --- Arrange
            var compiler = new Z80Assembler();
            string messageReceived = null;
            compiler.AssemblerMessageCreated += (s, args) => { messageReceived = args.Message; };

            // --- Act
            var output = compiler.Compile(source);

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            output.Segments.Count.ShouldBe(1);
            messageReceived.ShouldBe(message);
        }

        [TestMethod]
        [DataRow(".dgx \"....OOOO\"", new byte[] { 0x0F })]
        [DataRow(".dgx \">....OOOO\"", new byte[] { 0x0F })]
        [DataRow(".dgx \"<----OOOO\"", new byte[] { 0x0F })]
        [DataRow(".dgx \"___OOOO\"", new byte[] { 0x1E })]
        [DataRow(".dgx \"..OOOO\"", new byte[] { 0x3C })]
        [DataRow(".dgx \"-OOOO\"", new byte[] { 0x78 })]
        [DataRow(".dgx \"<___####\"", new byte[] { 0x1E })]
        [DataRow(".dgx \"<..OOOO\"", new byte[] { 0x3C })]
        [DataRow(".dgx \"<.OOOO\"", new byte[] { 0x78 })]
        [DataRow(".dgx \">...XXXX\"", new byte[] { 0x0F })]
        [DataRow(".dgx \">..xxxx\"", new byte[] { 0x0F })]
        [DataRow(".dgx \">.qqqq\"", new byte[] { 0x0F })]
        [DataRow(".dgx \">OOOO\"", new byte[] { 0x0F })]

        [DataRow(".dgx \" ....OOOO\"", new byte[] { 0x0F })]
        [DataRow(".dgx \" .... OOOO \"", new byte[] { 0x0F })]

        [DataRow(".dgx \"....OOOO ..OO\"", new byte[] { 0x0F, 0x30 })]
        [DataRow(".dgx \"....OOOO ..OOO\"", new byte[] { 0x0F, 0x38 })]
        [DataRow(".dgx \"....OOOO ..OOOO\"", new byte[] { 0x0F, 0x3C })]
        [DataRow(".dgx \">....OOOO ..OO\"", new byte[] { 0x00, 0xF3 })]
        [DataRow(".dgx \">....O OOO..OOO\"", new byte[] { 0x01, 0xE7 })]
        [DataRow(".dgx \">....OO OO..OOOO\"", new byte[] { 0x03, 0xCF })]
        public void DefgxPragmaWorksAsExpected(string source, byte[] expected)
        {
            CodeEmitWorks(source, expected);
        }

        [TestMethod]
        [DataRow(".dg ....OOOO", new byte[] { 0x0F })]
        [DataRow(".dg ....OOOO", new byte[] { 0x0F })]
        [DataRow(".dg ----OOOO", new byte[] { 0x0F })]
        [DataRow(".dg ___OOOO", new byte[] { 0x1E })]
        [DataRow(".dg ..OOOO", new byte[] { 0x3C })]
        [DataRow(".dg -OOOO", new byte[] { 0x78 })]
        [DataRow(".dg ___####", new byte[] { 0x1E })]
        [DataRow(".dg ..OOOO", new byte[] { 0x3C })]
        [DataRow(".dg .OOOO", new byte[] { 0x78 })]
        [DataRow(".dg ...XXXX", new byte[] { 0x1E })]
        [DataRow(".dg ..xxxx", new byte[] { 0x3C })]
        [DataRow(".dg .qqqq", new byte[] { 0x78 })]
        [DataRow(".dg OOOO", new byte[] { 0xF0 })]

        [DataRow(".dg   ....OOOO", new byte[] { 0x0F })]
        [DataRow(".dg  .... OOOO ", new byte[] { 0x0F })]

        [DataRow(".dg ....OOOO ..OO", new byte[] { 0x0F, 0x30 })]
        [DataRow(".dg ....OOOO ..OOO", new byte[] { 0x0F, 0x38 })]
        [DataRow(".dg ....OOOO ..OOOO", new byte[] { 0x0F, 0x3C })]
        [DataRow(".dg ....OOOO ..OO", new byte[] { 0x0F, 0x30 })]
        [DataRow(".dg ....O OOO..OOO", new byte[] { 0x0F, 0x38 })]
        [DataRow(".dg ....OO OO..OOOO", new byte[] { 0x0F, 0x3C })]
        [DataRow(".dg ....OO OO..OOOO; This is comment", new byte[] { 0x0F, 0x3C })]
        public void DefgPragmaWorksAsExpected(string source, byte[] expected)
        {
            CodeEmitWorks(source, expected);
        }

        [TestMethod]
        [DataRow(".error \"This is an error\"")]
        [DataRow(".ERROR \"This is an error\"")]
        [DataRow("error \"This is an error\"")]
        [DataRow("ERROR \"This is an error\"")]
        public void ErrorPragmaWorks(string source)
        {
            CodeRaisesError(source, Errors.Z0500);
        }

        [TestMethod]
        [DataRow(".error true")]
        [DataRow(".ERROR 123")]
        [DataRow("error 123.5")]
        public void ErrorPragmaWorksWithDifferentTypes(string source)
        {
            CodeRaisesError(source, Errors.Z0500);
        }

        [TestMethod]
        [DataRow("zxbasic")]
        [DataRow("ZXBASIC")]
        [DataRow(".zxbasic")]
        [DataRow(".ZXBASIC")]
        public void ZxBasicPragmaSetsSourceType(string source)
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(source);

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            output.SourceType.ShouldBe("zxbasic");
        }

        [TestMethod]
        [DataRow(".org #8000 \n zxbasic")]
        [DataRow("ld a,b \n zxbasic")]
        [DataRow(".model spectrum48 \n zxbasic")]
        [DataRow("#ifdef idvalue \n zxbasic \n #endif")]
        public void ZxBasicFailsIfNotFirst(string source)
        {
            CodeRaisesError(source, Errors.Z0450);
        }

        [TestMethod]
        public void BankPragmaWorksWithExistingSegment1()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                .model Spectrum128
                .org #6400
                ld a,b
                .bank 1
            ");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            output.Segments.Count.ShouldBe(2);
            output.Segments[0].StartAddress.ShouldBe((ushort)0x6400);
            output.Segments[0].Bank.ShouldBeNull();
            output.Segments[0].Displacement.ShouldBeNull();
            output.Segments[0].EmittedCode.Count.ShouldBe(1);
            output.Segments[1].StartAddress.ShouldBe((ushort)0xC000);
            output.Segments[1].Bank.ShouldBe((ushort)1);
            output.Segments[1].Displacement.ShouldBeNull();
            output.Segments[1].EmittedCode.Count.ShouldBe(0);
        }

        [TestMethod]
        public void BankPragmaWorksWithExistingSegment2()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                .model Spectrum128
                .org #6400
                ld a,b
                .bank 1
                ld a,b
                call .
            ");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            output.Segments.Count.ShouldBe(2);
            output.Segments[0].StartAddress.ShouldBe((ushort)0x6400);
            output.Segments[0].Bank.ShouldBeNull();
            output.Segments[0].Displacement.ShouldBeNull();
            output.Segments[0].EmittedCode.Count.ShouldBe(1);
            output.Segments[1].StartAddress.ShouldBe((ushort)0xC000);
            output.Segments[1].Bank.ShouldBe((ushort)1);
            output.Segments[1].Displacement.ShouldBeNull();
            output.Segments[1].EmittedCode.Count.ShouldBe(4);
            output.Segments[1].EmittedCode[0].ShouldBe((byte)0x78);
            output.Segments[1].EmittedCode[1].ShouldBe((byte)0xCD);
            output.Segments[1].EmittedCode[2].ShouldBe((byte)0x01);
            output.Segments[1].EmittedCode[3].ShouldBe((byte)0xC0);
        }

        [TestMethod]
        public void BankPragmaWorksWithExistingSegment3()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                .model Spectrum128
                .org #6400
                ld a,b
                .bank 7
                .org #E000
                ld a,b
                call .
            ");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            output.Segments.Count.ShouldBe(2);
            output.Segments[0].StartAddress.ShouldBe((ushort)0x6400);
            output.Segments[0].Bank.ShouldBeNull();
            output.Segments[0].Displacement.ShouldBeNull();
            output.Segments[0].EmittedCode.Count.ShouldBe(1);
            output.Segments[1].StartAddress.ShouldBe((ushort)0xE000);
            output.Segments[1].Bank.ShouldBe((ushort)7);
            output.Segments[1].Displacement.ShouldBeNull();
            output.Segments[1].EmittedCode.Count.ShouldBe(4);
            output.Segments[1].EmittedCode[0].ShouldBe((byte)0x78);
            output.Segments[1].EmittedCode[1].ShouldBe((byte)0xCD);
            output.Segments[1].EmittedCode[2].ShouldBe((byte)0x01);
            output.Segments[1].EmittedCode[3].ShouldBe((byte)0xE0);
        }

        [TestMethod]
        public void BankPragmaWorksWithNewSegment1()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                .model Spectrum128
                .bank 3
            ");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            output.Segments.Count.ShouldBe(1);
            output.Segments[0].StartAddress.ShouldBe((ushort)0xC000);
            output.Segments[0].Displacement.ShouldBeNull();
            output.Segments[0].EmittedCode.Count.ShouldBe(0);
            output.Segments[0].Bank.ShouldBe((ushort)3);
        }

        [TestMethod]
        public void BankPragmaWorksWithNewSegment2()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                .model Spectrum128
                .bank 1
                ld a,b
                call .
            ");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            output.Segments.Count.ShouldBe(1);
            output.Segments[0].StartAddress.ShouldBe((ushort)0xC000);
            output.Segments[0].Bank.ShouldBe((ushort)1);
            output.Segments[0].Displacement.ShouldBeNull();
            output.Segments[0].EmittedCode.Count.ShouldBe(4);
            output.Segments[0].EmittedCode[0].ShouldBe((byte)0x78);
            output.Segments[0].EmittedCode[1].ShouldBe((byte)0xCD);
            output.Segments[0].EmittedCode[2].ShouldBe((byte)0x01);
            output.Segments[0].EmittedCode[3].ShouldBe((byte)0xC0);
        }

        [TestMethod]
        public void BankPragmaWorksWithNewSegment3()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                .model Spectrum128
                .bank 4
                .org #e000
                ld a,b
                call .
            ");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            output.Segments.Count.ShouldBe(1);
            output.Segments[0].StartAddress.ShouldBe((ushort)0xE000);
            output.Segments[0].Bank.ShouldBe((ushort)4);
            output.Segments[0].Displacement.ShouldBeNull();
            output.Segments[0].EmittedCode.Count.ShouldBe(4);
            output.Segments[0].EmittedCode[0].ShouldBe((byte)0x78);
            output.Segments[0].EmittedCode[1].ShouldBe((byte)0xCD);
            output.Segments[0].EmittedCode[2].ShouldBe((byte)0x01);
            output.Segments[0].EmittedCode[3].ShouldBe((byte)0xE0);
        }

        [TestMethod]
        public void MultipleBankPragmasWorks()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                .model Spectrum128
                .bank 1
                ld a,b
                call .
                .bank 3
                ld a,b
                call .
            ");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            output.Segments.Count.ShouldBe(2);
            output.Segments[0].StartAddress.ShouldBe((ushort)0xC000);
            output.Segments[0].Bank.ShouldBe((ushort)1);
            output.Segments[0].Displacement.ShouldBeNull();
            output.Segments[0].EmittedCode.Count.ShouldBe(4);
            output.Segments[0].EmittedCode[0].ShouldBe((byte)0x78);
            output.Segments[0].EmittedCode[1].ShouldBe((byte)0xCD);
            output.Segments[0].EmittedCode[2].ShouldBe((byte)0x01);
            output.Segments[0].EmittedCode[3].ShouldBe((byte)0xC0);
            output.Segments[1].StartAddress.ShouldBe((ushort)0xC000);
            output.Segments[1].Bank.ShouldBe((ushort)3);
            output.Segments[1].Displacement.ShouldBeNull();
            output.Segments[1].EmittedCode.Count.ShouldBe(4);
            output.Segments[1].EmittedCode[0].ShouldBe((byte)0x78);
            output.Segments[1].EmittedCode[1].ShouldBe((byte)0xCD);
            output.Segments[1].EmittedCode[2].ShouldBe((byte)0x01);
            output.Segments[1].EmittedCode[3].ShouldBe((byte)0xC0);
        }

        [TestMethod]
        public void BankWithInvalidModelFails()
        {
            CodeRaisesError(".bank 4", Errors.Z0452);
        }

        [TestMethod]
        public void BankWithLabelFails()
        {
            CodeRaisesError(@"
                .model Spectrum128
                myLabel .bank 4
            ", Errors.Z0451);
        }

        [TestMethod]
        public void BankWithInvalidValueFails1()
        {
            CodeRaisesError(@"
                .model SpectrumP3
                .bank -1
            ", Errors.Z0453);
        }

        [TestMethod]
        public void BankWithInvalidValueFails2()
        {
            CodeRaisesError(@"
                .model SpectrumP3
                .bank 8
            ", Errors.Z0453);
        }

        [TestMethod]
        public void BankWithInvalidValueFails3()
        {
            CodeRaisesError(@"
                .model SpectrumP3
                .bank 123
            ", Errors.Z0453);
        }

        [TestMethod]
        public void CannotResuseBank()
        {
            CodeRaisesError(@"
                .model SpectrumP3
                .bank 1
                .bank 3
                .bank 1
            ", Errors.Z0454);
        }

        [TestMethod]
        public void MaximumBankLengthWorks()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                .model Spectrum128
                .bank 4
                .defs 0x4000, 0x34
            ");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
        }

        [TestMethod]
        public void MaximumBankLengthOverflows()
        {
            CodeRaisesError(@"
                .model Spectrum128
                .bank 4
                .org #8000
                .defs 0x4000, 0x34
                .defb 0x00
            ", Errors.Z0309);
        }

        [TestMethod]
        public void OffsetedBankPragmaWorksWithExistingSegment1()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                .model Spectrum128
                .org #6400
                ld a,b
                .bank 1, #400
            ");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            output.Segments.Count.ShouldBe(2);
            output.Segments[0].StartAddress.ShouldBe((ushort)0x6400);
            output.Segments[0].Bank.ShouldBeNull();
            output.Segments[0].Displacement.ShouldBeNull();
            output.Segments[0].EmittedCode.Count.ShouldBe(1);
            output.Segments[1].StartAddress.ShouldBe((ushort)0xC400);
            output.Segments[1].Bank.ShouldBe((ushort)1);
            output.Segments[1].Displacement.ShouldBeNull();
            output.Segments[1].EmittedCode.Count.ShouldBe(0);
        }

        [TestMethod]
        public void OffsetedBankPragmaWorksWithExistingSegment2()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                .model Spectrum128
                .org #6400
                ld a,b
                .bank 1, #400
                ld a,b
                call .
            ");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            output.Segments.Count.ShouldBe(2);
            output.Segments[0].StartAddress.ShouldBe((ushort)0x6400);
            output.Segments[0].Bank.ShouldBeNull();
            output.Segments[0].Displacement.ShouldBeNull();
            output.Segments[0].EmittedCode.Count.ShouldBe(1);
            output.Segments[1].StartAddress.ShouldBe((ushort)0xC400);
            output.Segments[1].Bank.ShouldBe((ushort)1);
            output.Segments[1].Displacement.ShouldBeNull();
            output.Segments[1].EmittedCode.Count.ShouldBe(4);
            output.Segments[1].EmittedCode[0].ShouldBe((byte)0x78);
            output.Segments[1].EmittedCode[1].ShouldBe((byte)0xCD);
            output.Segments[1].EmittedCode[2].ShouldBe((byte)0x01);
            output.Segments[1].EmittedCode[3].ShouldBe((byte)0xC4);
        }

        [TestMethod]
        public void OffsetedBankPragmaWorksWithExistingSegment3()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                .model Spectrum128
                .org #6400
                ld a,b
                .bank 7, #400
                .org #E000
                ld a,b
                call .
            ");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            output.Segments.Count.ShouldBe(2);
            output.Segments[0].StartAddress.ShouldBe((ushort)0x6400);
            output.Segments[0].Bank.ShouldBeNull();
            output.Segments[0].Displacement.ShouldBeNull();
            output.Segments[0].EmittedCode.Count.ShouldBe(1);
            output.Segments[1].StartAddress.ShouldBe((ushort)0xE000);
            output.Segments[1].Bank.ShouldBe((ushort)7);
            output.Segments[1].Displacement.ShouldBeNull();
            output.Segments[1].EmittedCode.Count.ShouldBe(4);
            output.Segments[1].EmittedCode[0].ShouldBe((byte)0x78);
            output.Segments[1].EmittedCode[1].ShouldBe((byte)0xCD);
            output.Segments[1].EmittedCode[2].ShouldBe((byte)0x01);
            output.Segments[1].EmittedCode[3].ShouldBe((byte)0xE0);
        }

        [TestMethod]
        public void OffsetedBankPragmaWorksWithNewSegment1()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                .model Spectrum128
                .bank 3, #400
            ");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            output.Segments.Count.ShouldBe(1);
            output.Segments[0].StartAddress.ShouldBe((ushort)0xC400);
            output.Segments[0].Displacement.ShouldBeNull();
            output.Segments[0].EmittedCode.Count.ShouldBe(0);
            output.Segments[0].Bank.ShouldBe((ushort)3);
        }

        [TestMethod]
        public void OffsetedBankPragmaWorksWithNewSegment2()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                .model Spectrum128
                .bank 1, #400
                ld a,b
                call .
            ");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            output.Segments.Count.ShouldBe(1);
            output.Segments[0].StartAddress.ShouldBe((ushort)0xC400);
            output.Segments[0].Bank.ShouldBe((ushort)1);
            output.Segments[0].Displacement.ShouldBeNull();
            output.Segments[0].EmittedCode.Count.ShouldBe(4);
            output.Segments[0].EmittedCode[0].ShouldBe((byte)0x78);
            output.Segments[0].EmittedCode[1].ShouldBe((byte)0xCD);
            output.Segments[0].EmittedCode[2].ShouldBe((byte)0x01);
            output.Segments[0].EmittedCode[3].ShouldBe((byte)0xC4);
        }

        [TestMethod]
        public void OffsetedBankPragmaWorksWithNewSegment3()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                .model Spectrum128
                .bank 4, #400
                .org #e000
                ld a,b
                call .
            ");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            output.Segments.Count.ShouldBe(1);
            output.Segments[0].StartAddress.ShouldBe((ushort)0xE000);
            output.Segments[0].Bank.ShouldBe((ushort)4);
            output.Segments[0].Displacement.ShouldBeNull();
            output.Segments[0].EmittedCode.Count.ShouldBe(4);
            output.Segments[0].EmittedCode[0].ShouldBe((byte)0x78);
            output.Segments[0].EmittedCode[1].ShouldBe((byte)0xCD);
            output.Segments[0].EmittedCode[2].ShouldBe((byte)0x01);
            output.Segments[0].EmittedCode[3].ShouldBe((byte)0xE0);
        }

        [TestMethod]
        public void OffsetedMultipleBankPragmasWorks()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                .model Spectrum128
                .bank 1, #2000
                ld a,b
                call .
                .bank 3, #1000
                ld a,b
                call .
            ");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            output.Segments.Count.ShouldBe(2);
            output.Segments[0].StartAddress.ShouldBe((ushort)0xE000);
            output.Segments[0].Bank.ShouldBe((ushort)1);
            output.Segments[0].Displacement.ShouldBeNull();
            output.Segments[0].EmittedCode.Count.ShouldBe(4);
            output.Segments[0].EmittedCode[0].ShouldBe((byte)0x78);
            output.Segments[0].EmittedCode[1].ShouldBe((byte)0xCD);
            output.Segments[0].EmittedCode[2].ShouldBe((byte)0x01);
            output.Segments[0].EmittedCode[3].ShouldBe((byte)0xE0);
            output.Segments[1].StartAddress.ShouldBe((ushort)0xD000);
            output.Segments[1].Bank.ShouldBe((ushort)3);
            output.Segments[1].Displacement.ShouldBeNull();
            output.Segments[1].EmittedCode.Count.ShouldBe(4);
            output.Segments[1].EmittedCode[0].ShouldBe((byte)0x78);
            output.Segments[1].EmittedCode[1].ShouldBe((byte)0xCD);
            output.Segments[1].EmittedCode[2].ShouldBe((byte)0x01);
            output.Segments[1].EmittedCode[3].ShouldBe((byte)0xD0);
        }

        [TestMethod]
        public void OffsetedBankWithInvalidModelFails()
        {
            CodeRaisesError(".bank 4, #1000", Errors.Z0452);
        }

        [TestMethod]
        public void OffsetedBankWithLabelFails()
        {
            CodeRaisesError(@"
                .model Spectrum128
                myLabel .bank 4, #1000
            ", Errors.Z0451);
        }

        [TestMethod]
        public void OffsetedBankWithInvalidValueFails1()
        {
            CodeRaisesError(@"
                .model SpectrumP3
                .bank 4, #5678
            ", Errors.Z0455);
        }

        [TestMethod]
        public void OffsetedBankWithInvalidValueFails2()
        {
            CodeRaisesError(@"
                .model SpectrumP3
                .bank 5, -100
            ", Errors.Z0455);
        }

        [TestMethod]
        public void OffsetedBankWithInvalidValueFails3()
        {
            CodeRaisesError(@"
                .model SpectrumP3
                .bank 1, #4000
            ", Errors.Z0455);
        }

        [TestMethod]
        public void CannotResuseOffsetedBank()
        {
            CodeRaisesError(@"
                .model SpectrumP3
                .bank 1, #1000
                .bank 3, #1000
                .bank 1, #1000
            ", Errors.Z0454);
        }

        [TestMethod]
        public void MaximumOffsetedBankLengthWorks()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                .model Spectrum128
                .bank 4, #1000
                .defs 0x3000, 0x34
            ");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
        }

        [TestMethod]
        public void MaximumOffsetedBankLengthOverflows()
        {
            CodeRaisesError(@"
                .model Spectrum128
                .bank 4, #1000
                .org #8000
                .defs 0x3000, 0x34
                .defb 0x00
            ", Errors.Z0309);
        }
    }
}