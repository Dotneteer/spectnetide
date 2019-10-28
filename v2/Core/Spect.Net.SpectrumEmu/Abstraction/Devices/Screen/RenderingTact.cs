namespace Spect.Net.SpectrumEmu.Abstraction.Devices.Screen
{
    /// <summary>
    /// This structure defines information related to a particular tact
    /// of ULA screen rendering
    /// </summary>
    public struct RenderingTact : IRenderingTact
    {
        /// <summary>
        /// Tha rendering phase to be applied for the particular tact
        /// </summary>
        public ScreenRenderingPhase Phase { get; set; }

        /// <summary>
        /// Display memory contention delay
        /// </summary>
        public byte ContentionDelay { get; set; }

        /// <summary>
        /// Display memory address used in the particular tact
        /// </summary>
        public ushort PixelByteToFetchAddress { get; set; }

        /// <summary>
        /// Display memory address used in the particular tact
        /// </summary>
        public ushort AttributeToFetchAddress { get; set; }

        /// <summary>
        /// Pixel X coordinate
        /// </summary>
        public ushort XPos { get; set; }

        /// <summary>
        /// Pixel Y coordinate
        /// </summary>
        public ushort YPos { get; set; }
    }
}