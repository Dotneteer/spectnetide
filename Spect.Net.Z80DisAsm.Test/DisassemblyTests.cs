using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Spect.Net.Z80DisAsm.Test.Helpers;
// ReSharper disable InconsistentNaming

namespace Spect.Net.Z80DisAsm.Test
{
    [TestClass]
    public class DisassemblyTests
    {
        private static byte[] s_Spectrum48Rom;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            s_Spectrum48Rom = FileHelper.ExtractResourceFile("ZXSpectrum48.bin");
        }

        [TestMethod]
        public void DisassemblerWorks()
        {
            // --- Arrange
            var project = new Z80DisAsmProject(s_Spectrum48Rom);
            var disasm = new Z80Disassembler(project);

            // --- Act
            var output = disasm.Disassemble(0, 100);
            foreach (var item in output.OutputItems)
            {
                Console.WriteLine(item);
            }
        }

        [TestMethod]
        public void StandardInstructions0X00WorkAaExpected()
        {
            // --- Act
            Z80Tester.Test("nop", 0x00);
            Z80Tester.Test("ld bc,$1234", 0x01, 0x34, 0x12);
            Z80Tester.Test("ld (bc),a", 0x02);
            Z80Tester.Test("inc bc", 0x03);
            Z80Tester.Test("inc b", 0x04);
            Z80Tester.Test("dec b", 0x05);
            Z80Tester.Test("ld b,$23", 0x06, 0x23);
            Z80Tester.Test("rlca", 0x07);
            Z80Tester.Test("ex af,af'", 0x08);
            Z80Tester.Test("add hl,bc", 0x09);
            Z80Tester.Test("ld a,(bc)", 0x0A);
            Z80Tester.Test("dec bc", 0x0B);
            Z80Tester.Test("inc c", 0x0C);
            Z80Tester.Test("dec c", 0x0D);
            Z80Tester.Test("ld c,$23", 0x0E, 0x23);
            Z80Tester.Test("rrca", 0x0F);
        }

        [TestMethod]
        public void StandardInstructions0X10WorkAsExpected()
        {
            // --- Act
            Z80Tester.Test("djnz L0002", 0x10, 0x00);
            Z80Tester.Test("djnz L0022", 0x10, 0x20);
            Z80Tester.Test("djnz LFFF2", 0x10, 0xF0);
            Z80Tester.Test("ld de,$1234", 0x11, 0x34, 0x12);
            Z80Tester.Test("ld (de),a", 0x12);
            Z80Tester.Test("inc de", 0x13);
            Z80Tester.Test("inc d", 0x14);
            Z80Tester.Test("dec d", 0x15);
            Z80Tester.Test("ld d,$23", 0x16, 0x23);
            Z80Tester.Test("rla", 0x17);
            Z80Tester.Test("jr L0002", 0x18, 0x00);
            Z80Tester.Test("jr L0022", 0x18, 0x20);
            Z80Tester.Test("jr LFFF2", 0x18, 0xF0);
            Z80Tester.Test("add hl,de", 0x19);
            Z80Tester.Test("ld a,(de)", 0x1A);
            Z80Tester.Test("dec de", 0x1B);
            Z80Tester.Test("inc e", 0x1C);
            Z80Tester.Test("dec e", 0x1D);
            Z80Tester.Test("ld e,$23", 0x1E, 0x23);
            Z80Tester.Test("rra", 0x1F);
        }

