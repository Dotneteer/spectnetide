using Microsoft.VisualStudio.TestTools.UnitTesting;
using Spect.Net.Z80Emu.Test.Helpers;
// ReSharper disable InconsistentNaming

namespace Spect.Net.Z80Emu.Test.Disasm
{
    [TestClass]
    public class BitInstructionsTests
    {
        [TestMethod]
        public void BitInstructions0X00WorkAsExpected()
        {
            // --- Act
            Z80Tester.Test("rlc b", 0xCB, 0x00);
            Z80Tester.Test("rlc c", 0xCB, 0x01);
            Z80Tester.Test("rlc d", 0xCB, 0x02);
            Z80Tester.Test("rlc e", 0xCB, 0x03);
            Z80Tester.Test("rlc h", 0xCB, 0x04);
            Z80Tester.Test("rlc l", 0xCB, 0x05);
            Z80Tester.Test("rlc (hl)", 0xCB, 0x06);
            Z80Tester.Test("rlc a", 0xCB, 0x07);
            Z80Tester.Test("rrc b", 0xCB, 0x08);
            Z80Tester.Test("rrc c", 0xCB, 0x09);
            Z80Tester.Test("rrc d", 0xCB, 0x0A);
            Z80Tester.Test("rrc e", 0xCB, 0x0B);
            Z80Tester.Test("rrc h", 0xCB, 0x0C);
            Z80Tester.Test("rrc l", 0xCB, 0x0D);
            Z80Tester.Test("rrc (hl)", 0xCB, 0x0E);
            Z80Tester.Test("rrc a", 0xCB, 0x0F);
        }

        [TestMethod]
        public void BitInstructions0X10WorkAsExpected()
        {
            // --- Act
            Z80Tester.Test("rl b", 0xCB, 0x10);
            Z80Tester.Test("rl c", 0xCB, 0x11);
            Z80Tester.Test("rl d", 0xCB, 0x12);
            Z80Tester.Test("rl e", 0xCB, 0x13);
            Z80Tester.Test("rl h", 0xCB, 0x14);
            Z80Tester.Test("rl l", 0xCB, 0x15);
            Z80Tester.Test("rl (hl)", 0xCB, 0x16);
            Z80Tester.Test("rl a", 0xCB, 0x17);
            Z80Tester.Test("rr b", 0xCB, 0x18);
            Z80Tester.Test("rr c", 0xCB, 0x19);
            Z80Tester.Test("rr d", 0xCB, 0x1A);
            Z80Tester.Test("rr e", 0xCB, 0x1B);
            Z80Tester.Test("rr h", 0xCB, 0x1C);
            Z80Tester.Test("rr l", 0xCB, 0x1D);
            Z80Tester.Test("rr (hl)", 0xCB, 0x1E);
            Z80Tester.Test("rr a", 0xCB, 0x1F);
        }

        [TestMethod]
        public void BitInstructions0X20WorkAsExpected()
        {
            // --- Act
            Z80Tester.Test("sla b", 0xCB, 0x20);
            Z80Tester.Test("sla c", 0xCB, 0x21);
            Z80Tester.Test("sla d", 0xCB, 0x22);
            Z80Tester.Test("sla e", 0xCB, 0x23);
            Z80Tester.Test("sla h", 0xCB, 0x24);
            Z80Tester.Test("sla l", 0xCB, 0x25);
            Z80Tester.Test("sla (hl)", 0xCB, 0x26);
            Z80Tester.Test("sla a", 0xCB, 0x27);
            Z80Tester.Test("sra b", 0xCB, 0x28);
            Z80Tester.Test("sra c", 0xCB, 0x29);
            Z80Tester.Test("sra d", 0xCB, 0x2A);
            Z80Tester.Test("sra e", 0xCB, 0x2B);
            Z80Tester.Test("sra h", 0xCB, 0x2C);
            Z80Tester.Test("sra l", 0xCB, 0x2D);
            Z80Tester.Test("sra (hl)", 0xCB, 0x2E);
            Z80Tester.Test("sra a", 0xCB, 0x2F);
        }

