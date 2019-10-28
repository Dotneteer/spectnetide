using Spect.Net.SpectrumEmu.Abstraction.Configuration;
using Spect.Net.SpectrumEmu.Abstraction.Devices.Screen;
using Spect.Net.SpectrumEmu.Abstraction.Providers;

namespace Spect.Net.SpectrumEmu.Test.Helpers
{
    public class TestPixelRenderer: VmComponentProviderBase, IScreenFrameProvider
    {
        private readonly ScreenConfiguration _displayPars;
        private byte[] _pixelMemory;
        
        public bool IsFrameReady { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        public TestPixelRenderer(IScreenConfiguration displayPars)
        {
            _displayPars = new ScreenConfiguration(displayPars);
        }

        /// <summary>
        /// The ULA signs that it's time to start a new frame
        /// </summary>
        public void StartNewFrame()
        {
            IsFrameReady = false;
        }

        /// <summary>
        /// Signs that the current frame is rendered and ready to be displayed
        /// </summary>
        /// <param name="frame">The buffer that contains the frame to display</param>
        public void DisplayFrame(byte[] frame)
        {
            IsFrameReady = true;
        }

        public void SetPixelMemory(byte[] pixelMemory)
        {
            _pixelMemory = pixelMemory;
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