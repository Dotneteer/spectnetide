namespace Spect.Net.SpectrumEmu.Devices.Next
{
    /// <summary>
    /// This class represents the Palette Index Register (0x40)
    /// </summary>
    public class PaletteIndexRegister : FeatureControlRegisterBase
    {
        /// <summary>
        /// Initializes a new instance of the feature control register.
        /// </summary>
        /// <remarks>
        /// Palette numbers from 0-127 are INK colors. Entries 0-7 are 
        /// the standard Spectrum INK colors. Palette numbers from 128-255 
        /// are PAPER colors. Entries 0-7 are the standard Spectrum PAPER 
        /// colors and border colors.
        /// Higher colors can only be used by enabling ULANext mode via 
        /// ULANext Control Register($43).
        /// </remarks>
        public PaletteIndexRegister() : base(0x40, false)
        {
        }
    }
}