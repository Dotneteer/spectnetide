using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Spect.Net.SpectrumEmu.Abstraction.Devices.Screen;
using Spect.Net.SpectrumEmu.Devices.Screen;
using Spect.Net.SpectrumEmu.Machine;

#pragma warning disable VSTHRD010

namespace Spect.Net.VsPackage.ToolWindows.SpectrumEmulator
{
    /// <summary>
    /// This control is responsible to display the bitmap that represents the
    /// ZX Spectrum screen. It also responds to the virtual machine state changes 
    /// and other VM-related events.
    /// </summary>
    /// <remarks>
    /// Most of the logic is implemented within the MachineViewModel held by
    /// a control instance. In order to display and run the VM, you should set up 
    /// the properties of MachineViewModel before instantiating this control.
    /// </remarks>
    public partial class SpectrumDisplayControl
    {
        private ScreenConfiguration _displayPars;
        private WriteableBitmap _bitmap;
        private bool _isReloaded;
        private byte[] _lastBuffer;
        private int _cpuFrameCount;
        private uint[] _colors;

        /// <summary>
        /// The ZX Spectrum virtual machine view model utilized by this user control
        /// </summary>
        public EmulatorViewModel Vm { get; set; }

        /// <summary>
        /// Provides keyboard input for the ZX Spectrum virtual machine
        /// </summary>
        public KeyboardScanner KeyboardScanner { get; }

        /// <summary>
        /// Initialize the control and sign that the next Loaded event will be the first one.
        /// </summary>
        public SpectrumDisplayControl()
        {
            InitializeComponent();
            KeyboardScanner = new KeyboardScanner();
            _isReloaded = false;
            _lastBuffer = null;
        }

        /// <summary>
        /// Initialize the Spectrum virtual machine dependencies when the user control is loaded
        /// </summary>
        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            Vm = DataContext as EmulatorViewModel;
            if (Vm == null) return;

            Vm.VmStateChanged += OnVmStateChanged;

            // --- Prepare the screen
            _colors = Spectrum48ScreenDevice.SpectrumColors.ToArray();
            _displayPars = Vm.Machine.ScreenConfiguration;
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
            if (_isReloaded && Vm.MachineState == VmState.Running)
            {
                Vm.Machine.BeeperProvider?.PlaySound();
            }

            // --- Register messages this control listens to
            Vm.KeyScanning += MachineOnKeyScanning;
            Vm.CpuFrameCompleted += MachineOnCpuFrameCompleted;
            Vm.RenderFrameCompleted += MachineOnRenderFrameCompleted;
        }

        private void MachineOnCpuFrameCompleted(object sender, CancelEventArgs e)
        {
            // ReSharper disable once ExplicitCallerInfoArgument
            Vm.RaisePropertyChanged("Machine");
        }

        /// <summary>
        /// Cleanup when the user control is unloaded
        /// </summary>
        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            // --- Un-register messages this control listens to
            if (Vm != null)
            {
                Vm.Machine.BeeperProvider?.PauseSound();
                Vm.Machine.VmStateChanged -= OnVmStateChanged;
                Vm.CpuFrameCompleted -= MachineOnCpuFrameCompleted;
                Vm.Machine.RenderFrameCompleted -= MachineOnRenderFrameCompleted;
            }

            // --- Sign that the next time we load the control, it is a reload
            _isReloaded = true;
        }

        /// <summary>
        /// Respond to the state changes of the Spectrum virtual machine
        /// </summary>
        private void OnVmStateChanged(object sender, VmStateChangedEventArgs args)
        {
            Dispatcher.Invoke(() =>
                {
                    switch (args.NewState)
                    {
                        case VmState.Stopped:
                            Vm.Machine.BeeperProvider?.KillSound();
                            Vm.FastLoadCompleted -= OnFastLoadCompleted;
                            break;
                        case VmState.Running:
                            Vm.Machine.BeeperProvider?.PlaySound();
                            Vm.FastLoadCompleted += OnFastLoadCompleted;
                            break;
                        case VmState.Paused:
                            Vm.Machine.BeeperProvider?.PauseSound();
                            break;
                    }
                },
                DispatcherPriority.Send);
        }

        /// <summary>
        /// Scans the keyboard at every 25th CPU frame cycle
        /// </summary>
        private void MachineOnKeyScanning(object sender, KeyStatusEventArgs e)
        {
            if (_cpuFrameCount++ % 25 == 0)
            {
                e.KeyStatusList.AddRange(KeyboardScanner.Scan());
            }
        }

        /// <summary>
        /// Takes care of refreshing the screen
        /// </summary>
        private void MachineOnRenderFrameCompleted(object sender, RenderFrameEventArgs e)
        {
            // --- Refresh the screen
            Dispatcher.Invoke(() =>
                {
                    _lastBuffer = e.ScreenPixels;
                    RefreshSpectrumScreen(_lastBuffer);
                },
                DispatcherPriority.Send
            );
        }

        /// <summary>
        /// It is time to restart playing the sound
        /// </summary>
        private void OnFastLoadCompleted(object sender, EventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                Vm.Machine.BeeperProvider.PlaySound();
            });
        }

        /// <summary>
        /// Resizes the Spectrum screen according to the specified parent area size
        /// </summary>
        public void ResizeFor(double width, double height)
        {
            if (Vm == null) return;

            var widthFactor = (int)(width / _displayPars.ScreenWidth);
            var heightFactor = (int)height / _displayPars.ScreenLines;
            var scale = Math.Min(widthFactor, heightFactor);

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
            try
            {
                unsafe
                {
                    var stride = _bitmap.BackBufferStride;
                    // Get a pointer to the back buffer.
                    var pBackBuffer = (int) _bitmap.BackBuffer;

                    for (var x = 0; x < width; x++)
                    {
                        for (var y = 0; y < height; y++)
                        {
                            var addr = pBackBuffer + y * stride + x * 4;
                            var pixelData = currentBuffer[y * width + x];
                            *(uint*) addr = _colors[pixelData & 0x0F];
                        }
                    }
                }

                _bitmap.AddDirtyRect(new Int32Rect(0, 0, width, height));
            }
            finally
            {
                _bitmap.Unlock();
            }
        }
    }
}
