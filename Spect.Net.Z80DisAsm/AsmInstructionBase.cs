namespace Spect.Net.Z80DisAsm
{
    public abstract class AsmInstructionBase
    {
        public byte OpCode { get; }
        public string InstructionPattern { get; }

        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        protected AsmInstructionBase(byte opCode, string instructionPattern)
        {
            OpCode = opCode;
            InstructionPattern = instructionPattern;
        }
    }
}