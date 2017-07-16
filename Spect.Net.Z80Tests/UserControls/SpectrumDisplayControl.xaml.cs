using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using NAudio.Wave;
using Spect.Net.SpectrumEmu.Devices.Beeper;
using Spect.Net.SpectrumEmu.Devices.Screen;
using Spect.Net.SpectrumEmu.Machine;
using Spect.Net.Z80Tests.SpectrumHost;
using Spect.Net.Z80Tests.ViewModels.SpectrumEmu;
using Spect.Net.Z80Tests.Views;

namespace Spect.Net.Z80Tests.UserControls
{
    /// <summary>
    /// Interaction logic for SpectrumDisplayControl.xaml
    /// </summary>
    public partial class SpectrumDisplayControl
    {
        public SpectrumDebugViewModel Vm { get; set; }

        public static int PixelSize = 2;

        private DisplayParameters _displayPars;
        private BeeperParameters _beeperPars;
        private BackgroundWorker _worker;
        private WriteableBitmap _bitmap;
        private WriteableBitmapRenderer _pixels;
        private KeyMapper _keyMapper;
        private bool _workerResult;
        private readonly DispatcherTimer _screenRefreshTimer;
        private DispatcherTimer _keyboardTimer;
        private KeyboardScanner _keyboardScanner;
        private IWavePlayer _waveOut;
        private WaveEarbitPulseProcessor _waveProcessor;


        public SpectrumDisplayControl()
        {
            InitializeComponent();
            if (ViewModelBase.IsInDesignModeStatic) return;

            InitWorker();
            InitDisplay();
            InitSound();
            Loaded += OnLoaded;

            // --- Set up the screen refresh timer
            _screenRefreshTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(20),
                IsEnabled = false
            };
            _screenRefreshTimer.Tick += OnScreenRefresh;

            Messenger.Default.Register<SpectrumVmPreparedToRunMessage>(this, OnInitializedMessageReceived);
            Messenger.Default.Register<AppClosesMessage>(this, OnAppCloses);
        }

        private void OnKeyboardScan(object sender, EventArgs e)
        {
            _keyboardScanner.Scan();
        }

