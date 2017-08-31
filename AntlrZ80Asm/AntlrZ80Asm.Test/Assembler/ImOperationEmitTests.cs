using AntlrZ80Asm.Assembler;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;

namespace AntlrZ80Asm.Test.Assembler
{
    [TestClass]
    public class ImOperationEmitTests : AssemblerTestBed
    {
        [TestMethod]
        public void ImOpsWorkAsExpected()
        {
            CodeEmitWorks("im 0", 0xED, 0x46);
            CodeEmitWorks("im 1", 0xED, 0x56);
            CodeEmitWorks("im 2", 0xED, 0x5E);
        }

        [TestMethod]
        public void InvalidImModeRaisesError()
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile("im 3");

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ShouldBeOfType<InvalidArgumentError>();
        }

    }
}