using Microsoft.VisualStudio.TestTools.UnitTesting;
using Spect.Net.Assembler.Assembler;

namespace Spect.Net.Assembler.Test.Assembler
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
        public void InvalidPortValueModeRaisesError()
        {
            CodeRaisesError("out (c),1", Errors.Z0006);
        }

        [TestMethod]
        public void InvalidIoOpsRaiseError()
        {
            CodeRaisesError("in b,(#fe)", Errors.Z0005);
            CodeRaisesError("in c,(#fe)", Errors.Z0005);
            CodeRaisesError("in d,(#fe)", Errors.Z0005);
            CodeRaisesError("in e,(#fe)", Errors.Z0005);
            CodeRaisesError("in h,(#fe)", Errors.Z0005);
            CodeRaisesError("in l,(#fe)", Errors.Z0005);
            CodeRaisesError("out (#fe),b", Errors.Z0005);
            CodeRaisesError("out (#fe),c", Errors.Z0005);
            CodeRaisesError("out (#fe),d", Errors.Z0005);
            CodeRaisesError("out (#fe),e", Errors.Z0005);
            CodeRaisesError("out (#fe),h", Errors.Z0005);
            CodeRaisesError("out (#fe),l", Errors.Z0005);
        }

    }
}