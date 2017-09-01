using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AntlrZ80Asm.Test.Assembler
{
    [TestClass]
    public class TrivialOperationEmitTests : AssemblerTestBed
    {
        [TestMethod]
        public void StandardTrivialOpsWorkAsExpected()
        {
            CodeEmitWorks("nop", 0x00);
            CodeEmitWorks("rlca", 0x07);
            CodeEmitWorks("rrca", 0x0F);
            CodeEmitWorks("rla", 0x17);
            CodeEmitWorks("rra", 0x1F);
            CodeEmitWorks("daa", 0x27);
            CodeEmitWorks("cpl", 0x2F);
            CodeEmitWorks("scf", 0x37);
            CodeEmitWorks("ccf", 0x3F);
            CodeEmitWorks("ret", 0xC9);
            CodeEmitWorks("halt", 0x76);
            CodeEmitWorks("exx", 0xD9);
            CodeEmitWorks("di", 0xF3);
            CodeEmitWorks("ei", 0xFB);
        }

        [TestMethod]
        public void ExtendedTrivialOpsWorkAsExpected()
        {
            CodeEmitWorks("neg", 0xED, 0x44);
            CodeEmitWorks("retn", 0xED, 0x45);
            CodeEmitWorks("reti", 0xED, 0x4D);
            CodeEmitWorks("rrd", 0xED, 0x67);
            CodeEmitWorks("rld", 0xED, 0x6F);
            CodeEmitWorks("ldi", 0xED, 0xA0);
            CodeEmitWorks("cpi", 0xED, 0xA1);
            CodeEmitWorks("ini", 0xED, 0xA2);
            CodeEmitWorks("outi", 0xED, 0xA3);
            CodeEmitWorks("ldd", 0xED, 0xA8);
            CodeEmitWorks("cpd", 0xED, 0xA9);
            CodeEmitWorks("ind", 0xED, 0xAA);
            CodeEmitWorks("outd", 0xED, 0xAB);
            CodeEmitWorks("ldir", 0xED, 0xB0);
            CodeEmitWorks("cpir", 0xED, 0xB1);
            CodeEmitWorks("inir", 0xED, 0xB2);
            CodeEmitWorks("otir", 0xED, 0xB3);
            CodeEmitWorks("lddr", 0xED, 0xB8);
            CodeEmitWorks("cpdr", 0xED, 0xB9);
            CodeEmitWorks("indr", 0xED, 0xBA);
            CodeEmitWorks("otdr", 0xED, 0xBB);
        }
    }
}
