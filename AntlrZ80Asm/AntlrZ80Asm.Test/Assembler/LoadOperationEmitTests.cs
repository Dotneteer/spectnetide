using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AntlrZ80Asm.Test.Assembler
{
    [TestClass]
    public class LoadOperationEmitTests : AssemblerTestBed
    {
        [TestMethod]
        public void Reg8ToReg8LoadOpsWorkAsExpected()
        {
            CodeEmitWorks("ld b,b", 0x40);
            CodeEmitWorks("ld b,c", 0x41);
            CodeEmitWorks("ld b,d", 0x42);
            CodeEmitWorks("ld b,e", 0x43);
            CodeEmitWorks("ld b,h", 0x44);
            CodeEmitWorks("ld b,l", 0x45);
            CodeEmitWorks("ld b,(hl)", 0x46);
            CodeEmitWorks("ld b,a", 0x47);

            CodeEmitWorks("ld c,b", 0x48);
            CodeEmitWorks("ld c,c", 0x49);
            CodeEmitWorks("ld c,d", 0x4A);
            CodeEmitWorks("ld c,e", 0x4B);
            CodeEmitWorks("ld c,h", 0x4C);
            CodeEmitWorks("ld c,l", 0x4D);
            CodeEmitWorks("ld c,(hl)", 0x4E);
            CodeEmitWorks("ld c,a", 0x4F);

            CodeEmitWorks("ld d,b", 0x50);
            CodeEmitWorks("ld d,c", 0x51);
            CodeEmitWorks("ld d,d", 0x52);
            CodeEmitWorks("ld d,e", 0x53);
            CodeEmitWorks("ld d,h", 0x54);
            CodeEmitWorks("ld d,l", 0x55);
            CodeEmitWorks("ld d,(hl)", 0x56);
            CodeEmitWorks("ld d,a", 0x57);

            CodeEmitWorks("ld e,b", 0x58);
            CodeEmitWorks("ld e,c", 0x59);
            CodeEmitWorks("ld e,d", 0x5A);
            CodeEmitWorks("ld e,e", 0x5B);
            CodeEmitWorks("ld e,h", 0x5C);
            CodeEmitWorks("ld e,l", 0x5D);
            CodeEmitWorks("ld e,(hl)", 0x5E);
            CodeEmitWorks("ld e,a", 0x5F);

            CodeEmitWorks("ld h,b", 0x60);
            CodeEmitWorks("ld h,c", 0x61);
            CodeEmitWorks("ld h,d", 0x62);
            CodeEmitWorks("ld h,e", 0x63);
            CodeEmitWorks("ld h,h", 0x64);
            CodeEmitWorks("ld h,l", 0x65);
            CodeEmitWorks("ld h,(hl)", 0x66);
            CodeEmitWorks("ld h,a", 0x67);

            CodeEmitWorks("ld l,b", 0x68);
            CodeEmitWorks("ld l,c", 0x69);
            CodeEmitWorks("ld l,d", 0x6A);
            CodeEmitWorks("ld l,e", 0x6B);
            CodeEmitWorks("ld l,h", 0x6C);
            CodeEmitWorks("ld l,l", 0x6D);
            CodeEmitWorks("ld l,(hl)", 0x6E);
            CodeEmitWorks("ld l,a", 0x6F);

            CodeEmitWorks("ld (hl),b", 0x70);
            CodeEmitWorks("ld (hl),c", 0x71);
            CodeEmitWorks("ld (hl),d", 0x72);
            CodeEmitWorks("ld (hl),e", 0x73);
            CodeEmitWorks("ld (hl),h", 0x74);
            CodeEmitWorks("ld (hl),l", 0x75);
            CodeEmitWorks("ld (hl),a", 0x77);

            CodeEmitWorks("ld a,b", 0x78);
            CodeEmitWorks("ld a,c", 0x79);
            CodeEmitWorks("ld a,d", 0x7A);
            CodeEmitWorks("ld a,e", 0x7B);
            CodeEmitWorks("ld a,h", 0x7C);
            CodeEmitWorks("ld a,l", 0x7D);
            CodeEmitWorks("ld a,(hl)", 0x7E);
            CodeEmitWorks("ld a,a", 0x7F);
        }

        [TestMethod]
        public void SpecReg8LoadOpsWorkAsExpected()
        {
            CodeEmitWorks("ld i,a", 0xED, 0x47);
            CodeEmitWorks("ld r,a", 0xED, 0x4F);
            CodeEmitWorks("ld a,i", 0xED, 0x57);
            CodeEmitWorks("ld a,r", 0xED, 0x5F);

            CodeEmitWorks("ld xh,b", 0xDD, 0x60);
            CodeEmitWorks("ld xh,c", 0xDD, 0x61);
            CodeEmitWorks("ld xh,d", 0xDD, 0x62);
            CodeEmitWorks("ld xh,e", 0xDD, 0x63);
            CodeEmitWorks("ld xh,xh", 0xDD, 0x64);
            CodeEmitWorks("ld xh,xl", 0xDD, 0x65);
            CodeEmitWorks("ld xh,a", 0xDD, 0x67);

            CodeEmitWorks("ld xl,b", 0xDD, 0x68);
            CodeEmitWorks("ld xl,c", 0xDD, 0x69);
            CodeEmitWorks("ld xl,d", 0xDD, 0x6A);
            CodeEmitWorks("ld xl,e", 0xDD, 0x6B);
            CodeEmitWorks("ld xl,xh", 0xDD, 0x6C);
            CodeEmitWorks("ld xl,xl", 0xDD, 0x6D);
            CodeEmitWorks("ld xl,a", 0xDD, 0x6F);

            CodeEmitWorks("ld yh,b", 0xFD, 0x60);
            CodeEmitWorks("ld yh,c", 0xFD, 0x61);
            CodeEmitWorks("ld yh,d", 0xFD, 0x62);
            CodeEmitWorks("ld yh,e", 0xFD, 0x63);
            CodeEmitWorks("ld yh,yh", 0xFD, 0x64);
            CodeEmitWorks("ld yh,yl", 0xFD, 0x65);
            CodeEmitWorks("ld yh,a", 0xFD, 0x67);

            CodeEmitWorks("ld yl,b", 0xFD, 0x68);
            CodeEmitWorks("ld yl,c", 0xFD, 0x69);
            CodeEmitWorks("ld yl,d", 0xFD, 0x6A);
            CodeEmitWorks("ld yl,e", 0xFD, 0x6B);
            CodeEmitWorks("ld yl,yh", 0xFD, 0x6C);
            CodeEmitWorks("ld yl,yl", 0xFD, 0x6D);
            CodeEmitWorks("ld yl,a", 0xFD, 0x6F);
        }

        [TestMethod]
        public void SpLoadOpsWorkAsExpected()
        {
            CodeEmitWorks("ld sp,hl", 0xF9);
            CodeEmitWorks("ld sp,ix", 0xDD, 0xF9);
            CodeEmitWorks("ld sp,iy", 0xFD, 0xF9);
        }

        [TestMethod]
        public void InvalidLoadOpsRaiseError()
        {
            CodeRaisesInvalidArgument("ld (hl),(hl)");

            CodeRaisesInvalidArgument("ld xh,h");
            CodeRaisesInvalidArgument("ld xh,l");
            CodeRaisesInvalidArgument("ld xh,(hl)");
            CodeRaisesInvalidArgument("ld xl,h");
            CodeRaisesInvalidArgument("ld xl,l");
            CodeRaisesInvalidArgument("ld xl,(hl)");

            CodeRaisesInvalidArgument("ld yh,h");
            CodeRaisesInvalidArgument("ld yh,l");
            CodeRaisesInvalidArgument("ld yh,(hl)");
            CodeRaisesInvalidArgument("ld yl,h");
            CodeRaisesInvalidArgument("ld yl,l");
            CodeRaisesInvalidArgument("ld yl,(hl)");

            CodeRaisesInvalidArgument("ld xh,yh");
            CodeRaisesInvalidArgument("ld xh,yl");
            CodeRaisesInvalidArgument("ld yh,xh");
            CodeRaisesInvalidArgument("ld yh,xl");
        }
    }
}