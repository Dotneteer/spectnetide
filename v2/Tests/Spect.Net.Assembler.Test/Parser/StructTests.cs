using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.Assembler.SyntaxTree.Statements;

namespace Spect.Net.Assembler.Test.Parser
{
    [TestClass]
    public class StructTests : ParserTestBed
    {
        [TestMethod]
        [DataRow(".ends")]
        [DataRow("ends")]
        [DataRow(".ENDS")]
        [DataRow("ENDS")]
        public void EndStructParsingWorks(string source)
        {
            // --- Act
            var visitor = Parse(source);

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            visitor.Compilation.Lines[0].ShouldBeOfType<StructEndStatement>();
        }

        [TestMethod]
        [DataRow(".struct")]
        [DataRow("struct")]
        [DataRow(".STRUCT")]
        [DataRow("STRUCT")]
        public void StructParsingWorks(string source)
        {
            // --- Act
            var visitor = Parse(source);

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0] as StructStatement;
            line.ShouldNotBeNull();
        }
    }
}