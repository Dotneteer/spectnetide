using AntlrZ80Asm.SyntaxTree;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;

namespace AntlrZ80Asm.Test.Parser
{
    [TestClass]
    public class ExchangeInstructionTests : ParserTestBed
    {
        [TestMethod]
        public void Load8BitRegTo8BitRegWorksAsExpected()
        {
            InstructionWorksAsExpected("ex af, af'", "AF'", "AF");
            InstructionWorksAsExpected("ex de, hl", "HL", "DE");
            InstructionWorksAsExpected("ex (sp), hl", "HL", "(SP)");
            InstructionWorksAsExpected("ex (sp), ix", "IX", "(SP)");
            InstructionWorksAsExpected("ex (sp), iy", "IY", "(SP)");
        }

        protected void InstructionWorksAsExpected(string instruction, string source, string dest)
        {
            // --- Act
            var visitor = Parse(instruction);

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0] as ExchangeInstruction;
            line.ShouldNotBeNull();
            line.Source.ShouldBe(source);
            line.Destination.ShouldBe(dest);
        }
    }
}