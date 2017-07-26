using System;
using System.Reflection;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using Spect.Net.SpectrumEmu.Abstraction.Providers;
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
        /// <summary>
        /// The ZX Spectrum virtual machine view model utilized by this user control
        /// </summary>
        public SpectrumVmViewModel Vm { get; set; }

        public KeyboardProvider KeyboardProvider { get; }

        public bool AllowKeyboardScan { get; set; }

        private ScreenConfiguration _displayPars;
        private BeeperConfiguration _beeperPars;
        private WriteableBitmap _bitmap;
        private IWavePlayer _waveOut;
        private WaveEarbitPulseProcessor _waveProcessor;

        private readonly IRomProvider _romProvider;
        private readonly IClockProvider _clockProvider;

        public SpectrumDisplayControl()
        {
            InitializeComponent();
            if (ViewModelBase.IsInDesignModeStatic) return;

            _romProvider = new ResourceRomProvider();
            _clockProvider = new ClockProvider();
            KeyboardProvider = new KeyboardProvider();

            InitDisplay();
            InitSound();
            AllowKeyboardScan = true;
            Messenger.Default.Register<SpectrumVmStateChangedMessage>(this, OnVmStateChanged);
            Messenger.Default.Register<SpectrumDisplayModeChangedMessage>(this, OnDisplayModeChanged);
            Messenger.Default.Register<DelegatingFrameRenderer.DisplayFrameMessage>(this, OnDisplayFrame);
        }

        /// <summary>
        /// We need to stop sound output when the app exists
        /// </summary>
        public void StopSound()
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
            if (Vm == null)
            {
                return;
            }

            Vm.RomProvider = _romProvider;
            Vm.ClockProvider = _clockProvider;
            Vm.KeyboardProvider = KeyboardProvider;
            Vm.FrameRenderer = new DelegatingFrameRenderer();
            Vm.SoundProcessor = _waveProcessor;
            Vm.LoadContentProvider = new TzxEmbeddedResourceLoadContentProvider(Assembly.GetEntryAssembly());
            Vm.SaveContentProvider = new TzxTempFileSaveContentProvider();
            Vm.StartVmCommand.Execute(null);
            Focus();
            Vm.DisplayMode = SpectrumDisplayMode.Fit;
            Vm.TapeSetName = "Border.tzx";
        }

        /// <summary>
        /// Cleanup when the user control is closed
        /// </summary>
        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            StopSound();
        }

        /// <summary>
        /// Manage the size change of the control
        /// </summary>
        private void OnSizeChanged(object sender, SizeChangedEventArgs args)
        {
            if (Vm == null) return;
            ResizeFor(args.NewSize.Width, args.NewSize.Height);
        }


        /// <summary>
        /// Respond to the state changes of the Spectrum virtual machine
        /// </summary>
        private void OnVmStateChanged(SpectrumVmStateChangedMessage message)
        {
            switch (message.NewState)
            {
                case SpectrumVmState.Stopped:
                    _waveOut.Stop();
                    break;
                case SpectrumVmState.Running:
                    _waveOut.Play();
                    break;
                case SpectrumVmState.Paused:
                    _waveOut.Pause();
                    break;
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
            Display.Width = _displayPars.ScreenWidth;
            Display.Height = _displayPars.ScreenLines;
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

        private void OnDisplayFrame(DelegatingFrameRenderer.DisplayFrameMessage message)
        {
            // --- Refresh the screen
            Dispatcher.Invoke(() =>
                {
                    RefreshSpectrumScreen(message.Buffer);
                    if (AllowKeyboardScan)
                    {
                        KeyboardProvider.Scan();
                    }
                },
                DispatcherPriority.Normal
            );
        }

        /// <summary>
        /// Refreshes the spectrum screen
        /// </summary>
        private void RefreshSpectrumScreen(byte[] currentBuffer)
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
                        var pixelData = currentBuffer[y * width + x];
                        *(uint*)addr = Vm.SpectrumVm.ScreenDevice.SpectrumColors[pixelData & 0x0F];
                    }
                }
            }
            _bitmap.AddDirtyRect(new Int32Rect(0, 0, width, height));
            _bitmap.Unlock();
        }
    }
}