        [TestMethod]
        public void BitInstructions0X30WorkAsExpected()
        {
            // --- Act
            Z80Tester.Test("sll b", 0xCB, 0x30);
            Z80Tester.Test("sll c", 0xCB, 0x31);
            Z80Tester.Test("sll d", 0xCB, 0x32);
            Z80Tester.Test("sll e", 0xCB, 0x33);
            Z80Tester.Test("sll h", 0xCB, 0x34);
            Z80Tester.Test("sll l", 0xCB, 0x35);
            Z80Tester.Test("sll (hl)", 0xCB, 0x36);
            Z80Tester.Test("sll a", 0xCB, 0x37);
            Z80Tester.Test("srl b", 0xCB, 0x38);
            Z80Tester.Test("srl c", 0xCB, 0x39);
            Z80Tester.Test("srl d", 0xCB, 0x3A);
            Z80Tester.Test("srl e", 0xCB, 0x3B);
            Z80Tester.Test("srl h", 0xCB, 0x3C);
            Z80Tester.Test("srl l", 0xCB, 0x3D);
            Z80Tester.Test("srl (hl)", 0xCB, 0x3E);
            Z80Tester.Test("srl a", 0xCB, 0x3F);
        }

        [TestMethod]
        public void BitInstructions0X40WorkAsExpected()
        {
            // --- Act
            Z80Tester.Test("bit 0,b", 0xCB, 0x40);
            Z80Tester.Test("bit 0,c", 0xCB, 0x41);
            Z80Tester.Test("bit 0,d", 0xCB, 0x42);
            Z80Tester.Test("bit 0,e", 0xCB, 0x43);
            Z80Tester.Test("bit 0,h", 0xCB, 0x44);
            Z80Tester.Test("bit 0,l", 0xCB, 0x45);
            Z80Tester.Test("bit 0,(hl)", 0xCB, 0x46);
            Z80Tester.Test("bit 0,a", 0xCB, 0x47);
            Z80Tester.Test("bit 1,b", 0xCB, 0x48);
            Z80Tester.Test("bit 1,c", 0xCB, 0x49);
            Z80Tester.Test("bit 1,d", 0xCB, 0x4A);
            Z80Tester.Test("bit 1,e", 0xCB, 0x4B);
            Z80Tester.Test("bit 1,h", 0xCB, 0x4C);
            Z80Tester.Test("bit 1,l", 0xCB, 0x4D);
            Z80Tester.Test("bit 1,(hl)", 0xCB, 0x4E);
            Z80Tester.Test("bit 1,a", 0xCB, 0x4F);
        }

        [TestMethod]
        public void BitInstructions0X50WorkAsExpected()
        {
            // --- Act
            Z80Tester.Test("bit 2,b", 0xCB, 0x50);
            Z80Tester.Test("bit 2,c", 0xCB, 0x51);
            Z80Tester.Test("bit 2,d", 0xCB, 0x52);
            Z80Tester.Test("bit 2,e", 0xCB, 0x53);
            Z80Tester.Test("bit 2,h", 0xCB, 0x54);
            Z80Tester.Test("bit 2,l", 0xCB, 0x55);
            Z80Tester.Test("bit 2,(hl)", 0xCB, 0x56);
            Z80Tester.Test("bit 2,a", 0xCB, 0x57);
            Z80Tester.Test("bit 3,b", 0xCB, 0x58);
            Z80Tester.Test("bit 3,c", 0xCB, 0x59);
            Z80Tester.Test("bit 3,d", 0xCB, 0x5A);
            Z80Tester.Test("bit 3,e", 0xCB, 0x5B);
            Z80Tester.Test("bit 3,h", 0xCB, 0x5C);
            Z80Tester.Test("bit 3,l", 0xCB, 0x5D);
            Z80Tester.Test("bit 3,(hl)", 0xCB, 0x5E);
            Z80Tester.Test("bit 3,a", 0xCB, 0x5F);
        }

