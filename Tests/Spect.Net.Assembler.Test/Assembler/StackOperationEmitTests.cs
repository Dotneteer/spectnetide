using Microsoft.VisualStudio.TestTools.UnitTesting;
using Spect.Net.Assembler.Assembler;

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
            CodeRaisesError("pop af'", Errors.Z0019);
            CodeRaisesError("push af'", Errors.Z0019);
            CodeRaisesError("pop a", Errors.Z0001);
            CodeRaisesError("push a", Errors.Z0001);
            CodeRaisesError("pop xl", Errors.Z0001);
            CodeRaisesError("push yh", Errors.Z0001);
            CodeRaisesError("pop (bc)", Errors.Z0001);
            CodeRaisesError("push (bc)", Errors.Z0001);
            CodeRaisesError("pop (#A234)", Errors.Z0001);
            CodeRaisesError("push (#A234)", Errors.Z0001);
            CodeRaisesError("pop (c)", Errors.Z0001);
            CodeRaisesError("push (c)", Errors.Z0001);
            CodeRaisesError("pop (ix+3)", Errors.Z0001);
            CodeRaisesError("push (iy-4)", Errors.Z0001);
            CodeRaisesError("pop #123", Errors.Z0024);
        }

        [TestMethod]
        public void ExtendedPushOpsWorkAsExpected()
        {
            CodeEmitWorks(".model next \r\n push #1234", 0xED, 0x8A, 0x34, 0x12);
        }

        [TestMethod]
        public void ExtendedPushOpsRaisesError()
        {
            CodeRaisesError("push #1234", Errors.Z0102);
        }
    }
}