using System.Diagnostics.Eventing.Reader;
using AntlrZ80Asm.SyntaxTree;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;

namespace AntlrZ80Asm.Test.Parser
{
    [TestClass]
    public class LoadReg8ToReg16AddrTests: ParserTestBed
    {
        [TestMethod]
        public void Load8BitToReg16AddrWorksAsExpected()
        {
            InstructionWorksAsExpected("ld (bc), a", "A", "BC");
            InstructionWorksAsExpected("ld (de), a", "A", "DE");
        }

        [TestMethod]
        public void Load8BitToIndexedAddrWorksAsExpected()
        {
            InstructionWorksAsExpected("ld (ix), a", "A", "IX", null);
            InstructionWorksAsExpected("ld (ix+#02), a", "A", "IX", "+");
            InstructionWorksAsExpected("ld (ix - #02), a", "A", "IX", "-");
            InstructionWorksAsExpected("ld (iy), a", "A", "IY", null);
            InstructionWorksAsExpected("ld (iy+#02), a", "A", "IY", "+");
            InstructionWorksAsExpected("ld (iy - #02), a", "A", "IY", "-");
        }

        protected void InstructionWorksAsExpected(string instruction, string source, string dest)
        {
            // --- Act
            var visitor = Parse(instruction);

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0] as LoadReg8ToRegAddrInstruction;
            line.ShouldNotBeNull();
            line.Source.ShouldBe(source);
            line.Destination.ShouldBe(dest);
        }

        protected void InstructionWorksAsExpected(string instruction, string source, string reg, string sign)
        {
            // --- Act
            var visitor = Parse(instruction);

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0] as LoadReg8ToRegAddrInstruction;
            line.ShouldNotBeNull();
            line.Source.ShouldBe(source);
            line.Destination.ShouldBeNull();
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
