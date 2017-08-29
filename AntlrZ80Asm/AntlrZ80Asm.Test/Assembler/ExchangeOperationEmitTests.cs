using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AntlrZ80Asm.Test.Assembler
{
    [TestClass]
    public class ExchangeOperationEmitTests : AssemblerTestBed
    {
        [TestMethod]
        public void ExchangeOpsWorkAsExpected()
        {
            CodeEmitWorks("ex af,af'", 0x08);
            CodeEmitWorks("ex (sp),hl'", 0xE3);
            CodeEmitWorks("ex de,hl'", 0xEB);
            CodeEmitWorks("ex (sp),ix'", 0xDD, 0xE3);
            CodeEmitWorks("ex (sp),iy'", 0xFD, 0xE3);
        }
    }
}