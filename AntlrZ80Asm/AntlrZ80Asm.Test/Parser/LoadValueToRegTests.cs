using AntlrZ80Asm.SyntaxTree;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;

namespace AntlrZ80Asm.Test.Parser
{
    [TestClass]
    public class LoadValueToRegTests : ParserTestBed
    {
        [TestMethod]
        public void LoadValueTo8BitRegWorksAsExpected()
        {
            InstructionWorksAsExpected("ld b, 12345", "B");
            InstructionWorksAsExpected("ld c, 12345", "C");
            InstructionWorksAsExpected("ld d, 12345", "D");
            InstructionWorksAsExpected("ld e, 12345", "E");
            InstructionWorksAsExpected("ld h, 12345", "H");
            InstructionWorksAsExpected("ld l, 12345", "L");
            InstructionWorksAsExpected("ld (hl), 12345", "(HL)");
            InstructionWorksAsExpected("ld a, 12345", "A");

            InstructionWorksAsExpected("ld bc, 12345", "BC");
            InstructionWorksAsExpected("ld de, 12345", "DE");
            InstructionWorksAsExpected("ld hl, 12345", "HL");
            InstructionWorksAsExpected("ld sp, 12345", "SP");
            InstructionWorksAsExpected("ld ix, 12345", "IX");
            InstructionWorksAsExpected("ld iy, 12345", "IY");
        }

        protected void InstructionWorksAsExpected(string instruction, string dest)
        {
            // --- Act
            var visitor = Parse(instruction);

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0] as LoadValueToRegInstruction;
            line.ShouldNotBeNull();
            line.Destination.ShouldBe(dest);
            line.Expression.ShouldNotBeNull();
        }
    }
}
