using Spect.Net.SpectrumEmu.Abstraction.Configuration;
using Spect.Net.SpectrumEmu.Devices.Floppy;

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
        public AudioConfigurationData Beeper { get; set; }

        /// <summary>
        /// The sound configuration data for this revision
        /// </summary>
        public AudioConfigurationData Sound { get; set; }

        /// <summary>
        /// The floppy configuration of this revision
        /// </summary>
        public FloppyConfiguration Floppy { get; set; }

        /// <summary>
        /// ULA Issue #
        /// </summary>
        public string UlaIssue { get; set; } = "3";
        
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
                Beeper = Beeper.Clone(),
                Sound = Sound?.Clone(),
                Floppy = Floppy?.Clone(),
                UlaIssue = UlaIssue
            };
        }
    }
}