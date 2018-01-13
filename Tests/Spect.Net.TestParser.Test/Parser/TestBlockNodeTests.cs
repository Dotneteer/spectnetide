using Antlr4.Runtime;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.TestParser.Generated;
using Spect.Net.TestParser.SyntaxTree.Expressions;
using Spect.Net.TestParser.SyntaxTree.TestSet;

namespace Spect.Net.TestParser.Test.Parser
{
    [TestClass]
    public class TestBlockNodeTests
    {
        [TestMethod]
        public void EmptyTestBlockWorks()
        {
            // --- Act
            var visitor = ParseTestBlock("test sample { act call #1234; }");

            // --- Assert
            visitor.TestKeywordSpan.StartLine.ShouldBe(1);
            visitor.TestKeywordSpan.StartColumn.ShouldBe(0);
            visitor.TestKeywordSpan.EndLine.ShouldBe(1);
            visitor.TestKeywordSpan.EndColumn.ShouldBe(3);

            visitor.TestIdSpan.StartLine.ShouldBe(1);
            visitor.TestIdSpan.StartColumn.ShouldBe(5);
            visitor.TestIdSpan.EndLine.ShouldBe(1);
            visitor.TestIdSpan.EndColumn.ShouldBe(10);

            visitor.TestId.ShouldBe("sample");
        }

        [TestMethod]
        public void TestBlockWithCategoryWorks()
        {
            // --- Act
            var visitor = ParseTestBlock("test sample { category hard; act call #1234; }");

            // --- Assert
            visitor.TestKeywordSpan.StartLine.ShouldBe(1);
            visitor.TestKeywordSpan.StartColumn.ShouldBe(0);
            visitor.TestKeywordSpan.EndLine.ShouldBe(1);
            visitor.TestKeywordSpan.EndColumn.ShouldBe(3);

            visitor.TestIdSpan.StartLine.ShouldBe(1);
            visitor.TestIdSpan.StartColumn.ShouldBe(5);
            visitor.TestIdSpan.EndLine.ShouldBe(1);
            visitor.TestIdSpan.EndColumn.ShouldBe(10);

            visitor.TestId.ShouldBe("sample");

            visitor.CategoryKeywordSpan.ShouldNotBeNull();
            var cs = visitor.CategoryKeywordSpan.Value;
            cs.StartLine.ShouldBe(1);
            cs.StartColumn.ShouldBe(14);
            cs.EndLine.ShouldBe(1);
            cs.EndColumn.ShouldBe(21);

            visitor.CategoryIdSpan.ShouldNotBeNull();
            cs = visitor.CategoryIdSpan.Value;
            cs.StartLine.ShouldBe(1);
            cs.StartColumn.ShouldBe(23);
            cs.EndLine.ShouldBe(1);
            cs.EndColumn.ShouldBe(26);

            visitor.Category.ShouldBe("hard");
        }

        [TestMethod]
        public void TestBlockWithActCallWorks()
        {
            // --- Act
            var visitor = ParseTestBlock("test sample { act call #1234; }");

            // --- Assert
            var act = visitor.Act;
            act.ShouldNotBeNull();
            act.IsCall.ShouldBeTrue();
            act.StartExpr.ShouldBeOfType<LiteralNode>();
            var kw = act.CallOrStartSpan;
            kw.StartLine.ShouldBe(1);
            kw.StartColumn.ShouldBe(18);
            kw.EndLine.ShouldBe(1);
            kw.EndColumn.ShouldBe(21);
        }

        [TestMethod]
        public void TestBlockWithStartWorks()
        {
            // --- Act
            var visitor = ParseTestBlock("test sample { act start #1234 stop #1235; }");

            // --- Assert
            var act = visitor.Act;
            act.ShouldNotBeNull();
            act.IsCall.ShouldBeFalse();
            act.StartExpr.ShouldBeOfType<LiteralNode>();
            var kw = act.CallOrStartSpan;
            kw.StartLine.ShouldBe(1);
            kw.StartColumn.ShouldBe(18);
            kw.EndLine.ShouldBe(1);
            kw.EndColumn.ShouldBe(22);

            act.StopExpr.ShouldBeOfType<LiteralNode>();
            kw = act.StopOrHaltSpan;
            kw.StartLine.ShouldBe(1);
            kw.StartColumn.ShouldBe(30);
            kw.EndLine.ShouldBe(1);
            kw.EndColumn.ShouldBe(33);
        }

