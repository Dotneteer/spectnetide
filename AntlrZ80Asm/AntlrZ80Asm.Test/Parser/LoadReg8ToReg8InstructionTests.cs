using AntlrZ80Asm.SyntaxTree;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;

namespace AntlrZ80Asm.Test.Parser
{
    [TestClass]
    public class LoadReg8ToReg8InstructionTests : ParserTestBed
    {
        protected void InstructionWorksAsExpected(string instruction, string source, string dest)
        {
            // --- Act
            var visitor = Parse(instruction);

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0] as LoadReg8ToReg8Instruction;
            line.ShouldNotBeNull();
            line.Source.ShouldBe(source);
            line.Destination.ShouldBe(dest);
        }

        [TestMethod]
        public void Load8BitRegTo8BitRegWorksAsExpected()
        {
            InstructionWorksAsExpected("ld b, b", "B", "B");
            InstructionWorksAsExpected("ld b, c", "C", "B");
            InstructionWorksAsExpected("ld b, d", "D", "B");
            InstructionWorksAsExpected("ld b, e", "E", "B");
            InstructionWorksAsExpected("ld b, h", "H", "B");
            InstructionWorksAsExpected("ld b, l", "L", "B");
            InstructionWorksAsExpected("ld b, (hl)", "(HL)", "B");
            InstructionWorksAsExpected("ld b, ( HL )", "(HL)", "B");
            InstructionWorksAsExpected("ld b, a", "A", "B");

            InstructionWorksAsExpected("ld c, b", "B", "C");
            InstructionWorksAsExpected("ld c, c", "C", "C");
            InstructionWorksAsExpected("ld c, d", "D", "C");
            InstructionWorksAsExpected("ld c, e", "E", "C");
            InstructionWorksAsExpected("ld c, h", "H", "C");
            InstructionWorksAsExpected("ld c, l", "L", "C");
            InstructionWorksAsExpected("ld c, (hl)", "(HL)", "C");
            InstructionWorksAsExpected("ld c, a", "A", "C");

            InstructionWorksAsExpected("ld d, b", "B", "D");
            InstructionWorksAsExpected("ld d, c", "C", "D");
            InstructionWorksAsExpected("ld d, d", "D", "D");
            InstructionWorksAsExpected("ld d, e", "E", "D");
            InstructionWorksAsExpected("ld d, h", "H", "D");
            InstructionWorksAsExpected("ld d, l", "L", "D");
            InstructionWorksAsExpected("ld d, (hl)", "(HL)", "D");
            InstructionWorksAsExpected("ld d, a", "A", "D");

            InstructionWorksAsExpected("ld e, b", "B", "E");
            InstructionWorksAsExpected("ld e, c", "C", "E");
            InstructionWorksAsExpected("ld e, d", "D", "E");
            InstructionWorksAsExpected("ld e, e", "E", "E");
            InstructionWorksAsExpected("ld e, h", "H", "E");
            InstructionWorksAsExpected("ld e, l", "L", "E");
            InstructionWorksAsExpected("ld e, (hl)", "(HL)", "E");
            InstructionWorksAsExpected("ld e, a", "A", "E");

            InstructionWorksAsExpected("ld h, b", "B", "H");
            InstructionWorksAsExpected("ld h, c", "C", "H");
            InstructionWorksAsExpected("ld h, d", "D", "H");
            InstructionWorksAsExpected("ld h, e", "E", "H");
            InstructionWorksAsExpected("ld h, h", "H", "H");
            InstructionWorksAsExpected("ld h, l", "L", "H");
            InstructionWorksAsExpected("ld h, (hl)", "(HL)", "H");
            InstructionWorksAsExpected("ld h, a", "A", "H");

            InstructionWorksAsExpected("ld l, b", "B", "L");
            InstructionWorksAsExpected("ld l, c", "C", "L");
            InstructionWorksAsExpected("ld l, d", "D", "L");
            InstructionWorksAsExpected("ld l, e", "E", "L");
            InstructionWorksAsExpected("ld l, h", "H", "L");
            InstructionWorksAsExpected("ld l, l", "L", "L");
            InstructionWorksAsExpected("ld l, (hl)", "(HL)", "L");
            InstructionWorksAsExpected("ld l, a", "A", "L");

            InstructionWorksAsExpected("ld (hl), b", "B", "(HL)");
            InstructionWorksAsExpected("ld (hl), c", "C", "(HL)");
            InstructionWorksAsExpected("ld (hl), d", "D", "(HL)");
            InstructionWorksAsExpected("ld (hl), e", "E", "(HL)");
            InstructionWorksAsExpected("ld (hl), h", "H", "(HL)");
            InstructionWorksAsExpected("ld (hl), l", "L", "(HL)");
            InstructionWorksAsExpected("ld (hl), (hl)", "(HL)", "(HL)"); // --- This is syntactically correct, though semantically not
            InstructionWorksAsExpected("ld (hl), a", "A", "(HL)");

            InstructionWorksAsExpected("ld a, i", "I", "A");
            InstructionWorksAsExpected("ld i, a", "A", "I");
            InstructionWorksAsExpected("ld a, r", "R", "A");
            InstructionWorksAsExpected("ld r, a", "A", "R");

            InstructionWorksAsExpected("ld b, xl", "XL", "B");
            InstructionWorksAsExpected("ld c, xl", "XL", "C");
            InstructionWorksAsExpected("ld d, xl", "XL", "D");
            InstructionWorksAsExpected("ld e, xl", "XL", "E");
            InstructionWorksAsExpected("ld h, xl", "XL", "H");
            InstructionWorksAsExpected("ld l, xl", "XL", "L");
            InstructionWorksAsExpected("ld a, xl", "XL", "A");

            InstructionWorksAsExpected("ld b, xh", "XH", "B");
            InstructionWorksAsExpected("ld c, xh", "XH", "C");
            InstructionWorksAsExpected("ld d, xh", "XH", "D");
            InstructionWorksAsExpected("ld e, xh", "XH", "E");
            InstructionWorksAsExpected("ld h, xh", "XH", "H");
            InstructionWorksAsExpected("ld l, xh", "XH", "L");
            InstructionWorksAsExpected("ld a, xh", "XH", "A");

            InstructionWorksAsExpected("ld b, yl", "YL", "B");
            InstructionWorksAsExpected("ld c, yl", "YL", "C");
            InstructionWorksAsExpected("ld d, yl", "YL", "D");
            InstructionWorksAsExpected("ld e, yl", "YL", "E");
            InstructionWorksAsExpected("ld h, yl", "YL", "H");
            InstructionWorksAsExpected("ld l, yl", "YL", "L");
            InstructionWorksAsExpected("ld a, yl", "YL", "A");

            InstructionWorksAsExpected("ld b, yh", "YH", "B");
            InstructionWorksAsExpected("ld c, yh", "YH", "C");
            InstructionWorksAsExpected("ld d, yh", "YH", "D");
            InstructionWorksAsExpected("ld e, yh", "YH", "E");
            InstructionWorksAsExpected("ld h, yh", "YH", "H");
            InstructionWorksAsExpected("ld l, yh", "YH", "L");
            InstructionWorksAsExpected("ld a, yh", "YH", "A");

        }
    }
}
