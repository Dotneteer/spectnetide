namespace Spect.Net.SpectrumEmu.Devices.Tape.Tzx
{
    /// <summary>
    /// Symbol repetitions
    /// </summary>
    public struct TzxPrle
    {
        /// <summary>
        /// Symbol represented
        /// </summary>
        public byte Symbol;

        /// <summary>
        /// Number of repetitions
        /// </summary>
        public ushort Repetitions;
    }
}