        [TestMethod]
        public void StandardInstructions0X20WorkAsExpected()
        {
            // --- Act
            Z80Tester.Test("jr nz,L0002", 0x20, 0x00);
            Z80Tester.Test("jr nz,L0022", 0x20, 0x20);
            Z80Tester.Test("jr nz,LFFF2", 0x20, 0xF0);
            Z80Tester.Test("ld hl,$1234", 0x21, 0x34, 0x12);
            Z80Tester.Test("ld ($3456),hl", 0x22, 0x56, 0x34);
            Z80Tester.Test("inc hl", 0x23);
            Z80Tester.Test("inc h", 0x24);
            Z80Tester.Test("dec h", 0x25);
            Z80Tester.Test("ld h,$23", 0x26, 0x23);
            Z80Tester.Test("daa", 0x27);
            Z80Tester.Test("jr z,L0002", 0x28, 0x00);
            Z80Tester.Test("jr z,L0022", 0x28, 0x20);
            Z80Tester.Test("jr z,LFFF2", 0x28, 0xF0);
            Z80Tester.Test("add hl,hl", 0x29);
            Z80Tester.Test("ld hl,($3456)", 0x2A, 0x56, 0x34);
            Z80Tester.Test("dec hl", 0x2B);
            Z80Tester.Test("inc l", 0x2C);
            Z80Tester.Test("dec l", 0x2D);
            Z80Tester.Test("ld l,$23", 0x2E, 0x23);
            Z80Tester.Test("cpl", 0x2F);
        }

        [TestMethod]
        public void StandardInstructions0X30WorkAsExpected()
        {
            // --- Act
            Z80Tester.Test("jr nc,L0002", 0x30, 0x00);
            Z80Tester.Test("jr nc,L0022", 0x30, 0x20);
            Z80Tester.Test("jr nc,LFFF2", 0x30, 0xF0);
            Z80Tester.Test("ld sp,$1234", 0x31, 0x34, 0x12);
            Z80Tester.Test("ld ($3456),a", 0x32, 0x56, 0x34);
            Z80Tester.Test("inc sp", 0x33);
            Z80Tester.Test("inc (hl)", 0x34);
            Z80Tester.Test("dec (hl)", 0x35);
            Z80Tester.Test("ld (hl),$23", 0x36, 0x23);
            Z80Tester.Test("scf", 0x37);
            Z80Tester.Test("jr c,L0002", 0x38, 0x00);
            Z80Tester.Test("jr c,L0022", 0x38, 0x20);
            Z80Tester.Test("jr c,LFFF2", 0x38, 0xF0);
            Z80Tester.Test("add hl,sp", 0x39);
            Z80Tester.Test("ld a,($3456)", 0x3A, 0x56, 0x34);
            Z80Tester.Test("dec sp", 0x3B);
            Z80Tester.Test("inc a", 0x3C);
            Z80Tester.Test("dec a", 0x3D);
            Z80Tester.Test("ld a,$23", 0x3E, 0x23);
            Z80Tester.Test("ccf", 0x3F);
        }

        [TestMethod]
        public void StandardInstructions0X40WorkAsExpected()
        {
            // --- Act
            Z80Tester.Test("ld b,b", 0x40);
            Z80Tester.Test("ld b,c", 0x41);
            Z80Tester.Test("ld b,d", 0x42);
            Z80Tester.Test("ld b,e", 0x43);
            Z80Tester.Test("ld b,h", 0x44);
            Z80Tester.Test("ld b,l", 0x45);
            Z80Tester.Test("ld b,(hl)", 0x46);
            Z80Tester.Test("ld b,a", 0x47);
            Z80Tester.Test("ld c,b", 0x48);
            Z80Tester.Test("ld c,c", 0x49);
            Z80Tester.Test("ld c,d", 0x4A);
            Z80Tester.Test("ld c,e", 0x4B);
            Z80Tester.Test("ld c,h", 0x4C);
            Z80Tester.Test("ld c,l", 0x4D);
            Z80Tester.Test("ld c,(hl)", 0x4E);
            Z80Tester.Test("ld c,a", 0x4F);
        }

        [TestMethod]
        public void StandardInstructions0X50WorkAsExpected()
        {
            // --- Act
            Z80Tester.Test("ld d,b", 0x50);
            Z80Tester.Test("ld d,c", 0x51);
            Z80Tester.Test("ld d,d", 0x52);
            Z80Tester.Test("ld d,e", 0x53);
            Z80Tester.Test("ld d,h", 0x54);
            Z80Tester.Test("ld d,l", 0x55);
            Z80Tester.Test("ld d,(hl)", 0x56);
            Z80Tester.Test("ld d,a", 0x57);
            Z80Tester.Test("ld e,b", 0x58);
            Z80Tester.Test("ld e,c", 0x59);
            Z80Tester.Test("ld e,d", 0x5A);
            Z80Tester.Test("ld e,e", 0x5B);
            Z80Tester.Test("ld e,h", 0x5C);
            Z80Tester.Test("ld e,l", 0x5D);
            Z80Tester.Test("ld e,(hl)", 0x5E);
            Z80Tester.Test("ld e,a", 0x5F);
        }

