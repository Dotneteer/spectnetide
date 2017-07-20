using Microsoft.VisualStudio.TestTools.UnitTesting;
using Spect.Net.SpectrumEmu.Test.Helpers;

// ReSharper disable InconsistentNaming

namespace Spect.Net.SpectrumEmu.Test.Disassembler
{
    [TestClass]
    public class IyBitOpTests
    {
        [TestMethod]
        public void BitInstructions0X00WorkAsExpected()
        {
            // --- Act
            Z80Tester.Test("rlc (iy+$3D),b", 0xFD, 0xCB, 0x3D, 0x00);
            Z80Tester.Test("rlc (iy+$3D),c", 0xFD, 0xCB, 0x3D, 0x01);
            Z80Tester.Test("rlc (iy+$3D),d", 0xFD, 0xCB, 0x3D, 0x02);
            Z80Tester.Test("rlc (iy+$3D),e", 0xFD, 0xCB, 0x3D, 0x03);
            Z80Tester.Test("rlc (iy+$3D),h", 0xFD, 0xCB, 0x3D, 0x04);
            Z80Tester.Test("rlc (iy+$3D),l", 0xFD, 0xCB, 0x3D, 0x05);
            Z80Tester.Test("rlc (iy+$3D)", 0xFD, 0xCB, 0x3D, 0x06);
            Z80Tester.Test("rlc (iy+$3D),a", 0xFD, 0xCB, 0x3D, 0x07);
            Z80Tester.Test("rrc (iy+$3D),b", 0xFD, 0xCB, 0x3D, 0x08);
            Z80Tester.Test("rrc (iy+$3D),c", 0xFD, 0xCB, 0x3D, 0x09);
            Z80Tester.Test("rrc (iy+$3D),d", 0xFD, 0xCB, 0x3D, 0x0A);
            Z80Tester.Test("rrc (iy+$3D),e", 0xFD, 0xCB, 0x3D, 0x0B);
            Z80Tester.Test("rrc (iy+$3D),h", 0xFD, 0xCB, 0x3D, 0x0C);
            Z80Tester.Test("rrc (iy+$3D),l", 0xFD, 0xCB, 0x3D, 0x0D);
            Z80Tester.Test("rrc (iy+$3D)", 0xFD, 0xCB, 0x3D, 0x0E);
            Z80Tester.Test("rrc (iy+$3D),a", 0xFD, 0xCB, 0x3D, 0x0F);
        }

        [TestMethod]
        public void BitInstructions0X10WorkAsExpected()
        {
            // --- Act
            Z80Tester.Test("rl (iy+$3D),b", 0xFD, 0xCB, 0x3D, 0x10);
            Z80Tester.Test("rl (iy+$3D),c", 0xFD, 0xCB, 0x3D, 0x11);
            Z80Tester.Test("rl (iy+$3D),d", 0xFD, 0xCB, 0x3D, 0x12);
            Z80Tester.Test("rl (iy+$3D),e", 0xFD, 0xCB, 0x3D, 0x13);
            Z80Tester.Test("rl (iy+$3D),h", 0xFD, 0xCB, 0x3D, 0x14);
            Z80Tester.Test("rl (iy+$3D),l", 0xFD, 0xCB, 0x3D, 0x15);
            Z80Tester.Test("rl (iy+$3D)", 0xFD, 0xCB, 0x3D, 0x16);
            Z80Tester.Test("rl (iy+$3D),a", 0xFD, 0xCB, 0x3D, 0x17);
            Z80Tester.Test("rr (iy+$3D),b", 0xFD, 0xCB, 0x3D, 0x18);
            Z80Tester.Test("rr (iy+$3D),c", 0xFD, 0xCB, 0x3D, 0x19);
            Z80Tester.Test("rr (iy+$3D),d", 0xFD, 0xCB, 0x3D, 0x1A);
            Z80Tester.Test("rr (iy+$3D),e", 0xFD, 0xCB, 0x3D, 0x1B);
            Z80Tester.Test("rr (iy+$3D),h", 0xFD, 0xCB, 0x3D, 0x1C);
            Z80Tester.Test("rr (iy+$3D),l", 0xFD, 0xCB, 0x3D, 0x1D);
            Z80Tester.Test("rr (iy+$3D)", 0xFD, 0xCB, 0x3D, 0x1E);
            Z80Tester.Test("rr (iy+$3D),a", 0xFD, 0xCB, 0x3D, 0x1F);
        }

