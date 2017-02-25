using System;
using System.ComponentModel;
using Spect.Net.Spectrum.Ula;

namespace Spect.Net.Z80Tests.SpectrumHost
{
    /// <summary>
    /// This class renders the Spectrum screen into a WriteableBitmap
    /// </summary>
    public class WriteableBitmapRenderer : IScreenPixelRenderer
    {
        private readonly BackgroundWorker _worker;
        private readonly DisplayParameters _videoParams;
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
        public WriteableBitmapRenderer(UlaScreenDevice screenDevice, BackgroundWorker worker)
        {
            _videoParams = screenDevice.DisplayParameters;
            _worker = worker;
            _frames = 0;
            var size = _videoParams.ScreenWidth*_videoParams.ScreenLines;
            _buffer1 = new byte[size];
            _buffer2 = new byte[size];
            for (var i = 0; i < size; i++)
            {
                _buffer1[i] = 0x00;
                _buffer2[i] = 0x00;
            }
            CurrentBuffer = _buffer1;
        }

        #region Implementation of IScreenPixelRenderer

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
            CurrentBuffer[y*_videoParams.ScreenWidth + x] = (byte) colorIndex;
        }

        /// <summary>
        /// Signs that the current frame is rendered and ready to be displayed
        /// </summary>
        public void DisplayFrame()
        {
            _worker.ReportProgress(_frames + 1);
        }

        ///// <summary>
        ///// Signs that the current frame is rendered and ready to be displayed
        ///// </summary>
        //public void RenderFrame()
        //{
        //    var width = _videoParams.ScreenWidth;
        //    var height = _videoParams.ScreenLines;

        //    _bitmap.Lock();
        //    unsafe
        //    {
        //        var stride = _bitmap.BackBufferStride;
        //        // Get a pointer to the back buffer.
        //        var pBackBuffer = (int)_bitmap.BackBuffer;

        //        for (var x = 0; x < width; x++)
        //        {
        //            for (var y = 0; y < height; y++)
        //            {
        //                var addr = pBackBuffer + y * stride + x * 4;
        //                var pixelData = _currentBuffer[y * width + x];
        //                *(uint*)addr = _screenDevice.SpectrumColors[pixelData & 0x0F];
        //            }
        //        }
        //    }
        //    _bitmap.AddDirtyRect(new Int32Rect(0, 0, width, height));
        //    _bitmap.Unlock();
        //    _frames++;
        //}

        #endregion
    }
}