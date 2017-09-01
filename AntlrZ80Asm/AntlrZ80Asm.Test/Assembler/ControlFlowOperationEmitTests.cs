using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AntlrZ80Asm.Test.Assembler
{
    [TestClass]
    public class ControlFlowOperationEmitTests : AssemblerTestBed
    {
        [TestMethod]
        public void DjnzOpWorksAsExpected()
        {
            // --- Stard address is #8000!
            CodeEmitWorks("djnz #8022", 0x10, 0x20);
            CodeEmitWorks("djnz #8000", 0x10, 0xFE);
            CodeEmitWorks("djnz #8081", 0x10, 0x7F);
            CodeEmitWorks("djnz #7F82", 0x10, 0x80);
            CodeEmitWorks("djnz #7F84", 0x10, 0x82);
        }

        [TestMethod]
        public void DjnzFailsWithFarAddress()
        {
            // --- Stard address is #8000!
            CodeRaisesRelativeAddressError("djnz #8082");
            CodeRaisesRelativeAddressError("djnz #8100");
            CodeRaisesRelativeAddressError("djnz #7F81");
            CodeRaisesRelativeAddressError("djnz #7F00");
        }

        [TestMethod]
        public void RstOpsWorkAsExpected()
        {
            CodeEmitWorks("rst 0", 0xC7);
            CodeEmitWorks("rst 8", 0xCF);
            CodeEmitWorks("rst #08", 0xCF);
            CodeEmitWorks("rst 08h", 0xCF);
            CodeEmitWorks("rst 08H", 0xCF);
            CodeEmitWorks("rst #10", 0xD7);
            CodeEmitWorks("rst #18", 0xDF);
            CodeEmitWorks("rst #20", 0xE7);
            CodeEmitWorks("rst #28", 0xEF);
            CodeEmitWorks("rst #30", 0xF7);
            CodeEmitWorks("rst #38", 0xFF);
        }

        [TestMethod]
        public void InvalidRstValueRaisesError()
        {
            CodeRaisesInvalidArgument("rst 23");
        }

    }
}