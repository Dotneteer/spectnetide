namespace Spect.Net.SpectrumEmu.Abstraction.Configuration
{
    /// <summary>
    /// This interface defines the memory configuration data the virtual machine
    /// </summary>
    public interface IMemoryConfiguration: IDeviceConfiguration
    {
        /// <summary>
        /// This flag indicates whether the device supports banking
        /// </summary>
        bool SupportsBanking { get; }

        /// <summary>
        /// Size of memory slots in KB
        /// </summary>
        /// <remarks>
        /// Accepted values are: 8, 16. Null, if banking is not supported.
        /// </remarks>
        int? SlotSize { get; }

        /// <summary>
        /// Number of RAM banks with the size of slots
        /// </summary>
        /// <remarks>
        /// Accepted values are: 4 ... 256
        /// Null, if banking is not supported.
        /// </remarks>
        int? RamBanks { get; }

        /// <summary>
        /// Type of memory contention
        /// </summary>
        MemoryContentionType ContentionType { get; }
    }
}