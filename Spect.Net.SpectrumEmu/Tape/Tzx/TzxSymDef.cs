namespace Spect.Net.SpectrumEmu.Tape.Tzx
{
    /// <summary>
    /// This block represents an extremely wide range of data encoding techniques.
    /// </summary>
    /// <remarks>
    /// The basic idea is that each loading component (pilot tone, sync pulses, data) 
    /// is associated to a specific sequence of pulses, where each sequence (wave) can 
    /// contain a different number of pulses from the others. In this way we can have 
    /// a situation where bit 0 is represented with 4 pulses and bit 1 with 8 pulses.
    /// </remarks>
    public struct TzxSymDef
    {
        /// <summary>
        /// Bit 0 - Bit 1: Starting symbol polarity
        /// </summary>
        /// <remarks>
        /// 00: opposite to the current level (make an edge, as usual) - default
        /// 01: same as the current level(no edge - prolongs the previous pulse)
        /// 10: force low level
        /// 11: force high level
        /// </remarks>
        public byte SymbolFlags;

        /// <summary>
        /// The array of pulse lengths
        /// </summary>
        public ushort[] PulseLengths;
    }
}