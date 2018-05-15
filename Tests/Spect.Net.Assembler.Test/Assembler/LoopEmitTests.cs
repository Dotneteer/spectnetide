using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.Assembler.Assembler;

namespace Spect.Net.Assembler.Test.Assembler
{
    [TestClass]
    public class LoopEmitTests : AssemblerTestBed
    {
        [TestMethod]
        public void EntPragmaCannotBeUsedInLoop()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                .loop 3
                .ent #8000;
                .endl
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0407);
        }

        [TestMethod]
        public void XentPragmaCannotBeUsedInLoop()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                .loop 3
                .xent #8000;
                .endl
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0407);
        }

        [TestMethod]
        [DataRow(".endl")]
        [DataRow("endl")]
        [DataRow(".ENDL")]
        [DataRow("ENDL")]
        [DataRow(".lend")]
        [DataRow("lend")]
        [DataRow(".LEND")]
        [DataRow("LEND")]
        public void EndLoopWithoutOpenTagFails(string source)
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
        public void MissingLoopEndIsDetected()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                .loop 3
                ld a,b
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0401);
        }

        [TestMethod]
        public void LoopFailsWithString()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                .loop ""hello""
                .endl
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0305);
        }

        [TestMethod]
        public void LoopWithEmptyBodyWorks()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                .loop 3
                .endl
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
        }

        [TestMethod]
        public void LabeledLoopWithEmptyBodyWorks()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                MyLoop: .loop 3
                    .endl
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            output.Symbols.ContainsKey("MYLOOP").ShouldBeTrue();
            output.Symbols["MYLOOP"].Value.ShouldBe((ushort) 0x8000);
        }

        [TestMethod]
        public void HangingLabeledLoopWithEmptyBodyWorks()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                MyLoop: 
                    .loop 3
                    .endl
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            output.Symbols.ContainsKey("MYLOOP").ShouldBeTrue();
            output.Symbols["MYLOOP"].Value.ShouldBe((ushort) 0x8000);
        }

        [TestMethod]
        public void EndLabeledLoopWithEmptyBodyWorks()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                .loop 3
                MyEnd: .endl
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            output.Symbols.ContainsKey("MYEND").ShouldBeFalse();
        }

        [TestMethod]
        public void HangingEndLabeledLoopWithEmptyBodyWorks()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                    .loop 3
                MyEnd: 
                    .endl
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            output.Symbols.ContainsKey("MYEND").ShouldBeFalse();
        }

        [TestMethod]
        public void InvalidCounterIsCaught()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                .loop 3+unknown
                .endl
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0201);
        }

        [TestMethod]
        public void FixupCounterIsCaught()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                    .loop 3+later
                    .endl
                later: .equ 5");

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0201);
        }

        [TestMethod]
        public void ValidCounterWorks()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                value: .equ 5
                    .loop 3+value
                    .endl
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
        }

        [TestMethod]
        public void TooGreatCounterIsCaught()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                    .loop #FFFF + 1
                    .endl
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0406);
        }

        [TestMethod]
        public void LoopEmitWithSingleLineAndNoInternalLabelWorks()
        {
            // --- Arrange
            const string SOURCE = @"
                .loop 2
                    ld bc,#1234
                .endl
                ";

            CodeEmitWorks(SOURCE, 0x01, 0x34, 0x12, 0x01, 0x34, 0x12);
        }

        [TestMethod]
        public void LoopEmitWithMultipleLineAndNoInternalLabelWorks()
        {
            // --- Arrange
            const string SOURCE = @"
                .loop 2
                    inc b
                    inc c
                    inc d
                .endl
                ";

            CodeEmitWorks(SOURCE, 0x04, 0x0C, 0x14, 0x04, 0x0C, 0x14);
        }

        [TestMethod]
        public void LoopWithInternalLabelWorks()
        {
            // --- Arrange
            const string SOURCE = @"
                .loop 2
                    ThisLabel: ld bc,ThisLabel
                .endl
                ";

            CodeEmitWorks(SOURCE, 0x01, 0x00, 0x80, 0x01, 0x03, 0x80);
        }

        [TestMethod]
        public void LoopWithInternalFixupLabelWorks()
        {
            // --- Arrange
            const string SOURCE = @"
                .loop 2
                    ld bc,ThisLabel
                    ThisLabel: nop
                .endl
                ";

            CodeEmitWorks(SOURCE, 0x01, 0x03, 0x80, 0x00, 0x01, 0x07, 0x80, 0x00);
        }

        [TestMethod]
        public void LoopWithStartLabelWorks()
        {
            // --- Arrange
            const string SOURCE = @"
                StartLabel: .loop 2
                    ld bc,StartLabel
                    nop
                .endl
                ";

            CodeEmitWorks(SOURCE, 0x01, 0x00, 0x80, 0x00, 0x01, 0x00, 0x80, 0x00);
        }

        [TestMethod]
        public void LoopWithEndLabelWorks()
        {
            // --- Arrange
            const string SOURCE = @"
                .loop 2
                    ld bc,EndLabel
                    nop
                EndLabel: .endl
                ";

            CodeEmitWorks(SOURCE, 0x01, 0x04, 0x80, 0x00, 0x01, 0x08, 0x80, 0x00);
        }

        [TestMethod]
        public void LoopWithExternalFixupLabelWorks()
        {
            // --- Arrange
            const string SOURCE = @"
                .loop 2
                    ld bc,OuterLabel
                    nop
                .endl
            OuterLabel: nop
                ";

            CodeEmitWorks(SOURCE, 0x01, 0x08, 0x80, 0x00, 0x01, 0x08, 0x80, 0x00, 0x00);
        }

        [TestMethod]
        public void TooManyErrorsStopProcessing()
        {
            // --- Arrange
            var options = new AssemblerOptions
            {
                MaxLoopErrorsToReport = 3
            };
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                    .loop 100
                        Value: .var 100 + Other;
                    .endl
                ", options);

            // --- Assert
            output.ErrorCount.ShouldBe(4);
            output.Errors[3].ErrorCode.ShouldBe(Errors.Z0408);
        }

        [TestMethod]
        public void NestedLoopEmitWithSingleLineAndNoInternalLabelWorks()
        {
            // --- Arrange
            const string SOURCE = @"
                .loop 2
                    ld bc,#1234
                    .loop 3
                        inc a
                    .endl
                .endl
                ";

            CodeEmitWorks(SOURCE, 0x01, 0x34, 0x12, 0x3C, 0x3C, 0x3C, 0x01, 0x34, 0x12, 0x3C, 0x3C, 0x3C);
        }

        [TestMethod]
        public void NestedLoopWithLabelsWorks()
        {
            // --- Arrange
            const string SOURCE = @"
                .loop 1
                    inc a
                    .loop 2
                        ld hl,EndLabel
                        ld bc,NopLabel
                    EndLabel: .endl
                    NopLabel: nop
                .endl
                ";

            CodeEmitWorks(SOURCE,
                0x3C, 0x21, 0x07, 0x80, 0x01, 0x0D, 0x80, 0x21, 0x0D, 0x80, 0x01, 0x0D, 0x80, 0x00);
        }

        [TestMethod]
        public void NestedLoopWithLabelsWorks2()
        {
            // --- Arrange
            const string SOURCE = @"
                .loop 1
                    inc a
                    .loop 2
                        ld hl,EndLabel
                        ld bc,NopLabel
                    EndLabel: 
                        nop
                        .endl
                    NopLabel: nop
                .endl
                ";

            CodeEmitWorks(SOURCE,
                0x3C, 0x21, 0x07, 0x80, 0x01, 0x0F, 0x80, 0x00, 0x21, 0x0E, 0x80, 0x01, 0x0F, 0x80, 0x00, 0x00);
        }

        [TestMethod]
        public void LoopWithVarWorks()
        {
            // --- Arrange
            const string SOURCE = @"
                index = 1;
                .loop 2
                    ld a,index
                    index = index + 1
                    nop
                EndLabel: .endl
                ";

            CodeEmitWorks(SOURCE, 0x3E, 0x01, 0x00, 0x3E, 0x02, 0x00);
        }

        [TestMethod]
        public void LoopWithNestedVarWorks()
        {
            // --- Arrange
            const string SOURCE = @"
                index = 1;
                .loop 2
                    index = 5
                    ld a,index
                    index := index + 1
                    nop
                EndLabel: .endl
                ";

            CodeEmitWorks(SOURCE, 0x3E, 0x05, 0x00, 0x3E, 0x05, 0x00);
        }

        [TestMethod]
        public void NestedLoopWithVarWorks()
        {
            // --- Arrange
            const string SOURCE = @"
                index = 1;
                .loop 2
                    ld a,index
                    .loop 3
                        index = index + 1
                    nop
                    .endl
                EndLabel: .endl
                ";

            CodeEmitWorks(SOURCE, 0x3E, 0x01, 0x00, 0x00, 0x00, 0x3E, 0x04, 0x00, 0x00, 0x00);
        }

        [TestMethod]
        public void LoopCounterWorks()
        {
            // --- Arrange
            const string SOURCE = @"
                .loop 3
                    .db $cnt
                .endl
                ";

            CodeEmitWorks(SOURCE, 0x01, 0x02, 0x03);
        }

        [TestMethod]
        public void LoopCounterFailsInGlobalScope()
        {
            // --- Arrange
            const string SOURCE = @"
                .db $cnt
                .loop 3
                    nop
                .endl
                ";

            CodeRaisesError(SOURCE, Errors.Z0412);
        }

        [TestMethod]
        public void NestedLoopCountersWork()
        {
            // --- Arrange
            const string SOURCE = @"
                .loop 3
                    .db $cnt
                    .loop 2
                        .db $cnt
                    .endl
                .endl
                ";

            CodeEmitWorks(SOURCE, 0x01, 0x01, 0x02, 0x02, 0x01, 0x02, 0x03, 0x01, 0x02);
        }
    }
}