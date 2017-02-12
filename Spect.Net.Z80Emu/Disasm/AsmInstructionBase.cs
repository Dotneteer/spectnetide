namespace Spect.Net.Z80Emu.Disasm
{
    /// <summary>
    /// This class represents an abstration of an instruction description
    /// </summary>
    public abstract class AsmInstructionBase
    {
        /// <summary>
        /// Operation code
        /// </summary>
        public byte OpCode { get; }

        /// <summary>
        /// Instruction pattern
        /// </summary>
        public string InstructionPattern { get; }

        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        protected AsmInstructionBase(byte opCode, string instructionPattern)
        {
            OpCode = opCode;
            InstructionPattern = instructionPattern;
        }

        /// <summary>
        /// Checks if the specified <paramref name="opCode"/> matches
        /// with this instruction
        /// </summary>
        /// <param name="opCode">Operation code to check</param>
        /// <returns>True, if the specified operation code matches; otherwise, false</returns>
        public abstract bool Matches(byte opCode);
    }
}