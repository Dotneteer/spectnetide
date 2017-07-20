using System.Collections.Generic;

namespace Spect.Net.SpectrumEmu.Disassembler
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
        /// Disassembly keywords that cannot be used as label names or other symbols
        /// </summary>
        public static readonly string[] DisAsmKeywords =
        {
            "A", "B", "C", "D", "E", "H", "L", "F", "BC", "DE", "HL", "AF", "IX",
            "IY", "SP", "IR", "PC", "NZ", "Z", "NC", "PO", "PE", "P", "M",
            "ADD", "ADC", "AND", "BIT", "CALL", "CCF", "CP", "CPD", "CPDR", "CPI",
            "CPIR", "CPL", "DAA", "DEC", "DI", "DJNZ", "EI", "EX", "EXX", "LD","LDD",
            "LDDR", "LDI", "LDIR", "IM", "IN", "INC", "IND", "INDR", "INI", "INIR",
            "JR", "JP", "NEG", "OR", "OTDR", "OTIR", "OUT", "OUTI", "OUTD", "POP",
            "PUSH", "RES", "RET", "RETI", "RETN", "RL", "RLA", "RLCA", "RLC", "RLD",
            "RR", "RRA", "RRC", "RRCA", "RRD", "RST", "SBC", "SCF", "SET", "SLA",
            "SLL", "SRA", "SRL", "SUB", "XOR"
        };

        /// <summary>
        /// Standard Z80 instructions
        /// </summary>
        private static readonly InstructionTable s_StandardInstructions = new InstructionTable(
            new List<OperationMapBase>
            {
                new SingleOperationMap(0x00, "nop"),
                new MaskedOperationMap(0x01, 0xCF, "ld #Q,#W"),
                new MaskedOperationMap(0x02, 0xEF, "ld (#Q),a"),
                new MaskedOperationMap(0x03, 0xCF, "inc #Q"),
                new MaskedOperationMap(0x04, 0xC7, "inc #q"),
                new MaskedOperationMap(0x05, 0xC7, "dec #q"),
                new MaskedOperationMap(0x06, 0xC7, "ld #q,#B"),
                new SingleOperationMap(0x07, "rlca"),
                new SingleOperationMap(0x08, "ex af,af'"),
                new MaskedOperationMap(0x09, 0xCF, "add hl,#Q"),
                new MaskedOperationMap(0x0A, 0xEF, "ld a,(#Q)"),
                new MaskedOperationMap(0x0B, 0xCF, "dec #Q"),
                new SingleOperationMap(0x0F, "rrca"),
                new SingleOperationMap(0x10, "djnz #r"),
                new SingleOperationMap(0x17, "rla"),
                new SingleOperationMap(0x18, "jr #r"),
                new SingleOperationMap(0x1F, "rra"),
                new SingleOperationMap(0x20, "jr nz,#r"),
                new SingleOperationMap(0x22, "ld (#W),hl"),
                new SingleOperationMap(0x27, "daa"),
                new SingleOperationMap(0x28, "jr z,#r"),
                new SingleOperationMap(0x2A, "ld hl,(#W)"),
                new SingleOperationMap(0x2F, "cpl"),
                new SingleOperationMap(0x30, "jr nc,#r"),
                new SingleOperationMap(0x32, "ld (#W),a"),
                new SingleOperationMap(0x37, "scf"),
                new SingleOperationMap(0x38, "jr c,#r"),
                new SingleOperationMap(0x3A, "ld a,(#W)"),
                new SingleOperationMap(0x3F, "ccf"),
                new MaskedOperationMap(0x40, 0xC0, "ld #q,#s"),
                new SingleOperationMap(0x76, "halt"),
                new MaskedOperationMap(0x80, 0xF8, "add a,#s"),
                new MaskedOperationMap(0x88, 0xF8, "adc a,#s"),
                new MaskedOperationMap(0x90, 0xF8, "sub #s"),
                new MaskedOperationMap(0x98, 0xF8, "sbc #s"),
                new MaskedOperationMap(0xA0, 0xF8, "and #s"),
                new MaskedOperationMap(0xA8, 0xF8, "xor #s"),
                new MaskedOperationMap(0xB0, 0xF8, "or #s"),
                new MaskedOperationMap(0xB8, 0xF8, "cp #s"),
                new SingleOperationMap(0xC0, "ret nz"),
                new MaskedOperationMap(0xC1, 0xCF, "pop #R"),
                new SingleOperationMap(0xC2, "jp nz,#L"),
                new SingleOperationMap(0xC3, "jp #L"),
                new SingleOperationMap(0xC4, "call nz,#L"),
                new MaskedOperationMap(0xC5, 0xCF, "push #R"),
                new SingleOperationMap(0xC6, "add a,#B"),
                new MaskedOperationMap(0xC7, 0xC7, "rst #8"),
                new SingleOperationMap(0xC8, "ret z"),
                new SingleOperationMap(0xC9, "ret"),
                new SingleOperationMap(0xCA, "jp z,#L"),
                new SingleOperationMap(0xCC, "call z,#L"),
                new SingleOperationMap(0xCD, "call #L"),
                new SingleOperationMap(0xCE, "adc a,#B"),
                new SingleOperationMap(0xD0, "ret nc"),
                new SingleOperationMap(0xD2, "jp nc,#L"),
                new SingleOperationMap(0xD3, "out (#B),a"),
                new SingleOperationMap(0xD4, "call nc,#L"),
                new SingleOperationMap(0xD6, "sub #B"),
                new SingleOperationMap(0xD8, "ret c"),
                new SingleOperationMap(0xD9, "exx"),
                new SingleOperationMap(0xDA, "jp c,#L"),
                new SingleOperationMap(0xDB, "in a,(#B)"),
                new SingleOperationMap(0xDC, "call c,#L"),
                new SingleOperationMap(0xDE, "sbc a,#B"),
                new SingleOperationMap(0xE0, "ret po"),
                new SingleOperationMap(0xE2, "jp po,#L"),
                new SingleOperationMap(0xE3, "ex (sp),hl"),
                new SingleOperationMap(0xE4, "call po,#L"),
                new SingleOperationMap(0xE6, "and #B"),
                new SingleOperationMap(0xE8, "ret pe"),
                new SingleOperationMap(0xE9, "jp (hl)"),
                new SingleOperationMap(0xEA, "jp pe,#L"),
                new SingleOperationMap(0xEB, "ex de,hl"),
                new SingleOperationMap(0xEC, "call pe,#L"),
                new SingleOperationMap(0xEE, "xor #B"),
                new SingleOperationMap(0xF0, "ret p"),
                new SingleOperationMap(0xF2, "jp p,#L"),
                new SingleOperationMap(0xF3, "di"),
                new SingleOperationMap(0xF4, "call p,#L"),
                new SingleOperationMap(0xF6, "or #B"),
                new SingleOperationMap(0xF8, "ret m"),
                new SingleOperationMap(0xF9, "ld sp,hl"),
                new SingleOperationMap(0xFA, "jp m,#L"),
                new SingleOperationMap(0xFB, "ei"),
                new SingleOperationMap(0xFC, "call m,#L"),
                new SingleOperationMap(0xFE, "cp #B")
            });

        private static readonly InstructionTable s_ExtendedInstructions = new InstructionTable(
            new List<OperationMapBase>
            {
                new MaskedOperationMap(0x00, 0xC0, null),
                new MaskedOperationMap(0x40, 0xC7, "in #q,(c)"),
                new MaskedOperationMap(0x41, 0xC7, "out (c),#q"),
                new MaskedOperationMap(0x42, 0xCF, "sbc hl,#Q"),
                new MaskedOperationMap(0x43, 0xCF, "ld (#W),#Q"),
                new MaskedOperationMap(0x44, 0xC7, "neg"),
                new MaskedOperationMap(0x45, 0xC7, "retn"),
                new SingleOperationMap(0x46, "im 0"),
                new SingleOperationMap(0x47, "ld i,a"),
                new MaskedOperationMap(0x4A, 0xCF, "adc hl,#Q"),
                new MaskedOperationMap(0x4B, 0xCF, "ld #Q,(#W)"),
                new SingleOperationMap(0x4D, "reti"),
                new SingleOperationMap(0x4E, "im 0"),
                new SingleOperationMap(0x4F, "ld r,a"),
                new SingleOperationMap(0x56, "im 1"),
                new SingleOperationMap(0x57, "ld a,i"),
                new SingleOperationMap(0x5E, "im 2"),
                new SingleOperationMap(0x5F, "ld a,r"),
                new SingleOperationMap(0x66, "im 0"),
                new SingleOperationMap(0x67, "rrd"),
                new SingleOperationMap(0x6E, "im 0"),
                new SingleOperationMap(0x6F, "rld"),
                new SingleOperationMap(0x70, "in (c)"),
                new SingleOperationMap(0x71, "out (c),0"),
                new SingleOperationMap(0x76, "im 1"),
                new SingleOperationMap(0x77, null),
                new SingleOperationMap(0x7E, "im 2"),
                new SingleOperationMap(0x7F, null),
                new SingleOperationMap(0xA0, "ldi"),
                new SingleOperationMap(0xA1, "cpi"),
                new SingleOperationMap(0xA2, "ini"),
                new SingleOperationMap(0xA3, "outi"),
                new MaskedOperationMap(0xA4, 0xC3, null),
                new SingleOperationMap(0xA8, "ldd"),
                new SingleOperationMap(0xA9, "cpd"),
                new SingleOperationMap(0xAA, "ind"),
                new SingleOperationMap(0xAB, "outd"),
                new SingleOperationMap(0xB0, "ldir"),
                new SingleOperationMap(0xB1, "cpir"),
                new SingleOperationMap(0xB2, "inir"),
                new SingleOperationMap(0xB3, "otir"),
                new SingleOperationMap(0xB8, "lddr"),
                new SingleOperationMap(0xB9, "cpdr"),
                new SingleOperationMap(0xBA, "indr"),
                new SingleOperationMap(0xBB, "otdr"),
                new MaskedOperationMap(0xC0, 0xC0, null),
            });

        private static readonly InstructionTable s_BitInstructions = new InstructionTable(
            new List<OperationMapBase>
            {
                new MaskedOperationMap(0x00, 0xF8, "rlc #s"),
                new MaskedOperationMap(0x08, 0xF8, "rrc #s"),
                new MaskedOperationMap(0x10, 0xF8, "rl #s"),
                new MaskedOperationMap(0x18, 0xF8, "rr #s"),
                new MaskedOperationMap(0x20, 0xF8, "sla #s"),
                new MaskedOperationMap(0x28, 0xF8, "sra #s"),
                new MaskedOperationMap(0x30, 0xF8, "sll #s"),
                new MaskedOperationMap(0x38, 0xF8, "srl #s"),
                new MaskedOperationMap(0x40, 0xC0, "bit #b,#s"),
                new MaskedOperationMap(0x80, 0xC0, "res #b,#s"),
                new MaskedOperationMap(0xC0, 0xC0, "set #b,#s")
            });

        private static readonly InstructionTable s_IndexedInstructions = new InstructionTable(
            new List<OperationMapBase>
            {
                new MaskedOperationMap(0x09, 0xCF, "add #X,#Q"),
                new SingleOperationMap(0x21, "ld #X,#W"),
                new SingleOperationMap(0x22, "ld (#W),#X"),
                new SingleOperationMap(0x23, "inc #X"),
                new SingleOperationMap(0x24, "inc #h"),
                new SingleOperationMap(0x25, "dec #h"),
                new SingleOperationMap(0x26, "ld #h,#B"),
                new SingleOperationMap(0x29, "add #X,#X"),
                new SingleOperationMap(0x2A, "ld #X,(#W)"),
                new SingleOperationMap(0x2B, "dec #X"),
                new SingleOperationMap(0x2C, "inc #l"),
                new SingleOperationMap(0x2D, "dec #l"),
                new SingleOperationMap(0x2E, "ld #l,#B"),
                new SingleOperationMap(0x34, "inc (#X#D)"),
                new SingleOperationMap(0x35, "dec (#X#D)"),
                new SingleOperationMap(0x36, "ld (#X#D),#B"),
                new MaskedOperationMap(0x44, 0xE7, "ld #q,#h"),
                new MaskedOperationMap(0x45, 0xE7, "ld #q,#l"),
                new MaskedOperationMap(0x46, 0xE7, "ld #q,(#X#D)"),
                new MaskedOperationMap(0x60, 0xF8, "ld #h,#s"),
                new SingleOperationMap(0x64, "ld #h,#h"),
                new SingleOperationMap(0x65, "ld #h,#l"),
                new SingleOperationMap(0x66, "ld h,(#X#D)"),
                new MaskedOperationMap(0x68, 0xF8, "ld #l,#s"),
                new SingleOperationMap(0x6C, "ld #l,#h"),
                new SingleOperationMap(0x6D, "ld #l,#l"),
                new SingleOperationMap(0x6E, "ld l,(#X#D)"),
                new MaskedOperationMap(0x70, 0xF8, "ld (#X#D),#s"),
                new SingleOperationMap(0x76, null),
                new SingleOperationMap(0x7C, "ld a,#h"),
                new SingleOperationMap(0x7D, "ld a,#l"),
                new SingleOperationMap(0x7E, "ld a,(#X#D)"),
                new SingleOperationMap(0x7F, null),
                new MaskedOperationMap(0x80, 0xEF, null),
                new SingleOperationMap(0x84, "add a,#h"),
                new SingleOperationMap(0x85, "add a,#l"),
                new SingleOperationMap(0x86, "add a,(#X#D)"),
                new SingleOperationMap(0x8C, "adc a,#h"),
                new SingleOperationMap(0x8D, "adc a,#l"),
                new SingleOperationMap(0x8E, "adc a,(#X#D)"),
                new SingleOperationMap(0x94, "sub #h"),
                new SingleOperationMap(0x95, "sub #l"),
                new SingleOperationMap(0x96, "sub (#X#D)"),
                new SingleOperationMap(0x9C, "sbc #h"),
                new SingleOperationMap(0x9D, "sbc #l"),
                new SingleOperationMap(0x9E, "sbc (#X#D)"),
                new SingleOperationMap(0xA4, "and #h"),
                new SingleOperationMap(0xA5, "and #l"),
                new SingleOperationMap(0xA6, "and (#X#D)"),
                new SingleOperationMap(0xAC, "xor #h"),
                new SingleOperationMap(0xAD, "xor #l"),
                new SingleOperationMap(0xAE, "xor (#X#D)"),
                new SingleOperationMap(0xB4, "or #h"),
                new SingleOperationMap(0xB5, "or #l"),
                new SingleOperationMap(0xB6, "or (#X#D)"),
                new SingleOperationMap(0xBC, "cp #h"),
                new SingleOperationMap(0xBD, "cp #l"),
                new SingleOperationMap(0xBE, "cp (#X#D)"),
                new SingleOperationMap(0xE1, "pop #X"),
                new SingleOperationMap(0xE3, "ex (sp),#X"),
                new SingleOperationMap(0xE5, "push #X"),
                new SingleOperationMap(0xE9, "jp (#X)"),
                new SingleOperationMap(0xF9, "ld sp,#X"),
            });

        private static readonly InstructionTable s_IndexedBitInstructions = new InstructionTable(
            new List<OperationMapBase>
            {
                new MaskedOperationMap(0x00, 0xF8, "rlc (#X#D),#s"),
                new SingleOperationMap(0x06, "rlc (#X#D)"),
                new MaskedOperationMap(0x08, 0xF8, "rrc (#X#D),#s"),
                new SingleOperationMap(0x0E, "rrc (#X#D)"),
                new MaskedOperationMap(0x10, 0xF8, "rl (#X#D),#s"),
                new SingleOperationMap(0x16, "rl (#X#D)"),
                new MaskedOperationMap(0x18, 0xF8, "rr (#X#D),#s"),
                new SingleOperationMap(0x1E, "rr (#X#D)"),
                new MaskedOperationMap(0x20, 0xF8, "sla (#X#D),#s"),
                new SingleOperationMap(0x26, "sla (#X#D)"),
                new MaskedOperationMap(0x28, 0xF8, "sra (#X#D),#s"),
                new SingleOperationMap(0x2E, "sra (#X#D)"),
                new MaskedOperationMap(0x30, 0xF8, "sll (#X#D),#s"),
                new SingleOperationMap(0x36, "sll (#X#D)"),
                new MaskedOperationMap(0x38, 0xF8, "srl (#X#D),#s"),
                new SingleOperationMap(0x3E, "srl (#X#D)"),
                new MaskedOperationMap(0x40, 0xC0, "bit #b,(#X#D)"),
                new MaskedOperationMap(0x80, 0xC0, "res #b,(#X#D),#s"),
                new MaskedOperationMap(0xC0, 0xC0, "set #b,(#X#D),#s"),
                new SingleOperationMap(0x86, "res #b,(#X#D)"),
                new SingleOperationMap(0x8E, "res #b,(#X#D)"),
                new SingleOperationMap(0x96, "res #b,(#X#D)"),
                new SingleOperationMap(0x9E, "res #b,(#X#D)"),
                new SingleOperationMap(0xA6, "res #b,(#X#D)"),
                new SingleOperationMap(0xAE, "res #b,(#X#D)"),
                new SingleOperationMap(0xB6, "res #b,(#X#D)"),
                new SingleOperationMap(0xBE, "res #b,(#X#D)"),
                new SingleOperationMap(0xC6, "set #b,(#X#D)"),
                new SingleOperationMap(0xCE, "set #b,(#X#D)"),
                new SingleOperationMap(0xD6, "set #b,(#X#D)"),
                new SingleOperationMap(0xDE, "set #b,(#X#D)"),
                new SingleOperationMap(0xE6, "set #b,(#X#D)"),
                new SingleOperationMap(0xEE, "set #b,(#X#D)"),
                new SingleOperationMap(0xF6, "set #b,(#X#D)"),
                new SingleOperationMap(0xFE, "set #b,(#X#D)")
            });
    }
}