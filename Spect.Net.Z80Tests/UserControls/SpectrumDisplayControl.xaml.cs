using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Spect.Net.Spectrum.Test.Helpers;
using Spect.Net.Spectrum.Utilities;
using Spect.Net.Z80Tests.SpectrumHost;

namespace Spect.Net.Z80Tests.UserControls
{
    /// <summary>
    /// Interaction logic for SpectrumDisplayControl.xaml
    /// </summary>
    public partial class SpectrumDisplayControl
    {
        public static int PixelSize = 2;
        private SpectrumAdvancedTestMachine _spectrum;
        private BackgroundWorker _worker;
        private WriteableBitmap _bitmap;
        private WriteableBitmapRenderer _pixels;

        public SpectrumDisplayControl()
        {
            InitializeComponent();
            long freq;
            KernelMethods.QueryPerformanceFrequency(out freq);
            long start;
            KernelMethods.QueryPerformanceCounter(out start);
            InitScreen();
            long end;
            KernelMethods.QueryPerformanceCounter(out end);
            Caption.Text = $"{(end - start) / (double)freq}";
            Loaded += (sender, args) =>
            {
                _worker.RunWorkerAsync();
            };
        }

        private void InitScreen()
        {
            _spectrum = new SpectrumAdvancedTestMachine();

            _bitmap = new WriteableBitmap(
                _spectrum.DisplayPars.ScreenWidth,
                _spectrum.DisplayPars.ScreenLines,
                96,
                96,
                PixelFormats.Bgr32,
                null);
            Display.Source = _bitmap;
            Display.Width = PixelSize * _spectrum.DisplayPars.ScreenWidth;
            Display.Height = PixelSize * _spectrum.DisplayPars.ScreenLines;
            Display.Stretch = Stretch.Fill;

            _worker = new BackgroundWorker();
            _worker.DoWork += Worker_DoWork;
            _worker.WorkerReportsProgress = true;
            _worker.ProgressChanged += WorkerOnProgressChanged;

            _pixels = new WriteableBitmapRenderer(_spectrum.ScreenDevice, _worker);
            _spectrum.ScreenDevice.SetScreenPixelRenderer(_pixels);

        }

        private void WorkerOnProgressChanged(object sender, ProgressChangedEventArgs progressChangedEventArgs)
        {
            var width = _spectrum.DisplayPars.ScreenWidth;
            var height = _spectrum.DisplayPars.ScreenLines;

            _bitmap.Lock();
            unsafe
            {
                var stride = _bitmap.BackBufferStride;
                // Get a pointer to the back buffer.
                var pBackBuffer = (int)_bitmap.BackBuffer;

                for (var x = 0; x < width; x++)
                {
                    for (var y = 0; y < height; y++)
                    {
                        var addr = pBackBuffer + y * stride + x * 4;
                        var pixelData = _pixels.CurrentBuffer[y * width + x];
                        *(uint*)addr = _spectrum.ScreenDevice.SpectrumColors[pixelData & 0x0F];
                    }
                }
            }
            _bitmap.AddDirtyRect(new Int32Rect(0, 0, width, height));
            _bitmap.Unlock();
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            _spectrum.ExecuteCycle(CancellationToken.None);
        }
    }
}
