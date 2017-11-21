namespace Spect.Net.SpectrumEmu.Abstraction.Configuration
{
    /// <summary>
    /// This class stores the configuration data for the ROM
    /// </summary>
    public sealed class RomConfigurationData : IRomConfiguration
    {
        /// <summary>
        /// The number of ROM banks
        /// </summary>
        public int NumberOfRoms { get; set; }

        /// <summary>
        /// The name of the ROM file
        /// </summary>
        public string RomName { get; set; }

        /// <summary>
        /// The index of the Spectrum 48K BASIC ROM
        /// </summary>
        public int Spectrum48RomIndex { get; set; }

        /// <summary>
        /// Returns a clone of this instance
        /// </summary>
        /// <returns>A clone of this instance</returns>
        public RomConfigurationData Clone()
        {
            return new RomConfigurationData
            {
                NumberOfRoms = NumberOfRoms,
                RomName = RomName,
                Spectrum48RomIndex = Spectrum48RomIndex
            };
        }
    }
}