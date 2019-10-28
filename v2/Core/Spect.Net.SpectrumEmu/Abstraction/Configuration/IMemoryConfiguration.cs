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

        /// <summary>
        /// Size of ZX Spectrum Next in MBytes
        /// </summary>
        /// <remarks>
        /// Next memory size in KBytes
        /// Accepted values:
        /// 0 - Legacy models that do not support Next memory mapping
        /// 512 - 512KBytes
        /// 1024 - 1024 KBytes
        /// 1536 - 1.5MBytes
        /// 2048 - 2 MBytes
        /// </remarks>
        int NextMemorySize { get; }
    }
}