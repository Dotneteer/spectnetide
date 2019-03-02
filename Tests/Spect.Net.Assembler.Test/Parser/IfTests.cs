using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.Assembler.SyntaxTree.Expressions;
using Spect.Net.Assembler.SyntaxTree.Statements;

namespace Spect.Net.Assembler.Test.Parser
{
    [TestClass]
    public class IfTests : ParserTestBed
    {
        [TestMethod]
        [DataRow(".endif")]
        [DataRow(".ENDIF")]
        [DataRow("endif")]
        [DataRow("ENDIF")]
        public void EndifParsingWorks(string source)
        {
            // --- Act
            var visitor = Parse(source);

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            visitor.Compilation.Lines[0].ShouldBeOfType<IfEndStatement>();
        }

        [TestMethod]
        [DataRow(".else")]
        [DataRow(".ELSE")]
        [DataRow("else")]
        [DataRow("ELSE")]
        public void ElseParsingWorks(string source)
        {
            // --- Act
            var visitor = Parse(source);

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            visitor.Compilation.Lines[0].ShouldBeOfType<ElseStatement>();
        }

        [TestMethod]
        [DataRow(".if 3")]
        [DataRow(".IF true")]
        [DataRow("if #3")]
        [DataRow("IF %1111")]
        public void IfParsingWorks(string source)
        {
            // --- Act
            var visitor = Parse(source);

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0] as IfStatement;
            line.ShouldNotBeNull();
            line.Expr.ShouldBeOfType<LiteralNode>();
            line.Type.ShouldBe(IfStatementType.If);
        }

        [TestMethod]
        [DataRow(".elif 3")]
        [DataRow(".ELIF true")]
        [DataRow("elif #3")]
        [DataRow("ELIF %1111")]
        public void ElifParsingWorks(string source)
        {
            // --- Act
            var visitor = Parse(source);

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0] as ElifStatement;
            line.ShouldNotBeNull();
            line.Expr.ShouldBeOfType<LiteralNode>();
        }

        [TestMethod]
        [DataRow(".ifused id")]
        [DataRow(".IFUSED ::MyId")]
        [DataRow("ifused MyStruct.Field")]
        [DataRow("IFUSED MyModule.MyNested.MyId")]
        public void IfUsedParsingWorks(string source)
        {
            // --- Act
            var visitor = Parse(source);

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0] as IfStatement;
            line.ShouldNotBeNull();
            line.Expr.ShouldBeNull();
            line.Symbol.ShouldNotBeNull();
            line.Type.ShouldBe(IfStatementType.IfUsed);
        }

        [TestMethod]
        [DataRow(".ifnused id")]
        [DataRow(".IFNUSED ::MyId")]
        [DataRow("ifnused MyStruct.Field")]
        [DataRow("IFNUSED MyModule.MyNested.MyId")]
        public void IfNUsedParsingWorks(string source)
        {
            // --- Act
            var visitor = Parse(source);

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0] as IfStatement;
            line.ShouldNotBeNull();
            line.Expr.ShouldBeNull();
            line.Symbol.ShouldNotBeNull();
            line.Type.ShouldBe(IfStatementType.IfNotUsed);
        }
    }
}
