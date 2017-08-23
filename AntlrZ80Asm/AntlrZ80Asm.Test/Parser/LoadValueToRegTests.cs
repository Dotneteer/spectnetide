using AntlrZ80Asm.SyntaxTree;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;

namespace AntlrZ80Asm.Test.Parser
{
    [TestClass]
    public class LoadValueToRegTests : ParserTestBed
    {
        protected void InstructionWorksAsExpected(string instruction, string source, string dest)
        {
            // --- Act
            var visitor = Parse(instruction);

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0] as LoadValueToRegInstruction;
            line.ShouldNotBeNull();
            //line.Expression.ShouldBeNull();
        }

        [TestMethod]
        public void LoadValueTo8BitRegWorksAsExpected()
        {
            InstructionWorksAsExpected("ld b, 12345", "B", "B");
        }
    }
}
