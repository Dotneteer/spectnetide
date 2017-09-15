using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Spect.Net.Assembler.Test.Assembler
{
    [TestClass]
    public class StackOperationEmitTests : AssemblerTestBed
    {
        [TestMethod]
        public void PushOpsWorkAsExpected()
        {
            CodeEmitWorks("push af", 0xF5);
            CodeEmitWorks("push bc", 0xC5);
            CodeEmitWorks("push de", 0xD5);
            CodeEmitWorks("push hl", 0xE5);
            CodeEmitWorks("push ix", 0xDD, 0xE5);
            CodeEmitWorks("push iy", 0xFD, 0xE5);
        }

        [TestMethod]
        public void PopOpsWorkAsExpected()
        {
            CodeEmitWorks("pop af", 0xF1);
            CodeEmitWorks("pop bc", 0xC1);
            CodeEmitWorks("pop de", 0xD1);
            CodeEmitWorks("pop hl", 0xE1);
            CodeEmitWorks("pop ix", 0xDD, 0xE1);
            CodeEmitWorks("pop iy", 0xFD, 0xE1);
        }

        [TestMethod]
        public void InvalidOpsRaiseArgumentError()
        {
            CodeRaisesInvalidArgument("pop af'");
            CodeRaisesInvalidArgument("push af'");
            CodeRaisesInvalidArgument("pop a");
            CodeRaisesInvalidArgument("push a");
            CodeRaisesInvalidArgument("pop xl");
            CodeRaisesInvalidArgument("push yh");
            CodeRaisesInvalidArgument("pop (bc)");
            CodeRaisesInvalidArgument("push (bc)");
            CodeRaisesInvalidArgument("pop (#A234)");
            CodeRaisesInvalidArgument("push (#A234)");
            CodeRaisesInvalidArgument("pop (c)");
            CodeRaisesInvalidArgument("push (c)");
            CodeRaisesInvalidArgument("pop (ix+3)");
            CodeRaisesInvalidArgument("push (iy-4)");
            CodeRaisesInvalidArgument("pop #123");
            CodeRaisesInvalidArgument("push #123");
        }

    }
}