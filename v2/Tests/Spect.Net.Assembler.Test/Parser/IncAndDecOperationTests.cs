using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.Assembler.SyntaxTree;
using Spect.Net.Assembler.SyntaxTree.Operations;

namespace Spect.Net.Assembler.Test.Parser
{
    [TestClass]
    public class IncAndDecOperationTests : ParserTestBed
    {
        [TestMethod]
        public void IncrementWorksAsExpected()
        {
            IncrementWorksAsExpected("inc b", "B");
            IncrementWorksAsExpected("inc c", "C");
            IncrementWorksAsExpected("inc d", "D");
            IncrementWorksAsExpected("inc e", "E");
            IncrementWorksAsExpected("inc h", "H");
            IncrementWorksAsExpected("inc l", "L");
            IncrementWorksAsExpected("inc (hl)", "(HL)");
            IncrementWorksAsExpected("inc a", "A");
            IncrementWorksAsExpected("inc bc", "BC");
            IncrementWorksAsExpected("inc de", "DE");
            IncrementWorksAsExpected("inc hl", "HL");
            IncrementWorksAsExpected("inc sp", "SP");
            IncrementWorksAsExpected("inc ix", "IX");
            IncrementWorksAsExpected("inc xh", "XH");
            IncrementWorksAsExpected("inc xl", "XL");
            IncrementWorksAsExpected("inc iy", "IY");
            IncrementWorksAsExpected("inc yh", "YH");
            IncrementWorksAsExpected("inc yl", "YL");

            IncrementWorksAsExpected("inc bc", "BC");
            IncrementWorksAsExpected("inc de", "DE");
            IncrementWorksAsExpected("inc hl", "HL");
            IncrementWorksAsExpected("inc sp", "SP");
            IncrementWorksAsExpected("inc ix", "IX");
            IncrementWorksAsExpected("inc iy", "IY");

            IncrementWorksAsExpected("inc (ix)", "IX", null);
            IncrementWorksAsExpected("inc (ix+#bc)", "IX", "+");
            IncrementWorksAsExpected("inc (ix-#0a)", "IX", "-");
            IncrementWorksAsExpected("inc (iy)", "IY", null);
            IncrementWorksAsExpected("inc (iy+#bc)", "IY", "+");
            IncrementWorksAsExpected("inc (iy-#0a)", "IY", "-");
        }

        [TestMethod]
        public void DecrementWorksAsExpected()
        {
            DecrementWorksAsExpected("dec b", "B");
            DecrementWorksAsExpected("dec c", "C");
            DecrementWorksAsExpected("dec d", "D");
            DecrementWorksAsExpected("dec e", "E");
            DecrementWorksAsExpected("dec h", "H");
            DecrementWorksAsExpected("dec l", "L");
            DecrementWorksAsExpected("dec (hl)", "(HL)");
            DecrementWorksAsExpected("dec a", "A");
            DecrementWorksAsExpected("dec bc", "BC");
            DecrementWorksAsExpected("dec de", "DE");
            DecrementWorksAsExpected("dec hl", "HL");
            DecrementWorksAsExpected("dec sp", "SP");
            DecrementWorksAsExpected("dec ix", "IX");
            DecrementWorksAsExpected("dec xh", "XH");
            DecrementWorksAsExpected("dec xl", "XL");
            DecrementWorksAsExpected("dec iy", "IY");
            DecrementWorksAsExpected("dec yh", "YH");
            DecrementWorksAsExpected("dec yl", "YL");

            DecrementWorksAsExpected("dec bc", "BC");
            DecrementWorksAsExpected("dec de", "DE");
            DecrementWorksAsExpected("dec hl", "HL");
            DecrementWorksAsExpected("dec sp", "SP");
            DecrementWorksAsExpected("dec ix", "IX");
            DecrementWorksAsExpected("dec iy", "IY");

            DecrementWorksAsExpected("dec (ix)", "IX", null);
            DecrementWorksAsExpected("dec (ix+#bc)", "IX", "+");
            DecrementWorksAsExpected("dec (ix-#0a)", "IX", "-");
            DecrementWorksAsExpected("dec (iy)", "IY", null);
            DecrementWorksAsExpected("dec (iy+#bc)", "IY", "+");
            DecrementWorksAsExpected("dec (iy-#0a)", "IY", "-");
        }

        protected void IncrementWorksAsExpected(string instruction, string register)
        {
            // --- Act
            var visitor = Parse(instruction);

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0] as CompoundOperation;
            line.ShouldNotBeNull();
            line.Mnemonic.ShouldBe("INC");
            line.Operand.Register.ShouldBe(register);
        }

        protected void IncrementWorksAsExpected(string instruction, string reg, string sign)
        {
            // --- Act
            var visitor = Parse(instruction);

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0] as CompoundOperation;
            line.ShouldNotBeNull();
            line.Mnemonic.ShouldBe("INC");
            line.Operand.Type.ShouldBe(OperandType.IndexedAddress);
            line.Operand.Register.ShouldBe(reg);
            line.Operand.Sign.ShouldBe(sign);
            if (sign == null)
            {
                line.Operand.Expression.ShouldBeNull();
            }
            else
            {
                line.Operand.Expression.ShouldNotBeNull();
            }
        }

        protected void DecrementWorksAsExpected(string instruction, string register)
        {
            // --- Act
            var visitor = Parse(instruction);

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0] as CompoundOperation;
            line.ShouldNotBeNull();
            line.Mnemonic.ShouldBe("DEC");
            line.Operand.Register.ShouldBe(register);
        }

        protected void DecrementWorksAsExpected(string instruction, string reg, string sign)
        {
            // --- Act
            var visitor = Parse(instruction);

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0] as CompoundOperation;
            line.ShouldNotBeNull();
            line.Mnemonic.ShouldBe("DEC");
            line.Operand.Type.ShouldBe(OperandType.IndexedAddress);
            line.Operand.Register.ShouldBe(reg);
            line.Operand.Sign.ShouldBe(sign);
            if (sign == null)
            {
                line.Operand.Expression.ShouldBeNull();
            }
            else
            {
                line.Operand.Expression.ShouldNotBeNull();
            }
        }
    }
}