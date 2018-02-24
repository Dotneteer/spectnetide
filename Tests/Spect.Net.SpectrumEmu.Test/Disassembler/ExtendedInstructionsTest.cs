using Microsoft.VisualStudio.TestTools.UnitTesting;
using Spect.Net.SpectrumEmu.Test.Helpers;

// ReSharper disable InconsistentNaming

namespace Spect.Net.SpectrumEmu.Test.Disassembler
{
    [TestClass]
    public class ExtendedInstructionsTest
    {
        [TestMethod]
        public void NextExtendedInstructionsWorkAsExpected()
        {
            // --- Act
            Z80Tester.TestExt("swapnib", 0xED, 0x23);
            Z80Tester.Test("nop", 0xED, 0x23);

            Z80Tester.TestExt("mirror a", 0xED, 0x24);
            Z80Tester.Test("nop", 0xED, 0x24);

            Z80Tester.TestExt("ld hl,sp", 0xED, 0x25);
            Z80Tester.Test("nop", 0xED, 0x25);

            Z80Tester.TestExt("mirror de", 0xED, 0x26);
            Z80Tester.Test("nop", 0xED, 0x26);

            Z80Tester.TestExt("test #C4", 0xED, 0x27, 0xC4);
            Z80Tester.Test("nop", 0xED, 0x27);

            Z80Tester.TestExt("mul", 0xED, 0x30);
            Z80Tester.Test("nop", 0xED, 0x30);

            Z80Tester.TestExt("add hl,a", 0xED, 0x31);
            Z80Tester.Test("nop", 0xED, 0x31);

            Z80Tester.TestExt("add de,a", 0xED, 0x32);
            Z80Tester.Test("nop", 0xED, 0x32);

            Z80Tester.TestExt("add bc,a", 0xED, 0x33);
            Z80Tester.Test("nop", 0xED, 0x33);

            Z80Tester.TestExt("inc dehl", 0xED, 0x37);
            Z80Tester.Test("nop", 0xED, 0x37);

            Z80Tester.TestExt("dec dehl", 0xED, 0x38);
            Z80Tester.Test("nop", 0xED, 0x38);

            Z80Tester.TestExt("add dehl,a", 0xED, 0x39);
            Z80Tester.Test("nop", 0xED, 0x39);

            Z80Tester.TestExt("add dehl,bc", 0xED, 0x3A);
            Z80Tester.Test("nop", 0xED, 0x3A);

            Z80Tester.TestExt("add dehl,#3456", 0xED, 0x3B, 0x56, 0x34);
            Z80Tester.Test("nop", 0xED, 0x3B);

            Z80Tester.TestExt("sub dehl,a", 0xED, 0x3C);
            Z80Tester.Test("nop", 0xED, 0x3C);

            Z80Tester.TestExt("sub dehl,bc", 0xED, 0x3D);
            Z80Tester.Test("nop", 0xED, 0x3D);

            Z80Tester.TestExt("push #34AF", 0xED, 0x8A, 0xAF, 0x34);
            Z80Tester.Test("nop", 0xED, 0x8A);

            Z80Tester.TestExt("popx", 0xED, 0x8B);
            Z80Tester.Test("nop", 0xED, 0x8B);

            Z80Tester.TestExt("nextreg #34,#56", 0xED, 0x91, 0x34, 0x56);
            Z80Tester.Test("nop", 0xED, 0x91);

            Z80Tester.TestExt("nextreg #34,a", 0xED, 0x92, 0x34);
            Z80Tester.Test("nop", 0xED, 0x92);
        }

        [TestMethod]
        public void ExtendedInstructions0X40WorkAsExpected()
        {
            // --- Act
            Z80Tester.Test("in b,(c)", 0xED, 0x40);
            Z80Tester.Test("out (c),b", 0xED, 0x41);
            Z80Tester.Test("sbc hl,bc", 0xED, 0x42);
            Z80Tester.Test("ld (#BC9A),bc", 0xED, 0x43, 0x9A, 0xBC);
            Z80Tester.Test("neg", 0xED, 0x44);
            Z80Tester.Test("retn", 0xED, 0x45);
            Z80Tester.Test("im 0", 0xED, 0x46);
            Z80Tester.Test("ld i,a", 0xED, 0x47);
            Z80Tester.Test("in c,(c)", 0xED, 0x48);
            Z80Tester.Test("out (c),c", 0xED, 0x49);
            Z80Tester.Test("adc hl,bc", 0xED, 0x4A);
            Z80Tester.Test("ld bc,(#BC9A)", 0xED, 0x4B, 0x9A, 0xBC);
            Z80Tester.Test("neg", 0xED, 0x4C);
            Z80Tester.Test("reti", 0xED, 0x4D);
            Z80Tester.Test("im 0", 0xED, 0x4E);
            Z80Tester.Test("ld r,a", 0xED, 0x4F);
        }

        [TestMethod]
        public void ExtendedInstructions0X50WorkAsExpected()
        {
            // --- Act
            Z80Tester.Test("in d,(c)", 0xED, 0x50);
            Z80Tester.Test("out (c),d", 0xED, 0x51);
            Z80Tester.Test("sbc hl,de", 0xED, 0x52);
            Z80Tester.Test("ld (#BC9A),de", 0xED, 0x53, 0x9A, 0xBC);
            Z80Tester.Test("neg", 0xED, 0x54);
            Z80Tester.Test("retn", 0xED, 0x55);
            Z80Tester.Test("im 1", 0xED, 0x56);
            Z80Tester.Test("ld a,i", 0xED, 0x57);
            Z80Tester.Test("in e,(c)", 0xED, 0x58);
            Z80Tester.Test("out (c),e", 0xED, 0x59);
            Z80Tester.Test("adc hl,de", 0xED, 0x5A);
            Z80Tester.Test("ld de,(#BC9A)", 0xED, 0x5B, 0x9A, 0xBC);
            Z80Tester.Test("neg", 0xED, 0x5C);
            Z80Tester.Test("retn", 0xED, 0x5D);
            Z80Tester.Test("im 2", 0xED, 0x5E);
            Z80Tester.Test("ld a,r", 0xED, 0x5F);
        }

