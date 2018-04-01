using Spect.Net.SpectrumEmu.Devices.Screen;

namespace Spect.Net.SpectrumEmu.Scripting
{
    /// <summary>
    /// Wraps a RenderingTact instance into a read-only class
    /// </summary>
    public class ScreenRenderingTact
    {
        /// <summary>
        /// Tha rendering phase to be applied for the particular tact
        /// </summary>
        public ScreenRenderingPhase Phase { get; }

        /// <summary>
        /// Display memory contention delay
        /// </summary>
        public byte ContentionDelay { get; }

        /// <summary>
        /// Display memory address used in the particular tact
        /// </summary>
        public ushort PixelByteToFetchAddress { get; }

        /// <summary>
        /// Display memory address used in the particular tact
        /// </summary>
        public ushort AttributeToFetchAddress { get; }

        /// <summary>
        /// Pixel X coordinate
        /// </summary>
        public ushort XPos { get; }

        /// <summary>
        /// Pixel Y coordinate
        /// </summary>
        public ushort YPos { get; }

        /// <summary>
        /// Initializes an instance from the specified RenderingTact instance
        /// </summary>
        /// <param name="rt">RenderingTact instance</param>
        public ScreenRenderingTact(RenderingTact rt)
        {
            Phase = rt.Phase;
            ContentionDelay = rt.ContentionDelay;
            PixelByteToFetchAddress = rt.PixelByteToFetchAddress;
            AttributeToFetchAddress = rt.AttributeToFetchAddress;
            XPos = rt.XPos;
            YPos = rt.YPos;
        }
    }
}