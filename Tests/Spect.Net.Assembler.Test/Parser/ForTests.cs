using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.Assembler.SyntaxTree.Statements;

namespace Spect.Net.Assembler.Test.Parser
{
    [TestClass]
    public class ForTests: ParserTestBed
    {
        [TestMethod]
        [DataRow(".next")]
        [DataRow("next")]
        [DataRow(".NEXT")]
        [DataRow("NEXT")]
        public void NextParsingWorks(string source)
        {
            // --- Act
            var visitor = Parse(source);

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            visitor.Compilation.Lines[0].ShouldBeOfType<NextStatement>();
        }

    }
}
