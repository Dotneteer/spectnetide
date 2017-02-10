using System.Collections.Generic;

namespace Spect.Net.Z80DisAsm
{
    public partial class Z80Disassembler
    {
        /// <summary>
        /// 16-bit register pairs for the #Q pragma
        /// </summary>
        private static readonly string[] s_Q16Regs = {"bc", "de", "hl", "sp"};

        /// <summary>
        /// 16-bit register pairs for the #R pragma
        /// </summary>
        private static readonly string[] s_R16Regs = { "bc", "de", "hl", "af" };

        /// <summary>
        /// 8-bit register pairs for the #q pragma
        /// </summary>
        private static readonly string[] s_Q8Regs = { "b", "c", "d", "e", "h", "l", "(hl)", "a" };

        /// <summary>
        /// Standard Z80 instructions
        /// </summary>
        private static readonly InstructionTable s_StandardInstructions = new InstructionTable(
            new List<AsmInstructionBase>
            {
                new SimpleInstruction(0x00, "nop"),
                new MaskedInstruction(0x01, 0xCF, "ld #Q,#W"),
                new MaskedInstruction(0x02, 0xEF, "ld (#Q),a"),
                new MaskedInstruction(0x03, 0xCF, "inc #Q"),
                new MaskedInstruction(0x04, 0xC7, "inc #q"),
                new MaskedInstruction(0x05, 0xC7, "dec #q"),
                new MaskedInstruction(0x06, 0xC7, "ld #q,#B"),
                new SimpleInstruction(0x07, "rlca"),
                new SimpleInstruction(0x08, "ex af,af'"),
                new MaskedInstruction(0x09, 0xCF, "add hl,#Q"),
                new MaskedInstruction(0x0A, 0xEF, "ld a,(#Q)"),
                new MaskedInstruction(0x0B, 0xCF, "dec #Q"),
                new SimpleInstruction(0x0F, "rrca"),
                new SimpleInstruction(0x10, "djnz #r"),
                new SimpleInstruction(0x17, "rla"),
                new SimpleInstruction(0x18, "jr #r"),
                new SimpleInstruction(0x1F, "rra"),
                new SimpleInstruction(0x20, "jr nz,#r"),
                new SimpleInstruction(0x22, "ld (#W),hl"),
                new SimpleInstruction(0x27, "daa"),
                new SimpleInstruction(0x28, "jr z,#r"),
                new SimpleInstruction(0x2A, "ld hl,(#W)"),
                new SimpleInstruction(0x2F, "cpl"),
                new SimpleInstruction(0x30, "jr nc,#r"),
                new SimpleInstruction(0x32, "ld (#W),a"),
                new SimpleInstruction(0x37, "scf"),
                new SimpleInstruction(0x38, "jr c,#r"),
                new SimpleInstruction(0x3A, "ld a,(#W)"),
                new SimpleInstruction(0x3F, "ccf"),
                new MaskedInstruction(0x40, 0xC0, "ld #q,#s"),
                new SimpleInstruction(0x76, "halt"),
                new MaskedInstruction(0x80, 0xF8, "add a,#s"),
                new MaskedInstruction(0x88, 0xF8, "adc a,#s"),
                new MaskedInstruction(0x90, 0xF8, "sub #s"),
                new MaskedInstruction(0x98, 0xF8, "sbc #s"),
                new MaskedInstruction(0xA0, 0xF8, "and #s"),
                new MaskedInstruction(0xA8, 0xF8, "xor #s"),
                new MaskedInstruction(0xB0, 0xF8, "or #s"),
                new MaskedInstruction(0xB8, 0xF8, "cp #s"),
                new SimpleInstruction(0xC0, "ret nz"),
                new MaskedInstruction(0xC1, 0xCF, "pop #R"),
                new SimpleInstruction(0xC2, "jp nz,#L"),
                new SimpleInstruction(0xC3, "jp #L"),
                new SimpleInstruction(0xC4, "call nz,#L"),
                new MaskedInstruction(0xC5, 0xCF, "push #R"),
                new SimpleInstruction(0xC6, "add a,#B"),
                new MaskedInstruction(0xC7, 0xC7, "rst #8"),
                new SimpleInstruction(0xC8, "ret z"),
                new SimpleInstruction(0xC9, "ret"),
                new SimpleInstruction(0xCA, "jp z,#L"),
                new SimpleInstruction(0xCC, "call z,#L"),
                new SimpleInstruction(0xCD, "call #L"),
                new SimpleInstruction(0xCE, "adc a,#B"),
                new SimpleInstruction(0xD0, "ret nc"),
                new SimpleInstruction(0xD2, "jp nc,#L"),
                new SimpleInstruction(0xD3, "out (#B),a"),
                new SimpleInstruction(0xD4, "call nc,#L"),
                new SimpleInstruction(0xD6, "sub #B"),
                new SimpleInstruction(0xD8, "ret c"),
                new SimpleInstruction(0xD9, "exx"),
                new SimpleInstruction(0xDA, "jp c,#L"),
                new SimpleInstruction(0xDB, "in a,(#B)"),
                new SimpleInstruction(0xDC, "call c,#L"),
                new SimpleInstruction(0xDE, "sbc a,#B"),
                new SimpleInstruction(0xE0, "ret po"),
                new SimpleInstruction(0xE2, "jp po,#L"),
                new SimpleInstruction(0xE3, "ex (sp),hl"),
                new SimpleInstruction(0xE4, "call po,#L"),
                new SimpleInstruction(0xE6, "and #B"),
                new SimpleInstruction(0xE8, "ret pe"),
                new SimpleInstruction(0xE9, "jp (hl)"),
                new SimpleInstruction(0xEA, "jp pe,#L"),
                new SimpleInstruction(0xEB, "ex de,hl"),
                new SimpleInstruction(0xEC, "call pe,#L"),
                new SimpleInstruction(0xEE, "xor #B"),
                new SimpleInstruction(0xF0, "ret p"),
                new SimpleInstruction(0xF2, "jp p,#L"),
                new SimpleInstruction(0xF3, "di"),
                new SimpleInstruction(0xF4, "call p,#L"),
                new SimpleInstruction(0xF6, "or #B"),
                new SimpleInstruction(0xF8, "ret m"),
                new SimpleInstruction(0xF9, "ld sp,hl"),
                new SimpleInstruction(0xFA, "jp m,#L"),
                new SimpleInstruction(0xFB, "ei"),
                new SimpleInstruction(0xFC, "call m,#L"),
                new SimpleInstruction(0xFE, "cp #B")
            });

