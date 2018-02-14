using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Spect.Net.Assembler.Test.Assembler
{
    [TestClass]
    public class LoadOperationRegressionTests : AssemblerTestBed
    {
        [TestMethod]
        public void LoadWithCharLiteralWorks()
        {
            CodeEmitWorks("ld a,\"0\"", 0x3E, 0x30);
            CodeEmitWorks("ld a,'0'", 0x3E, 0x30);
        }
    }
}
