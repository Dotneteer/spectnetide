using AntlrZ80Asm.SyntaxTree;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;

namespace AntlrZ80Asm.Test.Parser
{
    [TestClass]
    public class TextSpanTests : ParserTestBed
    {
        [TestMethod]
        public void LabelSpanWorksAsExpected()
        {
            // --- Act
            var visitor = Parse("   StartLabel: ld a,b");

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0];
            line.ShouldNotBeNull();
            line.LabelSpan.Start.ShouldBe(3);
            line.LabelSpan.End.ShouldBe(13);
        }
    }
}