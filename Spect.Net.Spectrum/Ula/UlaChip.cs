namespace Spect.Net.Spectrum.Ula
{
    /// <summary>
    /// This class represents the functions of the ULA chip
    /// </summary>
    public class UlaChip
    {
        public UlaVideoDisplayParameters DisplayParameters { get; }
        public UlaScreenRenderer ScreenRenderer { get; }
        public UlaBorderDevice BorderDevice { get; }

        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public UlaChip()
        {
            DisplayParameters = new UlaVideoDisplayParameters();
            ScreenRenderer = new UlaScreenRenderer(this);
            BorderDevice = new UlaBorderDevice(this);
        }
    }
}