namespace Spect.Net.SpectrumEmu.Abstraction
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
    }

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
        /// #of tacts within the frame
        /// </summary>
        int FrameTacts { get; }
        
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

    /// <summary>
    /// This interface represents a Spectrum virtual machine
    /// </summary>
    public interface ISpectrumVm : IFrameBoundDevice
    {
        /// <summary>
        /// Gets the frequency of the virtual machine's clock in Hz
        /// </summary>
        int ClockFrequeny { get; }
    }

    /// <summary>
    /// Represents a device that is attached to a hosting Spectrum
    /// virtual machine
    /// </summary>
    public interface ISpectrumBoundDevice
    {
        /// <summary>
        /// The virtual machine that hosts the device
        /// </summary>
        ISpectrumVm HostVm { get; }
    }
}