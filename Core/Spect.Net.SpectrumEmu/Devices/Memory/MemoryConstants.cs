namespace Spect.Net.SpectrumEmu.Devices.Memory
{
    /// <summary>
    /// This class constant values related to ZX Spectrum
    /// memory handling
    /// </summary>
    public sealed class MemoryConstants
    {
        /// <summary>
        /// Memory addressable with 16 bits
        /// </summary>
        public const int ADDRESSABLE_SIZE = 0x10000;

        /// <summary>
        /// Normal (ZX Spectrum 128/+2/+3) memory bank size
        /// </summary>
        public const int BANK_SIZE = 0x4000;

        /// <summary>
        /// Spectrum Next memory bank size
        /// </summary>
        public const int NEXT_BANK_SIZE = 0x2000;
    }
}