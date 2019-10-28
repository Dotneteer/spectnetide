namespace Spect.Net.SpectrumEmu.Abstraction.Devices.Screen
{
    /// <summary>
    /// This interface represents the data a rendering tact information entry holds.
    /// </summary>
    public interface IRenderingTact
    {
        /// <summary>
        /// Tha rendering phase to be applied for the particular tact
        /// </summary>
        ScreenRenderingPhase Phase { get; set; }

        /// <summary>
        /// Display memory contention delay
        /// </summary>
        byte ContentionDelay { get; set; }

        /// <summary>
        /// Display memory address used in the particular tact
        /// </summary>
        ushort PixelByteToFetchAddress { get; set; }

        /// <summary>
        /// Display memory address used in the particular tact
        /// </summary>
        ushort AttributeToFetchAddress { get; set; }

        /// <summary>
        /// Pixel X coordinate
        /// </summary>
        ushort XPos { get; set; }

        /// <summary>
        /// Pixel Y coordinate
        /// </summary>
        ushort YPos { get; set; }
    }
}