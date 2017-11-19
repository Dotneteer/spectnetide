namespace Spect.Net.SpectrumEmu.Abstraction.Configuration
{
    /// <summary>
    /// This class stores the configuration data for Z80 CPU
    /// </summary>
    public sealed class CpuConfigurationData : ICpuConfiguration
    {
        /// <summary>
        /// The clock frequency of the CPU
        /// </summary>
        public int BaseClockFrequency { get; set; }

        /// <summary>
        /// This value allows to multiply clock frequency. Accepted values are:
        /// 1, 2, 4, 8
        /// </summary>
        public int ClockMultiplier { get; set; }

        /// <summary>
        /// Reserved for future use
        /// </summary>
        public bool SupportsNextOperations { get; set; }

        /// <summary>
        /// Returns a clone of this instance
        /// </summary>
        /// <returns>A clone of this instance</returns>
        public CpuConfigurationData Clone()
        {
            return new CpuConfigurationData
            {
                BaseClockFrequency = BaseClockFrequency,
                ClockMultiplier = ClockMultiplier,
                SupportsNextOperations = SupportsNextOperations
            };
        }
    }
}