        [TestMethod]
        public void StandardInstructions0X60WorkAsExpected()
        {
            // --- Act
            Z80Tester.Test("ld h,b", 0x60);
            Z80Tester.Test("ld h,c", 0x61);
            Z80Tester.Test("ld h,d", 0x62);
            Z80Tester.Test("ld h,e", 0x63);
            Z80Tester.Test("ld h,h", 0x64);
            Z80Tester.Test("ld h,l", 0x65);
            Z80Tester.Test("ld h,(hl)", 0x66);
            Z80Tester.Test("ld h,a", 0x67);
            Z80Tester.Test("ld l,b", 0x68);
            Z80Tester.Test("ld l,c", 0x69);
            Z80Tester.Test("ld l,d", 0x6A);
            Z80Tester.Test("ld l,e", 0x6B);
            Z80Tester.Test("ld l,h", 0x6C);
            Z80Tester.Test("ld l,l", 0x6D);
            Z80Tester.Test("ld l,(hl)", 0x6E);
            Z80Tester.Test("ld l,a", 0x6F);
        }

        [TestMethod]
        public void StandardInstructions0X70WorkAsExpected()
        {
            // --- Act
            Z80Tester.Test("ld (hl),b", 0x70);
            Z80Tester.Test("ld (hl),c", 0x71);
            Z80Tester.Test("ld (hl),d", 0x72);
            Z80Tester.Test("ld (hl),e", 0x73);
            Z80Tester.Test("ld (hl),h", 0x74);
            Z80Tester.Test("ld (hl),l", 0x75);
            Z80Tester.Test("halt", 0x76);
            Z80Tester.Test("ld (hl),a", 0x77);
            Z80Tester.Test("ld a,b", 0x78);
            Z80Tester.Test("ld a,c", 0x79);
            Z80Tester.Test("ld a,d", 0x7A);
            Z80Tester.Test("ld a,e", 0x7B);
            Z80Tester.Test("ld a,h", 0x7C);
            Z80Tester.Test("ld a,l", 0x7D);
            Z80Tester.Test("ld a,(hl)", 0x7E);
            Z80Tester.Test("ld a,a", 0x7F);
        }

        [TestMethod]
        public void StandardInstructions0X80WorkAsExpected()
        {
            // --- Act
            Z80Tester.Test("add a,b", 0x80);
            Z80Tester.Test("add a,c", 0x81);
            Z80Tester.Test("add a,d", 0x82);
            Z80Tester.Test("add a,e", 0x83);
            Z80Tester.Test("add a,h", 0x84);
            Z80Tester.Test("add a,l", 0x85);
            Z80Tester.Test("add a,(hl)", 0x86);
            Z80Tester.Test("add a,a", 0x87);
            Z80Tester.Test("adc a,b", 0x88);
            Z80Tester.Test("adc a,c", 0x89);
            Z80Tester.Test("adc a,d", 0x8A);
            Z80Tester.Test("adc a,e", 0x8B);
            Z80Tester.Test("adc a,h", 0x8C);
            Z80Tester.Test("adc a,l", 0x8D);
            Z80Tester.Test("adc a,(hl)", 0x8E);
            Z80Tester.Test("adc a,a", 0x8F);
        }