        [TestMethod]
        public void BitInstructions0X60WorkAsExpected()
        {
            // --- Act
            Z80Tester.Test("bit 4,b", 0xCB, 0x60);
            Z80Tester.Test("bit 4,c", 0xCB, 0x61);
            Z80Tester.Test("bit 4,d", 0xCB, 0x62);
            Z80Tester.Test("bit 4,e", 0xCB, 0x63);
            Z80Tester.Test("bit 4,h", 0xCB, 0x64);
            Z80Tester.Test("bit 4,l", 0xCB, 0x65);
            Z80Tester.Test("bit 4,(hl)", 0xCB, 0x66);
            Z80Tester.Test("bit 4,a", 0xCB, 0x67);
            Z80Tester.Test("bit 5,b", 0xCB, 0x68);
            Z80Tester.Test("bit 5,c", 0xCB, 0x69);
            Z80Tester.Test("bit 5,d", 0xCB, 0x6A);
            Z80Tester.Test("bit 5,e", 0xCB, 0x6B);
            Z80Tester.Test("bit 5,h", 0xCB, 0x6C);
            Z80Tester.Test("bit 5,l", 0xCB, 0x6D);
            Z80Tester.Test("bit 5,(hl)", 0xCB, 0x6E);
            Z80Tester.Test("bit 5,a", 0xCB, 0x6F);
        }

        [TestMethod]
        public void BitInstructions0X70WorkAsExpected()
        {
            // --- Act
            Z80Tester.Test("bit 6,b", 0xCB, 0x70);
            Z80Tester.Test("bit 6,c", 0xCB, 0x71);
            Z80Tester.Test("bit 6,d", 0xCB, 0x72);
            Z80Tester.Test("bit 6,e", 0xCB, 0x73);
            Z80Tester.Test("bit 6,h", 0xCB, 0x74);
            Z80Tester.Test("bit 6,l", 0xCB, 0x75);
            Z80Tester.Test("bit 6,(hl)", 0xCB, 0x76);
            Z80Tester.Test("bit 6,a", 0xCB, 0x77);
            Z80Tester.Test("bit 7,b", 0xCB, 0x78);
            Z80Tester.Test("bit 7,c", 0xCB, 0x79);
            Z80Tester.Test("bit 7,d", 0xCB, 0x7A);
            Z80Tester.Test("bit 7,e", 0xCB, 0x7B);
            Z80Tester.Test("bit 7,h", 0xCB, 0x7C);
            Z80Tester.Test("bit 7,l", 0xCB, 0x7D);
            Z80Tester.Test("bit 7,(hl)", 0xCB, 0x7E);
            Z80Tester.Test("bit 7,a", 0xCB, 0x7F);
        }

        [TestMethod]
        public void BitInstructions0X80WorkAsExpected()
        {
            // --- Act
            Z80Tester.Test("res 0,b", 0xCB, 0x80);
            Z80Tester.Test("res 0,c", 0xCB, 0x81);
            Z80Tester.Test("res 0,d", 0xCB, 0x82);
            Z80Tester.Test("res 0,e", 0xCB, 0x83);
            Z80Tester.Test("res 0,h", 0xCB, 0x84);
            Z80Tester.Test("res 0,l", 0xCB, 0x85);
            Z80Tester.Test("res 0,(hl)", 0xCB, 0x86);
            Z80Tester.Test("res 0,a", 0xCB, 0x87);
            Z80Tester.Test("res 1,b", 0xCB, 0x88);
            Z80Tester.Test("res 1,c", 0xCB, 0x89);
            Z80Tester.Test("res 1,d", 0xCB, 0x8A);
            Z80Tester.Test("res 1,e", 0xCB, 0x8B);
            Z80Tester.Test("res 1,h", 0xCB, 0x8C);
            Z80Tester.Test("res 1,l", 0xCB, 0x8D);
            Z80Tester.Test("res 1,(hl)", 0xCB, 0x8E);
            Z80Tester.Test("res 1,a", 0xCB, 0x8F);
        }

        [TestMethod]
        public void BitInstructions0X90WorkAsExpected()
        {
            // --- Act
            Z80Tester.Test("res 2,b", 0xCB, 0x90);
            Z80Tester.Test("res 2,c", 0xCB, 0x91);
            Z80Tester.Test("res 2,d", 0xCB, 0x92);
            Z80Tester.Test("res 2,e", 0xCB, 0x93);
            Z80Tester.Test("res 2,h", 0xCB, 0x94);
            Z80Tester.Test("res 2,l", 0xCB, 0x95);
            Z80Tester.Test("res 2,(hl)", 0xCB, 0x96);
            Z80Tester.Test("res 2,a", 0xCB, 0x97);
            Z80Tester.Test("res 3,b", 0xCB, 0x98);
            Z80Tester.Test("res 3,c", 0xCB, 0x99);
            Z80Tester.Test("res 3,d", 0xCB, 0x9A);
            Z80Tester.Test("res 3,e", 0xCB, 0x9B);
            Z80Tester.Test("res 3,h", 0xCB, 0x9C);
            Z80Tester.Test("res 3,l", 0xCB, 0x9D);
            Z80Tester.Test("res 3,(hl)", 0xCB, 0x9E);
            Z80Tester.Test("res 3,a", 0xCB, 0x9F);
        }

