using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.Assembler.SyntaxTree.Operations;

namespace Spect.Net.Assembler.Test.Parser
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
        public void LabelOnlyLineWorksWithColon()
        {
            // --- Act
            var visitor = Parse("startLabel:");

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
            visitor.Compilation.Lines.Count.ShouldBe(1);
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

        [TestMethod]
        public void EndLineCommentsWorkWithMultipleLines()
        {
            // --- Act
            var visitor = Parse(@"
                startLabel nop; this is a comment
                nop");

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(2);
            var line = visitor.Compilation.Lines[0];
            line.ShouldBeOfType<TrivialOperation>();
            line.Label.ShouldBe("STARTLABEL");
        }

        [TestMethod]
        public void ErrorIsCaughtAfterTheOperation()
        {
            // --- Act
            var errors = ParseWithErrors("nop other");

            // --- Assert
            errors.Count.ShouldBe(1);
            var errorLine = errors[0];
            errorLine.SourceLine.ShouldBe(1);
            errorLine.Position.ShouldBe(4);
            errorLine.Token.ShouldBe("other");
        }

        [TestMethod]
        public void InvalidTokenIsCaught1()
        {
            // --- Act
            var errors = ParseWithErrors("984Qer");

            // --- Assert
            errors.Count.ShouldBe(1);
            var errorLine = errors[0];
            errorLine.SourceLine.ShouldBe(1);
            errorLine.Position.ShouldBe(0);
            errorLine.Token.ShouldBe("984");
        }
    }

}
