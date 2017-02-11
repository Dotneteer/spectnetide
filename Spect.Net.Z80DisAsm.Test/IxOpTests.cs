using Microsoft.VisualStudio.TestTools.UnitTesting;
using Spect.Net.Z80DisAsm.Test.Helpers;

namespace Spect.Net.Z80DisAsm.Test
{
    [TestClass]
    public class IxOpTests
    {
        [TestMethod]
        public void IndexedOps0X00WorkAsExpected()
        {
            // --- Act
            Z80Tester.Test("add ix,bc", 0xDD, 0x09);
            Z80Tester.Test("add ix,de", 0xDD, 0x19);
            Z80Tester.Test("ld ix,$1234", 0xDD, 0x21, 0x34, 0x12);
            Z80Tester.Test("ld ($1234),ix", 0xDD, 0x22, 0x34, 0x12);
            Z80Tester.Test("inc ix", 0xDD, 0x23);
            Z80Tester.Test("inc xh", 0xDD, 0x24);
            Z80Tester.Test("dec xh", 0xDD, 0x25);
            Z80Tester.Test("ld xh,$AB", 0xDD, 0x26, 0xAB);
            Z80Tester.Test("add ix,ix", 0xDD, 0x29);
            Z80Tester.Test("ld ix,($1234)", 0xDD, 0x2A, 0x34, 0x12);
            Z80Tester.Test("dec ix", 0xDD, 0x2B);
            Z80Tester.Test("inc xl", 0xDD, 0x2C);
            Z80Tester.Test("dec xl", 0xDD, 0x2D);
            Z80Tester.Test("ld xl,$AB", 0xDD, 0x2E, 0xAB);
            Z80Tester.Test("inc (ix+$3D)", 0xDD, 0x34, 0x3D);
            Z80Tester.Test("inc (ix-$51)", 0xDD, 0x34, 0xAF);
            Z80Tester.Test("dec (ix+$3D)", 0xDD, 0x35, 0x3D);
            Z80Tester.Test("dec (ix-$51)", 0xDD, 0x35, 0xAF);
            Z80Tester.Test("ld (ix+$3D),$12", 0xDD, 0x36, 0x3D, 0x12);
            Z80Tester.Test("ld (ix-$51),$12", 0xDD, 0x36, 0xAF, 0x12);
            Z80Tester.Test("add ix,sp", 0xDD, 0x39);
        }

        [TestMethod]
        public void IndexedOps0X40WorkAsExpected()
        {
            // --- Act
            Z80Tester.Test("ld b,xh", 0xDD, 0x44);
            Z80Tester.Test("ld b,xl", 0xDD, 0x45);
            Z80Tester.Test("ld b,(ix+$3D)", 0xDD, 0x46, 0x3D);
            Z80Tester.Test("ld b,(ix-$51)", 0xDD, 0x46, 0xAF);
            Z80Tester.Test("ld c,xh", 0xDD, 0x4C);
            Z80Tester.Test("ld c,xl", 0xDD, 0x4D);
            Z80Tester.Test("ld c,(ix+$3D)", 0xDD, 0x4E, 0x3D);
            Z80Tester.Test("ld c,(ix-$51)", 0xDD, 0x4E, 0xAF);
            Z80Tester.Test("ld d,xh", 0xDD, 0x54);
            Z80Tester.Test("ld d,xl", 0xDD, 0x55);
            Z80Tester.Test("ld d,(ix+$3D)", 0xDD, 0x56, 0x3D);
            Z80Tester.Test("ld d,(ix-$51)", 0xDD, 0x56, 0xAF);
            Z80Tester.Test("ld e,xh", 0xDD, 0x5C);
            Z80Tester.Test("ld e,xl", 0xDD, 0x5D);
            Z80Tester.Test("ld e,(ix+$3D)", 0xDD, 0x5E, 0x3D);
            Z80Tester.Test("ld e,(ix-$51)", 0xDD, 0x5E, 0xAF);
            Z80Tester.Test("ld xh,b", 0xDD, 0x60);
            Z80Tester.Test("ld xh,c", 0xDD, 0x61);
            Z80Tester.Test("ld xh,d", 0xDD, 0x62);
            Z80Tester.Test("ld xh,e", 0xDD, 0x63);
            Z80Tester.Test("ld xh,xh", 0xDD, 0x64);
            Z80Tester.Test("ld xh,xl", 0xDD, 0x65);
            Z80Tester.Test("ld h,(ix+$3D)", 0xDD, 0x66, 0x3D);
            Z80Tester.Test("ld xh,a", 0xDD, 0x67);
            Z80Tester.Test("ld xl,b", 0xDD, 0x68);
            Z80Tester.Test("ld xl,c", 0xDD, 0x69);
            Z80Tester.Test("ld xl,d", 0xDD, 0x6A);
            Z80Tester.Test("ld xl,e", 0xDD, 0x6B);
            Z80Tester.Test("ld xl,xh", 0xDD, 0x6C);
            Z80Tester.Test("ld xl,xl", 0xDD, 0x6D);
            Z80Tester.Test("ld l,(ix+$3D)", 0xDD, 0x6E, 0x3D);
            Z80Tester.Test("ld xl,a", 0xDD, 0x6F);
            Z80Tester.Test("ld (ix+$3D),b", 0xDD, 0x70, 0x3D);
            Z80Tester.Test("ld (ix+$3D),c", 0xDD, 0x71, 0x3D);
            Z80Tester.Test("ld (ix+$3D),d", 0xDD, 0x72, 0x3D);
            Z80Tester.Test("ld (ix+$3D),e", 0xDD, 0x73, 0x3D);
            Z80Tester.Test("ld (ix+$3D),h", 0xDD, 0x74, 0x3D);
            Z80Tester.Test("ld (ix+$3D),l", 0xDD, 0x75, 0x3D);
            Z80Tester.Test("halt", 0xDD, 0x76);
            Z80Tester.Test("ld (ix+$3D),a", 0xDD, 0x77, 0x3D);
            Z80Tester.Test("ld a,xh", 0xDD, 0x7C);
            Z80Tester.Test("ld a,xl", 0xDD, 0x7D);
            Z80Tester.Test("ld a,(ix+$3D)", 0xDD, 0x7E, 0x3D);
        }

