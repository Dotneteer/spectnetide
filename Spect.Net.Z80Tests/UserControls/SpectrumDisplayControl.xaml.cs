using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using GalaSoft.MvvmLight.Messaging;
using Spect.Net.SpectrumEmu.Ula;
using Spect.Net.Z80Tests.SpectrumHost;
using Spect.Net.Z80Tests.ViewModels.SpectrumEmu;

namespace Spect.Net.Z80Tests.UserControls
{
    /// <summary>
    /// Interaction logic for SpectrumDisplayControl.xaml
    /// </summary>
    public partial class SpectrumDisplayControl
    {
        public SpectrumEmuViewModel Vm { get; set; }

        public static int PixelSize = 2;

        private DisplayParameters _displayPars;
        private BackgroundWorker _worker;
        private WriteableBitmap _bitmap;
        private WriteableBitmapRenderer _pixels;
        private KeyMapper _keyMapper;

        public SpectrumDisplayControl()
        {
            InitializeComponent();
            InitWorker();
            Loaded += OnLoaded;

            Messenger.Default.Register<SpectrumVmInitializedMessage>(this, OnInitializedMessageReceived);
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            Vm = DataContext as SpectrumEmuViewModel;
            if (Vm == null) return;

            Vm.RomProvider = new ResourceRomProvider();
            Vm.ClockProvider = new HighResolutionClockProvider();
            Vm.ScreenPixelRenderer = _pixels = new WriteableBitmapRenderer(_displayPars, _worker);
            Vm.StartVmCommand.Execute(null);
            _keyMapper = new KeyMapper();
            Focus();
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
            Display.Width = PixelSize*_displayPars.ScreenWidth;
            Display.Height = PixelSize*_displayPars.ScreenLines;
            Display.Stretch = Stretch.Fill;
        }

        private void WorkerOnProgressChanged(object sender, ProgressChangedEventArgs progressChangedEventArgs)
        {
            if (Vm.SpectrumVm == null) return;
            Vm.IsVmRunning = true;
            Vm.UpdateCommandStates();

            var width = _displayPars.ScreenWidth;
            var height = _displayPars.ScreenLines;

            _bitmap.Lock();
            unsafe
            {
                var stride = _bitmap.BackBufferStride;
                // Get a pointer to the back buffer.
                var pBackBuffer = (int) _bitmap.BackBuffer;

                for (var x = 0; x < width; x++)
                {
                    for (var y = 0; y < height; y++)
                    {
                        var addr = pBackBuffer + y*stride + x*4;
                        var pixelData = _pixels.CurrentBuffer[y*width + x];
                        *(uint*) addr = Vm.SpectrumVm.ScreenDevice.SpectrumColors[pixelData & 0x0F];
                    }
                }
            }
            _bitmap.AddDirtyRect(new Int32Rect(0, 0, width, height));
            _bitmap.Unlock();
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            Vm.RunVm();
        }

        private void WorkerOnRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs runWorkerCompletedEventArgs)
        {
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            var spectrumKey = _keyMapper.GetSpectrumKeyCodeFor(e.Key);
            if (spectrumKey != null)
            {
                Vm.SetKeyStatus(spectrumKey.Value, true);
            }
        }

        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            var spectrumKey = _keyMapper.GetSpectrumKeyCodeFor(e.Key);
            if (spectrumKey != null)
            {
                Vm.SetKeyStatus(spectrumKey.Value, false);
            }
        }

        private void OnInitializedMessageReceived(SpectrumVmInitializedMessage msg)
        {
            _worker.RunWorkerAsync();
        }
    }
}
