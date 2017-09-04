using AntlrZ80Asm.Assembler;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;

namespace AntlrZ80Asm.Test.Assembler
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
            output.Segments[0].Displacement.ShouldBe(-100);
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
            output.Segments[0].Displacement.ShouldBe(0x1000);
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
            output.Segments[0].Displacement.ShouldBe(0x1000);
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
            output.Errors[0].ShouldBeOfType<PragmaError>();
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
            compiler.Symbols["MYSYMBOL"].ShouldBe((ushort)200);
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
            output.Errors[0].ShouldBeOfType<InvalidLabelError>();
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
            compiler.Fixups.Count.ShouldBe(1);
            var fixup = compiler.Fixups[0];
            fixup.Type.ShouldBe(FixupType.Equ);
            fixup.Expression.ShouldNotBeNull();
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
        public void DefbPragmaWorksWithFixup()
        {
            // --- Arrange
            var compiler = new Z80Assembler();
            var expected = new byte[] { 0x01, 0xAE, 122 };

            // --- Act
            var output = compiler.Compile(@"
                .defb #01, MySymbol, #AE, 122");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            output.Segments.Count.ShouldBe(1);
            var segment = output.Segments[0];
            segment.EmittedCode.Count.ShouldBe(expected.Length);
            for (var i = 0; i < expected.Length; i++)
            {
                segment.EmittedCode[i].ShouldBe(expected[i]);
            }
            compiler.Fixups.Count.ShouldBe(1);
            compiler.Fixups.Count.ShouldBe(1);
            var fixup = compiler.Fixups[0];
            fixup.Type.ShouldBe(FixupType.Bit8);
            fixup.Expression.ShouldNotBeNull();
        }
    }
}