using System.Runtime.InteropServices;

namespace Spect.Net.SpectrumEmu.Devices.Screen
{
    /// <summary>
    /// This structure defines information related to a particular tact
    /// of ULA screen rendering
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    public struct RenderingTact
    {
        /// <summary>
        /// Tha rendering phase to be applied for the particular tact
        /// </summary>
        [FieldOffset(0)]
        public ScreenRenderingPhase Phase;

        /// <summary>
        /// Display memory contention delay
        /// </summary>
        [FieldOffset(1)]
        public byte ContentionDelay;

        /// <summary>
        /// Display memory address used in the particular tact
        /// </summary>
        [FieldOffset(2)]
        public ushort PixelByteToFetchAddress;

        /// <summary>
        /// Display memory address used in the particular tact
        /// </summary>
        [FieldOffset(4)]
        public ushort AttributeToFetchAddress;

        /// <summary>
        /// Pixel X coordinate
        /// </summary>
        [FieldOffset(6)]
        public ushort XPos;

        /// <summary>
        /// Pixel Y coordinate
        /// </summary>
        [FieldOffset(8)]
        public ushort YPos;
    }
}