        [TestMethod]
        public void StandardInstructions0X90WorkAsExpected()
        {
            // --- Act
            Z80Tester.Test("sub b", 0x90);
            Z80Tester.Test("sub c", 0x91);
            Z80Tester.Test("sub d", 0x92);
            Z80Tester.Test("sub e", 0x93);
            Z80Tester.Test("sub h", 0x94);
            Z80Tester.Test("sub l", 0x95);
            Z80Tester.Test("sub (hl)", 0x96);
            Z80Tester.Test("sub a", 0x97);
            Z80Tester.Test("sbc b", 0x98);
            Z80Tester.Test("sbc c", 0x99);
            Z80Tester.Test("sbc d", 0x9A);
            Z80Tester.Test("sbc e", 0x9B);
            Z80Tester.Test("sbc h", 0x9C);
            Z80Tester.Test("sbc l", 0x9D);
            Z80Tester.Test("sbc (hl)", 0x9E);
            Z80Tester.Test("sbc a", 0x9F);
        }

        [TestMethod]
        public void StandardInstructions0XA0WorkAsExpected()
        {
            // --- Act
            Z80Tester.Test("and b", 0xA0);
            Z80Tester.Test("and c", 0xA1);
            Z80Tester.Test("and d", 0xA2);
            Z80Tester.Test("and e", 0xA3);
            Z80Tester.Test("and h", 0xA4);
            Z80Tester.Test("and l", 0xA5);
            Z80Tester.Test("and (hl)", 0xA6);
            Z80Tester.Test("and a", 0xA7);
            Z80Tester.Test("xor b", 0xA8);
            Z80Tester.Test("xor c", 0xA9);
            Z80Tester.Test("xor d", 0xAA);
            Z80Tester.Test("xor e", 0xAB);
            Z80Tester.Test("xor h", 0xAC);
            Z80Tester.Test("xor l", 0xAD);
            Z80Tester.Test("xor (hl)", 0xAE);
            Z80Tester.Test("xor a", 0xAF);
        }

        [TestMethod]
        public void StandardInstructions0XB0WorkAsExpected()
        {
            // --- Act
            Z80Tester.Test("or b", 0xB0);
            Z80Tester.Test("or c", 0xB1);
            Z80Tester.Test("or d", 0xB2);
            Z80Tester.Test("or e", 0xB3);
            Z80Tester.Test("or h", 0xB4);
            Z80Tester.Test("or l", 0xB5);
            Z80Tester.Test("or (hl)", 0xB6);
            Z80Tester.Test("or a", 0xB7);
            Z80Tester.Test("cp b", 0xB8);
            Z80Tester.Test("cp c", 0xB9);
            Z80Tester.Test("cp d", 0xBA);
            Z80Tester.Test("cp e", 0xBB);
            Z80Tester.Test("cp h", 0xBC);
            Z80Tester.Test("cp l", 0xBD);
            Z80Tester.Test("cp (hl)", 0xBE);
            Z80Tester.Test("cp a", 0xBF);
        }

        [TestMethod]
        public void StandardInstructions0XC0WorkAsExpected()
        {
            // --- Act
            Z80Tester.Test("ret nz", 0xC0);
            Z80Tester.Test("pop bc", 0xC1);
            Z80Tester.Test("jp nz,L5678", 0xC2, 0x78, 0x56);
            Z80Tester.Test("jp L5678", 0xC3, 0x78, 0x56);
            Z80Tester.Test("call nz,L5678", 0xC4, 0x78, 0x56);
            Z80Tester.Test("push bc", 0xC5);
            Z80Tester.Test("add a,$34", 0xC6, 0x34);
            Z80Tester.Test("rst $00", 0xC7);
            Z80Tester.Test("ret z", 0xC8);
            Z80Tester.Test("ret", 0xC9);
            Z80Tester.Test("jp z,L5678", 0xCA, 0x78, 0x56);
            // -- 0xCB is the bit operation prefix
            Z80Tester.Test("call z,L5678", 0xCC, 0x78, 0x56);
            Z80Tester.Test("call L5678", 0xCD, 0x78, 0x56);
            Z80Tester.Test("adc a,$34", 0xCE, 0x34);
            Z80Tester.Test("rst $08", 0xCF);
        }