        [TestMethod]
        public void BitInstructions0X20WorkAsExpected()
        {
            // --- Act
            Z80Tester.Test("sla (iy+$3D),b", 0xFD, 0xCB, 0x3D, 0x20);
            Z80Tester.Test("sla (iy+$3D),c", 0xFD, 0xCB, 0x3D, 0x21);
            Z80Tester.Test("sla (iy+$3D),d", 0xFD, 0xCB, 0x3D, 0x22);
            Z80Tester.Test("sla (iy+$3D),e", 0xFD, 0xCB, 0x3D, 0x23);
            Z80Tester.Test("sla (iy+$3D),h", 0xFD, 0xCB, 0x3D, 0x24);
            Z80Tester.Test("sla (iy+$3D),l", 0xFD, 0xCB, 0x3D, 0x25);
            Z80Tester.Test("sla (iy+$3D)", 0xFD, 0xCB, 0x3D, 0x26);
            Z80Tester.Test("sla (iy+$3D),a", 0xFD, 0xCB, 0x3D, 0x27);
            Z80Tester.Test("sra (iy+$3D),b", 0xFD, 0xCB, 0x3D, 0x28);
            Z80Tester.Test("sra (iy+$3D),c", 0xFD, 0xCB, 0x3D, 0x29);
            Z80Tester.Test("sra (iy+$3D),d", 0xFD, 0xCB, 0x3D, 0x2A);
            Z80Tester.Test("sra (iy+$3D),e", 0xFD, 0xCB, 0x3D, 0x2B);
            Z80Tester.Test("sra (iy+$3D),h", 0xFD, 0xCB, 0x3D, 0x2C);
            Z80Tester.Test("sra (iy+$3D),l", 0xFD, 0xCB, 0x3D, 0x2D);
            Z80Tester.Test("sra (iy+$3D)", 0xFD, 0xCB, 0x3D, 0x2E);
            Z80Tester.Test("sra (iy+$3D),a", 0xFD, 0xCB, 0x3D, 0x2F);
        }

        [TestMethod]
        public void BitInstructions0X30WorkAsExpected()
        {
            // --- Act
            Z80Tester.Test("sll (iy+$3D),b", 0xFD, 0xCB, 0x3D, 0x30);
            Z80Tester.Test("sll (iy+$3D),c", 0xFD, 0xCB, 0x3D, 0x31);
            Z80Tester.Test("sll (iy+$3D),d", 0xFD, 0xCB, 0x3D, 0x32);
            Z80Tester.Test("sll (iy+$3D),e", 0xFD, 0xCB, 0x3D, 0x33);
            Z80Tester.Test("sll (iy+$3D),h", 0xFD, 0xCB, 0x3D, 0x34);
            Z80Tester.Test("sll (iy+$3D),l", 0xFD, 0xCB, 0x3D, 0x35);
            Z80Tester.Test("sll (iy+$3D)", 0xFD, 0xCB, 0x3D, 0x36);
            Z80Tester.Test("sll (iy+$3D),a", 0xFD, 0xCB, 0x3D, 0x37);
            Z80Tester.Test("srl (iy+$3D),b", 0xFD, 0xCB, 0x3D, 0x38);
            Z80Tester.Test("srl (iy+$3D),c", 0xFD, 0xCB, 0x3D, 0x39);
            Z80Tester.Test("srl (iy+$3D),d", 0xFD, 0xCB, 0x3D, 0x3A);
            Z80Tester.Test("srl (iy+$3D),e", 0xFD, 0xCB, 0x3D, 0x3B);
            Z80Tester.Test("srl (iy+$3D),h", 0xFD, 0xCB, 0x3D, 0x3C);
            Z80Tester.Test("srl (iy+$3D),l", 0xFD, 0xCB, 0x3D, 0x3D);
            Z80Tester.Test("srl (iy+$3D)", 0xFD, 0xCB, 0x3D, 0x3E);
            Z80Tester.Test("srl (iy+$3D),a", 0xFD, 0xCB, 0x3D, 0x3F);
        }

