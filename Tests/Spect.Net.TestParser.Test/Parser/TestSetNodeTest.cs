using Antlr4.Runtime;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.TestParser.Generated;
using Spect.Net.TestParser.SyntaxTree.Expressions;
using Spect.Net.TestParser.SyntaxTree.TestSet;

namespace Spect.Net.TestParser.Test.Parser
{
    [TestClass]
    public class TestSetNodeTest : ParserTestBed
    {
        [TestMethod]
        public void EmptyTestSetWorks()
        {
            // --- Act
            var visitor = Parse("testset sample { source \"a.test\"; }");

            // --- Assert
            visitor.Compilation.TestSets.Count.ShouldBe(1);

            var block = visitor.Compilation.TestSets[0];

            block.Span.StartLine.ShouldBe(1);
            block.Span.StartColumn.ShouldBe(0);
            block.Span.EndLine.ShouldBe(1);
            block.Span.EndColumn.ShouldBe(34);

            block.TestSetKeywordSpan.StartLine.ShouldBe(1);
            block.TestSetKeywordSpan.StartColumn.ShouldBe(0);
            block.TestSetKeywordSpan.EndLine.ShouldBe(1);
            block.TestSetKeywordSpan.EndColumn.ShouldBe(6);

            block.TestSetIdSpan.StartLine.ShouldBe(1);
            block.TestSetIdSpan.StartColumn.ShouldBe(8);
            block.TestSetIdSpan.EndLine.ShouldBe(1);
            block.TestSetIdSpan.EndColumn.ShouldBe(13);

            block.TestSetId.ShouldBe("sample");

            var sc = block.SourceContext;
            sc.ShouldNotBeNull();
            sc.SourceKeywordSpan.StartLine.ShouldBe(1);
            sc.SourceKeywordSpan.StartColumn.ShouldBe(17);
            sc.SourceKeywordSpan.EndLine.ShouldBe(1);
            sc.SourceKeywordSpan.EndColumn.ShouldBe(22);

            sc.SourceFileSpan.StartLine.ShouldBe(1);
            sc.SourceFileSpan.StartColumn.ShouldBe(24);
            sc.SourceFileSpan.EndLine.ShouldBe(1);
            sc.SourceFileSpan.EndColumn.ShouldBe(31);
            sc.SourceFile.ShouldBe("a.test");
        }

        [TestMethod]
        public void SourceContextWithNoSymbolWorks()
        {
            // --- Act
            var sc = ParseSourceContext("source \"a.test\";");

            // --- Assert

            sc.ShouldNotBeNull();
            sc.SourceKeywordSpan.StartLine.ShouldBe(1);
            sc.SourceKeywordSpan.StartColumn.ShouldBe(0);
            sc.SourceKeywordSpan.EndLine.ShouldBe(1);
            sc.SourceKeywordSpan.EndColumn.ShouldBe(5);

            sc.SourceFileSpan.StartLine.ShouldBe(1);
            sc.SourceFileSpan.StartColumn.ShouldBe(7);
            sc.SourceFileSpan.EndLine.ShouldBe(1);
            sc.SourceFileSpan.EndColumn.ShouldBe(14);
            sc.SourceFile.ShouldBe("a.test");
        }

        [TestMethod]
        public void SourceContextWithSingleSymbolWorks()
        {
            // --- Act
            var sc = ParseSourceContext("source \"a.test\" symbols Debug;");

            // --- Assert
            sc.ShouldNotBeNull();
            sc.SourceKeywordSpan.StartLine.ShouldBe(1);
            sc.SourceKeywordSpan.StartColumn.ShouldBe(0);
            sc.SourceKeywordSpan.EndLine.ShouldBe(1);
            sc.SourceKeywordSpan.EndColumn.ShouldBe(5);

            sc.SourceFileSpan.StartLine.ShouldBe(1);
            sc.SourceFileSpan.StartColumn.ShouldBe(7);
            sc.SourceFileSpan.EndLine.ShouldBe(1);
            sc.SourceFileSpan.EndColumn.ShouldBe(14);
            sc.SourceFile.ShouldBe("a.test");

            var skw = sc.SymbolsKeywordSpan;
            skw.ShouldNotBeNull();
            skw.Value.StartLine.ShouldBe(1);
            skw.Value.StartColumn.ShouldBe(16);
            skw.Value.EndLine.ShouldBe(1);
            skw.Value.EndColumn.ShouldBe(22);

            sc.Symbols.Count.ShouldBe(1);
            sc.Symbols[0].Id.ShouldBe("Debug");
        }

