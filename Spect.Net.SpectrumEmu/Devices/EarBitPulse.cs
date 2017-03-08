namespace Spect.Net.SpectrumEmu.Devices
{
    /// <summary>
    /// Respresents a high or low pulse of the ear bit
    /// </summary>
    public struct EarBitPulse
    {
        /// <summary>
        /// True=High, False=Low
        /// </summary>
        public bool EarBit;

        /// <summary>
        /// Lenght of the pulse (given in Z80 tacts)
        /// </summary>
        public int Lenght;
    }
}