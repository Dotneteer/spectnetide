using Spect.Net.SpectrumEmu.Abstraction.Configuration;

namespace Spect.Net.SpectrumEmu.Abstraction.Models
{
    /// <summary>
    /// This class describes a revison of a particular Spectrum model
    /// </summary>
    public class SpectrumEdition
    {
        /// <summary>
        /// The CPU configuration data for this revision
        /// </summary>
        public CpuConfigurationData Cpu { get; set; }

        /// <summary>
        /// The ROM configuration data for this revision
        /// </summary>
        public RomConfigurationData Rom { get; set; }

        /// <summary>
        /// The RAM memory configuration data for this revision
        /// </summary>
        public MemoryConfigurationData Memory { get; set; }

        /// <summary>
        /// The screen configuration data for this revision
        /// </summary>
        public ScreenConfigurationData Screen { get; set; }

        /// <summary>
        /// The beeper configuration data for this revision
        /// </summary>
        public BeeperConfigurationData Beeper { get; set; }

        /// <summary>
        /// Returns a clone of this revision
        /// </summary>
        /// <returns>Cloned revision</returns>
        public SpectrumEdition Clone()
        {
            return new SpectrumEdition
            {
                Cpu = Cpu.Clone(),
                Rom = Rom.Clone(),
                Memory = Memory.Clone(),
                Screen = Screen.Clone(),
                Beeper = Beeper.Clone()
            };
        }
    }
}