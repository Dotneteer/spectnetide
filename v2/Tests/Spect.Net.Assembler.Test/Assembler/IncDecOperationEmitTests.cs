using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Spect.Net.Assembler.Test.Assembler
{
    [TestClass]
    public class IncDecOperationEmitTests : AssemblerTestBed
    {
        [TestMethod]
        public void IncrementRegisterOpsWorkAsExpected()
        {
            CodeEmitWorks("inc a", 0x3C);
            CodeEmitWorks("inc b", 0x04);
            CodeEmitWorks("inc c", 0x0C);
            CodeEmitWorks("inc d", 0x14);
            CodeEmitWorks("inc e", 0x1C);
            CodeEmitWorks("inc h", 0x24);
            CodeEmitWorks("inc l", 0x2C);
            CodeEmitWorks("inc (hl)", 0x34);

            CodeEmitWorks("inc bc", 0x03);
            CodeEmitWorks("inc de", 0x13);
            CodeEmitWorks("inc hl", 0x23);
            CodeEmitWorks("inc sp", 0x33);

            CodeEmitWorks("inc ix", 0xDD, 0x23);
            CodeEmitWorks("inc iy", 0xFD, 0x23);
        }

        [TestMethod]
        public void IncrementReg8IdxOpsWorkAsExpected()
        {
            CodeEmitWorks("inc xh", 0xDD, 0x24);
            CodeEmitWorks("inc xl", 0xDD, 0x2C);
            CodeEmitWorks("inc yh", 0xFD, 0x24);
            CodeEmitWorks("inc yl", 0xFD, 0x2C);

            CodeEmitWorks("inc ixh", 0xDD, 0x24);
            CodeEmitWorks("inc ixl", 0xDD, 0x2C);
            CodeEmitWorks("inc iyh", 0xFD, 0x24);
            CodeEmitWorks("inc iyl", 0xFD, 0x2C);
        }

        [TestMethod]
        public void IncrementWithIndexedAddressWorkAsExpected()
        {
            CodeEmitWorks("inc (ix)", 0xDD, 0x34, 0x00);
            CodeEmitWorks("inc (ix+#1A)", 0xDD, 0x34, 0x1A);
            CodeEmitWorks("inc (ix-#32)", 0xDD, 0x34, 0xCE);
            CodeEmitWorks("inc (ix+[3+4+5])", 0xDD, 0x34, 0x0C);
            CodeEmitWorks("inc (ix-[3+4+5])", 0xDD, 0x34, 0xF4);

            CodeEmitWorks("inc (iy)", 0xFD, 0x34, 0x00);
            CodeEmitWorks("inc (iy+#1A)", 0xFD, 0x34, 0x1A);
            CodeEmitWorks("inc (iy-#32)", 0xFD, 0x34, 0xCE);
            CodeEmitWorks("inc (iy+[3+4+5])", 0xFD, 0x34, 0x0C);
            CodeEmitWorks("inc (iy-[3+4+5])", 0xFD, 0x34, 0xF4);
        }

        [TestMethod]
        public void DecrementWithIndexedAddressWorkAsExpected()
        {
            CodeEmitWorks("dec (ix)", 0xDD, 0x35, 0x00);
            CodeEmitWorks("dec (ix+#1A)", 0xDD, 0x35, 0x1A);
            CodeEmitWorks("dec (ix-#32)", 0xDD, 0x35, 0xCE);
            CodeEmitWorks("dec (ix+[3+4+5])", 0xDD, 0x35, 0x0C);
            CodeEmitWorks("dec (ix-[3+4+5])", 0xDD, 0x35, 0xF4);

            CodeEmitWorks("dec (iy)", 0xFD, 0x35, 0x00);
            CodeEmitWorks("dec (iy+#1A)", 0xFD, 0x35, 0x1A);
            CodeEmitWorks("dec (iy-#32)", 0xFD, 0x35, 0xCE);
            CodeEmitWorks("dec (iy+[3+4+5])", 0xFD, 0x35, 0x0C);
            CodeEmitWorks("dec (iy-[3+4+5])", 0xFD, 0x35, 0xF4);
        }

        [TestMethod]
        public void DecrementRegisterOpsWorkAsExpected()
        {
            CodeEmitWorks("dec a", 0x3D);
            CodeEmitWorks("dec b", 0x05);
            CodeEmitWorks("dec c", 0x0D);
            CodeEmitWorks("dec d", 0x15);
            CodeEmitWorks("dec e", 0x1D);
            CodeEmitWorks("dec h", 0x25);
            CodeEmitWorks("dec l", 0x2D);
            CodeEmitWorks("dec (hl)", 0x35);

            CodeEmitWorks("dec bc", 0x0B);
            CodeEmitWorks("dec de", 0x1B);
            CodeEmitWorks("dec hl", 0x2B);
            CodeEmitWorks("dec sp", 0x3B);

            CodeEmitWorks("dec ix", 0xDD, 0x2B);
            CodeEmitWorks("dec iy", 0xFD, 0x2B);
        }

        [TestMethod]
        public void DecrementReg8IdxOpsWorkAsExpected()
        {
            CodeEmitWorks("dec xh", 0xDD, 0x25);
            CodeEmitWorks("dec xl", 0xDD, 0x2D);
            CodeEmitWorks("dec yh", 0xFD, 0x25);
            CodeEmitWorks("dec yl", 0xFD, 0x2D);

            CodeEmitWorks("dec ixh", 0xDD, 0x25);
            CodeEmitWorks("dec ixl", 0xDD, 0x2D);
            CodeEmitWorks("dec iyh", 0xFD, 0x25);
            CodeEmitWorks("dec iyl", 0xFD, 0x2D);
        }
    }
}