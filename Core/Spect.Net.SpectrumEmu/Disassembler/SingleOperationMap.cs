namespace Spect.Net.SpectrumEmu.Disassembler
{
    /// <summary>
    /// This class represents a single instruction with direct operation
    /// code mathcing.
    /// </summary>
    public class SingleOperationMap : OperationMapBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <param name="instructionPattern">Instruction pattern</param>
        /// <param name="extendedSet">Indicates a ZX Spectrum Next extended operation</param>
        public SingleOperationMap(byte opCode, string instructionPattern, bool extendedSet = false) : 
            base(opCode, instructionPattern, extendedSet)
        {
        }
    }
}