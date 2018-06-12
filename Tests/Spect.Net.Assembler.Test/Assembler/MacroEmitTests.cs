using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.Assembler.Assembler;

namespace Spect.Net.Assembler.Test.Assembler
{
    [TestClass]
    public class MacroEmitTests : AssemblerTestBed
    {
        [TestMethod]
        public void MacroWithNoLabelFails()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                .macro()
                .endm
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0400);
        }

        [TestMethod]
        public void MacroWithLabelWorks()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                MyMacro: .macro()
                    .endm
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            output.Macros.ContainsKey("MyMacro").ShouldBeTrue();
            var def = output.Macros["MyMacro"];
            def.ShouldNotBeNull();
            def.Section.FirstLine.ShouldBe(0);
            def.Section.LastLine.ShouldBe(1);
            def.MacroName.ShouldBe("MYMACRO");
        }

        [TestMethod]
        public void MacroWithHangingLabelWorks()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                MyMacro: 
                    .macro()
                    .endm
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            output.Macros.ContainsKey("MyMacro").ShouldBeTrue();
            var def = output.Macros["MyMacro"];
            def.ShouldNotBeNull();
            def.Section.FirstLine.ShouldBe(1);
            def.Section.LastLine.ShouldBe(2);
            def.MacroName.ShouldBe("MYMACRO");
        }

        [TestMethod]
        public void MacroWithArgumentsWorks()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                MyMacro: .macro(first, second)
                    .endm
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            output.Macros.ContainsKey("MyMacro").ShouldBeTrue();
            var def = output.Macros["MyMacro"];
            def.ShouldNotBeNull();
            def.Section.FirstLine.ShouldBe(0);
            def.Section.LastLine.ShouldBe(1);
            def.MacroName.ShouldBe("MYMACRO");
        }

        [TestMethod]
        public void MacroDefWithinMacroDefIsNotAllowed()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                MyMacro: .macro(first, second)
                Nested:  .macro()
                    .endm
                    .endm
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0404);
        }

        [TestMethod]
        public void MacroDefWithValidParameterNamesWorks()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                MyMacro: .macro(first, second)
                    {{first}}
                    ld a,{{second}}
                    .endm
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
        }

        [TestMethod]
        public void MacroDefWithInvalidParameterNamesFails()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                MyMacro: .macro(first, second)
                    {{first}}
                    ld b,{{unknown}}
                    ld {{other}},{{second}}
                    {{what}}
                    .endm
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(3);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0403);
            output.Errors[1].ErrorCode.ShouldBe(Errors.Z0403);
            output.Errors[2].ErrorCode.ShouldBe(Errors.Z0403);
        }

        [TestMethod]
        public void MacroWithNoEndFails()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                MyMacro: .macro(first, second)
                    ld a,b
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0401);
        }

        [TestMethod]
        [DataRow(".endm")]
        [DataRow("endm")]
        [DataRow(".ENDM")]
        [DataRow("ENDM")]
        [DataRow(".mend")]
        [DataRow("mend")]
        [DataRow(".MEND")]
        [DataRow("MEND")]
        public void EndMacroWithoutOpenTagFails(string source)
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
        public void MacroDefWithDuplicatedArgumentFails()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                MyMacro: .macro(first, second, first)
                    .endm
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0417);
        }

        [TestMethod]
        public void MacroInvocationWithUnknownNameFails()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                MyMacro()
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0418);
        }

        [TestMethod]
        public void MacroInvocationWithKnownNameWorks()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                MyMacro: .macro()
                    nop
                .endm
                MyMacro()
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
        }

        [TestMethod]
        public void MacroInvocationWithKnownNameAndArgumentsWorks()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                MyMacro: .macro(first, second)
                    nop
                .endm
                MyMacro(1, 2)
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
        }

        [TestMethod]
        public void MacroInvocationWithLessArgumentWorks()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                MyMacro: .macro(first)
                    nop
                .endm
                MyMacro()
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
        }

        [TestMethod]
        public void MacroInvocationWithMoreArgumentsFails()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                MyMacro: .macro(first)
                    nop
                .endm
                MyMacro(12, 13)
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0419);
        }

        [TestMethod]
        public void MacroArgEvaluationWithSingleArgumentWorks()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                MyMacro: .macro(first)
                    nop
                .endm
                value .equ 123
                MyMacro(value)
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
        }

        [TestMethod]
        public void MacroArgEvaluationWithSingleArgumentFails()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                MyMacro: .macro(first)
                    nop
                .endm
                MyMacro(value)
                value .equ 123
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0201);
        }

        [TestMethod]
        public void MacroArgEvaluationWithDivByZeroFails()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                MyMacro: .macro(first)
                    nop
                .endm
                value .equ 0
                MyMacro(1/value)
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0200);
        }

        [TestMethod]
        public void MacroArgEvaluationWithMultipleArgumentWorks()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                MyMacro: .macro(first, second)
                    nop
                .endm
                value .equ 123
                MyMacro(value, value + 2)
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
        }

        [TestMethod]
        public void MacroArgEvaluationWithMultipleArgumentFails()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                MyMacro: .macro(first, second)
                    nop
                .endm
                MyMacro(value, value + 2)
                value .equ 123
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(2);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0201);
            output.Errors[1].ErrorCode.ShouldBe(Errors.Z0201);
        }

        [TestMethod]
        public void SimpleMacroEmitWorks()
        {
            const string SOURCE = @"
                Simple: .macro()
                    nop
                    ld a,b
                    nop
                .endm
                Simple()";

            CodeEmitWorks(SOURCE, 0x00, 0x78, 0x00);
        }

        [TestMethod]
        public void MacroWithInternalLabelWorks()
        {
            // --- Arrange
            const string SOURCE = @"
                Simple: .macro()
                    ThisLabel: ld bc,ThisLabel
                .endm
                Simple()";

            CodeEmitWorks(SOURCE, 0x01, 0x00, 0x80);
        }

        [TestMethod]
        public void MacroWithInternalLabelInvokedMultipleTimesWorks()
        {
            // --- Arrange
            const string SOURCE = @"
                Simple: .macro()
                    ThisLabel: ld bc,ThisLabel
                .endm
                Simple()
                Simple()";

            CodeEmitWorks(SOURCE, 0x01, 0x00, 0x80, 0x01, 0x03, 0x80);
        }

        [TestMethod]
        public void MacroWithInternalFixupLabelWorks()
        {
            // --- Arrange
            const string SOURCE = @"
                Simple: .macro()
                    ld bc,ThisLabel
                    ThisLabel: nop
                .endm
                Simple()
                Simple()";

            CodeEmitWorks(SOURCE, 0x01, 0x03, 0x80, 0x00, 0x01, 0x07, 0x80, 0x00);
        }

        [TestMethod]
        public void LoopWithStartLabelWorks()
        {
            // --- Arrange
            const string SOURCE = @"
                Simple: .macro()
                    ld bc,Simple
                    nop
                .endm
                Simple()
                Simple()";

            CodeEmitWorks(SOURCE, 0x01, 0x00, 0x80, 0x00, 0x01, 0x04, 0x80, 0x00);
        }

        [TestMethod]
        public void MacroWithEndLabelWorks()
        {
            // --- Arrange
            const string SOURCE = @"
                Simple: .macro()
                    ld bc,EndLabel
                    nop
                EndLabel: .endm
                Simple()
                Simple()";

            CodeEmitWorks(SOURCE, 0x01, 0x04, 0x80, 0x00, 0x01, 0x08, 0x80, 0x00);
        }

        [TestMethod]
        public void MacroWithExternalFixupLabelWorks1()
        {
            // --- Arrange
            const string SOURCE = @"
                Simple: .macro()
                    ld bc,OuterLabel
                    nop
                .endm
                Simple()
                Simple()
                OuterLabel: nop";

            CodeEmitWorks(SOURCE, 0x01, 0x08, 0x80, 0x00, 0x01, 0x08, 0x80, 0x00, 0x00);
        }

        [TestMethod]
        public void MacroWithExternalFixupLabelWorks2()
        {
            // --- Arrange
            const string SOURCE = @"
                Simple: .macro()
                    ld bc,OuterLabel
                    nop
                .endm
                OuterLabel: nop
                Simple()
                Simple()";

            CodeEmitWorks(SOURCE, 0x00, 0x01, 0x00, 0x80, 0x00, 0x01, 0x00, 0x80, 0x00);
        }

        [TestMethod]
        public void MacroNestedLoopEmitWithSingleLineAndNoInternalLabelWorks()
        {
            // --- Arrange
            const string SOURCE = @"
                Simple: .macro()
                    ld bc,#1234
                    .loop 3
                        inc a
                    .endl
                .endm
                Simple()
                Simple()";

            CodeEmitWorks(SOURCE, 0x01, 0x34, 0x12, 0x3C, 0x3C, 0x3C, 0x01, 0x34, 0x12, 0x3C, 0x3C, 0x3C);
        }

        [TestMethod]
        public void MacroNestedLoopWithLabelsWorks()
        {
            // --- Arrange
            const string SOURCE = @"
                Simple: .macro()
                    inc a
                    .loop 2
                        ld hl,EndLabel
                        ld bc,NopLabel
                    EndLabel: .endl
                    NopLabel: nop
                .endm
                Simple()";

            CodeEmitWorks(SOURCE,
                0x3C, 0x21, 0x07, 0x80, 0x01, 0x0D, 0x80, 0x21, 0x0D, 0x80, 0x01, 0x0D, 0x80, 0x00);
        }

        [TestMethod]
        public void MacroNestedLoopWithLabelsWorks2()
        {
            // --- Arrange
            const string SOURCE = @"
                Simple: .macro()
                    inc a
                    .loop 2
                        ld hl,EndLabel
                        ld bc,NopLabel
                    EndLabel: 
                        nop
                    .endl
                    NopLabel: nop
                .endm
                Simple()";

            CodeEmitWorks(SOURCE,
                0x3C, 0x21, 0x07, 0x80, 0x01, 0x0F, 0x80, 0x00, 0x21, 0x0E, 0x80, 0x01, 0x0F, 0x80, 0x00, 0x00);
        }

        [TestMethod]
        public void MacroLoopWithVarWorks()
        {
            // --- Arrange
            const string SOURCE = @"
                Simple: .macro()
                    index = 1;
                    .loop 2
                        ld a,index
                        index = index + 1
                        nop
                    .endl
                .endm
                Simple()";

            CodeEmitWorks(SOURCE, 0x3E, 0x01, 0x00, 0x3E, 0x02, 0x00);
        }

        [TestMethod]
        public void MacroLoopWithNestedVarWorks()
        {
            // --- Arrange
            const string SOURCE = @"
                Simple: .macro()
                    index = 1;
                    .loop 2
                        index = 5
                        ld a,index
                        index := index + 1
                        nop
                    EndLabel: .endl
                .endm
                Simple()";

            CodeEmitWorks(SOURCE, 0x3E, 0x05, 0x00, 0x3E, 0x05, 0x00);
        }

        [TestMethod]
        public void LoopCounterWorksInMacro()
        {
            // --- Arrange
            const string SOURCE = @"
                Simple: .macro()
                    .loop 3
                        .db $cnt
                    .endl
                .endm
                Simple()
                Simple()";

            CodeEmitWorks(SOURCE, 0x01, 0x02, 0x03, 0x01, 0x02, 0x03);
        }

        [TestMethod]
        public void MacroRepeatEmitWithSingleLineAndNoInternalLabelWorks()
        {
            // --- Arrange
            const string SOURCE = @"
                Simple: .macro()
                    counter = 0
                    .repeat
                        ld bc,#1234
                        counter = counter + 1
                    .until counter == 2
                .endm
                Simple()";

            CodeEmitWorks(SOURCE, 0x01, 0x34, 0x12, 0x01, 0x34, 0x12);
        }

        [TestMethod]
        public void MacroRepeatEmitWithMultipleLineAndNoInternalLabelWorks()
        {
            // --- Arrange
            const string SOURCE = @"
                Simple: .macro()
                    counter = 0
                    .repeat
                        inc b
                        inc c
                        inc d
                        counter = counter + 1
                    .until counter == 2
                .endm
                Simple()";

            CodeEmitWorks(SOURCE, 0x04, 0x0C, 0x14, 0x04, 0x0C, 0x14);
        }

        [TestMethod]
        public void MacroRepeatEmitWithMultipleLineAndNoInternalLabelWorks2()
        {
            // --- Arrange
            const string SOURCE = @"
                Simple: .macro()
                    counter = 3
                    .repeat
                        inc b
                        inc c
                        inc d
                        counter = counter + 1
                    .until counter == 5
                .endm
                Simple()";

            CodeEmitWorks(SOURCE, 0x04, 0x0C, 0x14, 0x04, 0x0C, 0x14);
        }

        [TestMethod]
        public void MacroRepeatWithInternalLabelWorks()
        {
            // --- Arrange
            const string SOURCE = @"
                Simple: .macro()
                    counter = 0
                    .repeat
                        ThisLabel: ld bc,ThisLabel
                        counter = counter + 1
                    .until counter == 2
                .endm
                Simple()";

            CodeEmitWorks(SOURCE, 0x01, 0x00, 0x80, 0x01, 0x03, 0x80);
        }

        [TestMethod]
        public void MacroRepeatWithInternalFixupLabelWorks()
        {
            // --- Arrange
            const string SOURCE = @"
                Simple: .macro()
                    counter = 0
                    .repeat
                        ld bc,ThisLabel
                        ThisLabel: nop
                        counter = counter + 1
                    .until counter == 2
                .endm
                Simple()";

            CodeEmitWorks(SOURCE, 0x01, 0x03, 0x80, 0x00, 0x01, 0x07, 0x80, 0x00);
        }

        [TestMethod]
        public void MacroRepeatWithStartLabelWorks()
        {
            // --- Arrange
            const string SOURCE = @"
                Simple: .macro()
                    counter = 0
                    StartLabel: .repeat
                        ld bc,StartLabel
                        nop
                        counter = counter + 1
                    .until counter == 2
                .endm
                Simple()";

            CodeEmitWorks(SOURCE, 0x01, 0x00, 0x80, 0x00, 0x01, 0x00, 0x80, 0x00);
        }

        [TestMethod]
        public void MacroRepeatWithEndLabelWorks()
        {
            // --- Arrange
            const string SOURCE = @"
                Simple: .macro()
                    counter = 0
                    .repeat
                        ld bc,EndLabel
                        nop
                        counter = counter + 1
                    EndLabel .until counter == 2
                .endm
                Simple()";

            CodeEmitWorks(SOURCE, 0x01, 0x04, 0x80, 0x00, 0x01, 0x08, 0x80, 0x00);
        }

        [TestMethod]
        public void MacroRepeatWithExternalFixupLabelWorks1()
        {
            // --- Arrange
            const string SOURCE = @"
                Simple: .macro()
                    counter = 0
                    .repeat
                        ld bc,OuterLabel
                        nop
                        counter = counter + 1
                    .until counter == 2
                .endm
                OuterLabel: nop
                Simple()";

            CodeEmitWorks(SOURCE, 0x00, 0x01, 0x00, 0x80, 0x00, 0x01, 0x00, 0x80, 0x00);
        }

        [TestMethod]
        public void MacroRepeatWithExternalFixupLabelWorks2()
        {
            // --- Arrange
            const string SOURCE = @"
                Simple: .macro()
                    counter = 0
                    .repeat
                        ld bc,OuterLabel
                        nop
                        counter = counter + 1
                    .until counter == 2
                .endm
                Simple()
                OuterLabel: nop";

            CodeEmitWorks(SOURCE, 0x01, 0x08, 0x80, 0x00, 0x01, 0x08, 0x80, 0x00, 0x00);
        }

        [TestMethod]
        public void MacroRepeatNestedLoopEmitWithSingleLineAndNoInternalLabelWorks()
        {
            // --- Arrange
            const string SOURCE = @"
                Simple: .macro()
                    counter = 0
                    .repeat
                        ld bc,#1234
                        .loop 3
                            inc a
                        .endl
                        counter = counter + 1
                    .until counter == 2
                .endm
                Simple()";

            CodeEmitWorks(SOURCE, 0x01, 0x34, 0x12, 0x3C, 0x3C, 0x3C, 0x01, 0x34, 0x12, 0x3C, 0x3C, 0x3C);
        }

        [TestMethod]
        public void MacroWithRepeatCounterWorks()
        {
            // --- Arrange
            const string SOURCE = @"
                Simple: .macro()
                    counter = 0
                    .repeat
                        .db $cnt
                        counter = counter + 1
                    .until counter == 3
                .endm
                Simple()
                Simple()";

            CodeEmitWorks(SOURCE, 0x01, 0x02, 0x03, 0x01, 0x02, 0x03);
        }

        [TestMethod]
        public void MacroWhileEmitWithSingleLineAndNoInternalLabelWorks()
        {
            // --- Arrange
            const string SOURCE = @"
                Simple: .macro()
                    counter = 0
                    .while counter < 2
                        ld bc,#1234
                        counter = counter + 1
                    .wend
                .endm
                Simple()";

            CodeEmitWorks(SOURCE, 0x01, 0x34, 0x12, 0x01, 0x34, 0x12);
        }

        [TestMethod]
        public void MacroWhileEmitWithMultipleLineAndNoInternalLabelWorks()
        {
            // --- Arrange
            const string SOURCE = @"
                Simple: .macro()
                    counter = 0
                    .while counter < 2
                        inc b
                        inc c
                        inc d
                        counter = counter + 1
                    .wend
                .endm
                Simple()";

            CodeEmitWorks(SOURCE, 0x04, 0x0C, 0x14, 0x04, 0x0C, 0x14);
        }

        [TestMethod]
        public void MacroWhileWithInternalLabelWorks()
        {
            // --- Arrange
            const string SOURCE = @"
                Simple: .macro()
                    counter = 0
                    .while counter < 2
                        ThisLabel: ld bc,ThisLabel
                        counter = counter + 1
                    .wend
                .endm
                Simple()";

            CodeEmitWorks(SOURCE, 0x01, 0x00, 0x80, 0x01, 0x03, 0x80);
        }

        [TestMethod]
        public void MacroWhileWithInternalFixupLabelWorks()
        {
            // --- Arrange
            const string SOURCE = @"
                Simple: .macro()
                    counter = 0
                    .while counter < 2
                        ld bc,ThisLabel
                        ThisLabel: nop
                        counter = counter + 1
                    .wend
                .endm
                Simple()";

            CodeEmitWorks(SOURCE, 0x01, 0x03, 0x80, 0x00, 0x01, 0x07, 0x80, 0x00);
        }

        [TestMethod]
        public void MacroWhileWithStartLabelWorks()
        {
            // --- Arrange
            const string SOURCE = @"
                Simple: .macro()
                    counter = 0
                    StartLabel: .while counter < 2
                        ld bc,StartLabel
                        nop
                        counter = counter + 1
                    .wend
                .endm
                Simple()";

            CodeEmitWorks(SOURCE, 0x01, 0x00, 0x80, 0x00, 0x01, 0x00, 0x80, 0x00);
        }

        [TestMethod]
        public void MacroWhileWithEndLabelWorks()
        {
            // --- Arrange
            const string SOURCE = @"
                Simple: .macro()
                    counter = 0
                    .while counter < 2
                        ld bc,EndLabel
                        nop
                        counter = counter + 1
                    EndLabel .wend
                .endm
                Simple()";

            CodeEmitWorks(SOURCE, 0x01, 0x04, 0x80, 0x00, 0x01, 0x08, 0x80, 0x00);
        }

        [TestMethod]
        public void MacroWhileWithExternalFixupLabelWorks1()
        {
            // --- Arrange
            const string SOURCE = @"
                Simple: .macro()
                    counter = 0
                    .while counter < 2
                        ld bc,OuterLabel
                        nop
                        counter = counter + 1
                    .wend
                .endm
                Simple()
                OuterLabel: nop";

            CodeEmitWorks(SOURCE, 0x01, 0x08, 0x80, 0x00, 0x01, 0x08, 0x80, 0x00, 0x00);
        }

        [TestMethod]
        public void MacroWhileWithExternalFixupLabelWorks2()
        {
            // --- Arrange
            const string SOURCE = @"
                Simple: .macro()
                    counter = 0
                    .while counter < 2
                        ld bc,OuterLabel
                        nop
                        counter = counter + 1
                    .wend
                .endm
                OuterLabel: nop
                Simple()";

            CodeEmitWorks(SOURCE, 0x00, 0x01, 0x00, 0x80, 0x00, 0x01, 0x00, 0x80, 0x00);
        }

        [TestMethod]
        public void MacroWhileCounterWorks()
        {
            // --- Arrange
            const string SOURCE = @"
                Simple: .macro()
                    counter = 0;
                    .while counter < 3
                        .db $cnt
                        counter = counter + 1                    
                    .endw
                .endm
                Simple()
                Simple()";

            CodeEmitWorks(SOURCE, 0x01, 0x02, 0x03, 0x01, 0x02, 0x03);
        }

        [TestMethod]
        public void MacroForLoopEmitWithSingleLineAndNoInternalLabelWorks()
        {
            // --- Arrange
            const string SOURCE = @"
                Simple: .macro()
                    .for _i = 1 .to 2
                        ld bc,#1234
                    .next
                .endm
                Simple()";

            CodeEmitWorks(SOURCE, 0x01, 0x34, 0x12, 0x01, 0x34, 0x12);
        }

        [TestMethod]
        public void MacroForLoopEmitWithMultipleLineAndNoInternalLabelWorks()
        {
            // --- Arrange
            const string SOURCE = @"
                Simple: .macro()
                    .for _i = 1 .to 2
                        inc b
                        inc c
                        inc d
                    .next
                .endm
                Simple()";

            CodeEmitWorks(SOURCE, 0x04, 0x0C, 0x14, 0x04, 0x0C, 0x14);
        }

        [TestMethod]
        public void MacroBackwardForLoopEmitWithMultipleLineAndNoInternalLabelWorks()
        {
            // --- Arrange
            const string SOURCE = @"
                Simple: .macro()
                    .for _i = 2 .to 1 step -1
                        inc b
                        inc c
                        inc d
                    .next
                .endm
                Simple()";

            CodeEmitWorks(SOURCE, 0x04, 0x0C, 0x14, 0x04, 0x0C, 0x14);
        }

        [TestMethod]
        public void MacroForLoopWithInternalLabelWorks()
        {
            // --- Arrange
            const string SOURCE = @"
                Simple: .macro()
                    .for _i = 1 .to 2
                        ThisLabel: ld bc,ThisLabel
                    .next
                .endm
                Simple()";

            CodeEmitWorks(SOURCE, 0x01, 0x00, 0x80, 0x01, 0x03, 0x80);
        }

        [TestMethod]
        public void MacroForLoopWithInternalFixupLabelWorks()
        {
            // --- Arrange
            const string SOURCE = @"
                Simple: .macro()
                    .for _i = 2 .to 1 step -1
                        ld bc,ThisLabel
                        ThisLabel: nop
                    .next
                .endm
                Simple()";

            CodeEmitWorks(SOURCE, 0x01, 0x03, 0x80, 0x00, 0x01, 0x07, 0x80, 0x00);
        }

        [TestMethod]
        public void MacroForLoopWithStartLabelWorks()
        {
            // --- Arrange
            const string SOURCE = @"
                Simple: .macro()
                    StartLabel: .for _i = 1 to 2
                        ld bc,StartLabel
                        nop
                    .next
                .endm
                Simple()";

            CodeEmitWorks(SOURCE, 0x01, 0x00, 0x80, 0x00, 0x01, 0x00, 0x80, 0x00);
        }

        [TestMethod]
        public void MacroForLoopWithEndLabelWorks()
        {
            // --- Arrange
            const string SOURCE = @"
                Simple: .macro()
                    .for _i = 1 .to 2
                        ld bc,EndLabel
                        nop
                    EndLabel: .next
                .endm
                Simple()";

            CodeEmitWorks(SOURCE, 0x01, 0x04, 0x80, 0x00, 0x01, 0x08, 0x80, 0x00);
        }

        [TestMethod]
        public void MacroForLoopWithExternalFixupLabelWorks1()
        {
            // --- Arrange
            const string SOURCE = @"
                Simple: .macro()
                    .for _i = 2 to 1 step -1
                        ld bc,OuterLabel
                        nop
                    .next
                .endm
                Simple()
                OuterLabel: nop";

            CodeEmitWorks(SOURCE, 0x01, 0x08, 0x80, 0x00, 0x01, 0x08, 0x80, 0x00, 0x00);
        }

        [TestMethod]
        public void MacroForLoopWithExternalFixupLabelWorks2()
        {
            // --- Arrange
            const string SOURCE = @"
                Simple: .macro()
                    .for _i = 2 to 1 step -1
                        ld bc,OuterLabel
                        nop
                    .next
                .endm
                OuterLabel: nop
                Simple()";

            CodeEmitWorks(SOURCE, 0x00, 0x01, 0x00, 0x80, 0x00, 0x01, 0x00, 0x80, 0x00);
        }

        [TestMethod]
        public void MacroForLoopCounterWorks()
        {
            // --- Arrange
            const string SOURCE = @"
                Simple: .macro()
                    .for _i = 6 to 8
                        .db $cnt
                    .next
                .endm
                Simple()
                Simple()";

            CodeEmitWorks(SOURCE, 0x01, 0x02, 0x03, 0x01, 0x02, 0x03);
        }

        [TestMethod]
        public void MacroWithLoopVariableWorks()
        {
            // --- Arrange
            const string SOURCE = @"
                Simple: .macro()
                    .for _i = 6 to 8
                        .db _i
                    .next
                .endm
                Simple()
                Simple()";

            CodeEmitWorks(SOURCE, 0x06, 0x07, 0x08, 0x06, 0x07, 0x08);
        }

        [TestMethod]
        public void MultipleMacrosWork()
        {
            // --- Arrange
            const string SOURCE = @"
                Simple1: .macro()
                    .db #01
                .endm
                Simple2: .macro()
                    .db #02
                .endm
                Simple3: .macro()
                    .db #03
                .endm
                Simple2()
                Simple1()
                Simple3()
                Simple1()
                Simple2()";

            CodeEmitWorks(SOURCE, 0x02, 0x01, 0x03, 0x01, 0x02);
        }

        [TestMethod]
        public void MacroIfEmitsNothingWithFalseCondition()
        {
            // --- Arrange
            const string SOURCE = @"
                Simple: .macro()
                    cond = false;
                    .if cond
                        nop
                    .endif
                .endm
                Simple()";

            CodeEmitWorks(SOURCE);
        }

        [TestMethod]
        public void MacroIfEmitsCodeWithTrueCondition()
        {
            // --- Arrange
            const string SOURCE = @"
                Simple: .macro()
                    cond = true;
                    .if cond
                        nop
                    .endif
                .endm
                Simple()";

            CodeEmitWorks(SOURCE, 0x00);
        }

        [TestMethod]
        public void MacroIfEmitsCodeWithTrueConditionAndElse()
        {
            // --- Arrange
            const string SOURCE = @"
                Simple: .macro()
                    cond = true;
                    .if cond
                        nop
                    .else
                        inc c
                    .endif
                .endm
                Simple()";

            CodeEmitWorks(SOURCE, 0x00);
        }

        [TestMethod]
        public void MacroIfEmitsCodeWithFalseConditionAndElse()
        {
            // --- Arrange
            const string SOURCE = @"
                Simple: .macro()
                    cond = false;
                    .if cond
                        nop
                    .else
                        inc b
                    .endif
                .endm
                Simple()";

            CodeEmitWorks(SOURCE, 0x04);
        }

        [TestMethod]
        public void MacroIfEmitsNothingWithMultipleFalseCondition()
        {
            // --- Arrange
            const string SOURCE = @"
                Simple: .macro()
                    cond = false;
                    .if cond
                        nop
                    .elif cond
                        nop
                    .elif cond
                        nop
                    .endif
                .endm
                Simple()";

            CodeEmitWorks(SOURCE);
        }

        [TestMethod]
        [DataRow("0", 0x00)]
        [DataRow("1", 0x04)]
        [DataRow("2", 0x0C)]
        [DataRow("123", 0x14)]
        public void MacroIfEmitsBranchWithTrueCondition(string value, int emitted)
        {
            // --- Arrange
            var source = @"
                Simple: .macro()
                    cond = " + value + @"
                    .if cond == 0
                        nop
                    .elif cond == 1
                        inc b
                    .elif cond == 2
                        inc c
                    .else
                        inc d
                    .endif
                .endm
                Simple()";

            CodeEmitWorks(source, (byte)emitted);
        }

        [TestMethod]
        [DataRow("0", 0x03)]
        [DataRow("1", 0x04)]
        [DataRow("2", 0x05)]
        [DataRow("123", 0x6)]
        public void MacroIfHandlesEquProperly(string value, int emitted)
        {
            // --- Arrange
            var source = @"
                Simple: .macro()
                    cond = " + value + @"
                    .if cond == 0
                        value .equ 3
                    .elif cond == 1
                        value .equ 4
                    .elif cond == 2
                        value .equ 5
                    .else
                        value .equ 6
                    .endif
                    .db value
                .endm
                Simple()";

            CodeEmitWorks(source, (byte)emitted);
        }

        [TestMethod]
        [DataRow("0", 0x03)]
        [DataRow("1", 0x04)]
        [DataRow("2", 0x05)]
        [DataRow("123", 0x6)]
        public void MacroIfHandlesVarProperly(string value, int emitted)
        {
            // --- Arrange
            var source = @"
                Simple: .macro()
                    cond = " + value + @"
                    value = 0
                    .if cond == 0
                        value = 3
                    .elif cond == 1
                        value = 4
                    .elif cond == 2
                        value = 5
                    .else
                        value = 6
                    .endif
                    .db value
                .endm
                Simple()";

            CodeEmitWorks(source, (byte)emitted);
        }

        [TestMethod]
        [DataRow("0", 0x3C)]
        [DataRow("1", 0x04)]
        [DataRow("2", 0x0C)]
        [DataRow("123", 0x14)]
        public void MacroIfHandlesBranchStartLabels(string value, int emitted)
        {
            // --- Arrange
            var source = @"
                Simple: .macro()
                    cond = " + value + @"
                    .if cond == 0
                        Label: nop
                        inc a
                        ld bc,Label
                    .elif cond == 1
                        Label: nop
                        inc b
                        ld bc,Label
                    .elif cond == 2
                        Label: nop
                        inc c
                        ld bc,Label
                    .else
                        Label: nop
                        inc d
                        ld bc,Label
                    .endif
                .endm
                Simple()";

            CodeEmitWorks(source, 0x00, (byte)emitted, 0x01, 0x00, 0x80);
        }

        [TestMethod]
        [DataRow("0", 0x3C)]
        [DataRow("1", 0x04)]
        [DataRow("2", 0x0C)]
        [DataRow("123", 0x14)]
        public void MacroIfHandlesBranchStartHangingLabels(string value, int emitted)
        {
            // --- Arrange
            var source = @"
                Simple: .macro()
                    cond = " + value + @"
                    .if cond == 0
                      Label: 
                        nop
                        inc a
                        ld bc,Label
                    .elif cond == 1
                      Label: 
                        nop
                        inc b
                        ld bc,Label
                    .elif cond == 2
                      Label: 
                        nop
                        inc c
                        ld bc,Label
                    .else
                      Label: 
                        nop
                        inc d
                        ld bc,Label
                    .endif
                .endm
                Simple()";

            CodeEmitWorks(source, 0x00, (byte)emitted, 0x01, 0x00, 0x80);
        }

        [TestMethod]
        [DataRow("0", 0x3C)]
        [DataRow("1", 0x04)]
        [DataRow("2", 0x0C)]
        [DataRow("123", 0x14)]
        public void MacroIfHandlesBranchMiddleLabels(string value, int emitted)
        {
            // --- Arrange
            var source = @"
                Simple: .macro()
                    cond = " + value + @"
                    .if cond == 0
                        nop
                        Label: inc a
                        ld bc,Label
                    .elif cond == 1
                        nop
                        Label: inc b
                        ld bc,Label
                    .elif cond == 2
                        nop
                        Label: inc c
                        ld bc,Label
                    .else
                        nop
                        Label: inc d
                        ld bc,Label
                    .endif
                .endm
                Simple()";

            CodeEmitWorks(source, 0x00, (byte)emitted, 0x01, 0x01, 0x80);
        }

        [TestMethod]
        [DataRow("0", 0x3C)]
        [DataRow("1", 0x04)]
        [DataRow("2", 0x0C)]
        [DataRow("123", 0x14)]
        public void MacroIfHandlesBranchMiddleHangingLabels(string value, int emitted)
        {
            // --- Arrange
            var source = @"
                Simple: .macro()
                    cond = " + value + @"
                    .if cond == 0
                        nop
                      Label: 
                        inc a
                        ld bc,Label
                    .elif cond == 1
                        nop
                      Label: 
                        inc b
                        ld bc,Label
                    .elif cond == 2
                        nop
                      Label: 
                        inc c
                        ld bc,Label
                    .else
                        nop
                      Label: 
                        inc d
                        ld bc,Label
                    .endif
                .endm
                Simple()";

            CodeEmitWorks(source, 0x00, (byte)emitted, 0x01, 0x01, 0x80);
        }

        [TestMethod]
        [DataRow("0", 0x3C)]
        [DataRow("1", 0x04)]
        [DataRow("2", 0x0C)]
        [DataRow("123", 0x14)]
        public void MacroIfHandlesBranchEndLabels(string value, int emitted)
        {
            // --- Arrange
            var source = @"
                Simple: .macro()
                    cond = " + value + @"
                    .if cond == 0
                        nop
                        inc a
                        Label: ld bc,Label
                    .elif cond == 1
                        nop
                        inc b
                        Label: ld bc,Label
                    .elif cond == 2
                        nop
                        inc c
                        Label: ld bc,Label
                    .else
                        nop
                        inc d
                        Label: ld bc,Label
                    .endif
                .endm
                Simple()";

            CodeEmitWorks(source, 0x00, (byte)emitted, 0x01, 0x02, 0x80);
        }

        [TestMethod]
        [DataRow("0", 0x3C)]
        [DataRow("1", 0x04)]
        [DataRow("2", 0x0C)]
        [DataRow("123", 0x14)]
        public void MacroIfHandlesBranchEndHangingLabels(string value, int emitted)
        {
            // --- Arrange
            var source = @"
                Simple: .macro()
                    cond = " + value + @"
                    .if cond == 0
                        nop
                        inc a
                      Label: 
                        ld bc,Label
                    .elif cond == 1
                        nop
                        inc b
                      Label: 
                        ld bc,Label
                    .elif cond == 2
                        nop
                        inc c
                      Label: 
                        ld bc,Label
                    .else
                        nop
                        inc d
                      Label: 
                        ld bc,Label
                    .endif
                .endm
                Simple()";

            CodeEmitWorks(source, 0x00, (byte)emitted, 0x01, 0x02, 0x80);
        }

        [TestMethod]
        public void MacroIfRecognizesLabel()
        {
            // --- Arrange
            const string SOURCE = @"
                Simple: .macro()
                    .if true
                        value = 100
                    .else
                    .endif
                    ld hl,value
                .endm
                Simple()";

            CodeEmitWorks(SOURCE, 0x21, 0x64, 0x00);
        }

        [TestMethod]
        public void MacroIfRecognizesMissingLabel()
        {
            // --- Arrange
            const string SOURCE = @"
                Simple: .macro()
                    .if false
                        value = 100
                    .else
                    .endif
                    ld hl,value
                .endm
                Simple()";

            CodeRaisesError(SOURCE, Errors.Z0201);
        }

        [TestMethod]
        [DataRow("0", "0", 0x00)]
        [DataRow("0", "1", 0x01)]
        [DataRow("0", "5", 0x02)]
        [DataRow("1", "0", 0x03)]
        [DataRow("1", "1", 0x04)]
        [DataRow("1", "100", 0x05)]
        [DataRow("2", "0", 0x06)]
        [DataRow("2", "1", 0x07)]
        [DataRow("2", "123", 0x08)]
        [DataRow("123", "0", 0x09)]
        [DataRow("123", "1", 0x0A)]
        [DataRow("123", "123", 0x0B)]
        public void MacroNestedIfWorks(string row, string column, int emitted)
        {
            // --- Arrange
            var source = @"
                Simple: .macro()
                    row = " + row + @"
                    col = " + column + @"
                    .if row == 0
                        .if col == 0
                            .db #00
                        .elif col == 1
                            .db #01
                        .else
                            .db #02
                        .endif
                    .elif row == 1
                        .if col == 0
                            .db #03
                        .elif col == 1
                            .db #04
                        .else
                            .db #05
                        .endif
                    .elif row == 2
                        .if col == 0
                            .db #06
                        .elif col == 1
                            .db #07
                        .else
                            .db #08
                        .endif
                    .else
                        .if col == 0
                            .db #09
                        .elif col == 1
                            .db #0A
                        .else
                            .db #0B
                        .endif
                    .endif
                .endm
                Simple()";

            CodeEmitWorks(source, (byte)emitted);
        }

        [TestMethod]
        public void UnpassedMacroArgumentsGetEmptyString()
        {
            // --- Arrange
            const string SOURCE = @"
                Simple: .macro(arg1, arg2)
                    .if def({{arg1}})
                        ld a,b
                    .endif
                    .if def({{arg2}})
                        ld b,a
                    .endif
                .endm
                Simple()
                Simple(12)
                Simple(12, 13)
                Simple("""", 13)";

            CodeEmitWorks(SOURCE, 0x78, 0x78, 0x047, 0x47);
        }

        [TestMethod]
        public void UnpassedMacroArgumentsGetEmptyString2()
        {
            // --- Arrange
            const string SOURCE = @"
                Simple: .macro(arg1, arg2)
                    .if def({{arg1}})
                        ld a,b
                    .endif
                    .if def({{arg2}})
                        ld b,a
                    .endif
                .endm
                Simple(, 13)
                Simple(12)";

            CodeEmitWorks(SOURCE, 0x47, 0x78);
        }

        [TestMethod]
        public void DefAcceptOnlyMacroParameter()
        {
            // --- Arrange
            const string SOURCE = @"
                Simple: .macro(arg1, arg2)
                    .if def(arg1) ; This is en error
                        ld a,b
                    .endif
                    .if def({{arg2}})
                        ld b,a
                    .endif
                .endm";

            CodeRaisesError(SOURCE, Errors.Z0421);
        }

        [TestMethod]
        public void MacroArgCannotAcceptOtherArgument()
        {
            // --- Arrange
            const string SOURCE = @"
                Simple: 
                    .macro(arg1, arg2)
                        {{arg2}}
                    .endm
                Simple(c, ""ld a,{{arg1}}"")";

            CodeRaisesError(SOURCE, Errors.Z0422);
        }

        [TestMethod]
        public void UnpassedMacroArgumentsGetEmptyString3()
        {
            // --- Arrange
            const string SOURCE = @"
                LdBcDeHl:
                    .macro(bcVal, deVal, hlVal)
                        .if def({{bcVal}})
                            ld bc,{{bcVal}}
                        .endif
                        .if def({{deVal}})
                            ld de,{{deVal}}
                        .endif
                        .if def({{hlVal}})
                            ld hl,{{hlVal}}
                        .endif
                .endm
                LdBcDeHl(,#1000, #2000)";

            CodeEmitWorks(SOURCE, 0x11, 0x00, 0x10, 0x21, 0x00, 0x20);
        }

        [TestMethod]
        public void MacroInMacroWorks1()
        {
            // --- Arrange
            const string SOURCE = @"
                LdHl:
                    .macro(value)
                        ld hl,{{value}}    
                    .endm

                LdHl2:
                    .macro(value1, value2)
                        LdHl({{value1}})
                        LdHl({{value2}})
                    .endm

                LdHl2(2,3)";

            CodeEmitWorks(SOURCE, 0x21, 0x02, 0x00, 0x21, 0x03, 0x00);
        }

        [TestMethod]
        [DataRow("b", 0x78)]
        [DataRow("c", 0x79)]
        [DataRow("d", 0x7A)]
        [DataRow("e", 0x7B)]
        [DataRow("h", 0x7C)]
        [DataRow("l", 0x7D)]
        [DataRow("a", 0x7F)]
        [DataRow("bc", 0x00)]
        public void IsReg8WorksAsExpected(string param, int expected)
        {
            // --- Arrange
            var source = @"
                MyMacro:
                    .macro(mpar)
                        .if isreg8({{mpar}})
                            ld a,{{mpar}}
                        .else
                            nop
                        .endif
                .endm
                MyMacro(" + param + ")";

            CodeEmitWorks(source, (byte)expected);
        }


    }
}
