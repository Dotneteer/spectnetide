using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Spect.Net.Spectrum.Ula;
using Spect.Net.Spectrum.Utilities;

namespace Spect.Net.Z80Tests.UserControls
{
    /// <summary>
    /// Interaction logic for SpectrumDisplayControl.xaml
    /// </summary>
    public partial class SpectrumDisplayControl
    {
        private readonly UlaVideoDisplayParameters _videoParams;
        private long _frequency;

        private static uint[] SpectrumColors = new uint[] 
            { 
                0xFF000000, 0xFF0000AA, 0xFFAA0000, 0xFFAA00AA, 
                0xFF00AA00, 0xFF00AAAA, 0xFFAAAA00, 0xFFAAAAAA,
                0xFF000000, 0xFF0000FF, 0xFFFF0000, 0xFFFF00FF, 
                0xFF00FF00, 0xFF00FFFF, 0xFFFFFF00, 0xFFFFFFFF,
            };

        public static int PixelSize = 3;

        private readonly WriteableBitmap _bitmap;
        public SpectrumDisplayControl()
        {
            InitializeComponent();
            _videoParams = new UlaVideoDisplayParameters();

            _bitmap = new WriteableBitmap(
                _videoParams.ScreenWidth,
                _videoParams.DisplayLines,
                96,
                96,
                PixelFormats.Bgr32,
                null);
            Display.Source = _bitmap;
            Display.Width = PixelSize*_videoParams.ScreenWidth;
            Display.Height = PixelSize*_videoParams.DisplayLines;
            Display.Stretch = Stretch.Fill;

            long freq;
            KernelMethods.QueryPerformanceFrequency(out freq);
            _frequency = freq;
            InitScreen();
        }

        private void InitScreen()
        {
            long start;
            KernelMethods.QueryPerformanceCounter(out start);
            
            _bitmap.Lock();
            unsafe
            {
                // Get a pointer to the back buffer.
                var pBackBuffer = (int) _bitmap.BackBuffer;

                for (var x = 0; x < _videoParams.ScreenWidth; x++)
                {
                    for (var y = 0; y < _videoParams.DisplayLines; y++)
                    {
                        var addr = pBackBuffer + y*_bitmap.BackBufferStride + x*4;
                        var isScreenPixel = x >= _videoParams.BorderLeftPixels 
                            && x < _videoParams.BorderLeftPixels + _videoParams.DisplayWidth
                            && y >= _videoParams.BorderTopLines 
                            && y <= _videoParams.BorderTopLines +_videoParams.DisplayHeight;
                        var pixelData = (isScreenPixel ? 0 : 255) << 16;
                        pixelData |= isScreenPixel ? 255 : 0;
                        *(int*) addr = pixelData;
                    }
                }
            }
            _bitmap.AddDirtyRect(new Int32Rect(0, 0, _videoParams.ScreenWidth, _videoParams.DisplayLines));
            _bitmap.Unlock();

            long end;
            KernelMethods.QueryPerformanceCounter(out end);
            Caption.Text = $"{(end - start)/(double)_frequency}";
        }
    }
}
