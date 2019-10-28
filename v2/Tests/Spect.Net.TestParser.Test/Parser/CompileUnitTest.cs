using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;

namespace Spect.Net.TestParser.Test.Parser
{
    [TestClass]
    public class CompileUnitTest: ParserTestBed
    {
        [TestMethod]
        public void EmptyCompileUnitWorks()
        {
            // --- Act
            var visitor = Parse("");

            // --- Assert
            visitor.Compilation.TestSets.Count.ShouldBe(0);
        }
    }
}