        [TestMethod]
        public void BitInstructions0X40WorkAsExpected()
        {
            // --- Act
            Z80Tester.Test("bit 2,(iy+$3D)", 0xFD, 0xCB, 0x3D, 0x50);
            Z80Tester.Test("bit 2,(iy+$3D)", 0xFD, 0xCB, 0x3D, 0x51);
            Z80Tester.Test("bit 2,(iy+$3D)", 0xFD, 0xCB, 0x3D, 0x52);
            Z80Tester.Test("bit 2,(iy+$3D)", 0xFD, 0xCB, 0x3D, 0x53);
            Z80Tester.Test("bit 2,(iy+$3D)", 0xFD, 0xCB, 0x3D, 0x54);
            Z80Tester.Test("bit 2,(iy+$3D)", 0xFD, 0xCB, 0x3D, 0x55);
            Z80Tester.Test("bit 2,(iy+$3D)", 0xFD, 0xCB, 0x3D, 0x56);
            Z80Tester.Test("bit 2,(iy+$3D)", 0xFD, 0xCB, 0x3D, 0x57);
            Z80Tester.Test("bit 3,(iy+$3D)", 0xFD, 0xCB, 0x3D, 0x58);
            Z80Tester.Test("bit 3,(iy+$3D)", 0xFD, 0xCB, 0x3D, 0x59);
            Z80Tester.Test("bit 3,(iy+$3D)", 0xFD, 0xCB, 0x3D, 0x5A);
            Z80Tester.Test("bit 3,(iy+$3D)", 0xFD, 0xCB, 0x3D, 0x5B);
            Z80Tester.Test("bit 3,(iy+$3D)", 0xFD, 0xCB, 0x3D, 0x5C);
            Z80Tester.Test("bit 3,(iy+$3D)", 0xFD, 0xCB, 0x3D, 0x5D);
            Z80Tester.Test("bit 3,(iy+$3D)", 0xFD, 0xCB, 0x3D, 0x5E);
            Z80Tester.Test("bit 3,(iy+$3D)", 0xFD, 0xCB, 0x3D, 0x5F);
        }

        [TestMethod]
        public void BitInstructions0X50WorkAsExpected()
        {
            // --- Act
            Z80Tester.Test("bit 4,(iy+$3D)", 0xFD, 0xCB, 0x3D, 0x60);
            Z80Tester.Test("bit 4,(iy+$3D)", 0xFD, 0xCB, 0x3D, 0x61);
            Z80Tester.Test("bit 4,(iy+$3D)", 0xFD, 0xCB, 0x3D, 0x62);
            Z80Tester.Test("bit 4,(iy+$3D)", 0xFD, 0xCB, 0x3D, 0x63);
            Z80Tester.Test("bit 4,(iy+$3D)", 0xFD, 0xCB, 0x3D, 0x64);
            Z80Tester.Test("bit 4,(iy+$3D)", 0xFD, 0xCB, 0x3D, 0x65);
            Z80Tester.Test("bit 4,(iy+$3D)", 0xFD, 0xCB, 0x3D, 0x66);
            Z80Tester.Test("bit 4,(iy+$3D)", 0xFD, 0xCB, 0x3D, 0x67);
            Z80Tester.Test("bit 5,(iy+$3D)", 0xFD, 0xCB, 0x3D, 0x68);
            Z80Tester.Test("bit 5,(iy+$3D)", 0xFD, 0xCB, 0x3D, 0x69);
            Z80Tester.Test("bit 5,(iy+$3D)", 0xFD, 0xCB, 0x3D, 0x6A);
            Z80Tester.Test("bit 5,(iy+$3D)", 0xFD, 0xCB, 0x3D, 0x6B);
            Z80Tester.Test("bit 5,(iy+$3D)", 0xFD, 0xCB, 0x3D, 0x6C);
            Z80Tester.Test("bit 5,(iy+$3D)", 0xFD, 0xCB, 0x3D, 0x6D);
            Z80Tester.Test("bit 5,(iy+$3D)", 0xFD, 0xCB, 0x3D, 0x6E);
            Z80Tester.Test("bit 5,(iy+$3D)", 0xFD, 0xCB, 0x3D, 0x6F);
        }

