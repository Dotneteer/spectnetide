namespace Spect.Net.SpectrumEmu.Devices.Next
{
    /// <summary>
    /// Represents the ULA Next control register (0x43)
    /// </summary>
    public class UlaNextControlRegister : FeatureControlRegisterBase
    {
        /// <summary>
        /// Reserved for future use
        /// </summary>
        public bool Reserved => (LastValue & 0x80) != 0;

        /// <summary>
        /// Select normal (0) or second (1) palette to read/write
        /// </summary>
        public int SelectNornalOrSecondPalette => (LastValue & 0x40) == 0 ? 0 : 1;

        /// <summary>
        /// Select ULA (00), Layer 2 (01) or sprite (10) palette to read/write
        /// </summary>
        public int SelectUlaLayer2SpritePalette => (LastValue & 0x30) >> 4;

        /// <summary>
        /// Enable second palette on Sprites
        /// </summary>
        public bool EnableSecondPaletteOnSprites => (LastValue & 0x08) != 0;

        /// <summary>
        /// Enable second palette on Layer 2
        /// </summary>
        public bool EnableSecondPaletteOnLayer2 => (LastValue & 0x04) != 0;

        /// <summary>
        /// Enable second palette on ULANext
        /// </summary>
        public bool EnableSecondPaletteOnUlaNext => (LastValue & 0x02) != 0;

        /// <summary>
        /// Enable ULANext attributes
        /// </summary>
        public bool EnableUlaNextAttributes => (LastValue & 0x01) != 0;

        /// <summary>
        /// Initializes a new instance of the feature control register.
        /// </summary>
        public UlaNextControlRegister() : base(0x43, false)
        {
        }
    }
}