        [TestMethod]
        public void BitInstructions0XA0WorkAsExpected()
        {
            // --- Act
            Z80Tester.Test("res 4,b", 0xCB, 0xA0);
            Z80Tester.Test("res 4,c", 0xCB, 0xA1);
            Z80Tester.Test("res 4,d", 0xCB, 0xA2);
            Z80Tester.Test("res 4,e", 0xCB, 0xA3);
            Z80Tester.Test("res 4,h", 0xCB, 0xA4);
            Z80Tester.Test("res 4,l", 0xCB, 0xA5);
            Z80Tester.Test("res 4,(hl)", 0xCB, 0xA6);
            Z80Tester.Test("res 4,a", 0xCB, 0xA7);
            Z80Tester.Test("res 5,b", 0xCB, 0xA8);
            Z80Tester.Test("res 5,c", 0xCB, 0xA9);
            Z80Tester.Test("res 5,d", 0xCB, 0xAA);
            Z80Tester.Test("res 5,e", 0xCB, 0xAB);
            Z80Tester.Test("res 5,h", 0xCB, 0xAC);
            Z80Tester.Test("res 5,l", 0xCB, 0xAD);
            Z80Tester.Test("res 5,(hl)", 0xCB, 0xAE);
            Z80Tester.Test("res 5,a", 0xCB, 0xAF);
        }

        [TestMethod]
        public void BitInstructions0XB0WorkAsExpected()
        {
            // --- Act
            Z80Tester.Test("res 6,b", 0xCB, 0xB0);
            Z80Tester.Test("res 6,c", 0xCB, 0xB1);
            Z80Tester.Test("res 6,d", 0xCB, 0xB2);
            Z80Tester.Test("res 6,e", 0xCB, 0xB3);
            Z80Tester.Test("res 6,h", 0xCB, 0xB4);
            Z80Tester.Test("res 6,l", 0xCB, 0xB5);
            Z80Tester.Test("res 6,(hl)", 0xCB, 0xB6);
            Z80Tester.Test("res 6,a", 0xCB, 0xB7);
            Z80Tester.Test("res 7,b", 0xCB, 0xB8);
            Z80Tester.Test("res 7,c", 0xCB, 0xB9);
            Z80Tester.Test("res 7,d", 0xCB, 0xBA);
            Z80Tester.Test("res 7,e", 0xCB, 0xBB);
            Z80Tester.Test("res 7,h", 0xCB, 0xBC);
            Z80Tester.Test("res 7,l", 0xCB, 0xBD);
            Z80Tester.Test("res 7,(hl)", 0xCB, 0xBE);
            Z80Tester.Test("res 7,a", 0xCB, 0xBF);
        }

        [TestMethod]
        public void BitInstructions0XC0WorkAsExpected()
        {
            // --- Act
            Z80Tester.Test("set 0,b", 0xCB, 0xC0);
            Z80Tester.Test("set 0,c", 0xCB, 0xC1);
            Z80Tester.Test("set 0,d", 0xCB, 0xC2);
            Z80Tester.Test("set 0,e", 0xCB, 0xC3);
            Z80Tester.Test("set 0,h", 0xCB, 0xC4);
            Z80Tester.Test("set 0,l", 0xCB, 0xC5);
            Z80Tester.Test("set 0,(hl)", 0xCB, 0xC6);
            Z80Tester.Test("set 0,a", 0xCB, 0xC7);
            Z80Tester.Test("set 1,b", 0xCB, 0xC8);
            Z80Tester.Test("set 1,c", 0xCB, 0xC9);
            Z80Tester.Test("set 1,d", 0xCB, 0xCA);
            Z80Tester.Test("set 1,e", 0xCB, 0xCB);
            Z80Tester.Test("set 1,h", 0xCB, 0xCC);
            Z80Tester.Test("set 1,l", 0xCB, 0xCD);
            Z80Tester.Test("set 1,(hl)", 0xCB, 0xCE);
            Z80Tester.Test("set 1,a", 0xCB, 0xCF);
        }