        [TestMethod]
        public void IndexedOps0X80WorkAsExpected()
        {
            // --- Act
            Z80Tester.Test("add a,xh", 0xDD, 0x84);
            Z80Tester.Test("add a,xl", 0xDD, 0x85);
            Z80Tester.Test("add a,(ix+$3D)", 0xDD, 0x86, 0x3D);
            Z80Tester.Test("adc a,xh", 0xDD, 0x8C);
            Z80Tester.Test("adc a,xl", 0xDD, 0x8D);
            Z80Tester.Test("adc a,(ix+$3D)", 0xDD, 0x8E, 0x3D);
            Z80Tester.Test("sub xh", 0xDD, 0x94);
            Z80Tester.Test("sub xl", 0xDD, 0x95);
            Z80Tester.Test("sub (ix+$3D)", 0xDD, 0x96, 0x3D);
            Z80Tester.Test("sbc xh", 0xDD, 0x9C);
            Z80Tester.Test("sbc xl", 0xDD, 0x9D);
            Z80Tester.Test("sbc (ix+$3D)", 0xDD, 0x9E, 0x3D);
            Z80Tester.Test("and xh", 0xDD, 0xA4);
            Z80Tester.Test("and xl", 0xDD, 0xA5);
            Z80Tester.Test("and (ix+$3D)", 0xDD, 0xA6, 0x3D);
            Z80Tester.Test("xor xh", 0xDD, 0xAC);
            Z80Tester.Test("xor xl", 0xDD, 0xAD);
            Z80Tester.Test("xor (ix+$3D)", 0xDD, 0xAE, 0x3D);
            Z80Tester.Test("or xh", 0xDD, 0xB4);
            Z80Tester.Test("or xl", 0xDD, 0xB5);
            Z80Tester.Test("or (ix+$3D)", 0xDD, 0xB6, 0x3D);
            Z80Tester.Test("cp xh", 0xDD, 0xBC);
            Z80Tester.Test("cp xl", 0xDD, 0xBD);
            Z80Tester.Test("cp (ix+$3D)", 0xDD, 0xBE, 0x3D);
            Z80Tester.Test("pop ix", 0xDD, 0xE1);
            Z80Tester.Test("ex (sp),ix", 0xDD, 0xE3);
            Z80Tester.Test("push ix", 0xDD, 0xE5);
            Z80Tester.Test("jp (ix)", 0xDD, 0xE9);
            Z80Tester.Test("ld sp,ix", 0xDD, 0xF9);
        }
    }
}