        private void OnAppCloses(AppClosesMessage msg)
        {
            if (_waveOut == null) return;

            _waveOut.Dispose();
            _waveOut = null;
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            Vm = DataContext as SpectrumDebugViewModel;
            if (Vm == null) return;

            Vm.RomProvider = new ResourceRomProvider();
            Vm.ClockProvider = new ClockProvider();
            Vm.ScreenPixelRenderer = _pixels = new WriteableBitmapRenderer(_displayPars, _worker);
            Vm.SoundProcessor = _waveProcessor;
            Vm.TapeContentProvider = new TzxContentProvider();
            Vm.StartVmCommand.Execute(null);
            _keyMapper = new KeyMapper();
            Focus();

            _keyboardScanner = new KeyboardScanner(Vm);
            _keyboardTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(10),
                IsEnabled = true
            };
            _keyboardTimer.Tick += OnKeyboardScan;
        }

        /// <summary>
        /// Sets up the background worker that runs the Spectrum VM
        /// </summary>
        private void InitWorker()
        {
            _worker = new BackgroundWorker
            {
                WorkerReportsProgress = true
            };
            _worker.DoWork += RunSpectrumVm;
            _worker.ProgressChanged += WorkerOnProgressChanged;
            _worker.RunWorkerCompleted += SpectrumVmExecutionCycleCompleted;
        }

        /// <summary>
        /// Sets up the display related members
        /// </summary>
        private void InitDisplay()
        {
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

        private void InitSound()
        {
            _beeperPars = new BeeperParameters();
            _waveOut = new WaveOut
            {
                DesiredLatency = 150
            };
            _waveProcessor = new WaveEarbitPulseProcessor(_beeperPars);
            _waveOut.Init(_waveProcessor);
        }

        /// <summary>
        /// The Spectrum VM is initialized, the background worker can be started
        /// </summary>
        /// <param name="msg"></param>
        private void OnInitializedMessageReceived(SpectrumVmPreparedToRunMessage msg)
        {
            _screenRefreshTimer.Stop();
            _worker.RunWorkerAsync(new StartMode(msg.EmulationMode, msg.DebugStepMode));
        }

        /// <summary>
        /// The background worker completed the execution of the Spectrum VM
        /// </summary>
        private void SpectrumVmExecutionCycleCompleted(object sender, RunWorkerCompletedEventArgs runWorkerCompletedEventArgs)
        {
            _waveOut.Pause();
            if (!_workerResult && Vm.VmState != VmState.Paused) return;

            Vm.VmState = VmState.Paused;
            Messenger.Default.Send(new SpectrumVmExecCycleCompletedMessage());
            _screenRefreshTimer.Start();
        }

        /// <summary>
        /// Refreshes the screen whenever the background worker reports the progress
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="progressChangedEventArgs"></param>
        private void WorkerOnProgressChanged(object sender, ProgressChangedEventArgs progressChangedEventArgs)
        {
            if (Vm.SpectrumVm == null) return;
            Vm.VmState = VmState.Running;
            Vm.UpdateCommandStates();
            // ReSharper disable once ExplicitCallerInfoArgument
            Vm.RaisePropertyChanged(nameof(Vm.DebugInfoProvider));

            RefreshSpectrumScreen();
        }

        /// <summary>
        /// This method is executed by the background worker to run the execution
        /// cycle of the Spectrum VM
        /// </summary>
        private void RunSpectrumVm(object sender, DoWorkEventArgs e)
        {
            var startMode = e.Argument as StartMode;
            _waveOut.Play();
            _workerResult = Vm.RunVm(startMode?.EmulationMode ?? EmulationMode.Continuous, 
                startMode?.DebugStepMode ?? DebugStepMode.StopAtBreakpoint);
        }

        /// <summary>
        /// Process the key down event, set the Spectrum VM keyboard state
        /// </summary>
        /// <param name="keyArgs">Key arguments</param>
        public bool ProcessKeyDown(KeyEventArgs keyArgs)
        {
            return TranslateKeyStatus(keyArgs, true);
        }

        /// <summary>
        /// Process the key up event, set the Spectrum VM keyboard state
        /// </summary>
        /// <param name="keyArgs">Key arguments</param>
        public bool ProcessKeyUp(KeyEventArgs keyArgs)
        {
            return TranslateKeyStatus(keyArgs, false);
        }

        /// <summary>
        /// Process the key down event, set the Spectrum VM keyboard state
        /// </summary>
        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            //e.Handled = ProcessKeyDown(e);
        }

        /// <summary>
        /// Process the key up event, set the Spectrum VM keyboard state
        /// </summary>
        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            //e.Handled = ProcessKeyUp(e);
        }

        /// <summary>
        /// Refreshes the spectrum screen
        /// </summary>
        private void RefreshSpectrumScreen()
        {
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

        /// <summary>
        /// Refresh the Spectrum VM screen
        /// </summary>
        private void OnScreenRefresh(object sender, EventArgs e)
        {
            if (Vm.SpectrumVm == null) return;
            //Vm.SpectrumVm.RefreshShadowScreen();
            RefreshSpectrumScreen();
        }

        /// <summary>
        /// Translate the key to Spectrum keyboard status
        /// </summary>
        private bool TranslateKeyStatus(KeyEventArgs keyArgs, bool keyStatus)
        {
            var processed = false;
            var spectrumKeys = _keyMapper.GetSpectrumKeyCodeFor(keyArgs);
            if (spectrumKeys.First != null)
            {
                Vm.SetKeyStatus(spectrumKeys.First.Value, keyStatus);
                processed = true;
            }
            if (spectrumKeys.Second != null)
            {
                Vm.SetKeyStatus(spectrumKeys.Second.Value, keyStatus);
                processed = true;
            }
            return processed;
        }

        /// <summary>
        /// Class to store the execution cycle modes to pass to the background worker
        /// as argument
        /// </summary>
        private class StartMode
        {
            public readonly EmulationMode EmulationMode;

            public readonly DebugStepMode DebugStepMode;

            public StartMode(EmulationMode emulationMode, DebugStepMode debugStepMode)
            {
                EmulationMode = emulationMode;
                DebugStepMode = debugStepMode;
            }
        }
    }
}
