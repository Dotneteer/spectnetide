using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using Spect.Net.SpectrumEmu.Machine;
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
        public SpectrumDebugViewModel Vm { get; set; }

        public static int PixelSize = 1;

        private DisplayParameters _displayPars;
        private BackgroundWorker _worker;
        private WriteableBitmap _bitmap;
        private WriteableBitmapRenderer _pixels;
        private KeyMapper _keyMapper;
        private bool _workerResult;

        public SpectrumDisplayControl()
        {
            InitializeComponent();
            if (ViewModelBase.IsInDesignModeStatic) return;

            InitWorker();
            Loaded += OnLoaded;
            Messenger.Default.Register<SpectrumVmPreparedToRunMessage>(this, OnInitializedMessageReceived);
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            Vm = DataContext as SpectrumDebugViewModel;
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
            Vm.VmState = VmState.Running;
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
            var startMode = e.Argument as StartMode;
            _workerResult = Vm.RunVm(startMode?.EmulationMode ?? EmulationMode.Continuous, 
                startMode?.DebugStepMode ?? DebugStepMode.StopAtBreakpoint);
        }

        private void WorkerOnRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs runWorkerCompletedEventArgs)
        {
            if (!_workerResult && Vm.VmState != VmState.Paused) return;

            Vm.VmState = VmState.Paused;
            Messenger.Default.Send(new SpectrumVmExecCycleCompletedMessage());
        }

        public void ProcessKeyDown(Key key)
        {
            var spectrumKey = _keyMapper.GetSpectrumKeyCodeFor(key);
            if (spectrumKey != null)
            {
                Vm.SetKeyStatus(spectrumKey.Value, true);
            }
        }

        public void ProcessKeyUp(Key key)
        {
            var spectrumKey = _keyMapper.GetSpectrumKeyCodeFor(key);
            if (spectrumKey != null)
            {
                Vm.SetKeyStatus(spectrumKey.Value, false);
            }
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            ProcessKeyDown(e.Key);
        }

        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            ProcessKeyUp(e.Key);
        }

        private void OnInitializedMessageReceived(SpectrumVmPreparedToRunMessage msg)
        {
            _worker.RunWorkerAsync(new StartMode(msg.EmulationMode, msg.DebugStepMode));
        }

        private class StartMode
        {
            public readonly EmulationMode EmulationMode;

            public readonly DebugStepMode DebugStepMode;

            /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
            public StartMode(EmulationMode emulationMode, DebugStepMode debugStepMode)
            {
                EmulationMode = emulationMode;
                DebugStepMode = debugStepMode;
            }
        }
    }
}
