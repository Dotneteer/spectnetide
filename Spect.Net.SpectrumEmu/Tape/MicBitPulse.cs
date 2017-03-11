namespace Spect.Net.SpectrumEmu.Tape
{
    /// <summary>
    /// Respresents a high or low pulse of the ear bit
    /// </summary>
    public struct MicBitPulse
    {
        /// <summary>
        /// True=High, False=Low
        /// </summary>
        public bool MicBit;

        /// <summary>
        /// Lenght of the pulse (given in Z80 tacts)
        /// </summary>
        public int Lenght;
    }
}