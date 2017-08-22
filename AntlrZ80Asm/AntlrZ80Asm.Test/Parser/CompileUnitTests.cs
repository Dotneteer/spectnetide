using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;

namespace AntlrZ80Asm.Test.Parser
{
    [TestClass]
    public class CompileUnitTests: TestBed
    {
        [TestMethod]
        public void EmptyCompileUnitWorks()
        {
            // --- Act
            var visitor = Parse("");

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(0);
        }
    }
}
