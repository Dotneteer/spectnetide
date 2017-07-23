using System.Collections.Generic;
using System.ComponentModel;
using Spect.Net.SpectrumEmu.Abstraction.Providers;
using Spect.Net.SpectrumEmu.Devices.Screen;

namespace Spect.Net.Wpf
{
    /// <summary>
    /// This class renders the Spectrum screen into a WriteableBitmap
    /// </summary>
    public class WriteableBitmapRenderer : IScreenPixelRenderer
    {
        private readonly BackgroundWorker _worker;
        private readonly int _width;
        private readonly int _lines;
        private readonly int _frames;

        private readonly byte[] _buffer1;
        private readonly byte[] _buffer2;
        private readonly object _locker = new object();

        /// <summary>
        /// The current screen buffer
        /// </summary>
        public byte[] CurrentBuffer { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        public WriteableBitmapRenderer(ScreenConfiguration displayPars, BackgroundWorker worker)
        {
            _width = displayPars.ScreenWidth;
            _lines = displayPars.ScreenLines;
            _worker = worker;
            _frames = 0;
            var size = _width * _lines;
            _buffer1 = new byte[size];
            _buffer2 = new byte[size];
            CurrentBuffer = _buffer1;
            Reset();
        }

        #region Implementation of IScreenPixelRenderer

        /// <summary>
        /// Resets the renderer
        /// </summary>
        public void Reset()
        {
            var size = _width*_lines;
            for (var i = 0; i < size; i++)
            {
                _buffer1[i] = 0x00;
                _buffer2[i] = 0x00;
            }
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
            lock (_locker)
            {
                CurrentBuffer = CurrentBuffer == _buffer1 ? _buffer2 : _buffer1;
            }
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
            CurrentBuffer[y*_width + x] = (byte) colorIndex;
        }

        /// <summary>
        /// Signs that the current frame is rendered and ready to be displayed
        /// </summary>
        public void DisplayFrame()
        {
            _worker.ReportProgress(_frames + 1);
        }

        #endregion
    }
}