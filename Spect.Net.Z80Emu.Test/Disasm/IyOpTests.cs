using Microsoft.VisualStudio.TestTools.UnitTesting;
using Spect.Net.Z80Emu.Test.Helpers;

namespace Spect.Net.Z80Emu.Test.Disasm
{
    [TestClass]
    public class IyOpTests
    {
        [TestMethod]
        public void IndexedOps0X00WorkAsExpected()
        {
            // --- Act
            Z80Tester.Test("add iy,bc", 0xFD, 0x09);
            Z80Tester.Test("add iy,de", 0xFD, 0x19);
            Z80Tester.Test("ld iy,$1234", 0xFD, 0x21, 0x34, 0x12);
            Z80Tester.Test("ld ($1234),iy", 0xFD, 0x22, 0x34, 0x12);
            Z80Tester.Test("inc iy", 0xFD, 0x23);
            Z80Tester.Test("inc yh", 0xFD, 0x24);
            Z80Tester.Test("dec yh", 0xFD, 0x25);
            Z80Tester.Test("ld yh,$AB", 0xFD, 0x26, 0xAB);
            Z80Tester.Test("add iy,iy", 0xFD, 0x29);
            Z80Tester.Test("ld iy,($1234)", 0xFD, 0x2A, 0x34, 0x12);
            Z80Tester.Test("dec iy", 0xFD, 0x2B);
            Z80Tester.Test("inc yl", 0xFD, 0x2C);
            Z80Tester.Test("dec yl", 0xFD, 0x2D);
            Z80Tester.Test("ld yl,$AB", 0xFD, 0x2E, 0xAB);
            Z80Tester.Test("inc (iy+$3D)", 0xFD, 0x34, 0x3D);
            Z80Tester.Test("inc (iy-$51)", 0xFD, 0x34, 0xAF);
            Z80Tester.Test("dec (iy+$3D)", 0xFD, 0x35, 0x3D);
            Z80Tester.Test("dec (iy-$51)", 0xFD, 0x35, 0xAF);
            Z80Tester.Test("ld (iy+$3D),$12", 0xFD, 0x36, 0x3D, 0x12);
            Z80Tester.Test("ld (iy-$51),$12", 0xFD, 0x36, 0xAF, 0x12);
            Z80Tester.Test("add iy,sp", 0xFD, 0x39);
        }

        [TestMethod]
        public void IndexedOps0X40WorkAsExpected()
        {
            // --- Act
            Z80Tester.Test("ld b,yh", 0xFD, 0x44);
            Z80Tester.Test("ld b,yl", 0xFD, 0x45);
            Z80Tester.Test("ld b,(iy+$3D)", 0xFD, 0x46, 0x3D);
            Z80Tester.Test("ld b,(iy-$51)", 0xFD, 0x46, 0xAF);
            Z80Tester.Test("ld c,yh", 0xFD, 0x4C);
            Z80Tester.Test("ld c,yl", 0xFD, 0x4D);
            Z80Tester.Test("ld c,(iy+$3D)", 0xFD, 0x4E, 0x3D);
            Z80Tester.Test("ld c,(iy-$51)", 0xFD, 0x4E, 0xAF);
            Z80Tester.Test("ld d,yh", 0xFD, 0x54);
            Z80Tester.Test("ld d,yl", 0xFD, 0x55);
            Z80Tester.Test("ld d,(iy+$3D)", 0xFD, 0x56, 0x3D);
            Z80Tester.Test("ld d,(iy-$51)", 0xFD, 0x56, 0xAF);
            Z80Tester.Test("ld e,yh", 0xFD, 0x5C);
            Z80Tester.Test("ld e,yl", 0xFD, 0x5D);
            Z80Tester.Test("ld e,(iy+$3D)", 0xFD, 0x5E, 0x3D);
            Z80Tester.Test("ld e,(iy-$51)", 0xFD, 0x5E, 0xAF);
            Z80Tester.Test("ld yh,b", 0xFD, 0x60);
            Z80Tester.Test("ld yh,c", 0xFD, 0x61);
            Z80Tester.Test("ld yh,d", 0xFD, 0x62);
            Z80Tester.Test("ld yh,e", 0xFD, 0x63);
            Z80Tester.Test("ld yh,yh", 0xFD, 0x64);
            Z80Tester.Test("ld yh,yl", 0xFD, 0x65);
            Z80Tester.Test("ld h,(iy+$3D)", 0xFD, 0x66, 0x3D);
            Z80Tester.Test("ld yh,a", 0xFD, 0x67);
            Z80Tester.Test("ld yl,b", 0xFD, 0x68);
            Z80Tester.Test("ld yl,c", 0xFD, 0x69);
            Z80Tester.Test("ld yl,d", 0xFD, 0x6A);
            Z80Tester.Test("ld yl,e", 0xFD, 0x6B);
            Z80Tester.Test("ld yl,yh", 0xFD, 0x6C);
            Z80Tester.Test("ld yl,yl", 0xFD, 0x6D);
            Z80Tester.Test("ld l,(iy+$3D)", 0xFD, 0x6E, 0x3D);
            Z80Tester.Test("ld yl,a", 0xFD, 0x6F);
            Z80Tester.Test("ld (iy+$3D),b", 0xFD, 0x70, 0x3D);
            Z80Tester.Test("ld (iy+$3D),c", 0xFD, 0x71, 0x3D);
            Z80Tester.Test("ld (iy+$3D),d", 0xFD, 0x72, 0x3D);
            Z80Tester.Test("ld (iy+$3D),e", 0xFD, 0x73, 0x3D);
            Z80Tester.Test("ld (iy+$3D),h", 0xFD, 0x74, 0x3D);
            Z80Tester.Test("ld (iy+$3D),l", 0xFD, 0x75, 0x3D);
            Z80Tester.Test("halt", 0xFD, 0x76);
            Z80Tester.Test("ld (iy+$3D),a", 0xFD, 0x77, 0x3D);
            Z80Tester.Test("ld a,yh", 0xFD, 0x7C);
            Z80Tester.Test("ld a,yl", 0xFD, 0x7D);
            Z80Tester.Test("ld a,(iy+$3D)", 0xFD, 0x7E, 0x3D);
        }

