using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Spect.Net.Z80Tests.UserControls
{
    /// <summary>
    /// Interaction logic for SpectrumDisplayControl.xaml
    /// </summary>
    public partial class SpectrumDisplayControl
    {
        public const int BORDER_WIDTH = 48;
        public const int BORDER_HEIGHT = 48;
        public const int SCREEN_WIDTH = 256;
        public const int SCREEN_HEIGHT = 192;
        public const int DISPLAY_WIDTH = BORDER_WIDTH + SCREEN_WIDTH + BORDER_WIDTH;
        public const int DISPLAY_HEIGHT = BORDER_HEIGHT + SCREEN_HEIGHT + BORDER_HEIGHT;

        public static int PixelSize = 3;

        private readonly WriteableBitmap _bitmap;
        public SpectrumDisplayControl()
        {
            InitializeComponent();
            _bitmap = new WriteableBitmap(
                DISPLAY_WIDTH,
                DISPLAY_HEIGHT,
                96,
                96,
                PixelFormats.Bgr32,
                null);
            Display.Source = _bitmap;
            Display.Width = PixelSize*DISPLAY_WIDTH;
            Display.Height = PixelSize*DISPLAY_HEIGHT;
            Display.Stretch = Stretch.Fill;
            InitScreen();
        }

        private void InitScreen()
        {
            _bitmap.Lock();
            unsafe
            {
                // Get a pointer to the back buffer.
                var pBackBuffer = (int) _bitmap.BackBuffer;

                for (var x = 0; x < DISPLAY_WIDTH; x++)
                {
                    for (var y = 0; y < DISPLAY_HEIGHT; y++)
                    {
                        var addr = pBackBuffer + y*_bitmap.BackBufferStride + x*4;
                        var isScreenPixel = x >= BORDER_WIDTH && x < BORDER_WIDTH + SCREEN_WIDTH
                                            && y >= BORDER_HEIGHT && y <= BORDER_HEIGHT + SCREEN_HEIGHT;
                        var pixelData = (isScreenPixel ? 0 : 255) << 16;
                        pixelData |= isScreenPixel ? 255 : 0;
                        *(int*) addr = pixelData;
                    }
                }
            }
            _bitmap.AddDirtyRect(new Int32Rect(0, 0, DISPLAY_WIDTH, DISPLAY_HEIGHT));
            _bitmap.Unlock();
        }
    }
}