        [TestMethod]
        public void TestBlockWithHaltWorks()
        {
            // --- Act
            var visitor = ParseTestBlock("test sample { act start #1234 halt; }");

            // --- Assert
            var act = visitor.Act;
            act.ShouldNotBeNull();
            act.IsCall.ShouldBeFalse();
            act.StartExpr.ShouldBeOfType<LiteralNode>();
            var kw = act.CallOrStartSpan;
            kw.StartLine.ShouldBe(1);
            kw.StartColumn.ShouldBe(18);
            kw.EndLine.ShouldBe(1);
            kw.EndColumn.ShouldBe(22);

            act.IsHalt.ShouldBeTrue();
            act.StopExpr.ShouldBeNull();
            kw = act.StopOrHaltSpan;
            kw.StartLine.ShouldBe(1);
            kw.StartColumn.ShouldBe(30);
            kw.EndLine.ShouldBe(1);
            kw.EndColumn.ShouldBe(33);
        }

        [TestMethod]
        public void TestBlockWithNoNmiWorks()
        {
            // --- Act
            var visitor = ParseTestBlock("test sample { with nonmi; act call #1234; }");

            // --- Assert
            visitor.TestOptions.Options.Count.ShouldBe(1);
            visitor.TestOptions.Options[0].ShouldBeOfType<NoNmiTestOptionNode>();
        }

        [TestMethod]
        public void TestBlockWithTimeoutWorks()
        {
            // --- Act
            var visitor = ParseTestBlock("test sample { with timeout 1000; act call #1234; }");

            // --- Assert
            visitor.TestOptions.Options.Count.ShouldBe(1);
            var timeout = visitor.TestOptions.Options[0] as TimeoutTestOptionNode;
            timeout.ShouldNotBeNull();
            timeout.Expr.ShouldBeOfType<LiteralNode>();
        }

        [TestMethod]
        public void TestBlockWithTimeoutAndNoNmiWorks()
        {
            // --- Act
            var visitor = ParseTestBlock("test sample { with timeout 1000, nonmi; act call #1234; }");

            // --- Assert
            visitor.TestOptions.Options.Count.ShouldBe(2);
            var timeout = visitor.TestOptions.Options[0] as TimeoutTestOptionNode;
            timeout.ShouldNotBeNull();
            timeout.Expr.ShouldBeOfType<LiteralNode>();
            visitor.TestOptions.Options[1].ShouldBeOfType<NoNmiTestOptionNode>();
        }

        [TestMethod]
        public void TestBlockWithSingleParamWorks()
        {
            // --- Act
            var visitor = ParseTestBlock("test sample { params ab; act call #1234; }");

            // --- Assert
            visitor.Params.Count.ShouldBe(1);
            visitor.ParamsKeywordSpan.ShouldNotBeNull();
            var kw = visitor.ParamsKeywordSpan.Value;
            kw.StartLine.ShouldBe(1);
            kw.StartColumn.ShouldBe(14);
            kw.EndLine.ShouldBe(1);
            kw.EndColumn.ShouldBe(19);

            var param = visitor.Params[0];
            param.Span.StartLine.ShouldBe(1);
            param.Span.StartColumn.ShouldBe(21);
            param.Span.EndLine.ShouldBe(1);
            param.Span.EndColumn.ShouldBe(22);
            param.Id.ShouldBe("ab");
        }

        [TestMethod]
        public void TestBlockWithMultipleParamWorks()
        {
            // --- Act
            var visitor = ParseTestBlock("test sample { params par1, par2, par3; act call #1234; }");

            // --- Assert
            visitor.Params.Count.ShouldBe(3);
            visitor.ParamsKeywordSpan.ShouldNotBeNull();

            visitor.Params[0].Id.ShouldBe("par1");
            visitor.Params[1].Id.ShouldBe("par2");
            visitor.Params[2].Id.ShouldBe("par3");
        }

        /// <summary>
        /// Returns a visitor with the results of a single parsing pass
        /// </summary>
        /// <param name="textToParse">Z80 assembly code to parse</param>
        /// <param name="expectedErrors">Number of errors expected</param>
        /// <returns>
        /// Visitor with the syntax tree
        /// </returns>
        private static TestBlockNode ParseTestBlock(string textToParse, int expectedErrors = 0)
        {
            var inputStream = new AntlrInputStream(textToParse);
            var lexer = new Z80TestLexer(inputStream);
            var tokenStream = new CommonTokenStream(lexer);
            var parser = new Z80TestParser(tokenStream);
            var context = parser.testBlock();
            var visitor = new Z80TestVisitor();
            var result = (TestBlockNode)visitor.VisitTestBlock(context);
            parser.SyntaxErrors.Count.ShouldBe(expectedErrors);
            return result;
        }

    }
}