        [TestMethod]
        public void IndexedOps0X80WorkAsExpected()
        {
            // --- Act
            Z80Tester.Test("add a,yh", 0xFD, 0x84);
            Z80Tester.Test("add a,yl", 0xFD, 0x85);
            Z80Tester.Test("add a,(iy+$3D)", 0xFD, 0x86, 0x3D);
            Z80Tester.Test("adc a,yh", 0xFD, 0x8C);
            Z80Tester.Test("adc a,yl", 0xFD, 0x8D);
            Z80Tester.Test("adc a,(iy+$3D)", 0xFD, 0x8E, 0x3D);
            Z80Tester.Test("sub yh", 0xFD, 0x94);
            Z80Tester.Test("sub yl", 0xFD, 0x95);
            Z80Tester.Test("sub (iy+$3D)", 0xFD, 0x96, 0x3D);
            Z80Tester.Test("sbc yh", 0xFD, 0x9C);
            Z80Tester.Test("sbc yl", 0xFD, 0x9D);
            Z80Tester.Test("sbc (iy+$3D)", 0xFD, 0x9E, 0x3D);
            Z80Tester.Test("and yh", 0xFD, 0xA4);
            Z80Tester.Test("and yl", 0xFD, 0xA5);
            Z80Tester.Test("and (iy+$3D)", 0xFD, 0xA6, 0x3D);
            Z80Tester.Test("xor yh", 0xFD, 0xAC);
            Z80Tester.Test("xor yl", 0xFD, 0xAD);
            Z80Tester.Test("xor (iy+$3D)", 0xFD, 0xAE, 0x3D);
            Z80Tester.Test("or yh", 0xFD, 0xB4);
            Z80Tester.Test("or yl", 0xFD, 0xB5);
            Z80Tester.Test("or (iy+$3D)", 0xFD, 0xB6, 0x3D);
            Z80Tester.Test("cp yh", 0xFD, 0xBC);
            Z80Tester.Test("cp yl", 0xFD, 0xBD);
            Z80Tester.Test("cp (iy+$3D)", 0xFD, 0xBE, 0x3D);
            Z80Tester.Test("pop iy", 0xFD, 0xE1);
            Z80Tester.Test("ex (sp),iy", 0xFD, 0xE3);
            Z80Tester.Test("push iy", 0xFD, 0xE5);
            Z80Tester.Test("jp (iy)", 0xFD, 0xE9);
            Z80Tester.Test("ld sp,iy", 0xFD, 0xF9);
        }
    }
}