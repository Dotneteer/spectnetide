using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
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

            var block = visitor.Compilation.TestSets[0] as TestSetNode;
            block.ShouldNotBeNull();

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
        }
    }
}