        [TestMethod]
        public void StandardInstructions0XD0WorkAsExpected()
        {
            // --- Act
            Z80Tester.Test("ret nc", 0xD0);
            Z80Tester.Test("pop de", 0xD1);
            Z80Tester.Test("jp nc,L5678", 0xD2, 0x78, 0x56);
            Z80Tester.Test("out ($78),a", 0xD3, 0x78);
            Z80Tester.Test("call nc,L5678", 0xD4, 0x78, 0x56);
            Z80Tester.Test("push de", 0xD5);
            Z80Tester.Test("sub $34", 0xD6, 0x34);
            Z80Tester.Test("rst $10", 0xD7);
            Z80Tester.Test("ret c", 0xD8);
            Z80Tester.Test("exx", 0xD9);
            Z80Tester.Test("jp c,L5678", 0xDA, 0x78, 0x56);
            Z80Tester.Test("in a,($78)", 0xDB, 0x78);
            Z80Tester.Test("call c,L5678", 0xDC, 0x78, 0x56);
            // -- 0xDD is the IX operation prefix
            Z80Tester.Test("sbc a,$34", 0xDE, 0x34);
            Z80Tester.Test("rst $18", 0xDF);
        }

        [TestMethod]
        public void StandardInstructions0XE0WorkAsExpected()
        {
            // --- Act
            Z80Tester.Test("ret po", 0xE0);
            Z80Tester.Test("pop hl", 0xE1);
            Z80Tester.Test("jp po,L5678", 0xE2, 0x78, 0x56);
            Z80Tester.Test("ex (sp),hl", 0xE3);
            Z80Tester.Test("call po,L5678", 0xE4, 0x78, 0x56);
            Z80Tester.Test("push hl", 0xE5);
            Z80Tester.Test("and $34", 0xE6, 0x34);
            Z80Tester.Test("rst $20", 0xE7);
            Z80Tester.Test("ret pe", 0xE8);
            Z80Tester.Test("jp (hl)", 0xE9);
            Z80Tester.Test("jp pe,L5678", 0xEA, 0x78, 0x56);
            Z80Tester.Test("ex de,hl", 0xEB);
            Z80Tester.Test("call pe,L5678", 0xEC, 0x78, 0x56);
            // -- 0xED is the extended operation prefix
            Z80Tester.Test("xor $34", 0xEE, 0x34);
            Z80Tester.Test("rst $28", 0xEF);
        }

        [TestMethod]
        public void StandardInstructions0XF0WorkAsExpected()
        {
            // --- Act
            Z80Tester.Test("ret p", 0xF0);
            Z80Tester.Test("pop af", 0xF1);
            Z80Tester.Test("jp p,L5678", 0xF2, 0x78, 0x56);
            Z80Tester.Test("di", 0xF3);
            Z80Tester.Test("call p,L5678", 0xF4, 0x78, 0x56);
            Z80Tester.Test("push af", 0xF5);
            Z80Tester.Test("or $34", 0xF6, 0x34);
            Z80Tester.Test("rst $30", 0xF7);
            Z80Tester.Test("ret m", 0xF8);
            Z80Tester.Test("ld sp,hl", 0xF9);
            Z80Tester.Test("jp m,L5678", 0xFA, 0x78, 0x56);
            Z80Tester.Test("ei", 0xFB);
            Z80Tester.Test("call m,L5678", 0xFC, 0x78, 0x56);
            // -- 0xFD is the IY operation prefix
            Z80Tester.Test("cp $34", 0xFE, 0x34);
            Z80Tester.Test("rst $38", 0xFF);
        }

