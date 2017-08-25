using AntlrZ80Asm.SyntaxTree;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;

namespace AntlrZ80Asm.Test.Parser
{
    [TestClass]
    public class LoadRegToMemAddrTests: ParserTestBed
    {
        [TestMethod]
        public void LoadRegFromMemWorksAsExpected()
        {
            InstructionWorksAsExpected("ld (#1234),a", "A");
            InstructionWorksAsExpected("ld (#1234),bc", "BC");
            InstructionWorksAsExpected("ld (#1234),de", "DE");
            InstructionWorksAsExpected("ld (#1234),hl", "HL");
            InstructionWorksAsExpected("ld (#1234),ix", "IX");
            InstructionWorksAsExpected("ld (#1234),iy", "IY");
        }

        protected void InstructionWorksAsExpected(string instruction, string source)
        {
            // --- Act
            var visitor = Parse(instruction);

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0] as LoadRegToMemAddrInstruction;
            line.ShouldNotBeNull();
            line.Source.ShouldBe(source);
            line.Destination.ShouldNotBeNull();
        }
    }
}