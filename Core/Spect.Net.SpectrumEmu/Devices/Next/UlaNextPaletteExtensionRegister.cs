namespace Spect.Net.SpectrumEmu.Devices.Next
{
    /// <summary>
    /// ULANext Palette Extension (0x44)
    /// </summary>
    public class UlaNextPaletteExtensionRegister : FeatureControlRegisterBase
    {
        /// <summary>
        /// Used to send 9-bit (2-byte) color values to the ULANext Palette.
        /// </summary>
        public UlaNextPaletteExtensionRegister() : base(0x42, false)
        {
        }
    }
}