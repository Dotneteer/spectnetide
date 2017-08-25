using AntlrZ80Asm.SyntaxTree;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;

namespace AntlrZ80Asm.Test.Parser
{
    [TestClass]
    public class LoadReg8FromReg16AddrTests: ParserTestBed
    {
        [TestMethod]
        public void Load8BitToReg16AddrWorksAsExpected()
        {
            InstructionWorksAsExpected("ld a,(bc)", "BC", "A");
            InstructionWorksAsExpected("ld a,(de), a", "DE", "A");
        }

        [TestMethod]
        public void Load8BitToIndexedAddrWorksAsExpected()
        {
            InstructionWorksAsExpected("ld a,(ix)", "A", "IX", null);
            InstructionWorksAsExpected("ld a,(ix+#2)", "A", "IX", "+");
            InstructionWorksAsExpected("ld a,(ix-#2)", "A", "IX", "-");
            InstructionWorksAsExpected("ld a,(iy)", "A", "IY", null);
            InstructionWorksAsExpected("ld a,(iy+#2)", "A", "IY", "+");
            InstructionWorksAsExpected("ld a,(iy-#2)", "A", "IY", "-");
        }

        protected void InstructionWorksAsExpected(string instruction, string source, string dest)
        {
            // --- Act
            var visitor = Parse(instruction);

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0] as LoadReg8FromRegAddrInstruction;
            line.ShouldNotBeNull();
            line.Source.ShouldBe(source);
            line.Destination.ShouldBe(dest);
        }

        protected void InstructionWorksAsExpected(string instruction, string dest, string reg, string sign)
        {
            // --- Act
            var visitor = Parse(instruction);

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0] as LoadReg8FromRegAddrInstruction;
            line.ShouldNotBeNull();
            line.Destination.ShouldBe(dest);
            line.IndexedAddress.Register.ShouldBe(reg);
            line.IndexedAddress.Sign.ShouldBe(sign);
            if (sign == null)
            {
                line.IndexedAddress.Displacement.ShouldBeNull();
            }
            else
            {
                line.IndexedAddress.Displacement.ShouldNotBeNull();
            }
        }
    }
}