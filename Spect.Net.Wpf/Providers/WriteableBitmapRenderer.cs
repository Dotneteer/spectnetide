using System.Collections.Generic;
using System.ComponentModel;
using Spect.Net.SpectrumEmu.Abstraction.Providers;
using Spect.Net.SpectrumEmu.Devices.Screen;
// ReSharper disable ConvertToAutoProperty

namespace Spect.Net.Wpf.Providers
{
    /// <summary>
    /// This class renders the Spectrum screen into a WriteableBitmap
    /// </summary>
    public class WriteableBitmapRenderer : IScreenPixelRenderer
    {
        private readonly BackgroundWorker _worker;
        private readonly int _width;
        private readonly int _frames;

        private byte[] _currentBuffer;

        private readonly object _locker = new object();

        /// <summary>
        /// The current screen buffer
        /// </summary>
        public byte[] GetCurrentBuffer() => _currentBuffer;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        public WriteableBitmapRenderer(ScreenConfiguration displayPars, BackgroundWorker worker)
        {
            _width = displayPars.ScreenWidth;
            _worker = worker;
            _frames = 0;
            Reset();
        }

        #region Implementation of IScreenPixelRenderer

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
            //_currentBuffer[y*_width + x] = (byte) colorIndex;
        }

        /// <summary>
        /// Signs that the current frame is rendered and ready to be displayed
        /// </summary>
        public void DisplayFrame(byte[] frame)
        {
            _currentBuffer = frame;
            _worker.ReportProgress(_frames + 1);
        }

        #endregion
    }
}