        [TestMethod]
        public void ExtendedInstructions0X40WorkAsExpected()
        {
            // --- Act
            Z80Tester.Test("in b,(c)", 0xED, 0x40);
            Z80Tester.Test("out (c),b", 0xED, 0x41);
            Z80Tester.Test("sbc hl,bc", 0xED, 0x42);
            Z80Tester.Test("ld ($BC9A),bc", 0xED, 0x43, 0x9A, 0xBC);
            Z80Tester.Test("neg", 0xED, 0x44);
            Z80Tester.Test("retn", 0xED, 0x45);
            Z80Tester.Test("im 0", 0xED, 0x46);
            Z80Tester.Test("ld i,a", 0xED, 0x47);
            Z80Tester.Test("in c,(c)", 0xED, 0x48);
            Z80Tester.Test("out (c),c", 0xED, 0x49);
            Z80Tester.Test("adc hl,bc", 0xED, 0x4A);
            Z80Tester.Test("ld bc,($BC9A)", 0xED, 0x4B, 0x9A, 0xBC);
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
            Z80Tester.Test("ld ($BC9A),de", 0xED, 0x53, 0x9A, 0xBC);
            Z80Tester.Test("neg", 0xED, 0x54);
            Z80Tester.Test("retn", 0xED, 0x55);
            Z80Tester.Test("im 1", 0xED, 0x56);
            Z80Tester.Test("ld a,i", 0xED, 0x57);
            Z80Tester.Test("in e,(c)", 0xED, 0x58);
            Z80Tester.Test("out (c),e", 0xED, 0x59);
            Z80Tester.Test("adc hl,de", 0xED, 0x5A);
            Z80Tester.Test("ld de,($BC9A)", 0xED, 0x5B, 0x9A, 0xBC);
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
            Z80Tester.Test("ld ($BC9A),hl", 0xED, 0x63, 0x9A, 0xBC);
            Z80Tester.Test("neg", 0xED, 0x64);
            Z80Tester.Test("retn", 0xED, 0x65);
            Z80Tester.Test("im 0", 0xED, 0x66);
            Z80Tester.Test("rrd", 0xED, 0x67);
            Z80Tester.Test("in l,(c)", 0xED, 0x68);
            Z80Tester.Test("out (c),l", 0xED, 0x69);
            Z80Tester.Test("adc hl,hl", 0xED, 0x6A);
            Z80Tester.Test("ld hl,($BC9A)", 0xED, 0x6B, 0x9A, 0xBC);
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
            Z80Tester.Test("ld ($BC9A),sp", 0xED, 0x73, 0x9A, 0xBC);
            Z80Tester.Test("neg", 0xED, 0x74);
            Z80Tester.Test("retn", 0xED, 0x75);
            Z80Tester.Test("im 1", 0xED, 0x76);
            Z80Tester.Test("in a,(c)", 0xED, 0x78);
            Z80Tester.Test("out (c),a", 0xED, 0x79);
            Z80Tester.Test("adc hl,sp", 0xED, 0x7A);
            Z80Tester.Test("ld sp,($BC9A)", 0xED, 0x7B, 0x9A, 0xBC);
            Z80Tester.Test("neg", 0xED, 0x7C);
            Z80Tester.Test("retn", 0xED, 0x7D);
            Z80Tester.Test("im 2", 0xED, 0x7E);
        }

        [TestMethod]
        public void ExtendedInstructions0XA0WorkAsExpected()
        {
            // --- Act
            Z80Tester.Test("ldi", 0xED, 0xA0);
            Z80Tester.Test("cpi", 0xED, 0xA1);
            Z80Tester.Test("ini", 0xED, 0xA2);
            Z80Tester.Test("outi", 0xED, 0xA3);
            Z80Tester.Test("ldd", 0xED, 0xA8);
            Z80Tester.Test("cpd", 0xED, 0xA9);
            Z80Tester.Test("ind", 0xED, 0xAA);
            Z80Tester.Test("outd", 0xED, 0xAB);
        }

        [TestMethod]
        public void ExtendedInstructions0XB0WorkAsExpected()
        {
            // --- Act
            Z80Tester.Test("ldir", 0xED, 0xB0);
            Z80Tester.Test("cpir", 0xED, 0xB1);
            Z80Tester.Test("inir", 0xED, 0xB2);
            Z80Tester.Test("otir", 0xED, 0xB3);
            Z80Tester.Test("lddr", 0xED, 0xB8);
            Z80Tester.Test("cpdr", 0xED, 0xB9);
            Z80Tester.Test("indr", 0xED, 0xBA);
            Z80Tester.Test("otdr", 0xED, 0xBB);
        }

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
