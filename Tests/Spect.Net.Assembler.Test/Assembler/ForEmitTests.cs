using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.Assembler.Assembler;

namespace Spect.Net.Assembler.Test.Assembler
{
    [TestClass]
    public class ForEmitTests : AssemblerTestBed
    {
        [TestMethod]
        public void EntPragmaCannotBeUsedInForLoop()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                .for _i = 1 .to 3
                .ent #8000;
                .next
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0407);
        }

        [TestMethod]
        public void XentPragmaCannotBeUsedInForLoop()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                .for _i = 1 .to 3
                .xent #8000;
                .next
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0407);
        }

        [TestMethod]
        [DataRow(".next")]
        [DataRow("next")]
        [DataRow(".NEXT")]
        [DataRow("NEXT")]
        public void NextWithoutOpenTagFails(string source)
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
                .for _i = 1 to 3
                ld a,b
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0401);
        }

        [TestMethod]
        public void ForVariableInGlobalScopeCannotBeUsedAgain()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                _i = 0
                .for _i = 1 .to 3
                .next
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0414);
        }

        [TestMethod]
        public void ForVariableInLocalScopeCannotBeUsedAgain()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                _i = 0
                .for _i = 1 .to 3
                    .for _i = 1 to 2
                    .next
                .next
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0414);
        }

        [TestMethod]
        public void ForLoopFailsWithStringInFromValue()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                .for _i = ""hello"" .to 3
                .next
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0305);
        }

        [TestMethod]
        public void ForLoopFailsWithStringInToValue()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                .for _i = 3 .to ""hello""
                .next
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0305);
        }

        [TestMethod]
        public void ForLoopFailsWithStringInStepValue()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                .for _i = 3 .to 6 .step ""hello""
                .next
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0305);
        }

        [TestMethod]
        public void ForLoopWithEmptyBodyWorks()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                .for _i = 1 .to 3
                .next
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
        }

        [TestMethod]
        public void LabeledForLoopWithEmptyBodyWorks()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                MyLoop: .for _i = 1 to 3
                    .next
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            output.Symbols.ContainsKey("MYLOOP").ShouldBeTrue();
            output.Symbols["MYLOOP"].Value.ShouldBe((ushort)0x8000);
        }

        [TestMethod]
        public void HangingLabeledForLoopWithEmptyBodyWorks()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                MyLoop: 
                    .for _i = 1 to 3
                    .next
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            output.Symbols.ContainsKey("MYLOOP").ShouldBeTrue();
            output.Symbols["MYLOOP"].Value.ShouldBe((ushort)0x8000);
        }

        [TestMethod]
        public void EndLabeledForLoopWithEmptyBodyWorks()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                .for _i = 1 .to 3
                MyEnd: .next
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            output.Symbols.ContainsKey("MYEND").ShouldBeFalse();
        }

        [TestMethod]
        public void HangingEndLabeledForLoopWithEmptyBodyWorks()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                .for _i = 1 .to 3
                MyEnd: 
                    .next
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            output.Symbols.ContainsKey("MYEND").ShouldBeFalse();
        }

        [TestMethod]
        public void InvalidCounterInFromIsCaught()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                .for _i = 3+unknown to 5
                .next
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0201);
        }

        [TestMethod]
        public void InvalidCounterInToIsCaught()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                .for _i = 0 .to 3+unknown
                .next
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0201);
        }

        [TestMethod]
        public void InvalidCounterInStepIsCaught()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                .for _i = 0 .to 4 step 3+unknown
                .next
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0201);
        }

        [TestMethod]
        public void ZeroInStepIsCaught()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                .for _i = 0 .to 4 step 0
                .next
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0413);
        }

        [TestMethod]
        public void FloatZeroInStepIsCaught()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                .for _i = 0 .to 4 step 0.0
                .next
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0413);
        }

        [TestMethod]
        public void FixupCounterInFromIsCaught()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                    .for _i = 3+later to 4
                    .next
                later: .equ 5");

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0201);
        }

        [TestMethod]
        public void FixupCounterInToIsCaught()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                    .for _i = 1 to 3+later
                    .next
                later: .equ 5");

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0201);
        }

        [TestMethod]
        public void FixupCounterInStepIsCaught()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                    .for _i = 1 to 4 step  3+later
                    .next
                later: .equ 5");

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0201);
        }

        [TestMethod]
        public void ValidCounterInFromWorks()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                value: .equ 5
                    .for _i = 3+value .to 10
                    .next
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
        }

        [TestMethod]
        public void ValidCounterInToWorks()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                value: .equ 5
                    .for _i = 0 .to 3+value
                    .next
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
        }

        [TestMethod]
        public void ValidCounterInStepWorks()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                value: .equ 5
                    .for _i = 0 .to 10 step 3+value
                    .next
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
        }

        [TestMethod]
        public void TooLongCycleIsCaught()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                    .for _i = 0.0 to 100.0 step 0.00001
                    .next
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0409);
        }

        [TestMethod]
        public void ForLoopEmitWithSingleLineAndNoInternalLabelWorks()
        {
            // --- Arrange
            const string SOURCE = @"
                .for _i = 1 .to 2
                    ld bc,#1234
                .next
                ";

            CodeEmitWorks(SOURCE, 0x01, 0x34, 0x12, 0x01, 0x34, 0x12);
        }

        [TestMethod]
        public void ForLoopEmitWithMultipleLineAndNoInternalLabelWorks()
        {
            // --- Arrange
            const string SOURCE = @"
                .for _i = 1 .to 2
                    inc b
                    inc c
                    inc d
                .next
                ";

            CodeEmitWorks(SOURCE, 0x04, 0x0C, 0x14, 0x04, 0x0C, 0x14);
        }

        [TestMethod]
        public void BackwardForLoopEmitWithMultipleLineAndNoInternalLabelWorks()
        {
            // --- Arrange
            const string SOURCE = @"
                .for _i = 2 .to 1 step -1
                    inc b
                    inc c
                    inc d
                .next
                ";

            CodeEmitWorks(SOURCE, 0x04, 0x0C, 0x14, 0x04, 0x0C, 0x14);
        }

        [TestMethod]
        public void ForLoopWithInternalLabelWorks()
        {
            // --- Arrange
            const string SOURCE = @"
                .for _i = 1 .to 2
                    ThisLabel: ld bc,ThisLabel
                .next
                ";

            CodeEmitWorks(SOURCE, 0x01, 0x00, 0x80, 0x01, 0x03, 0x80);
        }

        [TestMethod]
        public void ForLoopWithInternalFixupLabelWorks()
        {
            // --- Arrange
            const string SOURCE = @"
                .for _i = 2 .to 1 step -1
                    ld bc,ThisLabel
                    ThisLabel: nop
                .next
                ";

            CodeEmitWorks(SOURCE, 0x01, 0x03, 0x80, 0x00, 0x01, 0x07, 0x80, 0x00);
        }

        [TestMethod]
        public void ForLoopWithStartLabelWorks()
        {
            // --- Arrange
            const string SOURCE = @"
                StartLabel: .for _i = 1 to 2
                    ld bc,StartLabel
                    nop
                .next
                ";

            CodeEmitWorks(SOURCE, 0x01, 0x00, 0x80, 0x00, 0x01, 0x00, 0x80, 0x00);
        }

        [TestMethod]
        public void ForLoopWithEndLabelWorks()
        {
            // --- Arrange
            const string SOURCE = @"
                .for _i = 1 .to 2
                    ld bc,EndLabel
                    nop
                EndLabel: .next
                ";

            CodeEmitWorks(SOURCE, 0x01, 0x04, 0x80, 0x00, 0x01, 0x08, 0x80, 0x00);
        }

        [TestMethod]
        public void ForLoopWithExternalFixupLabelWorks()
        {
            // --- Arrange
            const string SOURCE = @"
                .for _i = 2 to 1 step -1
                    ld bc,OuterLabel
                    nop
                .next
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
                    .for _i = 1 .to 100
                        Value: .var 100 + Other;
                    .next
                ", options);

            // --- Assert
            output.ErrorCount.ShouldBe(4);
            output.Errors[3].ErrorCode.ShouldBe(Errors.Z0408);
        }

        [TestMethod]
        public void NestedForLoopEmitWithSingleLineAndNoInternalLabelWorks()
        {
            // --- Arrange
            const string SOURCE = @"
                .for _i = 1 to 2
                    ld bc,#1234
                    .for _j = 1 to 3
                        inc a
                    .next
                .next
                ";

            CodeEmitWorks(SOURCE, 0x01, 0x34, 0x12, 0x3C, 0x3C, 0x3C, 0x01, 0x34, 0x12, 0x3C, 0x3C, 0x3C);
        }

        [TestMethod]
        public void NestedForLoopWithLabelsWorks()
        {
            // --- Arrange
            const string SOURCE = @"
                .for _i = 1 to 1
                    inc a
                    .for _j = 1 to 2
                        ld hl,EndLabel
                        ld bc,NopLabel
                    EndLabel: .next
                    NopLabel: nop
                .next
                ";

            CodeEmitWorks(SOURCE,
                0x3C, 0x21, 0x07, 0x80, 0x01, 0x0D, 0x80, 0x21, 0x0D, 0x80, 0x01, 0x0D, 0x80, 0x00);
        }

        [TestMethod]
        public void NestedForLoopWithLabelsWorks2()
        {
            // --- Arrange
            const string SOURCE = @"
                .for _i = 1 to 1
                    inc a
                    .for _j = 1 to 2
                        ld hl,EndLabel
                        ld bc,NopLabel
                    EndLabel: 
                        nop
                        .next
                    NopLabel: nop
                .next
                ";

            CodeEmitWorks(SOURCE,
                0x3C, 0x21, 0x07, 0x80, 0x01, 0x0F, 0x80, 0x00, 0x21, 0x0E, 0x80, 0x01, 0x0F, 0x80, 0x00, 0x00);
        }

        [TestMethod]
        public void ForLoopWithVarWorks()
        {
            // --- Arrange
            const string SOURCE = @"
                index = 1;
                .for _i = 1 to 2
                    ld a,index
                    index = index + 1
                    nop
                EndLabel: .next
                ";

            CodeEmitWorks(SOURCE, 0x3E, 0x01, 0x00, 0x3E, 0x02, 0x00);
        }

        [TestMethod]
        public void ForLoopWithNestedVarWorks()
        {
            // --- Arrange
            const string SOURCE = @"
                index = 1;
                .for _i = 1 to 2
                    index = 5
                    ld a,index
                    index := index + 1
                    nop
                EndLabel: .next
                ";

            CodeEmitWorks(SOURCE, 0x3E, 0x05, 0x00, 0x3E, 0x05, 0x00);
        }

        [TestMethod]
        public void NestedForLoopWithVarWorks()
        {
            // --- Arrange
            const string SOURCE = @"
                index = 1;
                .for _i = 1 to 2
                    ld a,index
                    .for _j = 1 to 3
                        index = index + 1
                    nop
                    .next
                EndLabel: .next
                ";

            CodeEmitWorks(SOURCE, 0x3E, 0x01, 0x00, 0x00, 0x00, 0x3E, 0x04, 0x00, 0x00, 0x00);
        }

        [TestMethod]
        public void ForLoopCounterWorks()
        {
            // --- Arrange
            const string SOURCE = @"
                .for _i = 6 to 8
                    .db $cnt
                .next
                ";

            CodeEmitWorks(SOURCE, 0x01, 0x02, 0x03);
        }

        [TestMethod]
        public void NestedForLoopCountersWork()
        {
            // --- Arrange
            const string SOURCE = @"
                .for _i = 6 to 8
                    .db $cnt
                    .for _j = 1 to 2
                        .db $cnt
                    .next
                .next
                ";

            CodeEmitWorks(SOURCE, 0x01, 0x01, 0x02, 0x02, 0x01, 0x02, 0x03, 0x01, 0x02);
        }

        [TestMethod]
        public void LoopVariableWorks()
        {
            // --- Arrange
            const string SOURCE = @"
                .for _i = 6 to 8
                    .db _i
                .next
                ";

            CodeEmitWorks(SOURCE, 0x06, 0x07, 0x08);
        }

        [TestMethod]
        public void NestedLoopVariablesWork()
        {
            // --- Arrange
            const string SOURCE = @"
                .for _i = 6 to 8
                  .for _j = 1 to 2
                    .db #10 * _i + _j
                  .next
                .next
                ";

            CodeEmitWorks(SOURCE, 0x61, 0x62, 0x71, 0x72, 0x81, 0x82);
        }


    }
}
