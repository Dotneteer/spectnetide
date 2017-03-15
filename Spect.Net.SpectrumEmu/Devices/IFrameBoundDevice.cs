namespace Spect.Net.SpectrumEmu.Devices
{
    /// <summary>
    /// This interface describes a device the operation of which is
    /// bound to the ULA screen frame generation 
    /// </summary>
    public interface IFrameBoundDevice: IVmDevice
    {
        /// <summary>
        /// Announces that the device should start a new frame
        /// </summary>
        void StartNewFrame();

        /// <summary>
        /// Signs that the current frame has been completed
        /// </summary>
        void SignFrameCompleted();
    }
}