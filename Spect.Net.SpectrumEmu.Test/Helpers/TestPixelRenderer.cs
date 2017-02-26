using Spect.Net.SpectrumEmu.Ula;

namespace Spect.Net.SpectrumEmu.Test.Helpers
{
    public class TestPixelRenderer: IScreenPixelRenderer
    {
        private readonly DisplayParameters _displayPars;
        private readonly byte[] _pixelMemory;
        
        public bool IsFrameReady { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        public TestPixelRenderer(DisplayParameters displayPars)
        {
            _displayPars = displayPars;
            var size = _displayPars.ScreenWidth*_displayPars.ScreenLines;
            _pixelMemory = new byte[size];
            for (var i = 0; i < size; i++) _pixelMemory[i] = 0xFF;
        }

        /// <summary>
        /// The ULA signs that it's time to start a new frame
        /// </summary>
        public void StartNewFrame()
        {
            IsFrameReady = false;
        }

        /// <summary>
        /// Renders the (<paramref name="x"/>, <paramref name="y"/>) pixel
        /// on the screen with the specified <paramref name="colorIndex"/>
        /// </summary>
        /// <param name="x">Horizontal coordinate</param>
        /// <param name="y">Vertical coordinate</param>
        /// <param name="colorIndex">Index of the color (0x00..0x0F)</param>
        public void RenderPixel(int x, int y, int colorIndex)
        {
            _pixelMemory[y * _displayPars.ScreenWidth + x] = (byte)colorIndex;
        }

        /// <summary>
        /// Signs that the current frame is rendered and ready to be displayed
        /// </summary>
        public void DisplayFrame()
        {
            IsFrameReady = true;
        }

        /// <summary>
        /// Gets the pixel at the specified location
        /// </summary>
        /// <param name="row">Pixel row</param>
        /// <param name="column">Pixel column</param>
        /// <returns>Pixel color index</returns>
        public byte this[int row, int column] => _pixelMemory[row*_displayPars.ScreenWidth + column];
    }
}