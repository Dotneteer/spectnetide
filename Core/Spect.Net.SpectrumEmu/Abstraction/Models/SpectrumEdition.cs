using Spect.Net.SpectrumEmu.Abstraction.Configuration;
using Spect.Net.SpectrumEmu.Devices.Beeper;

namespace Spect.Net.SpectrumEmu.Abstraction.Models
{
    /// <summary>
    /// This class describes a revison of a particular Spectrum model
    /// </summary>
    public class SpectrumEdition
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
        /// The CPU configuration data for this revision
        /// </summary>
        public CpuConfigurationData CpuConfiguration { get; set; }

        /// <summary>
        /// The screen configuration data for this revision
        /// </summary>
        public ScreenConfigurationData ScreenConfiguration { get; set; }

        /// <summary>
        /// The beeper configuration data for this revision
        /// </summary>
        public BeeperConfiguration BeeperConfiguration { get; set; }

        /// <summary>
        /// Returns a clone of this revision
        /// </summary>
        /// <returns>Cloned revision</returns>
        public SpectrumEdition Clone()
        {
            return new SpectrumEdition
            {
                NumberOfRoms = NumberOfRoms,
                RomSize = RomSize,
                RamSize = RamSize,
                CpuConfiguration = CpuConfiguration.Clone(),
                ScreenConfiguration = ScreenConfiguration.Clone(),
                BeeperConfiguration = BeeperConfiguration.Clone()
            };
        }
    }
}