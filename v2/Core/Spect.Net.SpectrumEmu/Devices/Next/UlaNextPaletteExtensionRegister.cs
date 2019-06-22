namespace Spect.Net.SpectrumEmu.Devices.Next
{
    /// <summary>
    /// ULANext Palette Extension (0x44)
    /// </summary>
    public class UlaNextPaletteExtensionRegister : FeatureControlRegisterBase
    {
        /// <summary>
        /// Indicates if the first byte of the palett data is set
        /// </summary>
        public bool FirstByteSet { get; set; }

        /// <summary>
        /// Used to send 9-bit (2-byte) color values to the ULANext Palette.
        /// </summary>
        public UlaNextPaletteExtensionRegister() : base(0x44, false)
        {
            FirstByteSet = false;
        }
    }
}