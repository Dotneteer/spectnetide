namespace Spect.Net.SpectrumEmu.Devices.Next
{
    /// <summary>
    /// This class represents the Palette Value Register (0x41)
    /// </summary>
    public class UlaNextInkColorMaskRegister : FeatureControlRegisterBase
    {
        /// <summary>
        /// Specifies mask to extract ink color from attribute cell value in ULANext mode.
        /// </summary>
        public UlaNextInkColorMaskRegister() : base(0x42, false)
        {
        }
    }
}