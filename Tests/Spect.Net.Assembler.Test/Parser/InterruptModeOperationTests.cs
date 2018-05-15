using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.Assembler.SyntaxTree;
using Spect.Net.Assembler.SyntaxTree.Operations;

namespace Spect.Net.Assembler.Test.Parser
{
    [TestClass]
    public class InterruptModeOperationTests : ParserTestBed
    {
        [TestMethod]
        public void ImOperationWorkAsExpected()
        {
            ImOperationWorks("im 0");
            ImOperationWorks("im 1");
            ImOperationWorks("im 2");
        }

        protected void ImOperationWorks(string instruction)
        {
            // --- Act
            var visitor = Parse(instruction);

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0] as CompoundOperation;
            line.ShouldNotBeNull();
            line.Mnemonic.ShouldBe("IM");
            line.Operand.ShouldNotBeNull();
            line.Operand.Type.ShouldBe(OperandType.Expr);
            line.Operand2.ShouldBeNull();
            line.Operand3.ShouldBeNull();
        }
    }
}