        [TestMethod]
        public void ExtendedInstructions0X60WorkAsExpected()
        {
            // --- Act
            Z80Tester.Test("in h,(c)", 0xED, 0x60);
            Z80Tester.Test("out (c),h", 0xED, 0x61);
            Z80Tester.Test("sbc hl,hl", 0xED, 0x62);
            Z80Tester.Test("ld (#BC9A),hl", 0xED, 0x63, 0x9A, 0xBC);
            Z80Tester.Test("neg", 0xED, 0x64);
            Z80Tester.Test("retn", 0xED, 0x65);
            Z80Tester.Test("im 0", 0xED, 0x66);
            Z80Tester.Test("rrd", 0xED, 0x67);
            Z80Tester.Test("in l,(c)", 0xED, 0x68);
            Z80Tester.Test("out (c),l", 0xED, 0x69);
            Z80Tester.Test("adc hl,hl", 0xED, 0x6A);
            Z80Tester.Test("ld hl,(#BC9A)", 0xED, 0x6B, 0x9A, 0xBC);
            Z80Tester.Test("neg", 0xED, 0x6C);
            Z80Tester.Test("retn", 0xED, 0x6D);
            Z80Tester.Test("im 0", 0xED, 0x6E);
            Z80Tester.Test("rld", 0xED, 0x6F);
        }

        [TestMethod]
        public void ExtendedInstructions0X70WorkAsExpected()
        {
            // --- Act
            Z80Tester.Test("in (c)", 0xED, 0x70);
            Z80Tester.Test("out (c),0", 0xED, 0x71);
            Z80Tester.Test("sbc hl,sp", 0xED, 0x72);
            Z80Tester.Test("ld (#BC9A),sp", 0xED, 0x73, 0x9A, 0xBC);
            Z80Tester.Test("neg", 0xED, 0x74);
            Z80Tester.Test("retn", 0xED, 0x75);
            Z80Tester.Test("im 1", 0xED, 0x76);
            Z80Tester.Test("nop", 0xED, 0x77);
            Z80Tester.Test("in a,(c)", 0xED, 0x78);
            Z80Tester.Test("out (c),a", 0xED, 0x79);
            Z80Tester.Test("adc hl,sp", 0xED, 0x7A);
            Z80Tester.Test("ld sp,(#BC9A)", 0xED, 0x7B, 0x9A, 0xBC);
            Z80Tester.Test("neg", 0xED, 0x7C);
            Z80Tester.Test("retn", 0xED, 0x7D);
            Z80Tester.Test("im 2", 0xED, 0x7E);
            Z80Tester.Test("nop", 0xED, 0x7F);
        }

        [TestMethod]
        public void ExtendedInstructions0XA0WorkAsExpected()
        {
            // --- Act
            Z80Tester.Test("ldi", 0xED, 0xA0);
            Z80Tester.Test("cpi", 0xED, 0xA1);
            Z80Tester.Test("ini", 0xED, 0xA2);
            Z80Tester.Test("outi", 0xED, 0xA3);
            Z80Tester.Test("nop", 0xED, 0xA4);
            Z80Tester.Test("nop", 0xED, 0xA5);
            Z80Tester.Test("nop", 0xED, 0xA6);
            Z80Tester.Test("nop", 0xED, 0xA7);
            Z80Tester.Test("ldd", 0xED, 0xA8);
            Z80Tester.Test("cpd", 0xED, 0xA9);
            Z80Tester.Test("ind", 0xED, 0xAA);
            Z80Tester.Test("outd", 0xED, 0xAB);
            Z80Tester.Test("nop", 0xED, 0xAC);
            Z80Tester.Test("nop", 0xED, 0xAD);
            Z80Tester.Test("nop", 0xED, 0xAE);
            Z80Tester.Test("nop", 0xED, 0xAF);
        }

        [TestMethod]
        public void ExtendedInstructions0XB0WorkAsExpected()
        {
            // --- Act
            Z80Tester.Test("ldir", 0xED, 0xB0);
            Z80Tester.Test("cpir", 0xED, 0xB1);
            Z80Tester.Test("inir", 0xED, 0xB2);
            Z80Tester.Test("otir", 0xED, 0xB3);
            Z80Tester.Test("nop", 0xED, 0xB4);
            Z80Tester.Test("nop", 0xED, 0xB5);
            Z80Tester.Test("nop", 0xED, 0xB6);
            Z80Tester.Test("nop", 0xED, 0xB7);
            Z80Tester.Test("lddr", 0xED, 0xB8);
            Z80Tester.Test("cpdr", 0xED, 0xB9);
            Z80Tester.Test("indr", 0xED, 0xBA);
            Z80Tester.Test("otdr", 0xED, 0xBB);
            Z80Tester.Test("nop", 0xED, 0xBC);
            Z80Tester.Test("nop", 0xED, 0xBD);
            Z80Tester.Test("nop", 0xED, 0xBE);
            Z80Tester.Test("nop", 0xED, 0xBF);
        }

        [TestMethod]
        public void InvalidExtendedInstructionsWorkAsNop()
        {
            // --- Act
            for (var op = 0x00; op < 0x40; op++)
            {
                Z80Tester.Test("nop", 0xED, (byte)op);
            }
            for (var op = 0xC0; op < 0x100; op++)
            {
                Z80Tester.Test("nop", 0xED, (byte)op);
            }
        }
    }
}