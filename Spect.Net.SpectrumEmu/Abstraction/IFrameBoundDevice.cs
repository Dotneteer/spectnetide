namespace Spect.Net.SpectrumEmu.Abstraction
{
    /// <summary>
    /// This device is bound to a rendering frame of the Spectrum virtual machine
    /// </summary>
    public interface IFrameBoundDevice : IDevice
    {
        /// <summary>
        /// #of frames rendered
        /// </summary>
        int FrameCount { get; }

        /// <summary>
        /// The current tact within the frame
        /// </summary>
        int CurrentFrameTact { get; }

        /// <summary>
        /// Overflow from the previous frame, given in #of tacts 
        /// </summary>
        int Overflow { get; }

        /// <summary>
        /// Allow the device to react to the start of a new frame
        /// </summary>
        void OnNewFrame();

        /// <summary>
        /// Allow the device to react to the completion of a frame
        /// </summary>
        void OnFrameCompleted();
    }
}