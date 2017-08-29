using AntlrZ80Asm.SyntaxTree;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;

namespace AntlrZ80Asm.Test.Parser
{
    [TestClass]
    public class PreprocessorTests : ParserTestBed
    {
        [TestMethod]
        public void PreprocessorDirectivesWorkAsExpected()
        {
            PreprocessingWorks("#ifdef myId", "#IFDEF", "MYID");
            PreprocessingWorks("#ifndef myId", "#IFNDEF", "MYID");
            PreprocessingWorks("#define myId", "#DEFINE", "MYID");
            PreprocessingWorks("#undef myId", "#UNDEF", "MYID");
            PreprocessingWorks("#else", "#ELSE", null);
            PreprocessingWorks("#endif", "#ENDIF", null);
        }

        protected void PreprocessingWorks(string instruction, string mnemonic, string identifier)
        {
            // --- Act
            var visitor = Parse(instruction);

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0] as PreprocessorDirective;
            line.ShouldNotBeNull();
            line.Mnemonic.ShouldBe(mnemonic);
            line.Identifier.ShouldBe(identifier);
        }
    }
}
