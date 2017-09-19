using Microsoft.VisualStudio.TestTools.UnitTesting;
using Spect.Net.Assembler.Assembler;

namespace Spect.Net.Assembler.Test.Assembler
{
    [TestClass]
    public class ControlFlowOperationEmitTests : AssemblerTestBed
    {
        [TestMethod]
        public void DjnzOpWorksAsExpected()
        {
            // --- Start address is #8000!
            CodeEmitWorks("djnz #8022", 0x10, 0x20);
            CodeEmitWorks("djnz #8000", 0x10, 0xFE);
            CodeEmitWorks("djnz #8081", 0x10, 0x7F);
            CodeEmitWorks("djnz #7F82", 0x10, 0x80);
            CodeEmitWorks("djnz #7F84", 0x10, 0x82);
        }

        [TestMethod]
        public void DjnzFailsWithFarAddress()
        {
            // --- Start address is #8000!
            CodeRaisesError("djnz #8082", Errors.Z0022);
            CodeRaisesError("djnz #8100", Errors.Z0022);
            CodeRaisesError("djnz #7F81", Errors.Z0022);
            CodeRaisesError("djnz #7F00", Errors.Z0022);
        }

        [TestMethod]
        public void JrOpsWorksAsExpected()
        {
            // --- Start address is #8000!
            CodeEmitWorks("jr #8022", 0x18, 0x20);
            CodeEmitWorks("jr #8000", 0x18, 0xFE);
            CodeEmitWorks("jr #8081", 0x18, 0x7F);
            CodeEmitWorks("jr #7F82", 0x18, 0x80);
            CodeEmitWorks("jr #7F84", 0x18, 0x82);

            CodeEmitWorks("jr nz,#8022", 0x20, 0x20);
            CodeEmitWorks("jr nz,#8000", 0x20, 0xFE);
            CodeEmitWorks("jr nz,#8081", 0x20, 0x7F);
            CodeEmitWorks("jr nz,#7F82", 0x20, 0x80);
            CodeEmitWorks("jr nz,#7F84", 0x20, 0x82);

            CodeEmitWorks("jr z,#8022", 0x28, 0x20);
            CodeEmitWorks("jr z,#8000", 0x28, 0xFE);
            CodeEmitWorks("jr z,#8081", 0x28, 0x7F);
            CodeEmitWorks("jr z,#7F82", 0x28, 0x80);
            CodeEmitWorks("jr z,#7F84", 0x28, 0x82);

            CodeEmitWorks("jr nc,#8022", 0x30, 0x20);
            CodeEmitWorks("jr nc,#8000", 0x30, 0xFE);
            CodeEmitWorks("jr nc,#8081", 0x30, 0x7F);
            CodeEmitWorks("jr nc,#7F82", 0x30, 0x80);
            CodeEmitWorks("jr nc,#7F84", 0x30, 0x82);

            CodeEmitWorks("jr c,#8022", 0x38, 0x20);
            CodeEmitWorks("jr c,#8000", 0x38, 0xFE);
            CodeEmitWorks("jr c,#8081", 0x38, 0x7F);
            CodeEmitWorks("jr c,#7F82", 0x38, 0x80);
            CodeEmitWorks("jr c,#7F84", 0x38, 0x82);
        }