        private static readonly InstructionTable s_ExtendedInstructions = new InstructionTable(
            new List<AsmInstructionBase>
            {
                new MaskedInstruction(0x40, 0xC7, "in #q,(c)"),
                new MaskedInstruction(0x41, 0xC7, "out (c),#q"),
                new MaskedInstruction(0x42, 0xCF, "sbc hl,#Q"),
                new MaskedInstruction(0x43, 0xCF, "ld (#W),#Q"),
                new MaskedInstruction(0x44, 0xC7, "neg"),
                new MaskedInstruction(0x45, 0xC7, "retn"),
                new SimpleInstruction(0x46, "im 0"),
                new SimpleInstruction(0x47, "ld i,a"),
                new MaskedInstruction(0x4A, 0xCF, "adc hl,#Q"),
                new MaskedInstruction(0x4B, 0xCF, "ld #Q,(#W)"),
                new SimpleInstruction(0x4D, "reti"),
                new SimpleInstruction(0x4E, "im 0"),
                new SimpleInstruction(0x4F, "ld r,a"),
                new SimpleInstruction(0x56, "im 1"),
                new SimpleInstruction(0x57, "ld a,i"),
                new SimpleInstruction(0x5E, "im 2"),
                new SimpleInstruction(0x5F, "ld a,r"),
                new SimpleInstruction(0x66, "im 0"),
                new SimpleInstruction(0x67, "rrd"),
                new SimpleInstruction(0x6E, "im 0"),
                new SimpleInstruction(0x6F, "rld"),
                new SimpleInstruction(0x70, "in (c)"),
                new SimpleInstruction(0x71, "out (c),0"),
                new SimpleInstruction(0x76, "im 1"),
                new SimpleInstruction(0x7E, "im 2"),
                new SimpleInstruction(0xA0, "ldi"),
                new SimpleInstruction(0xA1, "cpi"),
                new SimpleInstruction(0xA2, "ini"),
                new SimpleInstruction(0xA3, "outi"),
                new SimpleInstruction(0xA8, "ldd"),
                new SimpleInstruction(0xA9, "cpd"),
                new SimpleInstruction(0xAA, "ind"),
                new SimpleInstruction(0xAB, "outd"),
                new SimpleInstruction(0xB0, "ldir"),
                new SimpleInstruction(0xB1, "cpir"),
                new SimpleInstruction(0xB2, "inir"),
                new SimpleInstruction(0xB3, "otir"),
                new SimpleInstruction(0xB8, "lddr"),
                new SimpleInstruction(0xB9, "cpdr"),
                new SimpleInstruction(0xBA, "indr"),
                new SimpleInstruction(0xBB, "otdr"),
            });

        private static readonly InstructionTable s_BitInstructions = new InstructionTable(
            new List<AsmInstructionBase>
            {
                new MaskedInstruction(0x00, 0xF8, "rlc #s"),
                new MaskedInstruction(0x08, 0xF8, "rrc #s"),
                new MaskedInstruction(0x10, 0xF8, "rl #s"),
                new MaskedInstruction(0x18, 0xF8, "rr #s"),
                new MaskedInstruction(0x20, 0xF8, "sla #s"),
                new MaskedInstruction(0x28, 0xF8, "sra #s"),
                new MaskedInstruction(0x30, 0xF8, "sll #s"),
                new MaskedInstruction(0x38, 0xF8, "srl #s"),
                new MaskedInstruction(0x40, 0xC0, "bit #b,#s"),
                new MaskedInstruction(0x80, 0xC0, "res #b,#s"),
                new MaskedInstruction(0xC0, 0xC0, "set #b,#s"),
            });

        private static readonly InstructionTable s_IndexedInstructions = new InstructionTable(
            new List<AsmInstructionBase>
            {
            });

        private static readonly InstructionTable s_IndexedBitInstructions = new InstructionTable(
            new List<AsmInstructionBase>
            {
            });
    }
}