namespace Spect.Net.SpectrumEmu.Abstraction.Devices
{
    /// <summary>
    /// This interface represents a device that has an internal
    /// clock working with a particular frequency
    /// </summary>
    public interface IClockBoundDevice : IDevice
    {
        /// <summary>
        /// Gets the current tact of the device -- the clock cycles since
        /// the device was reset
        /// </summary>
        long Tacts { get; }
    }
}