using System.Collections.Generic;

namespace Spect.Net.Z80DisAsm
{
    public partial class Z80Disassembler
    {
        private static readonly InstructionTable s_StandardInstructions = new InstructionTable(
            new List<AsmInstructionBase>
            {
                new SimpleInstruction(0x00, "nop"),
                new SimpleInstruction(0x07, "rlca"),
                new SimpleInstruction(0x08, "ex af,af'"),
                new SimpleInstruction(0x0F, "rrca"),
                new SimpleInstruction(0x10, "djnz #r"),
                new SimpleInstruction(0x17, "rla"),
                new SimpleInstruction(0x18, "jr #r"),
                new SimpleInstruction(0x1F, "rra"),
                new SimpleInstruction(0x20, "jr nz,#r"),
                new SimpleInstruction(0x27, "daa"),
                new SimpleInstruction(0x28, "jr z,#r"),
                new SimpleInstruction(0x2F, "cpl"),
                new SimpleInstruction(0x30, "jr nc,#r"),
                new SimpleInstruction(0x37, "scf"),
                new SimpleInstruction(0x38, "jr c,#r"),
                new SimpleInstruction(0x3F, "ccf"),
                new SimpleInstruction(0xC0, "ret nz"),
                new SimpleInstruction(0xC2, "jp nz,#L"),
                new SimpleInstruction(0xC3, "jp #L"),

                new SimpleInstruction(0xF3, "di")
            });

        private static readonly InstructionTable s_ExtendedInstructions = new InstructionTable(
            new List<AsmInstructionBase>
            {
            });

        private static readonly InstructionTable s_BitInstructions = new InstructionTable(
            new List<AsmInstructionBase>
            {
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