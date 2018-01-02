namespace Spect.Net.SpectrumEmu.Abstraction.Configuration
{
    /// <summary>
    /// This class represents the configuration data for memory.
    /// </summary>
    public class MemoryConfigurationData : IMemoryConfiguration
    {
        /// <summary>
        /// This flag indicates whether the device supports banking
        /// </summary>
        public bool SupportsBanking { get; set; }

        /// <summary>
        /// Size of memory slots in KB
        /// </summary>
        /// <remarks>
        /// Accepted values are: 8, 16. Null, if banking is not supported.
        /// </remarks>
        public int? SlotSize { get; set; }

        /// <summary>
        /// Number of RAM banks with the size of slots
        /// </summary>
        /// <remarks>
        /// Accepted values are: 4 ... 256
        /// Null, if banking is not supported.
        /// </remarks>
        public int? RamBanks { get; set; }

        /// <summary>
        /// Type of memory contention
        /// </summary>
        public MemoryContentionType ContentionType { get; set; }

        /// <summary>
        /// Returns a clone of this instance
        /// </summary>
        /// <returns>A clone of this instance</returns>
        public MemoryConfigurationData Clone()
        {
            return new MemoryConfigurationData
            {
                SupportsBanking = SupportsBanking,
                SlotSize = SlotSize,
                RamBanks = RamBanks,
                ContentionType = ContentionType
            };
        }
    }
}