namespace Spect.Net.SpectrumEmu.Disassembler
{
    /// <summary>
    /// This class represents a set of instructions that can be specified with
    /// a mask.
    /// </summary>
    public class MaskedOperationMap : OperationMapBase
    {
        /// <summary>
        /// Instruction mask
        /// </summary>
        public byte Mask { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <param name="mask">Operation mask</param>
        /// <param name="instructionPattern">Instruction pattern</param>
        /// <param name="extendedSet">Indicates a ZX Spectrum Next extended operation</param>
        public MaskedOperationMap(byte opCode, byte mask, string instructionPattern, bool extendedSet = false) 
            : base(opCode, instructionPattern, extendedSet)
        {
            Mask = mask;
        }
    }
}