using AntlrZ80Asm.SyntaxTree.Operations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;

namespace AntlrZ80Asm.Test.Parser
{
    [TestClass]
    public class InterruptModeOperationTests : ParserTestBed
    {
        [TestMethod]
        public void ImOperationWorkAsExpected()
        {
            ImOperationWorks("im 0", "IM", "0");
            ImOperationWorks("im 1", "IM", "1");
            ImOperationWorks("im 2", "IM", "2");
        }

        protected void ImOperationWorks(string instruction, string type, string mode)
        {
            // --- Act
            var visitor = Parse(instruction);

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0] as InterruptModeOperation;
            line.ShouldNotBeNull();
            line.Mnemonic.ShouldBe(type);
            line.Mode.ShouldBe(mode);
        }
    }
}