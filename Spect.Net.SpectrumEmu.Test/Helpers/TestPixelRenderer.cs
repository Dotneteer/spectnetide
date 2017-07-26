using System.Collections.Generic;
using Spect.Net.SpectrumEmu.Abstraction.Providers;
using Spect.Net.SpectrumEmu.Devices.Screen;

namespace Spect.Net.SpectrumEmu.Test.Helpers
{
    public class TestPixelRenderer: IFrameRenderer
    {
        private readonly ScreenConfiguration _displayPars;
        private byte[] _pixelMemory;
        
        public bool IsFrameReady { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        public TestPixelRenderer(ScreenConfiguration displayPars)
        {
            _displayPars = displayPars;
        }

        /// <summary>
        /// Resets the renderer
        /// </summary>
        public void Reset()
        {
        }

        /// <summary>
        /// Sets the palette that should be used with the renderer
        /// </summary>
        /// <param name="palette"></param>
        public void SetPalette(IList<uint> palette)
        {
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