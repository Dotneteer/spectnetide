namespace Spect.Net.SpectrumEmu.Devices.Border
{
    /// <summary>
    /// This class represents the border user by the ULA
    /// </summary>
    public class BorderDevice : IBorderDevice
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

        /// <summary>
        /// Resets this device
        /// </summary>
        public void Reset()
        {
        }
    }
}