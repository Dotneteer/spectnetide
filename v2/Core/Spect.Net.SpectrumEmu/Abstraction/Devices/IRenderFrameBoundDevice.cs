using System;
using Spect.Net.SpectrumEmu.Machine;

namespace Spect.Net.SpectrumEmu.Abstraction.Devices
{
    /// <summary>
    /// This device is bound to a rendering frame of the Spectrum virtual machine
    /// </summary>
    public interface IRenderFrameBoundDevice : IDevice
    {
        /// <summary>
        /// #of frames rendered
        /// </summary>
        int FrameCount { get; }

        /// <summary>
        /// Overflow from the previous frame, given in #of tacts 
        /// </summary>
        int Overflow { get; set; }

        /// <summary>
        /// Allow the device to react to the start of a new frame
        /// </summary>
        void OnNewFrame();

        /// <summary>
        /// Allow the device to react to the completion of a frame
        /// </summary>
        void OnFrameCompleted();

        /// <summary>
        /// Allow external entities respond to frame completion
        /// </summary>
        event EventHandler FrameCompleted;
    }
}