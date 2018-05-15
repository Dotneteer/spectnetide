using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.Assembler.Assembler;
using Spect.Net.Assembler.SyntaxTree.Expressions;

namespace Spect.Net.Assembler.Test.Assembler
{
    [TestClass]
    public class FixupTests : AssemblerTestBed
    {
        [TestMethod]
        public void EquFixupWorkAsExpected()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                    ld a,b
                Symbol1 .equ Symbol2 + 1
                    ld b,c
                Symbol2 .equ 122
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            output.Segments.Count.ShouldBe(1);
            output.Symbols["SYMBOL1"].Value.ShouldBe((ushort)123);
            output.Symbols["SYMBOL2"].Value.ShouldBe((ushort)122);
        }

        [TestMethod]
        public void EquFixupRaisesErrorWhenCircularReference()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                    ld a,b
                Symbol1 .equ Symbol2 + 1
                    ld b,c
                Symbol2 .equ Symbol1 + 1 
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(2);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0201);
            output.Errors[1].ErrorCode.ShouldBe(Errors.Z0201);
        }

        [TestMethod]
        public void EquFixupRaisesErrorWhenEvaluationError()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                    ld a,b
                Symbol1 .equ Symbol2 + 1
                    ld b,c
                Symbol2 .equ 3/0 
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0200);
        }

        [TestMethod]
        public void Bit8FixupWorkAsExpected()
        {
            // --- Arrange
            var compiler = new Z80Assembler();
            var expected = new byte[] { 0x06, 0xF3 };

            // --- Act
            var output = compiler.Compile(@"
                Symbol1 .equ Symbol2 + 1
                    ld b,Symbol1
                Symbol2 .equ #F2
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            output.Segments.Count.ShouldBe(1);
            var segment = output.Segments[0];
            output.Symbols["SYMBOL1"].Value.ShouldBe((ushort)0xF3);
            output.Symbols["SYMBOL2"].Value.ShouldBe((ushort)0xF2);
            segment.EmittedCode.Count.ShouldBe(expected.Length);
            for (var i = 0; i < expected.Length; i++)
            {
                segment.EmittedCode[i].ShouldBe(expected[i]);
            }
        }

        [TestMethod]
        public void Bit8FixupRaisesErrorWhenCircularReference()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                Symbol1 .equ Symbol2 + 1
                    ld b,Symbol1
                Symbol2 .equ Symbol1 + 1
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(3);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0201);
            output.Errors[1].ErrorCode.ShouldBe(Errors.Z0201);
            output.Errors[2].ErrorCode.ShouldBe(Errors.Z0201);
        }

        [TestMethod]
        public void Bit8FixupRaisesErrorWhenEvaluationError()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                Symbol1 .equ Symbol2 + 1
                    ld b,Symbol1/0
                Symbol2 .equ #F2
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0200);
        }

        [TestMethod]
        public void Bit16FixupWorkAsExpected()
        {
            // --- Arrange
            var compiler = new Z80Assembler();
            var expected = new byte[] { 0x01, 0x04, 0x80 };

            // --- Act
            var output = compiler.Compile(@"
                Symbol1 .equ Symbol2 + 1
                    ld bc,Symbol1
                Symbol2 .equ $
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            output.Segments.Count.ShouldBe(1);
            var segment = output.Segments[0];
            output.Symbols["SYMBOL1"].Value.ShouldBe((ushort)0x8004);
            output.Symbols["SYMBOL2"].Value.ShouldBe((ushort)0x8003);
            segment.EmittedCode.Count.ShouldBe(expected.Length);
            for (var i = 0; i < expected.Length; i++)
            {
                segment.EmittedCode[i].ShouldBe(expected[i]);
            }
        }

        [TestMethod]
        public void Bit16FixupRaisesErrorWhenCircularReference()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                Symbol1 .equ Symbol2 + 1
                    ld bc,Symbol1
                Symbol2 .equ Symbol1 + 1
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(3);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0201);
            output.Errors[1].ErrorCode.ShouldBe(Errors.Z0201);
            output.Errors[2].ErrorCode.ShouldBe(Errors.Z0201);
        }

        [TestMethod]
        public void Bit16FixupRaisesErrorWhenEvaluationError()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                 Symbol1 .equ Symbol2 + 1
                    ld bc,Symbol1/0
                 Symbol2 .equ $
               ");

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0200);
        }

        [TestMethod]
        public void JrFixupWithJumpForwardWorkAsExpected()
        {
            // --- Arrange
            var compiler = new Z80Assembler();
            var expected = new byte[] { 0x20, 0x02, 0x06, 0x0A, 0x00 };

            // --- Act
            var output = compiler.Compile(@"
                    jr nz,ForwAddr
                    ld b,#A
                ForwAddr nop
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            output.Segments.Count.ShouldBe(1);
            var segment = output.Segments[0];
            output.Symbols["FORWADDR"].Value.ShouldBe((ushort)0x8004);
            segment.EmittedCode.Count.ShouldBe(expected.Length);
            for (var i = 0; i < expected.Length; i++)
            {
                segment.EmittedCode[i].ShouldBe(expected[i]);
            }
        }

        [TestMethod]
        public void JrFixupWithJumpBackWorkAsExpected()
        {
            // --- Arrange
            var compiler = new Z80Assembler();
            var expected = new byte[] { 0x00, 0x06, 0x0A, 0x20, 0xFB, 0x00 };

            // --- Act
            var output = compiler.Compile(@"
                BackAddr nop
                    ld b,#A
                    jr nz,BackAddr
                    nop
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            output.Segments.Count.ShouldBe(1);
            var segment = output.Segments[0];
            output.Symbols["BACKADDR"].Value.ShouldBe((ushort)0x8000);
            segment.EmittedCode.Count.ShouldBe(expected.Length);
            for (var i = 0; i < expected.Length; i++)
            {
                segment.EmittedCode[i].ShouldBe(expected[i]);
            }
        }

        [TestMethod]
        public void JrFixupWithJumpForwardFailsWithFarJump()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                    jr nz,ForwAddr
                    ld b,#A
                    .skip #8103, #00
                ForwAddr nop
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0022);
        }

        [TestMethod]
        public void JrFixupWithJumpBackFailsWithFarJump()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                BackAddr nop
                    .skip #8100
                    ld b,#A
                    jr nz,BackAddr
                    nop
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0022);
        }

        [TestMethod]
        public void StringEquWithImmediateEvaluationWorks()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                Symbol1 .equ ""hello""
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            output.Segments.Count.ShouldBe(1);
            output.Symbols["SYMBOL1"].Type.ShouldBe(ExpressionValueType.String);
            output.Symbols["SYMBOL1"].AsString().ShouldBe("hello");
        }

        [TestMethod]
        public void StringEquWithFixupWorks()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                Symbol1 .equ Symbol2 + ""you""
                Symbol2 .equ ""hello""
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            output.Segments.Count.ShouldBe(1);
            output.Symbols["SYMBOL1"].Type.ShouldBe(ExpressionValueType.String);
            output.Symbols["SYMBOL1"].AsString().ShouldBe("helloyou");
            output.Symbols["SYMBOL2"].Type.ShouldBe(ExpressionValueType.String);
            output.Symbols["SYMBOL2"].AsString().ShouldBe("hello");
        }

        [TestMethod]
        public void StringEquWith16BitExpectedRaisesAnError()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                Symbol1 .equ ""hello""
                        ld hl,Symbol1
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0305);
        }

        [TestMethod]
        public void StringEquWith16BitFixupRaisesAnError()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                Symbol1 .equ ""hello"" + Symbol2
                        ld hl,Symbol1
                Symbol2 .equ ""you""
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0305);
        }
    }
}