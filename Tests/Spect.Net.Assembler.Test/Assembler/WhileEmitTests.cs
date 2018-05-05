using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.Assembler.Assembler;

namespace Spect.Net.Assembler.Test.Assembler
{
    [TestClass]
    public class WhileEmitTests : AssemblerTestBed
    {
        [TestMethod]
        public void EntPragmaCannotBeUsedInWhile()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                counter = 0
                .while counter < 3
                .ent #8000
                counter = counter + 1
                .wend
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0407);
        }

        [TestMethod]
        public void XentPragmaCannotBeUsedInWhile()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                counter = 0
                .while counter < 3
                .xent #8000
                counter = counter + 1
                .wend
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0407);
        }

        [TestMethod]
        [DataRow(".wend")]
        [DataRow("wend")]
        [DataRow(".WEND")]
        [DataRow("WEND")]
        [DataRow(".endw")]
        [DataRow("endw")]
        [DataRow(".ENDW")]
        [DataRow("ENDW")]
        public void EndwWithoutWhileFails(string source)
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
        public void WhileFailsWithString()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                .while ""Hello""
                .wend
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0305);
        }

        [TestMethod]
        public void WhileFailsWithTooLongLoop()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                .while true 
                .wend
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0409);
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
                    counter = 0;
                    .while counter < 100
                        Value: .var 100 + Other;
                        counter = counter + 1;
                    .wend
                ", options);

            // --- Assert
            output.ErrorCount.ShouldBe(4);
            output.Errors[3].ErrorCode.ShouldBe(Errors.Z0408);
        }

        [TestMethod]
        public void WhileWithEmptyBodyWorks()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                .while false
                .wend
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
        }

        [TestMethod]
        public void WhileExecutesZeroTimes()
        {
            const string SOURCE = @"
                .while false
                    inc a
                .wend
                ";

            // --- Assert
            CodeEmitWorks(SOURCE);
        }

        [TestMethod]
        public void LabeledWhileWithEmptyBodyWorks()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                counter = 1;
                MyLoop: .while counter <= 3
                    counter = counter + 1
                    .wend
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            output.Symbols.ContainsKey("MYLOOP").ShouldBeTrue();
            output.Symbols["MYLOOP"].Value.ShouldBe((ushort)0x8000);
        }

        [TestMethod]
        public void HangingLabeledWhileWithEmptyBodyWorks()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                counter = 1;
                MyLoop: 
                    .while counter <= 3
                    counter = counter + 1
                    .wend
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            output.Symbols.ContainsKey("MYLOOP").ShouldBeTrue();
            output.Symbols["MYLOOP"].Value.ShouldBe((ushort)0x8000);
        }

        [TestMethod]
        public void EndLabeledWhileWithEmptyBodyWorks()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                counter = 1;
                    .while counter <= 3
                        counter = counter + 1
                    MyEnd: .wend
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            output.Symbols.ContainsKey("MYEND").ShouldBeFalse();
        }

        [TestMethod]
        public void HangingEndLabeledWhileWithEmptyBodyWorks()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                counter = 1;
                    .while counter <= 3
                        counter = counter + 1
                MyEnd: 
                    .wend
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            output.Symbols.ContainsKey("MYEND").ShouldBeFalse();
        }

        [TestMethod]
        public void InvalidConditionIsCaught()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                .while 3+unknown
                .wend
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
                later: .equ 5
                count = 1
                .while count < later
                    count = count + 1
                .wend");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
        }

        [TestMethod]
        public void WhileEmitWithSingleLineAndNoInternalLabelWorks()
        {
            // --- Arrange
            const string SOURCE = @"
                counter = 0
                .while counter < 2
                    ld bc,#1234
                    counter = counter + 1
                .wend
                ";

            CodeEmitWorks(SOURCE, 0x01, 0x34, 0x12, 0x01, 0x34, 0x12);
        }

        [TestMethod]
        public void WhileEmitWithMultipleLineAndNoInternalLabelWorks()
        {
            // --- Arrange
            const string SOURCE = @"
                counter = 0
                .while counter < 2
                    inc b
                    inc c
                    inc d
                    counter = counter + 1
                .wend
                ";

            CodeEmitWorks(SOURCE, 0x04, 0x0C, 0x14, 0x04, 0x0C, 0x14);
        }

        [TestMethod]
        public void WhileWithInternalLabelWorks()
        {
            // --- Arrange
            const string SOURCE = @"
                counter = 0
                .while counter < 2
                    ThisLabel: ld bc,ThisLabel
                    counter = counter + 1
                .wend
                ";

            CodeEmitWorks(SOURCE, 0x01, 0x00, 0x80, 0x01, 0x03, 0x80);
        }

        [TestMethod]
        public void WhileWithInternalFixupLabelWorks()
        {
            // --- Arrange
            const string SOURCE = @"
                counter = 0
                .while counter < 2
                    ld bc,ThisLabel
                    ThisLabel: nop
                    counter = counter + 1
                .wend
                ";

            CodeEmitWorks(SOURCE, 0x01, 0x03, 0x80, 0x00, 0x01, 0x07, 0x80, 0x00);
        }

        [TestMethod]
        public void WhileWithStartLabelWorks()
        {
            // --- Arrange
            const string SOURCE = @"
                counter = 0
                StartLabel: .while counter < 2
                    ld bc,StartLabel
                    nop
                    counter = counter + 1
                .wend
                ";

            CodeEmitWorks(SOURCE, 0x01, 0x00, 0x80, 0x00, 0x01, 0x00, 0x80, 0x00);
        }

        [TestMethod]
        public void WhileWithEndLabelWorks()
        {
            // --- Arrange
            const string SOURCE = @"
                counter = 0
                .while counter < 2
                    ld bc,EndLabel
                    nop
                    counter = counter + 1
                EndLabel .wend
                ";

            CodeEmitWorks(SOURCE, 0x01, 0x04, 0x80, 0x00, 0x01, 0x08, 0x80, 0x00);
        }

        [TestMethod]
        public void WhileWithExternalFixupLabelWorks()
        {
            // --- Arrange
            const string SOURCE = @"
                counter = 0
                .while counter < 2
                    ld bc,OuterLabel
                    nop
                    counter = counter + 1
                .wend
            OuterLabel: nop
                ";

            CodeEmitWorks(SOURCE, 0x01, 0x08, 0x80, 0x00, 0x01, 0x08, 0x80, 0x00, 0x00);
        }

        [TestMethod]
        public void WhileNestedLoopEmitWithSingleLineAndNoInternalLabelWorks()
        {
            // --- Arrange
            const string SOURCE = @"
                counter = 0
                .while counter < 2
                    ld bc,#1234
                    .loop 3
                        inc a
                    .endl
                    counter = counter + 1
                .wend
                ";

            CodeEmitWorks(SOURCE, 0x01, 0x34, 0x12, 0x3C, 0x3C, 0x3C, 0x01, 0x34, 0x12, 0x3C, 0x3C, 0x3C);
        }

        [TestMethod]
        public void WhileNestedWhileEmitWithSingleLineAndNoInternalLabelWorks()
        {
            // --- Arrange
            const string SOURCE = @"
                counter = 0
                .while counter < 2
                    ld bc,#1234
                    counter1 = 0
                    .while counter1 < 3
                        inc a
                        counter1 = counter1 + 1
                    .wend
                    counter = counter + 1
                .wend
                ";

            CodeEmitWorks(SOURCE, 0x01, 0x34, 0x12, 0x3C, 0x3C, 0x3C, 0x01, 0x34, 0x12, 0x3C, 0x3C, 0x3C);
        }

        [TestMethod]
        public void WhileNestedLoopWithLabelsWorks()
        {
            // --- Arrange
            const string SOURCE = @"
                counter = 0
                .while counter < 1
                    inc a
                    .loop 2
                        ld hl,EndLabel
                        ld bc,NopLabel
                    EndLabel: .endl
                    NopLabel: nop
                    counter = counter + 1
                .wend
                ";

            CodeEmitWorks(SOURCE,
                0x3C, 0x21, 0x07, 0x80, 0x01, 0x0D, 0x80, 0x21, 0x0D, 0x80, 0x01, 0x0D, 0x80, 0x00);
        }

        [TestMethod]
        public void WhileNestedWhileWithLabelsWorks()
        {
            // --- Arrange
            const string SOURCE = @"
                counter = 0
                .while counter < 1
                    inc a
                    counter1 = 0
                    .while counter1 < 2
                        ld hl,EndLabel
                        ld bc,NopLabel
                        counter1 = counter1 + 1
                    EndLabel:.wend
                    NopLabel: nop
                    counter = counter + 1
                .wend
                ";

            CodeEmitWorks(SOURCE,
                0x3C, 0x21, 0x07, 0x80, 0x01, 0x0D, 0x80, 0x21, 0x0D, 0x80, 0x01, 0x0D, 0x80, 0x00);
        }

        [TestMethod]
        public void WhileCounterWorks()
        {
            // --- Arrange
            const string SOURCE = @"
                counter = 0;
                .while counter < 3
                    .db $cnt
                    counter = counter + 1                    
                .endw
                ";

            CodeEmitWorks(SOURCE, 0x01, 0x02, 0x03);
        }

        [TestMethod]
        public void NestedLoopCountersWork()
        {
            // --- Arrange
            const string SOURCE = @"
                counter = 0;
                .while counter < 3
                    .db $cnt
                    .loop 2
                        .db $cnt
                    .endl
                    counter = counter + 1                    
                .endw
                ";

            CodeEmitWorks(SOURCE, 0x01, 0x01, 0x02, 0x02, 0x01, 0x02, 0x03, 0x01, 0x02);
        }

    }
}