        [TestMethod]
        public void BitInstructions0X60WorkAsExpected()
        {
            // --- Act
            Z80Tester.Test("bit 6,(iy+$3D)", 0xFD, 0xCB, 0x3D, 0x70);
            Z80Tester.Test("bit 6,(iy+$3D)", 0xFD, 0xCB, 0x3D, 0x71);
            Z80Tester.Test("bit 6,(iy+$3D)", 0xFD, 0xCB, 0x3D, 0x72);
            Z80Tester.Test("bit 6,(iy+$3D)", 0xFD, 0xCB, 0x3D, 0x73);
            Z80Tester.Test("bit 6,(iy+$3D)", 0xFD, 0xCB, 0x3D, 0x74);
            Z80Tester.Test("bit 6,(iy+$3D)", 0xFD, 0xCB, 0x3D, 0x75);
            Z80Tester.Test("bit 6,(iy+$3D)", 0xFD, 0xCB, 0x3D, 0x76);
            Z80Tester.Test("bit 6,(iy+$3D)", 0xFD, 0xCB, 0x3D, 0x77);
            Z80Tester.Test("bit 7,(iy+$3D)", 0xFD, 0xCB, 0x3D, 0x78);
            Z80Tester.Test("bit 7,(iy+$3D)", 0xFD, 0xCB, 0x3D, 0x79);
            Z80Tester.Test("bit 7,(iy+$3D)", 0xFD, 0xCB, 0x3D, 0x7A);
            Z80Tester.Test("bit 7,(iy+$3D)", 0xFD, 0xCB, 0x3D, 0x7B);
            Z80Tester.Test("bit 7,(iy+$3D)", 0xFD, 0xCB, 0x3D, 0x7C);
            Z80Tester.Test("bit 7,(iy+$3D)", 0xFD, 0xCB, 0x3D, 0x7D);
            Z80Tester.Test("bit 7,(iy+$3D)", 0xFD, 0xCB, 0x3D, 0x7E);
            Z80Tester.Test("bit 7,(iy+$3D)", 0xFD, 0xCB, 0x3D, 0x7F);
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
            Z80Tester.Test("res 0,(iy+$3D),b", 0xFD, 0xCB, 0x3D, 0x80);
            Z80Tester.Test("res 0,(iy+$3D),c", 0xFD, 0xCB, 0x3D, 0x81);
            Z80Tester.Test("res 0,(iy+$3D),d", 0xFD, 0xCB, 0x3D, 0x82);
            Z80Tester.Test("res 0,(iy+$3D),e", 0xFD, 0xCB, 0x3D, 0x83);
            Z80Tester.Test("res 0,(iy+$3D),h", 0xFD, 0xCB, 0x3D, 0x84);
            Z80Tester.Test("res 0,(iy+$3D),l", 0xFD, 0xCB, 0x3D, 0x85);
            Z80Tester.Test("res 0,(iy+$3D)", 0xFD, 0xCB, 0x3D, 0x86);
            Z80Tester.Test("res 0,(iy+$3D),a", 0xFD, 0xCB, 0x3D, 0x87);
            Z80Tester.Test("res 1,(iy+$3D),b", 0xFD, 0xCB, 0x3D, 0x88);
            Z80Tester.Test("res 1,(iy+$3D),c", 0xFD, 0xCB, 0x3D, 0x89);
            Z80Tester.Test("res 1,(iy+$3D),d", 0xFD, 0xCB, 0x3D, 0x8A);
            Z80Tester.Test("res 1,(iy+$3D),e", 0xFD, 0xCB, 0x3D, 0x8B);
            Z80Tester.Test("res 1,(iy+$3D),h", 0xFD, 0xCB, 0x3D, 0x8C);
            Z80Tester.Test("res 1,(iy+$3D),l", 0xFD, 0xCB, 0x3D, 0x8D);
            Z80Tester.Test("res 1,(iy+$3D)", 0xFD, 0xCB, 0x3D, 0x8E);
            Z80Tester.Test("res 1,(iy+$3D),a", 0xFD, 0xCB, 0x3D, 0x8F);
        }