        [TestMethod]
        public void BitInstructions0XD0WorkAsExpected()
        {
            // --- Act
            Z80Tester.Test("set 2,b", 0xCB, 0xD0);
            Z80Tester.Test("set 2,c", 0xCB, 0xD1);
            Z80Tester.Test("set 2,d", 0xCB, 0xD2);
            Z80Tester.Test("set 2,e", 0xCB, 0xD3);
            Z80Tester.Test("set 2,h", 0xCB, 0xD4);
            Z80Tester.Test("set 2,l", 0xCB, 0xD5);
            Z80Tester.Test("set 2,(hl)", 0xCB, 0xD6);
            Z80Tester.Test("set 2,a", 0xCB, 0xD7);
            Z80Tester.Test("set 3,b", 0xCB, 0xD8);
            Z80Tester.Test("set 3,c", 0xCB, 0xD9);
            Z80Tester.Test("set 3,d", 0xCB, 0xDA);
            Z80Tester.Test("set 3,e", 0xCB, 0xDB);
            Z80Tester.Test("set 3,h", 0xCB, 0xDC);
            Z80Tester.Test("set 3,l", 0xCB, 0xDD);
            Z80Tester.Test("set 3,(hl)", 0xCB, 0xDE);
            Z80Tester.Test("set 3,a", 0xCB, 0xDF);
        }

        [TestMethod]
        public void BitInstructions0XE0WorkAsExpected()
        {
            // --- Act
            Z80Tester.Test("set 4,b", 0xCB, 0xE0);
            Z80Tester.Test("set 4,c", 0xCB, 0xE1);
            Z80Tester.Test("set 4,d", 0xCB, 0xE2);
            Z80Tester.Test("set 4,e", 0xCB, 0xE3);
            Z80Tester.Test("set 4,h", 0xCB, 0xE4);
            Z80Tester.Test("set 4,l", 0xCB, 0xE5);
            Z80Tester.Test("set 4,(hl)", 0xCB, 0xE6);
            Z80Tester.Test("set 4,a", 0xCB, 0xE7);
            Z80Tester.Test("set 5,b", 0xCB, 0xE8);
            Z80Tester.Test("set 5,c", 0xCB, 0xE9);
            Z80Tester.Test("set 5,d", 0xCB, 0xEA);
            Z80Tester.Test("set 5,e", 0xCB, 0xEB);
            Z80Tester.Test("set 5,h", 0xCB, 0xEC);
            Z80Tester.Test("set 5,l", 0xCB, 0xED);
            Z80Tester.Test("set 5,(hl)", 0xCB, 0xEE);
            Z80Tester.Test("set 5,a", 0xCB, 0xEF);
        }

        [TestMethod]
        public void BitInstructions0XF0WorkAsExpected()
        {
            // --- Act
            Z80Tester.Test("set 6,b", 0xCB, 0xF0);
            Z80Tester.Test("set 6,c", 0xCB, 0xF1);
            Z80Tester.Test("set 6,d", 0xCB, 0xF2);
            Z80Tester.Test("set 6,e", 0xCB, 0xF3);
            Z80Tester.Test("set 6,h", 0xCB, 0xF4);
            Z80Tester.Test("set 6,l", 0xCB, 0xF5);
            Z80Tester.Test("set 6,(hl)", 0xCB, 0xF6);
            Z80Tester.Test("set 6,a", 0xCB, 0xF7);
            Z80Tester.Test("set 7,b", 0xCB, 0xF8);
            Z80Tester.Test("set 7,c", 0xCB, 0xF9);
            Z80Tester.Test("set 7,d", 0xCB, 0xFA);
            Z80Tester.Test("set 7,e", 0xCB, 0xFB);
            Z80Tester.Test("set 7,h", 0xCB, 0xFC);
            Z80Tester.Test("set 7,l", 0xCB, 0xFD);
            Z80Tester.Test("set 7,(hl)", 0xCB, 0xFE);
            Z80Tester.Test("set 7,a", 0xCB, 0xFF);
        }
    }
}