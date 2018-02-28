namespace Spect.Net.SpectrumEmu.Devices.Next
{
    /// <summary>
    /// This class represents the Palette Value Register (0x41)
    /// </summary>
    public class PaletteValueRegister : FeatureControlRegisterBase
    {
        /// <summary>
        /// Used to upload 8-bit colors to the ULANext palette.
        /// </summary>
        public PaletteValueRegister() : base(0x41, false)
        {
        }
    }
}