        [TestMethod]
        public void JrFailsWithFarAddress()
        {
            // --- Start address is #8000!
            CodeRaisesError("djnz #8082", Errors.Z0022);
            CodeRaisesError("djnz #8100", Errors.Z0022);
            CodeRaisesError("djnz #7F81", Errors.Z0022);
            CodeRaisesError("djnz #7F00", Errors.Z0022);

            CodeRaisesError("jr #8082", Errors.Z0022);
            CodeRaisesError("jr #8100", Errors.Z0022);
            CodeRaisesError("jr #7F81", Errors.Z0022);
            CodeRaisesError("jr #7F00", Errors.Z0022);

            CodeRaisesError("jr nz,#8082", Errors.Z0022);
            CodeRaisesError("jr nz,#8100", Errors.Z0022);
            CodeRaisesError("jr nz,#7F81", Errors.Z0022);
            CodeRaisesError("jr nz,#7F00", Errors.Z0022);

            CodeRaisesError("jr z,#8082", Errors.Z0022);
            CodeRaisesError("jr z,#8100", Errors.Z0022);
            CodeRaisesError("jr z,#7F81", Errors.Z0022);
            CodeRaisesError("jr z,#7F00", Errors.Z0022);

            CodeRaisesError("jr nc,#8082", Errors.Z0022);
            CodeRaisesError("jr nc,#8100", Errors.Z0022);
            CodeRaisesError("jr nc,#7F81", Errors.Z0022);
            CodeRaisesError("jr nc,#7F00", Errors.Z0022);

            CodeRaisesError("jr c,#8082", Errors.Z0022);
            CodeRaisesError("jr c,#8100", Errors.Z0022);
            CodeRaisesError("jr c,#7F81", Errors.Z0022);
            CodeRaisesError("jr c,#7F00", Errors.Z0022);
        }

        [TestMethod]
        public void JpOpsWorksAsExpected()
        {
            CodeEmitWorks("jp #3024", 0xC3, 0x24, 0x30);
            CodeEmitWorks("jp nz,#3024", 0xC2, 0x24, 0x30);
            CodeEmitWorks("jp z,#3024", 0xCA, 0x24, 0x30);
            CodeEmitWorks("jp nc,#3024", 0xD2, 0x24, 0x30);
            CodeEmitWorks("jp c,#3024", 0xDA, 0x24, 0x30);
            CodeEmitWorks("jp po,#3024", 0xE2, 0x24, 0x30);
            CodeEmitWorks("jp pe,#3024", 0xEA, 0x24, 0x30);
            CodeEmitWorks("jp p,#3024", 0xF2, 0x24, 0x30);
            CodeEmitWorks("jp m,#3024", 0xFA, 0x24, 0x30);

            CodeEmitWorks("jp (hl)", 0xE9);
            CodeEmitWorks("jp (ix)", 0xDD, 0xE9);
            CodeEmitWorks("jp (iy)", 0xFD, 0xE9);
        }

        [TestMethod]
        public void CallOpsWorksAsExpected()
        {
            CodeEmitWorks("call #3024", 0xCD, 0x24, 0x30);
            CodeEmitWorks("call nz,#3024", 0xC4, 0x24, 0x30);
            CodeEmitWorks("call z,#3024", 0xCC, 0x24, 0x30);
            CodeEmitWorks("call nc,#3024", 0xD4, 0x24, 0x30);
            CodeEmitWorks("call c,#3024", 0xDC, 0x24, 0x30);
            CodeEmitWorks("call po,#3024", 0xE4, 0x24, 0x30);
            CodeEmitWorks("call pe,#3024", 0xEC, 0x24, 0x30);
            CodeEmitWorks("call p,#3024", 0xF4, 0x24, 0x30);
            CodeEmitWorks("call m,#3024", 0xFC, 0x24, 0x30);
        }

        [TestMethod]
        public void RetOpsWorksAsExpected()
        {
            CodeEmitWorks("ret", 0xC9);
            CodeEmitWorks("ret nz", 0xC0);
            CodeEmitWorks("ret z", 0xC8);
            CodeEmitWorks("ret nc", 0xD0);
            CodeEmitWorks("ret c", 0xD8);
            CodeEmitWorks("ret po", 0xE0);
            CodeEmitWorks("ret pe", 0xE8);
            CodeEmitWorks("ret p", 0xF0);
            CodeEmitWorks("ret m", 0xF8);
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
        public void InvalidOpsRaiseError()
        {
            CodeRaisesError("rst 23", Errors.Z0018);
            CodeRaisesError("jp (bc)", Errors.Z0016);
            CodeRaisesError("jp (de)", Errors.Z0016);
            CodeRaisesError("jp (sp)", Errors.Z0016);
            CodeRaisesError("jp (ix+3)", Errors.Z0016);
            CodeRaisesError("jp (iy+3)", Errors.Z0016);
            CodeRaisesError("jp nz,(hl)", Errors.Z0017);
        }

    }
}