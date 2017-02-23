namespace Spect.Net.Spectrum.Ula
{
    /// <summary>
    /// This class represents the border user by the ULA
    /// </summary>
    public class UlaBorderDevice
    {
        private int _borderColor;

        /// <summary>
        /// Gets or sets the ULA border color
        /// </summary>
        public int BorderColor
        {
            get { return _borderColor; }
            set { _borderColor = value & 0x07; }
        }
    }
}