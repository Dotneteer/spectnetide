using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using Spect.Net.SpectrumEmu.Devices.Beeper;
using Spect.Net.SpectrumEmu.Devices.Screen;
using Spect.Net.Wpf.Audio;
using Spect.Net.Wpf.Providers;

namespace Spect.Net.Wpf.SpectrumControl
{
    /// <summary>
    /// Interaction logic for SpectrumDisplayControl.xaml
    /// </summary>
    public partial class SpectrumDisplayControl
    {
        public SpectrumVmViewModel Vm { get; set; }

        public static int PixelSize = 1;

        private ScreenConfiguration _displayPars;
        private BeeperConfiguration _beeperPars;
        private BackgroundWorker _worker;
        private WriteableBitmap _bitmap;
        private WriteableBitmapRenderer _pixels;
        private bool _workerResult;
        private KeyboardProvider _keyboardProvider;
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
            Messenger.Default.Register<SpectrumVmStateChangedMessage>(this, OnVmStateChanged);
            Messenger.Default.Register<SpectrumDisplayModeChangedMessage>(this, OnDisplayModeChanged);
        }

        /// <summary>
        /// We need to stop sound output when the app exists
        /// </summary>
        private void OnAppExit(object sender, ExitEventArgs exitEventArgs)
        {
            if (_waveOut == null) return;

            _waveOut.Dispose();
            _waveOut = null;
        }

        /// <summary>
        /// Initialize the Spectrum virtual machine dependencies when the user control is loaded
        /// </summary>
        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {

            Vm = DataContext as SpectrumVmViewModel;
            if (Vm == null) return;

            Application.Current.Exit += OnAppExit;
            Unloaded += OnUnloaded;
            SizeChanged += OnSizeChanged;

            Vm.RomProvider = new ResourceRomProvider();
            Vm.ClockProvider = new ClockProvider();
            Vm.KeyboardProvider = _keyboardProvider = new KeyboardProvider();
            Vm.ScreenPixelRenderer = _pixels = new WriteableBitmapRenderer(_displayPars, _worker);
            Vm.SoundProcessor = _waveProcessor;
            Vm.LoadContentProvider = new TzxEmbeddedResourceLoadContentProvider(Assembly.GetEntryAssembly());
            Vm.SaveContentProvider = new TzxTempFileSaveContentProvider();
            Vm.StartVmCommand.Execute(null);
            Focus();
            Vm.DisplayMode = SpectrumDisplayMode.Fit;
        }

        /// <summary>
        /// Cleanup when the user control is closed
        /// </summary>
        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            Application.Current.Exit -= OnAppExit;
        }

        /// <summary>
        /// Manage the size change of the control
        /// </summary>
        private void OnSizeChanged(object sender, SizeChangedEventArgs args)
        {
            ResizeFor(args.NewSize.Width, args.NewSize.Height);
        }

        /// <summary>
        /// Respond to the state changes of the Spectrum virtual machine
        /// </summary>
        private void OnVmStateChanged(SpectrumVmStateChangedMessage message)
        {
            if (message.NewState == SpectrumVmState.Running)
            {
                _worker.RunWorkerAsync();
            }
        }

        /// <summary>
        /// Responds to the change of display mode
        /// </summary>
        private void OnDisplayModeChanged(SpectrumDisplayModeChangedMessage message)
        {
            ResizeFor(ActualWidth, ActualHeight);
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
            _displayPars = new ScreenConfiguration();

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
            _beeperPars = new BeeperConfiguration();
            _waveOut = new WaveOut
            {
                DesiredLatency = 100
            };
            _waveProcessor = new WaveEarbitPulseProcessor(_beeperPars);
            _waveOut.Init(_waveProcessor);
        }

        private void ResizeFor(double width, double height)
        {
            if (Vm.DisplayMode >= SpectrumDisplayMode.Normal && Vm.DisplayMode <= SpectrumDisplayMode.Zoom5)
            {
                var scale = (int) Vm.DisplayMode;
                PixelScale.ScaleX = PixelScale.ScaleY = scale;
                return;
            }
            var widthFactor = (int)(width / _displayPars.ScreenWidth);
            var heightFactor = (int)height / _displayPars.ScreenLines;
            var factor = Math.Min(widthFactor, heightFactor);
            if (factor < (int)SpectrumDisplayMode.Normal) factor = (int)SpectrumDisplayMode.Normal;
            else if (factor > (int)SpectrumDisplayMode.Zoom5) factor = (int)SpectrumDisplayMode.Zoom5;
            PixelScale.ScaleX = PixelScale.ScaleY = factor;
        }


        /// <summary>
        /// The background worker completed the execution of the Spectrum VM
        /// </summary>
        private void SpectrumVmExecutionCycleCompleted(object sender, RunWorkerCompletedEventArgs runWorkerCompletedEventArgs)
        {
            _waveOut.Pause();
            if (!_workerResult && Vm.VmState != SpectrumVmState.Paused) return;

            Vm.VmState = SpectrumVmState.Paused;
        }

        /// <summary>
        /// Refreshes the screen whenever the background worker reports the progress
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="progressChangedEventArgs"></param>
        private void WorkerOnProgressChanged(object sender, ProgressChangedEventArgs progressChangedEventArgs)
        {
            if (Vm.SpectrumVm == null) return;
            Vm.VmState = SpectrumVmState.Running;
            Vm.UpdateCommandStates();
            RefreshSpectrumScreen();
        }

        /// <summary>
        /// This method is executed by the background worker to run the execution
        /// cycle of the Spectrum VM
        /// </summary>
        private void RunSpectrumVm(object sender, DoWorkEventArgs e)
        {
            _waveOut.Play();
            _workerResult = Vm.RunVm();
        }

        /// <summary>
        /// Process the key down event, set the Spectrum VM keyboard state
        /// </summary>
        /// <param name="keyArgs">Key arguments</param>
        public void ProcessKeyDown(KeyEventArgs keyArgs)
        {
            _keyboardProvider.Scan();
        }

        /// <summary>
        /// Process the key up event, set the Spectrum VM keyboard state
        /// </summary>
        /// <param name="keyArgs">Key arguments</param>
        public void ProcessKeyUp(KeyEventArgs keyArgs)
        {
            _keyboardProvider.Scan();
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
                var pBackBuffer = (int)_bitmap.BackBuffer;

                for (var x = 0; x < width; x++)
                {
                    for (var y = 0; y < height; y++)
                    {
                        var addr = pBackBuffer + y * stride + x * 4;
                        var pixelData = _pixels.CurrentBuffer[y * width + x];
                        *(uint*)addr = Vm.SpectrumVm.ScreenDevice.SpectrumColors[pixelData & 0x0F];
                    }
                }
            }
            _bitmap.AddDirtyRect(new Int32Rect(0, 0, width, height));
            _bitmap.Unlock();
        }
    }
}
