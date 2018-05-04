using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.Assembler.SyntaxTree.Expressions;
using Spect.Net.Assembler.SyntaxTree.Statements;

namespace Spect.Net.Assembler.Test.Parser
{
    [TestClass]
    public class WhileTests: ParserTestBed
    {
        [TestMethod]
        [DataRow(".endw")]
        [DataRow("endw")]
        [DataRow(".ENDW")]
        [DataRow("ENDW")]
        [DataRow(".wend")]
        [DataRow("wend")]
        [DataRow(".WEND")]
        [DataRow("WEND")]
        public void EndWhileParsingWorks(string source)
        {
            // --- Act
            var visitor = Parse(source);

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            visitor.Compilation.Lines[0].ShouldBeOfType<WhileEndStatement>();
        }

        [TestMethod]
        [DataRow(".while #3")]
        [DataRow("while 123")]
        [DataRow(".WHILE #34")]
        [DataRow("WHILE %1111")]
        public void WhileParsingWorks(string source)
        {
            // --- Act
            var visitor = Parse(source);

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0] as WhileStatement;
            line.ShouldNotBeNull();
            line.Expr.ShouldBeOfType<LiteralNode>();
        }
    }
}
