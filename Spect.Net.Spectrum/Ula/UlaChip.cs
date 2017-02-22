namespace Spect.Net.Spectrum.Ula
{
    /// <summary>
    /// This class represents the functions of the ULA chip
    /// </summary>
    public class UlaChip
    {
        public UlaClock Clock { get; }
        public UlaVideoDisplayParameters DisplayParameters { get; }
        public UlaScreenRenderer ScreenRenderer { get; }
        public UlaBorderDevice BorderDevice { get; }

        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public UlaChip()
        {
            Clock = new UlaClock();
            DisplayParameters = new UlaVideoDisplayParameters();
            ScreenRenderer = new UlaScreenRenderer(this);
            BorderDevice = new UlaBorderDevice();
        }

        /// <summary>
        /// Resets the ULA chip and its devices
        /// </summary>
        public void Reset()
        {
            // TODO: Implement the UL reset
        }
    }
}