        [TestMethod]
        public void BitInstructions0X90WorkAsExpected()
        {
            // --- Act
            Z80Tester.Test("res 2,(iy+$3D),b", 0xFD, 0xCB, 0x3D, 0x90);
            Z80Tester.Test("res 2,(iy+$3D),c", 0xFD, 0xCB, 0x3D, 0x91);
            Z80Tester.Test("res 2,(iy+$3D),d", 0xFD, 0xCB, 0x3D, 0x92);
            Z80Tester.Test("res 2,(iy+$3D),e", 0xFD, 0xCB, 0x3D, 0x93);
            Z80Tester.Test("res 2,(iy+$3D),h", 0xFD, 0xCB, 0x3D, 0x94);
            Z80Tester.Test("res 2,(iy+$3D),l", 0xFD, 0xCB, 0x3D, 0x95);
            Z80Tester.Test("res 2,(iy+$3D)", 0xFD, 0xCB, 0x3D, 0x96);
            Z80Tester.Test("res 2,(iy+$3D),a", 0xFD, 0xCB, 0x3D, 0x97);
            Z80Tester.Test("res 3,(iy+$3D),b", 0xFD, 0xCB, 0x3D, 0x98);
            Z80Tester.Test("res 3,(iy+$3D),c", 0xFD, 0xCB, 0x3D, 0x99);
            Z80Tester.Test("res 3,(iy+$3D),d", 0xFD, 0xCB, 0x3D, 0x9A);
            Z80Tester.Test("res 3,(iy+$3D),e", 0xFD, 0xCB, 0x3D, 0x9B);
            Z80Tester.Test("res 3,(iy+$3D),h", 0xFD, 0xCB, 0x3D, 0x9C);
            Z80Tester.Test("res 3,(iy+$3D),l", 0xFD, 0xCB, 0x3D, 0x9D);
            Z80Tester.Test("res 3,(iy+$3D)", 0xFD, 0xCB, 0x3D, 0x9E);
            Z80Tester.Test("res 3,(iy+$3D),a", 0xFD, 0xCB, 0x3D, 0x9F);
        }

