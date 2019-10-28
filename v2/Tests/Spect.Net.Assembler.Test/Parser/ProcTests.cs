using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.Assembler.SyntaxTree.Statements;

namespace Spect.Net.Assembler.Test.Parser
{
    [TestClass]
    public class ProcTests : ParserTestBed
    {
        [TestMethod]
        [DataRow(".endp")]
        [DataRow("endp")]
        [DataRow(".ENDP")]
        [DataRow("ENDP")]
        [DataRow(".pend")]
        [DataRow("pend")]
        [DataRow(".PEND")]
        [DataRow("PEND")]
        public void EndProcParsingWorks(string source)
        {
            // --- Act
            var visitor = Parse(source);

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            visitor.Compilation.Lines[0].ShouldBeOfType<ProcEndStatement>();
        }

        [TestMethod]
        [DataRow(".proc")]
        [DataRow("proc")]
        [DataRow(".PROC")]
        [DataRow("PROC")]
        public void LoopParsingWorks(string source)
        {
            // --- Act
            var visitor = Parse(source);

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0] as ProcStatement;
            line.ShouldNotBeNull();
        }
    }
}