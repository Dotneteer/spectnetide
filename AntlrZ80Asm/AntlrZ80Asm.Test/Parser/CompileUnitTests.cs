using AntlrZ80Asm.SyntaxTree.Operations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;

namespace AntlrZ80Asm.Test.Parser
{
    [TestClass]
    public class CompileUnitTests: ParserTestBed
    {
        [TestMethod]
        public void EmptyCompileUnitWorks()
        {
            // --- Act
            var visitor = Parse("");

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(0);
        }

        [TestMethod]
        public void LabelsWork()
        {
            // --- Act
            var visitor = Parse("startLabel nop");

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0];
            line.Label.ShouldBe("STARTLABEL");
        }

        [TestMethod]
        public void LabelsWorkWithColon()
        {
            // --- Act
            var visitor = Parse("startLabel: nop");

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0];
            line.Label.ShouldBe("STARTLABEL");
        }

        [TestMethod]
        public void FullLineCommentsWorks()
        {
            // --- Act
            var visitor = Parse("; This is a comment");

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(0);
        }

        [TestMethod]
        public void EndLineCommentsWork()
        {
            // --- Act
            var visitor = Parse("startLabel nop; this is a comment");

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0];
            line.ShouldBeOfType<TrivialOperation>();
            line.Label.ShouldBe("STARTLABEL");
        }
    }

}
