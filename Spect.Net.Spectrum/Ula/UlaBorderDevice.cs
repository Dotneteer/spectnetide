namespace Spect.Net.Spectrum.Ula
{
    /// <summary>
    /// This class represents the border user by the ULA
    /// </summary>
    public class UlaBorderDevice
    {
        private byte _borderColor;

        /// <summary>
        /// Gets or sets the ULA border color
        /// </summary>
        public byte BorderColor
        {
            get { return _borderColor; }
            set { _borderColor = (byte) (value & 0x07); }
        }
    }
}