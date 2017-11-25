using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using GalaSoft.MvvmLight.Messaging;
using Spect.Net.SpectrumEmu.Devices.Screen;
using Spect.Net.SpectrumEmu.Machine;
using Spect.Net.Wpf.Mvvm;
using Spect.Net.Wpf.Mvvm.Messages;
using Spect.Net.Wpf.Providers;
using MachineViewModel = Spect.Net.WpfClient.Machine.MachineViewModel;

namespace Spect.Net.WpfClient.SpectrumControl
{
    /// <summary>
    /// This control is responsible to display the bitmap that represents the
    /// ZX Spectrum screen. It also responds to the virtual machine state changes 
    /// and other VM-related events.
    /// </summary>
    /// <remarks>
    /// Most of the logic is implemented withtin the MachineViewModel held by
    /// a control instance. In order to display and run the VM, you should set up 
    /// the properties of MachineViewModel before instantiating this control.
    /// </remarks>
    public partial class SpectrumDisplayControl
    {
        private ScreenConfiguration _displayPars;
        private WriteableBitmap _bitmap;
        private bool _isReloaded;
        private byte[] _lastBuffer;

        /// <summary>
        /// The ZX Spectrum virtual machine view model utilized by this user control
        /// </summary>
        public MachineViewModel Vm { get; set; }

        /// <summary>
        /// Initialize the control and sign that the next Loaded event will be the first one.
        /// </summary>
        public SpectrumDisplayControl()
        {
            InitializeComponent();
            _isReloaded = false;
            _lastBuffer = null;
        }

        /// <summary>
        /// Initialize the Spectrum virtual machine dependencies when the user control is loaded
        /// </summary>
        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            Vm = DataContext as MachineViewModel;
            if (Vm == null) return;

            // --- Prepare the screen
            _displayPars = new ScreenConfiguration(Vm.ScreenConfiguration);
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
            if (_isReloaded && Vm.VmState == VmState.Running)
            {
                Vm.BeeperProvider.PlaySound();
            }

            // --- Register messages this control listens to
            Messenger.Default.Register<VmStateChangedMessage>(this, OnVmStateChanged);
            Messenger.Default.Register<VmDisplayModeChangedMessage>(this, OnDisplayModeChanged);
            Messenger.Default.Register<DelegatingScreenFrameProvider.VmDisplayFrameReadyMessage>(this, OnDisplayFrame);

            // --- Now, the control is fully loaded and ready to work
            Messenger.Default.Send(new SpectrumControlLoadedMessage());

            // --- Apply the current screen size
            // ReSharper disable once PossibleNullReferenceException
            OnDisplayModeChanged(new VmDisplayModeChangedMessage(Vm.DisplayMode));
        }

        /// <summary>
        /// Cleanup when the user control is unloaded
        /// </summary>
        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            Vm?.BeeperProvider?.PauseSound();

            // --- Unregister messages this control listens to
            Messenger.Default.Unregister<VmStateChangedMessage>(this);
            Messenger.Default.Unregister<VmDisplayModeChangedMessage>(this);
            Messenger.Default.Unregister<DelegatingScreenFrameProvider.VmDisplayFrameReadyMessage>(this);

            // --- Sign that the next time we load the control, it is a reload
            _isReloaded = true;
        }

        /// <summary>
        /// Respond to the state changes of the Spectrum virtual machine
        /// </summary>
        /// <remarks>
        /// This method is called from a background thread!
        /// </remarks>
        private void OnVmStateChanged(VmStateChangedMessage message)
        {
            Dispatcher.Invoke(() =>
                {
                    switch (message.NewState)
                    {
                        case VmState.Stopped:
                            Vm.BeeperProvider.KillSound();
                            Vm.SpectrumVm.TapeDevice.LoadCompleted -= OnFastLoadCompleted;
                            break;
                        case VmState.Running:
                            Vm.BeeperProvider.PlaySound();
                            Vm.SpectrumVm.TapeDevice.LoadCompleted += OnFastLoadCompleted;
                            break;
                        case VmState.Paused:
                            Vm.BeeperProvider.PauseSound();
                            break;
                    }
                },
                DispatcherPriority.Send);
        }

        /// <summary>
        /// Responds to the change of display mode
        /// </summary>
        private void OnDisplayModeChanged(VmDisplayModeChangedMessage message)
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
        private void OnDisplayFrame(DelegatingScreenFrameProvider.VmDisplayFrameReadyMessage message)
        {
            // --- Refresh the screen
            Dispatcher.Invoke(() =>
                {
                    _lastBuffer = message.Buffer;
                    RefreshSpectrumScreen(_lastBuffer);
                    Vm.KeyboardProvider.Scan(Vm.AllowKeyboardScan);
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
        private void OnFastLoadCompleted(object sender, EventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                Vm.SpectrumVm.BeeperDevice.Reset();
                Vm.BeeperProvider.PlaySound();
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
        private void RefreshSpectrumScreen(IReadOnlyList<byte> currentBuffer)
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
