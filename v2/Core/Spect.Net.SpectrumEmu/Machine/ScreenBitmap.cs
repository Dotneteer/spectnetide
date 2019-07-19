using System;
using Spect.Net.SpectrumEmu.Abstraction.Devices;

namespace Spect.Net.SpectrumEmu.Machine
{
    /// <summary>
    /// Represents the current screen's pixels, including the border
    /// </summary>
    public sealed class ScreenBitmap
    {
        private readonly IScreenDevice _screenDevice;
        private readonly int _height;
        private readonly int _width;
        private readonly int _displayHeight;
        private readonly int _displayWidth;
        private readonly int _borderTopLines;
        private readonly int _borderLeftPixels;

        public ScreenBitmap(IScreenDevice screenDevice)
        {
            _screenDevice = screenDevice;
            var config = screenDevice.ScreenConfiguration;
            _height = config.ScreenLines;
            _width = config.ScreenWidth;
            _displayHeight = config.DisplayLines;
            _displayWidth = config.DisplayWidth;
            _borderTopLines = config.BorderTopLines;
            _borderLeftPixels = config.BorderLeftPixels;
        }

        /// <summary>
        /// Gets the array of pixels that represent the line with the specified number
        /// </summary>
        /// <param name="lineNo">Zero-based line number</param>
        /// <returns></returns>
        public byte[] this[int lineNo]
        {
            get
            {
                if (lineNo < 0 || lineNo >= _height)
                {
                    throw new IndexOutOfRangeException();
                }
                var lineBytes = new byte[_width];
                var pixels = _screenDevice.GetPixelBuffer();
                for (var i = lineNo * _width; i < lineNo * _width + _width; i++)
                {
                    lineBytes[i] = pixels[i];
                }
                return lineBytes;
            }
        }

        /// <summary>
        /// Gets the pixel in the specified position
        /// </summary>
        /// <param name="row">Row index</param>
        /// <param name="col">Column index</param>
        /// <returns></returns>
        public byte this[int row, int col]
        {
            get
            {
                if (row < 0 || row >= _height)
                {
                    throw new IndexOutOfRangeException();
                }
                if (col < 0 || col >= _width)
                {
                    throw new IndexOutOfRangeException();
                }
                var pixels = _screenDevice.GetPixelBuffer();
                return pixels[row * _width + col];
            }
        }

        /// <summary>
        /// Gets the pixel in the specified display position (border is omitted)
        /// </summary>
        /// <param name="row">Row index</param>
        /// <param name="col">Column index</param>
        /// <returns></returns>
        public byte GetDisplayPixel(int row, int col)
        {
            if (row < 0 || row >= _displayHeight)
            {
                throw new IndexOutOfRangeException();
            }
            if (col < 0 || col >= _displayWidth)
            {
                throw new IndexOutOfRangeException();
            }
            var pixels = _screenDevice.GetPixelBuffer();
            return pixels[(row + _borderTopLines) * _width + _borderLeftPixels + col];
        }
    }

}