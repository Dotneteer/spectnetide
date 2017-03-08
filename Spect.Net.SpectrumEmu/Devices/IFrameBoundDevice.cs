namespace Spect.Net.SpectrumEmu.Devices
{
    /// <summary>
    /// This interface describes a device that is bound to an ULA screen frame
    /// </summary>
    public interface IFrameBoundDevice
    {
        /// <summary>
        /// Resets this device
        /// </summary>
        void Reset();

        /// <summary>
        /// Announdec that the device should start a new frame
        /// </summary>
        void StartNewFrame();

        /// <summary>
        /// Signs that the current frame has been completed
        /// </summary>
        void SignFrameCompleted();
    }
}