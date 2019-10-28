using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.Assembler.SyntaxTree.Operations;

namespace Spect.Net.Assembler.Test.Parser
{
    [TestClass]
    public class ControlFlowOperationTests : ParserTestBed
    {
        [TestMethod]
        public void NoConditionOperationWorksAsExpected()
        {
            NoConditionOperationWorks("djnz #1234", "DJNZ");
            NoConditionOperationWorks("djnz label", "DJNZ");
            NoConditionOperationWorks("jr #1234", "JR");
            NoConditionOperationWorks("jr label", "JR");
            NoConditionOperationWorks("jp #1234", "JP");
            NoConditionOperationWorks("jp label", "JP");
            NoConditionOperationWorks("call #1234", "CALL");
            NoConditionOperationWorks("call label", "CALL");
            NoConditionOperationWorks("rst #20", "RST");
            NoConditionOperationWorks("rst label", "RST");
        }

        [TestMethod]
        public void JumpToRegisterAddressWorksAsExpected()
        {
            JumpToRegisterAddressWorks("jp (hl)", "(HL)");
            JumpToRegisterAddressWorks("jp (ix)", "IX");
            JumpToRegisterAddressWorks("jp (iy)", "IY");
        }

        [TestMethod]
        public void ConditionOperationWorksAsExpected()
        {
            ConditionOperationWorks("jr z, #1234", "JR", "Z");
            ConditionOperationWorks("jr nz, #1234", "JR", "NZ");
            ConditionOperationWorks("jr c, #1234", "JR", "C");
            ConditionOperationWorks("jr nc, #1234", "JR", "NC");

            ConditionOperationWorks("jp z, #1234", "JP", "Z");
            ConditionOperationWorks("jp nz, #1234", "JP", "NZ");
            ConditionOperationWorks("jp c, #1234", "JP", "C");
            ConditionOperationWorks("jp nc, #1234", "JP", "NC");
            ConditionOperationWorks("jp pe, #1234", "JP", "PE");
            ConditionOperationWorks("jp po, #1234", "JP", "PO");
            ConditionOperationWorks("jp p, #1234", "JP", "P");
            ConditionOperationWorks("jp m, #1234", "JP", "M");

            ConditionOperationWorks("call z, #1234", "CALL", "Z");
            ConditionOperationWorks("call nz, #1234", "CALL", "NZ");
            ConditionOperationWorks("call c, #1234", "CALL", "C");
            ConditionOperationWorks("call nc, #1234", "CALL", "NC");
            ConditionOperationWorks("call pe, #1234", "CALL", "PE");
            ConditionOperationWorks("call po, #1234", "CALL", "PO");
            ConditionOperationWorks("call p, #1234", "CALL", "P");
            ConditionOperationWorks("call m, #1234", "CALL", "M");

            ConditionOperationWorks("ret nz", "RET", "NZ", true);
            ConditionOperationWorks("ret z", "RET", "Z", true);
            ConditionOperationWorks("ret nc", "RET", "NC", true);
            ConditionOperationWorks("ret c", "RET", "C", true);
            ConditionOperationWorks("ret po", "RET", "PO", true);
            ConditionOperationWorks("ret pe", "RET", "PE", true);
            ConditionOperationWorks("ret p", "RET", "P", true);
            ConditionOperationWorks("ret m", "RET", "M", true);
        }

        protected void NoConditionOperationWorks(string instruction, string type)
        {
            // --- Act
            var visitor = Parse(instruction);

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0] as CompoundOperation;
            line.ShouldNotBeNull();
            line.Mnemonic.ShouldBe(type);
            line.Operand.Expression.ShouldNotBeNull();
            line.Operand2.ShouldBeNull();
            line.Operand3.ShouldBeNull();
        }

        protected void JumpToRegisterAddressWorks(string instruction, string reg)
        {
            // --- Act
            var visitor = Parse(instruction);

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0] as CompoundOperation;
            line.ShouldNotBeNull();
            line.Mnemonic.ShouldBe("JP");
            line.Operand.Expression.ShouldBeNull();
            line.Operand.Register.ShouldBe(reg);
        }

        protected void ConditionOperationWorks(string instruction, string type, string condition, bool nullTarget = false)
        {
            // --- Act
            var visitor = Parse(instruction);

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0] as CompoundOperation;
            line.ShouldNotBeNull();
            line.Mnemonic.ShouldBe(type);
            if (nullTarget)
            {
                line.Operand2?.Expression.ShouldBeNull();
            }
            else
            {
                line.Operand2.Expression.ShouldNotBeNull();
            }
            line.Operand2?.Register.ShouldBeNull();
            if (condition.ToUpper() == "C")
            {
                line.Operand.Register.ShouldBe("C");
            }
            else
            {
                line.Operand.Condition.ShouldBe(condition);
            }
        }
    }
}
