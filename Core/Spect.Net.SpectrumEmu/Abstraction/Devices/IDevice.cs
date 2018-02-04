namespace Spect.Net.SpectrumEmu.Abstraction.Devices
{
    /// <summary>
    /// This interface represents an abstract device that is intended to specify
    /// the behavior of any device.
    /// </summary>
    public interface IDevice
    {
        /// <summary>
        /// Resets this device
        /// </summary>
        void Reset();

        /// <summary>
        /// Gets the state of the device so that the state can be saved
        /// </summary>
        /// <returns>The object that describes the state of the device</returns>
        object GetState();

        /// <summary>
        /// Sets the state of the device from the specified object
        /// </summary>
        /// <param name="state">Device state</param>
        void SetState(object state);
    }
}