        [TestMethod]
        public void BitInstructions0XA0WorkAsExpected()
        {
            // --- Act
            Z80Tester.Test("res 4,(iy+$3D),b", 0xFD, 0xCB, 0x3D, 0xA0);
            Z80Tester.Test("res 4,(iy+$3D),c", 0xFD, 0xCB, 0x3D, 0xA1);
            Z80Tester.Test("res 4,(iy+$3D),d", 0xFD, 0xCB, 0x3D, 0xA2);
            Z80Tester.Test("res 4,(iy+$3D),e", 0xFD, 0xCB, 0x3D, 0xA3);
            Z80Tester.Test("res 4,(iy+$3D),h", 0xFD, 0xCB, 0x3D, 0xA4);
            Z80Tester.Test("res 4,(iy+$3D),l", 0xFD, 0xCB, 0x3D, 0xA5);
            Z80Tester.Test("res 4,(iy+$3D)", 0xFD, 0xCB, 0x3D, 0xA6);
            Z80Tester.Test("res 4,(iy+$3D),a", 0xFD, 0xCB, 0x3D, 0xA7);
            Z80Tester.Test("res 5,(iy+$3D),b", 0xFD, 0xCB, 0x3D, 0xA8);
            Z80Tester.Test("res 5,(iy+$3D),c", 0xFD, 0xCB, 0x3D, 0xA9);
            Z80Tester.Test("res 5,(iy+$3D),d", 0xFD, 0xCB, 0x3D, 0xAA);
            Z80Tester.Test("res 5,(iy+$3D),e", 0xFD, 0xCB, 0x3D, 0xAB);
            Z80Tester.Test("res 5,(iy+$3D),h", 0xFD, 0xCB, 0x3D, 0xAC);
            Z80Tester.Test("res 5,(iy+$3D),l", 0xFD, 0xCB, 0x3D, 0xAD);
            Z80Tester.Test("res 5,(iy+$3D)", 0xFD, 0xCB, 0x3D, 0xAE);
            Z80Tester.Test("res 5,(iy+$3D),a", 0xFD, 0xCB, 0x3D, 0xAF);
        }

        [TestMethod]
        public void BitInstructions0XB0WorkAsExpected()
        {
            // --- Act
            Z80Tester.Test("res 6,(iy+$3D),b", 0xFD, 0xCB, 0x3D, 0xB0);
            Z80Tester.Test("res 6,(iy+$3D),c", 0xFD, 0xCB, 0x3D, 0xB1);
            Z80Tester.Test("res 6,(iy+$3D),d", 0xFD, 0xCB, 0x3D, 0xB2);
            Z80Tester.Test("res 6,(iy+$3D),e", 0xFD, 0xCB, 0x3D, 0xB3);
            Z80Tester.Test("res 6,(iy+$3D),h", 0xFD, 0xCB, 0x3D, 0xB4);
            Z80Tester.Test("res 6,(iy+$3D),l", 0xFD, 0xCB, 0x3D, 0xB5);
            Z80Tester.Test("res 6,(iy+$3D)", 0xFD, 0xCB, 0x3D, 0xB6);
            Z80Tester.Test("res 6,(iy+$3D),a", 0xFD, 0xCB, 0x3D, 0xB7);
            Z80Tester.Test("res 7,(iy+$3D),b", 0xFD, 0xCB, 0x3D, 0xB8);
            Z80Tester.Test("res 7,(iy+$3D),c", 0xFD, 0xCB, 0x3D, 0xB9);
            Z80Tester.Test("res 7,(iy+$3D),d", 0xFD, 0xCB, 0x3D, 0xBA);
            Z80Tester.Test("res 7,(iy+$3D),e", 0xFD, 0xCB, 0x3D, 0xBB);
            Z80Tester.Test("res 7,(iy+$3D),h", 0xFD, 0xCB, 0x3D, 0xBC);
            Z80Tester.Test("res 7,(iy+$3D),l", 0xFD, 0xCB, 0x3D, 0xBD);
            Z80Tester.Test("res 7,(iy+$3D)", 0xFD, 0xCB, 0x3D, 0xBE);
            Z80Tester.Test("res 7,(iy+$3D),a", 0xFD, 0xCB, 0x3D, 0xBF);
        }

