using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.TestParser.SyntaxTree;

namespace Spect.Net.TestParser.Test.Parser
{
    [TestClass]
    public class IncludeDirectiveTest: ParserTestBed
    {
        [TestMethod]
        public void IncludeDirectiveWorks()
        {
            // --- Act
            var visitor = Parse("include \"a.test\"");

            // --- Assert
            visitor.Compilation.LanguageBlocks.Count.ShouldBe(1);
            var block = visitor.Compilation.LanguageBlocks[0] as IncludeDirective;
            block.ShouldNotBeNull();
            block.Span.StartLine.ShouldBe(1);
            block.Span.StartColumn.ShouldBe(0);
            block.Span.EndLine.ShouldBe(1);
            block.Span.EndColumn.ShouldBe(15);

            block.IncludeKeywordSpan.StartLine.ShouldBe(1);
            block.IncludeKeywordSpan.StartColumn.ShouldBe(0);
            block.IncludeKeywordSpan.EndLine.ShouldBe(1);
            block.IncludeKeywordSpan.EndColumn.ShouldBe(6);

            block.Filename.ShouldBe("a.test");
        }

        [TestMethod]
        public void IncludeDirectiveWithMultipleLinesWorks()
        {
            // --- Act
            var visitor = Parse("include \r\n   \"a.test\"");

            // --- Assert
            visitor.Compilation.LanguageBlocks.Count.ShouldBe(1);
            var block = visitor.Compilation.LanguageBlocks[0] as IncludeDirective;
            block.ShouldNotBeNull();
            block.Span.StartLine.ShouldBe(1);
            block.Span.StartColumn.ShouldBe(0);
            block.Span.EndLine.ShouldBe(2);
            block.Span.EndColumn.ShouldBe(10);

            block.StringSpan.StartLine.ShouldBe(2);
            block.StringSpan.StartColumn.ShouldBe(3);
            block.StringSpan.EndLine.ShouldBe(2);
            block.StringSpan.EndColumn.ShouldBe(10);
            block.Filename.ShouldBe("a.test");
        }
    }
}