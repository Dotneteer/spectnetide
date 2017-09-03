using AntlrZ80Asm.Assembler;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;

namespace AntlrZ80Asm.Test.Assembler
{
    [TestClass]
    public class IoOperationEmitTests : AssemblerTestBed
    {
        [TestMethod]
        public void InOpsWorkAsExpected()
        {
            CodeEmitWorks("in a,(#FE)", 0xDB, 0xFE);
            CodeEmitWorks("in a,(c)", 0xED, 0x78);
            CodeEmitWorks("in b,(c)", 0xED, 0x40);
            CodeEmitWorks("in c,(c)", 0xED, 0x48);
            CodeEmitWorks("in d,(c)", 0xED, 0x50);
            CodeEmitWorks("in e,(c)", 0xED, 0x58);
            CodeEmitWorks("in h,(c)", 0xED, 0x60);
            CodeEmitWorks("in l,(c)", 0xED, 0x68);
            CodeEmitWorks("in (c)", 0xED, 0x70);
        }

        [TestMethod]
        public void InWithUnresolvedPortWorkAsExpected()
        {
            CodeEmitWorks("in a,(Port)", FixupType.Bit8, 1, 0xDB, 0x00);
        }

        [TestMethod]
        public void OutOpsWorkAsExpected()
        {
            CodeEmitWorks("out (#FE),a", 0xD3, 0xFE);
            CodeEmitWorks("out (c),a", 0xED, 0x79);
            CodeEmitWorks("out (c),b", 0xED, 0x41);
            CodeEmitWorks("out (c),c", 0xED, 0x49);
            CodeEmitWorks("out (c),d", 0xED, 0x51);
            CodeEmitWorks("out (c),e", 0xED, 0x59);
            CodeEmitWorks("out (c),h", 0xED, 0x61);
            CodeEmitWorks("out (c),l", 0xED, 0x69);
            CodeEmitWorks("out (c),0", 0xED, 0x71);
        }

        [TestMethod]
        public void OutWithUnresolvedPortWorkAsExpected()
        {
            CodeEmitWorks("out (Port),a", FixupType.Bit8, 1, 0xD3, 0x00);
        }

        [TestMethod]
        public void InvalidPortValueModeRaisesError()
        {
            CodeRaisesInvalidArgument("out (c),1");
        }

        [TestMethod]
        public void InvalidIoOpsRaiseError()
        {
            CodeRaisesInvalidArgument("in b,(#fe)");
            CodeRaisesInvalidArgument("in c,(#fe)");
            CodeRaisesInvalidArgument("in d,(#fe)");
            CodeRaisesInvalidArgument("in e,(#fe)");
            CodeRaisesInvalidArgument("in h,(#fe)");
            CodeRaisesInvalidArgument("in l,(#fe)");
            CodeRaisesInvalidArgument("out (#fe),b");
            CodeRaisesInvalidArgument("out (#fe),c");
            CodeRaisesInvalidArgument("out (#fe),d");
            CodeRaisesInvalidArgument("out (#fe),e");
            CodeRaisesInvalidArgument("out (#fe),h");
            CodeRaisesInvalidArgument("out (#fe),l");
        }

    }
}