        [TestMethod]
        public void BitInstructions0XC0WorkAsExpected()
        {
            // --- Act
            Z80Tester.Test("set 0,(iy+$3D),b", 0xFD, 0xCB, 0x3D, 0xC0);
            Z80Tester.Test("set 0,(iy+$3D),c", 0xFD, 0xCB, 0x3D, 0xC1);
            Z80Tester.Test("set 0,(iy+$3D),d", 0xFD, 0xCB, 0x3D, 0xC2);
            Z80Tester.Test("set 0,(iy+$3D),e", 0xFD, 0xCB, 0x3D, 0xC3);
            Z80Tester.Test("set 0,(iy+$3D),h", 0xFD, 0xCB, 0x3D, 0xC4);
            Z80Tester.Test("set 0,(iy+$3D),l", 0xFD, 0xCB, 0x3D, 0xC5);
            Z80Tester.Test("set 0,(iy+$3D)", 0xFD, 0xCB, 0x3D, 0xC6);
            Z80Tester.Test("set 0,(iy+$3D),a", 0xFD, 0xCB, 0x3D, 0xC7);
            Z80Tester.Test("set 1,(iy+$3D),b", 0xFD, 0xCB, 0x3D, 0xC8);
            Z80Tester.Test("set 1,(iy+$3D),c", 0xFD, 0xCB, 0x3D, 0xC9);
            Z80Tester.Test("set 1,(iy+$3D),d", 0xFD, 0xCB, 0x3D, 0xCA);
            Z80Tester.Test("set 1,(iy+$3D),e", 0xFD, 0xCB, 0x3D, 0xCB);
            Z80Tester.Test("set 1,(iy+$3D),h", 0xFD, 0xCB, 0x3D, 0xCC);
            Z80Tester.Test("set 1,(iy+$3D),l", 0xFD, 0xCB, 0x3D, 0xCD);
            Z80Tester.Test("set 1,(iy+$3D)", 0xFD, 0xCB, 0x3D, 0xCE);
            Z80Tester.Test("set 1,(iy+$3D),a", 0xFD, 0xCB, 0x3D, 0xCF);
        }

        [TestMethod]
        public void BitInstructions0XD0WorkAsExpected()
        {
            // --- Act
            Z80Tester.Test("set 2,(iy+$3D),b", 0xFD, 0xCB, 0x3D, 0xD0);
            Z80Tester.Test("set 2,(iy+$3D),c", 0xFD, 0xCB, 0x3D, 0xD1);
            Z80Tester.Test("set 2,(iy+$3D),d", 0xFD, 0xCB, 0x3D, 0xD2);
            Z80Tester.Test("set 2,(iy+$3D),e", 0xFD, 0xCB, 0x3D, 0xD3);
            Z80Tester.Test("set 2,(iy+$3D),h", 0xFD, 0xCB, 0x3D, 0xD4);
            Z80Tester.Test("set 2,(iy+$3D),l", 0xFD, 0xCB, 0x3D, 0xD5);
            Z80Tester.Test("set 2,(iy+$3D)", 0xFD, 0xCB, 0x3D, 0xD6);
            Z80Tester.Test("set 2,(iy+$3D),a", 0xFD, 0xCB, 0x3D, 0xD7);
            Z80Tester.Test("set 3,(iy+$3D),b", 0xFD, 0xCB, 0x3D, 0xD8);
            Z80Tester.Test("set 3,(iy+$3D),c", 0xFD, 0xCB, 0x3D, 0xD9);
            Z80Tester.Test("set 3,(iy+$3D),d", 0xFD, 0xCB, 0x3D, 0xDA);
            Z80Tester.Test("set 3,(iy+$3D),e", 0xFD, 0xCB, 0x3D, 0xDB);
            Z80Tester.Test("set 3,(iy+$3D),h", 0xFD, 0xCB, 0x3D, 0xDC);
            Z80Tester.Test("set 3,(iy+$3D),l", 0xFD, 0xCB, 0x3D, 0xDD);
            Z80Tester.Test("set 3,(iy+$3D)", 0xFD, 0xCB, 0x3D, 0xDE);
            Z80Tester.Test("set 3,(iy+$3D),a", 0xFD, 0xCB, 0x3D, 0xDF);
        }

