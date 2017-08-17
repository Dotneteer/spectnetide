using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using GalaSoft.MvvmLight.Messaging;
using Spect.Net.SpectrumEmu.Devices.Screen;
using Spect.Net.SpectrumEmu.Devices.Tape;
using Spect.Net.Wpf.Providers;

namespace Spect.Net.Wpf.SpectrumControl
{
    /// <summary>
    /// This control is responsible to display the bitmap that represents the
    /// ZX Spectrum screen. It also responds to the virtual machine state changes 
    /// and other VM-related events.
    /// </summary>
    /// <remarks>
    /// Most of the logic is implemented withtin the SpectrumVmViewModel held by
    /// a control instance. In order to display and run the VM, you should set up 
    /// the properties of SpectrumVmViewModel before instantiating this control.
    /// </remarks>
    public partial class SpectrumDisplayControl
    {
        private ScreenConfiguration _displayPars;
        private WriteableBitmap _bitmap;
        private bool _isReloaded;

        /// <summary>
        /// The ZX Spectrum virtual machine view model utilized by this user control
        /// </summary>
        public SpectrumVmViewModel Vm { get; set; }

        /// <summary>
        /// Initialize the control and sign that the next Loaded event will be the first one.
        /// </summary>
        public SpectrumDisplayControl()
        {
            InitializeComponent();
            _isReloaded = false;
        }

        /// <summary>
        /// Initialize the Spectrum virtual machine dependencies when the user control is loaded
        /// </summary>
        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            Vm = DataContext as SpectrumVmViewModel;
            if (Vm == null) return;

            // --- Prepare the screen
            _displayPars = Vm.ScreenConfiguration;
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

            // --- When the control is reloaded, resume playing the sound
            if (_isReloaded && Vm.VmState == SpectrumVmState.Running)
            {
                Vm.EarBitFrameProvider.PlaySound();
            }

            // --- Register messages this control listens to
            Messenger.Default.Register<SpectrumVmStateChangedMessage>(this, OnVmStateChanged);
            Messenger.Default.Register<SpectrumDisplayModeChangedMessage>(this, OnDisplayModeChanged);
            Messenger.Default.Register<DelegatingScreenFrameProvider.DisplayFrameMessage>(this, OnDisplayFrame);
            Messenger.Default.Register<FastLoadCompletedMessage>(this, OnFastLoadCompleted);

            // --- Now, the control is fully loaded and ready to work
            Messenger.Default.Send(new SpectrumControlFullyLoaded(this));

            // --- Apply the current screen size
            OnDisplayModeChanged(new SpectrumDisplayModeChangedMessage(Vm.DisplayMode));
        }

        /// <summary>
        /// Cleanup when the user control is unloaded
        /// </summary>
        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            Vm.EarBitFrameProvider?.PauseSound();

            // --- Unregister messages this control listens to
            Messenger.Default.Unregister<SpectrumVmStateChangedMessage>(this);
            Messenger.Default.Unregister<SpectrumDisplayModeChangedMessage>(this);
            Messenger.Default.Unregister<DelegatingScreenFrameProvider.DisplayFrameMessage>(this);
            Messenger.Default.Unregister<FastLoadCompletedMessage>(this);

            // --- Sign that the next time we load the control, it is a reload
            _isReloaded = true;
        }

        /// <summary>
        /// Respond to the state changes of the Spectrum virtual machine
        /// </summary>
        /// <remarks>
        /// This method is called from a background thread!
        /// </remarks>
        private void OnVmStateChanged(SpectrumVmStateChangedMessage message)
        {
            Dispatcher.Invoke(() =>
                {
                    switch (message.NewState)
                    {
                        case SpectrumVmState.Stopped:
                            Vm.EarBitFrameProvider.KillSound();
                            break;
                        case SpectrumVmState.Running:
                            Vm.EarBitFrameProvider.PlaySound();
                            break;
                        case SpectrumVmState.Paused:
                            Vm.EarBitFrameProvider.PauseSound();
                            break;
                    }
                },
                DispatcherPriority.Send);
        }

        /// <summary>
        /// Responds to the change of display mode
        /// </summary>
        private void OnDisplayModeChanged(SpectrumDisplayModeChangedMessage message)
        {
            ResizeFor(ActualWidth, ActualHeight);
        }

        /// <summary>
        /// The new screen frame is ready, it is time to display it
        /// </summary>
        /// <param name="message">Message with the screen buffer</param>
        /// <remarks>
        /// This method is called from a background thread!
        /// </remarks>
        private void OnDisplayFrame(DelegatingScreenFrameProvider.DisplayFrameMessage message)
        {
            // --- Refresh the screen
            Dispatcher.Invoke(() =>
                {
                    RefreshSpectrumScreen(message.Buffer);
                    Messenger.Default.Send(new SpectrumScreenRefreshedMessage());
                    if (Vm.AllowKeyboardScan)
                    {
                        Vm.KeyboardProvider.Scan();
                    }
                },
                DispatcherPriority.Send
            );
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
        /// It is time to restart playing the sound
        /// </summary>
        private void OnFastLoadCompleted(FastLoadCompletedMessage msg)
        {
            Dispatcher.Invoke(() =>
            {
                Vm.SpectrumVm.BeeperDevice.Reset();
                Vm.EarBitFrameProvider.PlaySound();
            });
        }

        /// <summary>
        /// Resizes the Spectrum screen according to the specified parent area size
        /// </summary>
        private void ResizeFor(double width, double height)
        {
            var scale = (int)Vm.DisplayMode;
            if (Vm.DisplayMode < SpectrumDisplayMode.Normal || Vm.DisplayMode > SpectrumDisplayMode.Zoom5)
            {
                var widthFactor = (int)(width / _displayPars.ScreenWidth);
                var heightFactor = (int)height / _displayPars.ScreenLines;
                scale = Math.Min(widthFactor, heightFactor);
                if (scale < (int)SpectrumDisplayMode.Normal) scale = (int)SpectrumDisplayMode.Normal;
                else if (scale > (int)SpectrumDisplayMode.Zoom5) scale = (int)SpectrumDisplayMode.Zoom5;
            }

            Display.Width = _displayPars.ScreenWidth * scale;
            Display.Height = _displayPars.ScreenLines * scale;
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
                        *(uint*)addr = Spectrum48ScreenDevice.SpectrumColors[pixelData & 0x0F];
                    }
                }
            }
            _bitmap.AddDirtyRect(new Int32Rect(0, 0, width, height));
            _bitmap.Unlock();
        }
    }
}