        [TestMethod]
        public void SourceContextWithMultipleSymbolWorks()
        {
            // --- Act
            var sc = ParseSourceContext("source \"a.test\" symbols Debug Other;");

            // --- Assert
            sc.ShouldNotBeNull();
            sc.SourceKeywordSpan.StartLine.ShouldBe(1);
            sc.SourceKeywordSpan.StartColumn.ShouldBe(0);
            sc.SourceKeywordSpan.EndLine.ShouldBe(1);
            sc.SourceKeywordSpan.EndColumn.ShouldBe(5);

            sc.SourceFileSpan.StartLine.ShouldBe(1);
            sc.SourceFileSpan.StartColumn.ShouldBe(7);
            sc.SourceFileSpan.EndLine.ShouldBe(1);
            sc.SourceFileSpan.EndColumn.ShouldBe(14);
            sc.SourceFile.ShouldBe("a.test");

            var skw = sc.SymbolsKeywordSpan;
            skw.ShouldNotBeNull();
            skw.Value.StartLine.ShouldBe(1);
            skw.Value.StartColumn.ShouldBe(16);
            skw.Value.EndLine.ShouldBe(1);
            skw.Value.EndColumn.ShouldBe(22);

            sc.Symbols.Count.ShouldBe(2);
            sc.Symbols[0].Id.ShouldBe("Debug");
            sc.Symbols[1].Id.ShouldBe("Other");
        }

        [TestMethod]
        public void TestSetWithMachineContextWorks()
        {
            // --- Act
            var mc = ParseMachineContext("machine Spectrum48;");

            // --- Assert
            mc.Id.ShouldBe("Spectrum48");

            var mk = mc.MachineKeywordSpan;
            mk.StartLine.ShouldBe(1);
            mk.StartColumn.ShouldBe(0);
            mk.EndLine.ShouldBe(1);
            mk.EndColumn.ShouldBe(6);

            var idk = mc.IdSpan;
            idk.StartLine.ShouldBe(1);
            idk.StartColumn.ShouldBe(8);
            idk.EndLine.ShouldBe(1);
            idk.EndColumn.ShouldBe(17);
        }

        [TestMethod]
        public void NonmiOptionWorks()
        {
            // --- Act
            var visitor = ParseTestOptions("with nonmi;");

            // --- Assert
            var kw = visitor.WithKeywordSpan;
            kw.StartLine.ShouldBe(1);
            kw.StartColumn.ShouldBe(0);
            kw.EndLine.ShouldBe(1);
            kw.EndColumn.ShouldBe(3);

            visitor.Options.Count.ShouldBe(1);
            var option = visitor.Options[0] as NoNmiTestOptionNode;
            option.ShouldNotBeNull();
            option.Span.StartLine.ShouldBe(1);
            option.Span.StartColumn.ShouldBe(5);
            option.Span.EndLine.ShouldBe(1);
            option.Span.EndColumn.ShouldBe(9);

            var okw = option.OptionKeywordSpan;
            okw.StartLine.ShouldBe(1);
            okw.StartColumn.ShouldBe(5);
            okw.EndLine.ShouldBe(1);
            okw.EndColumn.ShouldBe(9);
        }

        [TestMethod]
        public void TimeoutOptionWorks()
        {
            // --- Act
            var visitor = ParseTestOptions("with timeout 1000;");

            // --- Assert
            var kw = visitor.WithKeywordSpan;
            kw.StartLine.ShouldBe(1);
            kw.StartColumn.ShouldBe(0);
            kw.EndLine.ShouldBe(1);
            kw.EndColumn.ShouldBe(3);

            visitor.Options.Count.ShouldBe(1);
            var option = visitor.Options[0] as TimeoutTestOptionNode;
            option.ShouldNotBeNull();
            option.Span.StartLine.ShouldBe(1);
            option.Span.StartColumn.ShouldBe(5);
            option.Span.EndLine.ShouldBe(1);
            option.Span.EndColumn.ShouldBe(16);
            option.Expr.ShouldBeOfType<LiteralNode>();

            var okw = option.OptionKeywordSpan;
            okw.StartLine.ShouldBe(1);
            okw.StartColumn.ShouldBe(5);
            okw.EndLine.ShouldBe(1);
            okw.EndColumn.ShouldBe(11);
        }