        [TestMethod]
        public void BitInstructions0XE0WorkAsExpected()
        {
            // --- Act
            Z80Tester.Test("set 4,(iy+$3D),b", 0xFD, 0xCB, 0x3D, 0xE0);
            Z80Tester.Test("set 4,(iy+$3D),c", 0xFD, 0xCB, 0x3D, 0xE1);
            Z80Tester.Test("set 4,(iy+$3D),d", 0xFD, 0xCB, 0x3D, 0xE2);
            Z80Tester.Test("set 4,(iy+$3D),e", 0xFD, 0xCB, 0x3D, 0xE3);
            Z80Tester.Test("set 4,(iy+$3D),h", 0xFD, 0xCB, 0x3D, 0xE4);
            Z80Tester.Test("set 4,(iy+$3D),l", 0xFD, 0xCB, 0x3D, 0xE5);
            Z80Tester.Test("set 4,(iy+$3D)", 0xFD, 0xCB, 0x3D, 0xE6);
            Z80Tester.Test("set 4,(iy+$3D),a", 0xFD, 0xCB, 0x3D, 0xE7);
            Z80Tester.Test("set 5,(iy+$3D),b", 0xFD, 0xCB, 0x3D, 0xE8);
            Z80Tester.Test("set 5,(iy+$3D),c", 0xFD, 0xCB, 0x3D, 0xE9);
            Z80Tester.Test("set 5,(iy+$3D),d", 0xFD, 0xCB, 0x3D, 0xEA);
            Z80Tester.Test("set 5,(iy+$3D),e", 0xFD, 0xCB, 0x3D, 0xEB);
            Z80Tester.Test("set 5,(iy+$3D),h", 0xFD, 0xCB, 0x3D, 0xEC);
            Z80Tester.Test("set 5,(iy+$3D),l", 0xFD, 0xCB, 0x3D, 0xED);
            Z80Tester.Test("set 5,(iy+$3D)", 0xFD, 0xCB, 0x3D, 0xEE);
            Z80Tester.Test("set 5,(iy+$3D),a", 0xFD, 0xCB, 0x3D, 0xEF);
        }

        [TestMethod]
        public void BitInstructions0XF0WorkAsExpected()
        {
            // --- Act
            Z80Tester.Test("set 6,(iy+$3D),b", 0xFD, 0xCB, 0x3D, 0xF0);
            Z80Tester.Test("set 6,(iy+$3D),c", 0xFD, 0xCB, 0x3D, 0xF1);
            Z80Tester.Test("set 6,(iy+$3D),d", 0xFD, 0xCB, 0x3D, 0xF2);
            Z80Tester.Test("set 6,(iy+$3D),e", 0xFD, 0xCB, 0x3D, 0xF3);
            Z80Tester.Test("set 6,(iy+$3D),h", 0xFD, 0xCB, 0x3D, 0xF4);
            Z80Tester.Test("set 6,(iy+$3D),l", 0xFD, 0xCB, 0x3D, 0xF5);
            Z80Tester.Test("set 6,(iy+$3D)", 0xFD, 0xCB, 0x3D, 0xF6);
            Z80Tester.Test("set 6,(iy+$3D),a", 0xFD, 0xCB, 0x3D, 0xF7);
            Z80Tester.Test("set 7,(iy+$3D),b", 0xFD, 0xCB, 0x3D, 0xF8);
            Z80Tester.Test("set 7,(iy+$3D),c", 0xFD, 0xCB, 0x3D, 0xF9);
            Z80Tester.Test("set 7,(iy+$3D),d", 0xFD, 0xCB, 0x3D, 0xFA);
            Z80Tester.Test("set 7,(iy+$3D),e", 0xFD, 0xCB, 0x3D, 0xFB);
            Z80Tester.Test("set 7,(iy+$3D),h", 0xFD, 0xCB, 0x3D, 0xFC);
            Z80Tester.Test("set 7,(iy+$3D),l", 0xFD, 0xCB, 0x3D, 0xFD);
            Z80Tester.Test("set 7,(iy+$3D)", 0xFD, 0xCB, 0x3D, 0xFE);
            Z80Tester.Test("set 7,(iy+$3D),a", 0xFD, 0xCB, 0x3D, 0xFF);
        }
    }
}