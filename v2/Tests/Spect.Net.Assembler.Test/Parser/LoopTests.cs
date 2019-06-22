using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.Assembler.SyntaxTree.Expressions;
using Spect.Net.Assembler.SyntaxTree.Statements;

namespace Spect.Net.Assembler.Test.Parser
{
    [TestClass]
    public class LoopTests : ParserTestBed
    {
        [TestMethod]
        [DataRow(".endl")]
        [DataRow("endl")]
        [DataRow(".ENDL")]
        [DataRow("ENDL")]
        [DataRow(".lend")]
        [DataRow("lend")]
        [DataRow(".LEND")]
        [DataRow("LEND")]
        public void EndLoopParsingWorks(string source)
        {
            // --- Act
            var visitor = Parse(source);

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            visitor.Compilation.Lines[0].ShouldBeOfType<LoopEndStatement>();
        }

        [TestMethod]
        [DataRow(".loop #3")]
        [DataRow("loop 123")]
        [DataRow(".LOOP #34")]
        [DataRow("LOOP %1111")]
        public void LoopParsingWorks(string source)
        {
            // --- Act
            var visitor = Parse(source);

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0] as LoopStatement;
            line.ShouldNotBeNull();
            line.Expression.ShouldBeOfType<LiteralNode>();
        }

        [TestMethod]
        [DataRow(".break")]
        [DataRow("break")]
        [DataRow(".BREAK")]
        [DataRow("BREAK")]
        public void BreakParsingWorks(string source)
        {
            // --- Act
            var visitor = Parse(source);

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            visitor.Compilation.Lines[0].ShouldBeOfType<BreakStatement>();
        }

        [TestMethod]
        [DataRow(".continue")]
        [DataRow("continue")]
        [DataRow(".CONTINUE")]
        [DataRow("CONTINUE")]
        public void ContinueParsingWorks(string source)
        {
            // --- Act
            var visitor = Parse(source);

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            visitor.Compilation.Lines[0].ShouldBeOfType<ContinueStatement>();
        }
    }
}