        [TestMethod]
        public void MultipleOptionsWork1()
        {
            // --- Act
            var visitor = ParseTestOptions("with timeout 1000, nonmi;");

            // --- Assert
            visitor.Options.Count.ShouldBe(2);
            var option1 = visitor.Options[0] as TimeoutTestOptionNode;
            option1.ShouldNotBeNull();
            option1.Expr.ShouldBeOfType<LiteralNode>();
            var option2 = visitor.Options[1] as NoNmiTestOptionNode;
            option2.ShouldNotBeNull();
        }

        [TestMethod]
        public void MultipleOptionsWork2()
        {
            // --- Act
            var visitor = ParseTestOptions("with nonmi, timeout 1000;");

            // --- Assert
            visitor.Options.Count.ShouldBe(2);
            var option1 = visitor.Options[0] as NoNmiTestOptionNode;
            option1.ShouldNotBeNull();
            var option2 = visitor.Options[1] as TimeoutTestOptionNode;
            option2.ShouldNotBeNull();
            option2.Expr.ShouldBeOfType<LiteralNode>();
        }

        /// <summary>
        /// Returns a visitor with the results of a single parsing pass
        /// </summary>
        /// <param name="textToParse">Z80 assembly code to parse</param>
        /// <param name="expectedErrors">Number of errors expected</param>
        /// <returns>
        /// Visitor with the syntax tree
        /// </returns>
        private static MachineContextNode ParseMachineContext(string textToParse, int expectedErrors = 0)
        {
            var inputStream = new AntlrInputStream(textToParse);
            var lexer = new Z80TestLexer(inputStream);
            var tokenStream = new CommonTokenStream(lexer);
            var parser = new Z80TestParser(tokenStream);
            var context = parser.machineContext();
            var visitor = new Z80TestVisitor();
            var result = (MachineContextNode)visitor.VisitMachineContext(context);
            parser.SyntaxErrors.Count.ShouldBe(expectedErrors);
            return result;
        }

        /// <summary>
        /// Returns a visitor with the results of a single parsing pass
        /// </summary>
        /// <param name="textToParse">Z80 assembly code to parse</param>
        /// <param name="expectedErrors">Number of errors expected</param>
        /// <returns>
        /// Visitor with the syntax tree
        /// </returns>
        private static SourceContextNode ParseSourceContext(string textToParse, int expectedErrors = 0)
        {
            var inputStream = new AntlrInputStream(textToParse);
            var lexer = new Z80TestLexer(inputStream);
            var tokenStream = new CommonTokenStream(lexer);
            var parser = new Z80TestParser(tokenStream);
            var context = parser.sourceContext();
            var visitor = new Z80TestVisitor();
            var result = (SourceContextNode)visitor.VisitSourceContext(context);
            parser.SyntaxErrors.Count.ShouldBe(expectedErrors);
            return result;
        }

        /// <summary>
        /// Returns a visitor with the results of a single parsing pass
        /// </summary>
        /// <param name="textToParse">Z80 assembly code to parse</param>
        /// <param name="expectedErrors">Number of errors expected</param>
        /// <returns>
        /// Visitor with the syntax tree
        /// </returns>
        private static TestOptionsNode ParseTestOptions(string textToParse, int expectedErrors = 0)
        {
            var inputStream = new AntlrInputStream(textToParse);
            var lexer = new Z80TestLexer(inputStream);
            var tokenStream = new CommonTokenStream(lexer);
            var parser = new Z80TestParser(tokenStream);
            var context = parser.testOptions();
            var visitor = new Z80TestVisitor();
            var result = (TestOptionsNode)visitor.VisitTestOptions(context);
            parser.SyntaxErrors.Count.ShouldBe(expectedErrors);
            return result;
        }

    }
}
