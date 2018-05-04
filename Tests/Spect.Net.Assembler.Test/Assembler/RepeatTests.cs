using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.Assembler.Assembler;

namespace Spect.Net.Assembler.Test.Assembler
{
    [TestClass]
    public class RepeatTests : AssemblerTestBed
    {
        [TestMethod]
        public void EntPragmaCannotBeUsedInRepeat()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                .repeat
                .ent #8000
                .until true
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
                .repeat
                .xent #8000
                .until true
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0407);
        }

        [TestMethod]
        [DataRow(".until true")]
        [DataRow("until true")]
        [DataRow(".UNTIL true")]
        [DataRow("UNTIL true")]
        public void UntilWithoutRepeatFails(string source)
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
        public void UntilFailsWithString()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                .repeat 
                .until ""Hello""
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0305);
        }

        [TestMethod]
        public void RepeatFailsWithTooLongLoop()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                .repeat 
                .until false
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
                    .repeat
                        Value: .var 100 + Other;
                        counter = counter + 1;
                    .until counter >= 100
                ", options);

            // --- Assert
            output.ErrorCount.ShouldBe(4);
            output.Errors[3].ErrorCode.ShouldBe(Errors.Z0408);
        }

        [TestMethod]
        public void RepeatWithEmptyBodyWorks()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                .repeat
                .until true
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
        }

        [TestMethod]
        public void RepeatExecutesOnce()
        {
            const string SOURCE = @"
                .repeat
                    inc a
                .until true
                ";

            // --- Assert
            CodeEmitWorks(SOURCE, 0x3C);
        }

        [TestMethod]
        public void LabeledRepeatWithEmptyBodyWorks()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                counter = 1;
                MyLoop: .repeat
                    counter = counter + 1
                    .until counter > 3
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            output.Symbols.ContainsKey("MYLOOP").ShouldBeTrue();
            output.Symbols["MYLOOP"].Value.ShouldBe((ushort)0x8000);
        }

        [TestMethod]
        public void HangingLabeledRepeatWithEmptyBodyWorks()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                counter = 1;
                MyLoop: 
                    .repeat
                        counter = counter + 1
                    .until counter > 3
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            output.Symbols.ContainsKey("MYLOOP").ShouldBeTrue();
            output.Symbols["MYLOOP"].Value.ShouldBe((ushort)0x8000);
        }

        [TestMethod]
        public void EndLabeledRepeatWithEmptyBodyWorks()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                counter = 1;
                    .repeat
                        counter = counter + 1
                    MyEnd: .until counter > 3
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            output.Symbols.ContainsKey("MYEND").ShouldBeFalse();
        }

        [TestMethod]
        public void HangingEndLabeledRepeatWithEmptyBodyWorks()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                counter = 1;
                    .repeat
                        counter = counter + 1
                    MyEnd: 
                        .until counter > 3
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
                .repeat
                .until 3+unknown
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
                count = 1
                .repeat
                    count = count + 1
                .until count > later
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
                later: .equ 5
                count = 1
                .repeat
                    count = count + 1
                .until count > later");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
        }

        [TestMethod]
        public void RepeatEmitWithSingleLineAndNoInternalLabelWorks()
        {
            // --- Arrange
            const string SOURCE = @"
                counter = 0
                .repeat
                    ld bc,#1234
                    counter = counter + 1
                .until counter == 2
                ";

            CodeEmitWorks(SOURCE, 0x01, 0x34, 0x12, 0x01, 0x34, 0x12);
        }

        [TestMethod]
        public void RepeatEmitWithMultipleLineAndNoInternalLabelWorks()
        {
            // --- Arrange
            const string SOURCE = @"
                counter = 0
                .repeat
                    inc b
                    inc c
                    inc d
                    counter = counter + 1
                .until counter == 2
                ";

            CodeEmitWorks(SOURCE, 0x04, 0x0C, 0x14, 0x04, 0x0C, 0x14);
        }

        [TestMethod]
        public void RepeatWithInternalLabelWorks()
        {
            // --- Arrange
            const string SOURCE = @"
                counter = 0
                .repeat
                    ThisLabel: ld bc,ThisLabel
                    counter = counter + 1
                .until counter == 2
                ";

            CodeEmitWorks(SOURCE, 0x01, 0x00, 0x80, 0x01, 0x03, 0x80);
        }

        [TestMethod]
        public void RepeatWithInternalFixupLabelWorks()
        {
            // --- Arrange
            const string SOURCE = @"
                counter = 0
                .repeat
                    ld bc,ThisLabel
                    ThisLabel: nop
                    counter = counter + 1
                .until counter == 2
                ";

            CodeEmitWorks(SOURCE, 0x01, 0x03, 0x80, 0x00, 0x01, 0x07, 0x80, 0x00);
        }

        [TestMethod]
        public void RepeatWithStartLabelWorks()
        {
            // --- Arrange
            const string SOURCE = @"
                counter = 0
                StartLabel: .repeat
                    ld bc,StartLabel
                    nop
                    counter = counter + 1
                .until counter == 2
                ";

            CodeEmitWorks(SOURCE, 0x01, 0x00, 0x80, 0x00, 0x01, 0x00, 0x80, 0x00);
        }

        [TestMethod]
        public void RepeatWithEndLabelWorks()
        {
            // --- Arrange
            const string SOURCE = @"
                counter = 0
                .repeat
                    ld bc,EndLabel
                    nop
                    counter = counter + 1
                EndLabel .until counter == 2
                ";

            CodeEmitWorks(SOURCE, 0x01, 0x04, 0x80, 0x00, 0x01, 0x08, 0x80, 0x00);
        }

        [TestMethod]
        public void RepeatWithExternalFixupLabelWorks()
        {
            // --- Arrange
            const string SOURCE = @"
                counter = 0
                .repeat
                    ld bc,OuterLabel
                    nop
                    counter = counter + 1
                .until counter == 2
            OuterLabel: nop
                ";

            CodeEmitWorks(SOURCE, 0x01, 0x08, 0x80, 0x00, 0x01, 0x08, 0x80, 0x00, 0x00);
        }

        [TestMethod]
        public void RepeatNestedLoopEmitWithSingleLineAndNoInternalLabelWorks()
        {
            // --- Arrange
            const string SOURCE = @"
                counter = 0
                .repeat
                    ld bc,#1234
                    .loop 3
                        inc a
                    .endl
                    counter = counter + 1
                .until counter == 2
                ";

            CodeEmitWorks(SOURCE, 0x01, 0x34, 0x12, 0x3C, 0x3C, 0x3C, 0x01, 0x34, 0x12, 0x3C, 0x3C, 0x3C);
        }

        [TestMethod]
        public void RepeatNestedRepeatEmitWithSingleLineAndNoInternalLabelWorks()
        {
            // --- Arrange
            const string SOURCE = @"
                counter = 0
                .repeat
                    ld bc,#1234
                    counter1 = 0
                    .repeat
                        inc a
                        counter1 = counter1 + 1
                    .until counter1 == 3
                    counter = counter + 1
                .until counter == 2
                ";

            CodeEmitWorks(SOURCE, 0x01, 0x34, 0x12, 0x3C, 0x3C, 0x3C, 0x01, 0x34, 0x12, 0x3C, 0x3C, 0x3C);
        }

        [TestMethod]
        public void RepeatNestedLoopWithLabelsWorks()
        {
            // --- Arrange
            const string SOURCE = @"
                counter = 0
                .repeat
                    inc a
                    .loop 2
                        ld hl,EndLabel
                        ld bc,NopLabel
                    EndLabel: .endl
                    NopLabel: nop
                    counter = counter + 1
                .until counter == 1
                ";

            CodeEmitWorks(SOURCE,
                0x3C, 0x21, 0x07, 0x80, 0x01, 0x0D, 0x80, 0x21, 0x0D, 0x80, 0x01, 0x0D, 0x80, 0x00);
        }

        [TestMethod]
        public void RepeatNestedRepeatWithLabelsWorks()
        {
            // --- Arrange
            const string SOURCE = @"
                counter = 0
                .repeat
                    inc a
                    counter1 = 0
                    .repeat
                        ld hl,EndLabel
                        ld bc,NopLabel
                        counter1 = counter1 + 1
                    EndLabel:.until counter1 == 2
                    NopLabel: nop
                    counter = counter + 1
                .until counter == 1
                ";

            CodeEmitWorks(SOURCE,
                0x3C, 0x21, 0x07, 0x80, 0x01, 0x0D, 0x80, 0x21, 0x0D, 0x80, 0x01, 0x0D, 0x80, 0x00);
        }
    }
}
