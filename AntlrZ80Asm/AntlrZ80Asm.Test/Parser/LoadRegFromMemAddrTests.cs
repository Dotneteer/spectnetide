using AntlrZ80Asm.SyntaxTree;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;

namespace AntlrZ80Asm.Test.Parser
{
    [TestClass]
    public class LoadRegFromMemAddrTests: ParserTestBed
    {
        [TestMethod]
        public void LoadRegFromMemWorksAsExpected()
        {
            InstructionWorksAsExpected("ld a,(#1234)", "A");
            InstructionWorksAsExpected("ld bc,(#1234)", "BC");
            InstructionWorksAsExpected("ld de,(#1234)", "DE");
            InstructionWorksAsExpected("ld hl,(#1234)", "HL");
            InstructionWorksAsExpected("ld ix,(#1234)", "IX");
            InstructionWorksAsExpected("ld iy,(#1234)", "IY");
        }

        protected void InstructionWorksAsExpected(string instruction, string dest)
        {
            // --- Act
            var visitor = Parse(instruction);

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0] as LoadRegFromMemAddrInstruction;
            line.ShouldNotBeNull();
            line.Destination.ShouldBe(dest);
            line.Source.ShouldNotBeNull();
        }
    }
}