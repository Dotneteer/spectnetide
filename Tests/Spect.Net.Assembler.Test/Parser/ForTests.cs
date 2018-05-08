using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.Assembler.SyntaxTree.Expressions;
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

        [TestMethod]
        [DataRow("for myVar = 0 to 100")]
        [DataRow(".for myVar = 0 .to 100")]
        [DataRow("FOR myVar = 0 TO 100")]
        [DataRow(".FOR myVar = 0 .TO 100")]
        public void ForParsingWorkWithoutStep(string source)
        {
            // --- Act
            var visitor = Parse(source);

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0] as ForStatement;
            line.ShouldNotBeNull();
            line.ForVariable.ShouldBe("MYVAR");
            line.From.ShouldBeOfType<LiteralNode>();
            line.To.ShouldBeOfType<LiteralNode>();
            line.Step.ShouldBeNull();
        }
    }
}
