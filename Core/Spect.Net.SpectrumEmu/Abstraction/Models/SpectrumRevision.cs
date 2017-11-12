namespace Spect.Net.SpectrumEmu.Abstraction.Models
{
    /// <summary>
    /// This class describes a revison of a particular Spectrum model
    /// </summary>
    public class SpectrumRevision
    {
        /// <summary>
        /// The number or ROMs that belong to the model
        /// </summary>
        public int NumberOfRoms { get; set; }

        /// <summary>
        /// The size of the ROM
        /// </summary>
        public int RomSize { get; set; }

        /// <summary>
        /// The size of the RAM
        /// </summary>
        public int RamSize { get; set; }

        /// <summary>
        /// The screen configuration data for this revision
        /// </summary>
        public ScreenConfigurationData ScreenConfiguration { get; set; }

        /// <summary>
        /// Returns a clone of this revision
        /// </summary>
        /// <returns>Cloned revision</returns>
        public SpectrumRevision Clone()
        {
            return new SpectrumRevision
            {
                NumberOfRoms = NumberOfRoms,
                RomSize = RomSize,
                RamSize = RamSize,
                ScreenConfiguration = ScreenConfiguration.Clone()
            };
        }
    }
}