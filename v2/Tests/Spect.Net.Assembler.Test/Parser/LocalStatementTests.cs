using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.Assembler.SyntaxTree.Statements;

namespace Spect.Net.Assembler.Test.Parser
{
    [TestClass]
    public class LocalStatementTests : ParserTestBed
    {
        [TestMethod]
        [DataRow("local amount")]
        [DataRow("LOCAL id")]
        [DataRow("LOCAL id1, id2, id3")]
        [DataRow(".local amount")]
        [DataRow(".LOCAL id")]
        public void LocalStatementParsingWorks(string source)
        {
            // --- Act
            var visitor = Parse(source);

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            visitor.Compilation.Lines[0].ShouldBeOfType<LocalStatement>();
        }

    }
}
