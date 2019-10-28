using System.ComponentModel;

namespace Spect.Net.SpectrumEmu.Machine
{
    /// <summary>
    /// This class represents the arguments of an event when the a ZX Spectrum
    /// screen frame is rendered
    /// </summary>
    public class RenderFrameEventArgs : CancelEventArgs
    {
        public RenderFrameEventArgs(byte[] screenPixels)
        {
            ScreenPixels = screenPixels;
            Cancel = false;
        }

        /// <summary>
        /// The pixels of the screen, from left to right, from top to bottom.
        /// Each byte represents a single pixel value.
        /// </summary>
        public byte[] ScreenPixels { get; }
    }
}