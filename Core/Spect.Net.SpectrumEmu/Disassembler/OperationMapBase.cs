namespace Spect.Net.SpectrumEmu.Disassembler
{
    /// <summary>
    /// This class represents an abstration of an instruction description
    /// </summary>
    public abstract class OperationMapBase
    {
        /// <summary>
        /// Operation code
        /// </summary>
        public byte OpCode { get; }

        /// <summary>
        /// Instruction pattern
        /// </summary>
        public string InstructionPattern { get; }

        /// <summary>
        /// Indocates that this instruction is a ZX Spectrum Next operation
        /// </summary>
        public bool ExtendedSet { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <param name="instructionPattern">Instruction pattern</param>
        /// <param name="extendedSet">Indicates a ZX Spectrum Next extended operation</param>
        protected OperationMapBase(byte opCode, string instructionPattern, bool extendedSet = false)
        {
            OpCode = opCode;
            InstructionPattern = instructionPattern;
            ExtendedSet = extendedSet;
        }
    }
}