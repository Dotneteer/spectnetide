using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.Assembler.Assembler;

namespace Spect.Net.Assembler.Test.Assembler
{
    [TestClass]
    public class IfEmitTests : AssemblerTestBed
    {
        [TestMethod]
        public void SimpleIfWorks()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                .if true 
                .endif
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
        }

        [TestMethod]
        public void SimpleIfDefWithStartLabelWorks()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                Start: .if true 
                .endif
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            output.Symbols.ContainsKey("START").ShouldBeTrue();
            output.Symbols["START"].Value.ShouldBe((ushort)0x8000);
        }

        [TestMethod]
        public void SimpleIfElseWorks()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                .if true 
                .else
                .endif
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
        }

        [TestMethod]
        public void SimpleElIfWorks()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                .if true 
                .elif true
                .endif
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
        }

        [TestMethod]
        public void SimpleIfElifElseWorks()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                .if true 
                .elif true
                .else
                .endif
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
        }

        [TestMethod]
        public void MultipleIfElifWorks()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                .if true 
                .elif true
                .elif false
                .endif
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
        }

        [TestMethod]
        public void MultipleIfElifElseWorks()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                .if true 
                .elif true
                .elif false
                .else
                .endif
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
        }

        [TestMethod]
        public void LabeledElIfFails()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                .if true 
                Label: .elif true
                .endif
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0411);
        }

        [TestMethod]
        public void HangingLabeledElIfFails()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                    .if true 
                Label:
                    .elif true
                    .endif
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0411);
        }

        [TestMethod]
        public void LabeledElseFails()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                .if true 
                Label: .else
                .endif
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0411);
        }

        [TestMethod]
        public void HangingLabeledElseFails()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                    .if true 
                Label:
                    .else
                    .endif
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0411);
        }

        [TestMethod]
        public void LabeledElIfElseFails()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                .if true 
                Label: .elif true
                .else
                .endif
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0411);
        }

        [TestMethod]
        public void MultipleLabelIssuesDetected()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                    .if true 
                Label1: 
                    .elif true
                Label2: .elif true
                Label3:
                    .else
                    .endif
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(3);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0411);
            output.Errors[1].ErrorCode.ShouldBe(Errors.Z0411);
            output.Errors[2].ErrorCode.ShouldBe(Errors.Z0411);
        }

        [TestMethod]
        public void ElifAfterElseFails()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                    .if true 
                    .else
                    .elif true
                    .endif
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0410);
        }

        [TestMethod]
        public void ElseAfterElseFails()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                    .if true 
                    .else
                    .else
                    .endif
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0410);
        }

        [TestMethod]
        public void MultipleElifAfterElseFails()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                    .if true 
                    .else
                    .elif false
                    .elif false
                    .endif
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(2);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0410);
            output.Errors[1].ErrorCode.ShouldBe(Errors.Z0410);
        }

        [TestMethod]
        public void MultipleElifAndElseAfterElseFails()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                    .if true 
                    .elif true
                    .else
                    .elif false
                    .elif false
                    .else
                    .else
                    .endif
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(4);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0410);
            output.Errors[1].ErrorCode.ShouldBe(Errors.Z0410);
            output.Errors[2].ErrorCode.ShouldBe(Errors.Z0410);
            output.Errors[3].ErrorCode.ShouldBe(Errors.Z0410);
        }

        [TestMethod]
        public void IfWithStringConditionFails1()
        {
            // --- Arrange
            const string SOURCE = @"
                .if ""cond""
                    nop
                .endif
                ";

            CodeRaisesError(SOURCE, Errors.Z0305);
        }

        [TestMethod]
        public void IfWithStringConditionFails2()
        {
            // --- Arrange
            const string SOURCE = @"
                .if false
                    nop
                .elif ""cond""
                    nop
                .endif
                ";

            CodeRaisesError(SOURCE, Errors.Z0305);
        }

        [TestMethod]
        public void IfWithStringConditionFails3()
        {
            // --- Arrange
            const string SOURCE = @"
                .if false
                    nop
                .elif false
                    nop
                .elif ""cond""
                    nop
                .endif
                ";

            CodeRaisesError(SOURCE, Errors.Z0305);
        }

        [TestMethod]
        public void IfEmitsNothingWithFalseCondition()
        {
            // --- Arrange
            const string SOURCE = @"
                cond = false;
                .if cond
                    nop
                .endif
                ";

            CodeEmitWorks(SOURCE);
        }

        [TestMethod]
        public void IfEmitsCodeWithTrueCondition()
        {
            // --- Arrange
            const string SOURCE = @"
                cond = true;
                .if cond
                    nop
                .endif
                ";

            CodeEmitWorks(SOURCE, 0x00);
        }

        [TestMethod]
        public void IfEmitsCodeWithTrueConditionAndElse()
        {
            // --- Arrange
            const string SOURCE = @"
                cond = true;
                .if cond
                    nop
                .else
                    inc c
                .endif
                ";

            CodeEmitWorks(SOURCE, 0x00);
        }

        [TestMethod]
        public void IfEmitsCodeWithFalseConditionAndElse()
        {
            // --- Arrange
            const string SOURCE = @"
                cond = false;
                .if cond
                    nop
                .else
                    inc b
                .endif
                ";

            CodeEmitWorks(SOURCE, 0x04);
        }

        [TestMethod]
        public void IfEmitsNothingWithMultipleFalseCondition()
        {
            // --- Arrange
            const string SOURCE = @"
                cond = false;
                .if cond
                    nop
                .elif cond
                    nop
                .elif cond
                    nop
                .endif
                ";

            CodeEmitWorks(SOURCE);
        }

        [TestMethod]
        [DataRow("0", 0x00)]
        [DataRow("1", 0x04)]
        [DataRow("2", 0x0C)]
        [DataRow("123", 0x14)]
        public void IfEmitsBranchWithTrueCondition(string value, int emitted)
        {
            // --- Arrange
            var source = @"
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
                ";

            CodeEmitWorks(source, (byte)emitted);
        }

        [TestMethod]
        [DataRow("0", 0x03)]
        [DataRow("1", 0x04)]
        [DataRow("2", 0x05)]
        [DataRow("123", 0x6)]
        public void IfHandlesEquProperly(string value, int emitted)
        {
            // --- Arrange
            var source = @"
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
                ";

            CodeEmitWorks(source, (byte)emitted);
        }

        [TestMethod]
        [DataRow("0", 0x03)]
        [DataRow("1", 0x04)]
        [DataRow("2", 0x05)]
        [DataRow("123", 0x6)]
        public void IfHandlesVarProperly(string value, int emitted)
        {
            // --- Arrange
            var source = @"
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
                ";

            CodeEmitWorks(source, (byte)emitted);
        }

        [TestMethod]
        [DataRow("0", 0x3C)]
        [DataRow("1", 0x04)]
        [DataRow("2", 0x0C)]
        [DataRow("123", 0x14)]
        public void IfHandlesBranchStartLabels(string value, int emitted)
        {
            // --- Arrange
            var source = @"
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
                ";

            CodeEmitWorks(source, 0x00, (byte)emitted, 0x01, 0x00, 0x80);
        }

        [TestMethod]
        [DataRow("0", 0x3C)]
        [DataRow("1", 0x04)]
        [DataRow("2", 0x0C)]
        [DataRow("123", 0x14)]
        public void IfHandlesBranchStartHangingLabels(string value, int emitted)
        {
            // --- Arrange
            var source = @"
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
                ";

            CodeEmitWorks(source, 0x00, (byte)emitted, 0x01, 0x00, 0x80);
        }

        [TestMethod]
        [DataRow("0", 0x3C)]
        [DataRow("1", 0x04)]
        [DataRow("2", 0x0C)]
        [DataRow("123", 0x14)]
        public void IfHandlesBranchMiddleLabels(string value, int emitted)
        {
            // --- Arrange
            var source = @"
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
                ";

            CodeEmitWorks(source, 0x00, (byte)emitted, 0x01, 0x01, 0x80);
        }

        [TestMethod]
        [DataRow("0", 0x3C)]
        [DataRow("1", 0x04)]
        [DataRow("2", 0x0C)]
        [DataRow("123", 0x14)]
        public void IfHandlesBranchMiddleHangingLabels(string value, int emitted)
        {
            // --- Arrange
            var source = @"
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
                ";

            CodeEmitWorks(source, 0x00, (byte)emitted, 0x01, 0x01, 0x80);
        }

        [TestMethod]
        [DataRow("0", 0x3C)]
        [DataRow("1", 0x04)]
        [DataRow("2", 0x0C)]
        [DataRow("123", 0x14)]
        public void IfHandlesBranchEndLabels(string value, int emitted)
        {
            // --- Arrange
            var source = @"
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
                ";

            CodeEmitWorks(source, 0x00, (byte)emitted, 0x01, 0x02, 0x80);
        }

        [TestMethod]
        [DataRow("0", 0x3C)]
        [DataRow("1", 0x04)]
        [DataRow("2", 0x0C)]
        [DataRow("123", 0x14)]
        public void IfHandlesBranchEndHangingLabels(string value, int emitted)
        {
            // --- Arrange
            var source = @"
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
                ";

            CodeEmitWorks(source, 0x00, (byte)emitted, 0x01, 0x02, 0x80);
        }

        [TestMethod]
        public void IfRecognizesLabel()
        {
            // --- Arrange
            const string SOURCE = @"
                .if true
                    value = 100
                .else
                .endif
                ld hl,value
                ";

            CodeEmitWorks(SOURCE, 0x21, 0x64, 0x00);
        }

        [TestMethod]
        public void IfRecognizesMissingLabel()
        {
            // --- Arrange
            const string SOURCE = @"
                .if false
                    value = 100
                .else
                .endif
                ld hl,value
                ";

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
        public void NestedIfWorks(string row, string column, int emitted)
        {
            // --- Arrange
            var source = @"
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
                ";

            CodeEmitWorks(source, (byte)emitted);
        }
    }
}
