using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.Assembler.SyntaxTree.Expressions;
using Spect.Net.Assembler.SyntaxTree.Statements;

namespace Spect.Net.Assembler.Test.Parser
{
    [TestClass]
    public class RepeatTests : ParserTestBed
    {
        [TestMethod]
        [DataRow(".repeat")]
        [DataRow("repeat")]
        [DataRow(".REPEAT")]
        [DataRow("REPEAT")]
        public void RepeatParsingWorks(string source)
        {
            // --- Act
            var visitor = Parse(source);

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            visitor.Compilation.Lines[0].ShouldBeOfType<RepeatStatement>();
        }

        [TestMethod]
        [DataRow(".until false")]
        [DataRow("until false")]
        [DataRow(".UNTIL false")]
        [DataRow("UNTIL false")]
        public void UntilParsingWorks(string source)
        {
            // --- Act
            var visitor = Parse(source);

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0] as UntilStatement;
            line.ShouldNotBeNull();
            line.Expr.ShouldBeOfType<LiteralNode>();
        }

    }
}
