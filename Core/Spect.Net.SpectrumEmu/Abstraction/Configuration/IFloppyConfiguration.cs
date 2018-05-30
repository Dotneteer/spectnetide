namespace Spect.Net.SpectrumEmu.Abstraction.Configuration
{
    /// <summary>
    /// This interface represents the configuration of the floppy device
    /// </summary>
    public interface IFloppyConfiguration : IDeviceConfiguration
    {
        /// <summary>
        /// Has the computer a floppy drive
        /// </summary>
        bool FloppyPresent { get; }

        /// <summary>
        /// Is drive B present?
        /// </summary>
        bool DriveBPresent { get; }
    }
}