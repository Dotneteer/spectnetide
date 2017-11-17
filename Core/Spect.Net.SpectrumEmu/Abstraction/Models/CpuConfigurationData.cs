namespace Spect.Net.SpectrumEmu.Abstraction.Models
{
    /// <summary>
    /// This class stores the configuration data for Z80 CPU
    /// </summary>
    public class CpuConfigurationData : ICpuConfiguration
    {
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
                ClockMultiplier = ClockMultiplier,
                SupportsNextOperations = SupportsNextOperations
            };
        }
    }
}