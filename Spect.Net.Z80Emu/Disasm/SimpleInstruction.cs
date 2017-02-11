namespace Spect.Net.Z80Emu.Disasm
{
    /// <summary>
    /// This class represents a single instruction with direct operation
    /// code mathcing.
    /// </summary>
    public class SimpleInstruction : AsmInstructionBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        public SimpleInstruction(byte opCode, string instructionPattern) : base(opCode, instructionPattern)
        {
        }

        /// <summary>
        /// Checks if the specified <paramref name="opCode"/> matches
        /// with this instruction
        /// </summary>
        /// <param name="opCode">Operation code to check</param>
        /// <returns>True, if the specified operation code matches; otherwise, false</returns>
        public override bool Matches(byte opCode)
        {
            return opCode == OpCode;
        }
    }
}