namespace Spect.Net.Spectrum.Ula
{
    /// <summary>
    /// This class represents the border user by the ULA
    /// </summary>
    public class UlaBorderDevice
    {
        private UlaChip _ulaChip;
        private byte _borderColor;

        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public UlaBorderDevice(UlaChip ulaChip)
        {
            _ulaChip = ulaChip;
        }

        /// <summary>
        /// Gets or sets the ULA border color
        /// </summary>
        public byte BorderColor
        {
            get { return _borderColor; }
            set { _borderColor = (byte)(value & 0x07); }
        }
    }
}