namespace Spect.Net.SpectrumEmu.Abstraction.Configuration
{
    /// <summary>
    /// This interface defines the configuration data for Z80 CPU
    /// </summary>
    public interface ICpuConfiguration: IDeviceConfiguration
    {
        /// <summary>
        /// This value allows to multiply clock frequency. Accepted values are:
        /// 1, 2, 4, 8
        /// </summary>
        int ClockMultiplier { get; }

        /// <summary>
        /// Reserved for future use
        /// </summary>
        bool SupportsNextOperations { get; }
    }
}