using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Spect.Net.SpectrumEmu.Ula;
using Spect.Net.Z80Tests.ViewModels;

namespace Spect.Net.Z80Tests.SpectrumHost
{
    /// <summary>
    /// Interaction logic for SpectrumHostView.xaml
    /// </summary>
    public partial class SpectrumHostView
    {
        public static int PixelSize = 3;

        private readonly SpectrumEmuViewModel _vm;
        private DisplayParameters _displayPars;
        private BackgroundWorker _worker;
        private WriteableBitmap _bitmap;
        private WriteableBitmapRenderer _pixels;

        

        public SpectrumHostView()
        {
            InitializeComponent();
            InitWorker();
            DataContext = _vm = new SpectrumEmuViewModel();
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            _vm.RomProvider = new ResourceRomProvider();
            _vm.ClockProvider = new HighResolutionClockProvider();
            _vm.ScreenPixelRenderer = _pixels = new WriteableBitmapRenderer(_displayPars, _worker);
            _vm.InitVmCommand.Execute(null);
            _worker.RunWorkerAsync();
        }

        private void InitWorker()
        {
            _worker = new BackgroundWorker();
            _worker.WorkerReportsProgress = true;
            _worker.DoWork += Worker_DoWork;
            _worker.ProgressChanged += WorkerOnProgressChanged;
            _worker.RunWorkerCompleted += WorkerOnRunWorkerCompleted;

            _displayPars = new DisplayParameters();

            _bitmap = new WriteableBitmap(
                _displayPars.ScreenWidth,
                _displayPars.ScreenLines,
                96,
                96,
                PixelFormats.Bgr32,
                null);
            Display.Source = _bitmap;
            Display.Width = PixelSize * _displayPars.ScreenWidth;
            Display.Height = PixelSize * _displayPars.ScreenLines;
            Display.Stretch = Stretch.Fill;
        }

        private void WorkerOnProgressChanged(object sender, ProgressChangedEventArgs progressChangedEventArgs)
        {
            var width = _displayPars.ScreenWidth;
            var height = _displayPars.ScreenLines;

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
                        *(uint*)addr = _vm.SpectrumVm.ScreenDevice.SpectrumColors[pixelData & 0x0F];
                    }
                }
            }
            _bitmap.AddDirtyRect(new Int32Rect(0, 0, width, height));
            _bitmap.Unlock();
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            _vm.RunVmCommand.Execute(null);
        }

        private void WorkerOnRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs runWorkerCompletedEventArgs)
        {
        }
    }
}
