using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.Assembler.Assembler;

namespace Spect.Net.Assembler.Test.Assembler
{
    [TestClass]
    public class IfNusedEmitTests : AssemblerTestBed
    {
        [TestMethod]
        public void SimpleIfNusedWorks()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                .ifnused MyId 
                .endif
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
        }

        [TestMethod]
        public void CompoundIfNusedWorks()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                .ifnused MyModule.MyId 
                .endif
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
        }

        [TestMethod]
        public void CompoundIfNusedWithGlobalStartWorks()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                .ifnused ::MyModule.MyId 
                .endif
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
        }

        [TestMethod]
        public void SimpleIfNusedWithStartLabelWorks()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                Start: .ifnused MyId 
                .endif
                ");

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            output.Symbols.ContainsKey("START").ShouldBeTrue();
            output.Symbols["START"].Value.Value.ShouldBe((ushort)0x8000);
        }

        [TestMethod]
        public void SimpleIfNusedElseWorks()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(@"
                .ifnused MyId 
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
                .ifnused MyId 
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
                .ifnused MyId 
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
                .ifnused MyId 
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
                .ifnused MyId 
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
                .ifnused MyId 
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
                    .ifnused MyId 
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
                .ifnused MyId 
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
                    .ifnused MyId 
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
                .ifnused MyId 
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
                    .ifnused MyId 
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
                    .ifnused MyId 
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
                    .ifnused MyId 
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
                    .ifnused MyId 
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
                    .ifnused MyId 
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
        public void IfNusedEmitsNothingWithFalseCondition()
        {
            // --- Arrange
            const string SOURCE = @"
                cond = 3
                useCond = cond
                .ifnused cond
                    nop
                .endif
                ";

            CodeEmitWorks(SOURCE);
        }

        [TestMethod]
        public void IfNusedEmitsCodeWithTrueCondition()
        {
            // --- Arrange
            const string SOURCE = @"
                cond = 3
                .ifnused cond
                    nop
                .endif
                ";

            CodeEmitWorks(SOURCE, 0x00);
        }

        [TestMethod]
        public void IfNusedEmitsCodeWithTrueConditionAndElse()
        {
            // --- Arrange
            const string SOURCE = @"
                cond = 3
                .ifnused cond
                    nop
                .else
                    inc c
                .endif
                ";

            CodeEmitWorks(SOURCE, 0x00);
        }

        [TestMethod]
        public void IfUsedEmitsCodeWithFalseConditionAndElse()
        {
            // --- Arrange
            const string SOURCE = @"
                cond = 3
                useCod = cond
                .ifnused cond
                    nop
                .else
                    inc b
                .endif
                ";

            CodeEmitWorks(SOURCE, 0x04);
        }

        [TestMethod]
        public void IfNusedEmitsNothingWithMultipleFalseCondition()
        {
            // --- Arrange
            const string SOURCE = @"
                cond = false;
                useCond = cond
                .ifnused cond
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
        [DataRow("3", 0x03)]
        [DataRow("1\nuseCond = cond\n", 0x04)]
        [DataRow("2\nuseCond = cond\n", 0x05)]
        [DataRow("123\nuseCond = cond\n", 0x6)]
        public void IfNusedHandlesEquProperly(string value, int emitted)
        {
            // --- Arrange
            var source = $@"
                cond = {value}
                .ifnused cond
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
        [DataRow("3", 0x03)]
        [DataRow("1\nuseCond = cond\n", 0x04)]
        [DataRow("2\nuseCond = cond\n", 0x05)]
        [DataRow("123\nuseCond = cond\n", 0x6)]
        public void IfNusedHandlesVarProperly(string value, int emitted)
        {
            // --- Arrange
            var source = $@"
                cond = {value}
                .ifnused cond
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
        [DataRow("3", 0x3C)]
        [DataRow("1\nuseCond = cond\n", 0x04)]
        [DataRow("2\nuseCond = cond\n", 0x0C)]
        [DataRow("123\nuseCond = cond\n", 0x14)]
        public void IfNusedHandlesBranchStartLabels(string value, int emitted)
        {
            // --- Arrange
            var source = $@"
                cond = {value}
                .ifnused cond
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
        [DataRow("3", 0x3C)]
        [DataRow("1\nuseCond = cond\n", 0x04)]
        [DataRow("2\nuseCond = cond\n", 0x0C)]
        [DataRow("123\nuseCond = cond\n", 0x14)]
        public void IfNusedHandlesBranchStartHangingLabels(string value, int emitted)
        {
            // --- Arrange
            var source = $@"
                cond = {value}
                .ifnused cond
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
        [DataRow("3", 0x3C)]
        [DataRow("1\nuseCond = cond\n", 0x04)]
        [DataRow("2\nuseCond = cond\n", 0x0C)]
        [DataRow("123\nuseCond = cond\n", 0x14)]
        public void IfUsedHandlesBranchMiddleLabels(string value, int emitted)
        {
            // --- Arrange
            var source = $@"
                cond = {value}
                .ifnused cond
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
        [DataRow("3", 0x3C)]
        [DataRow("1\nuseCond = cond\n", 0x04)]
        [DataRow("2\nuseCond = cond\n", 0x0C)]
        [DataRow("123\nuseCond = cond\n", 0x14)]
        public void IfNusedHandlesBranchMiddleHangingLabels(string value, int emitted)
        {
            // --- Arrange
            var source = $@"
                cond = {value}
                .ifnused cond
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
        [DataRow("3", 0x3C)]
        [DataRow("1\nuseCond = cond\n", 0x04)]
        [DataRow("2\nuseCond = cond\n", 0x0C)]
        [DataRow("123\nuseCond = cond\n", 0x14)]
        public void IfNusedHandlesBranchEndLabels(string value, int emitted)
        {
            // --- Arrange
            var source = $@"
                cond = {value}
                .ifnused cond
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
        [DataRow("3", 0x3C)]
        [DataRow("1\nuseCond = cond\n", 0x04)]
        [DataRow("2\nuseCond = cond\n", 0x0C)]
        [DataRow("123\nuseCond = cond\n", 0x14)]
        public void IfNusedHandlesBranchEndHangingLabels(string value, int emitted)
        {
            // --- Arrange
            var source = $@"
                cond = {value}
                .ifnused cond
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
        public void IfNusedRecognizesLabel()
        {
            // --- Arrange
            const string SOURCE = @"
                cond = 3
                .ifnused cond
                    value = 100
                .else
                .endif
                ld hl,value
                ";

            CodeEmitWorks(SOURCE, 0x21, 0x64, 0x00);
        }

        [TestMethod]
        public void IfNusedRecognizesMissingLabel()
        {
            // --- Arrange
            const string SOURCE = @"
                cond = 3
                useCond = cond
                .ifnused cond
                    value = 100
                .else
                .endif
                ld hl,value
                ";

            CodeRaisesError(SOURCE, Errors.Z0201);
        }
    }
}