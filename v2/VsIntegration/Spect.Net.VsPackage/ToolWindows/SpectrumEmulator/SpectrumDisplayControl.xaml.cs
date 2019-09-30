using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Spect.Net.SpectrumEmu.Abstraction.Devices.Screen;
using Spect.Net.SpectrumEmu.Abstraction.Devices.Tape;
using Spect.Net.SpectrumEmu.Devices.Screen;
using Spect.Net.SpectrumEmu.Machine;
using Spect.Net.VsPackage.SolutionItems;

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
        private const string FILE_EXISTS_MESSAGE = "The exported tape file exists in the project. " +
                                                   "Would you like to override it?";

        private const string INVALID_FOLDER_MESSAGE = "The tape folder specified in the Options dialog " +
                                                      "contains invalid characters or an absolute path. Go to the Options dialog and " +
                                                      "fix the issue so that you can add the tape file to the project.";

        private ScreenConfiguration _displayPars;
        private WriteableBitmap _bitmap;
        private bool _isReloaded;
        private byte[] _lastBuffer;
        private byte[] _savedBuffer;
        private int _cpuFrameCount;
        private uint[] _colors;
        private CancellationTokenSource _cancellationSource;
        private Task _shadowRenderingTask;

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
        /// Resizes the Spectrum screen according to the specified parent area size
        /// </summary>
        public void ResizeFor(double width, double height)
        {
            if (Vm == null) return;

            var widthFactor = (int)(width / _displayPars.ScreenWidth);
            var heightFactor = (int)height / _displayPars.ScreenLines;
            var scale = Math.Min(widthFactor, heightFactor);
            if (scale < 1) scale = 1;

            Display.Width = _displayPars.ScreenWidth * scale;
            Display.Height = _displayPars.ScreenLines * scale;
        }

        /// <summary>
        /// Forces the refresh of the ZX Spectrum screen
        /// </summary>
        public void ForceRefresh()
        {
            Dispatcher.Invoke(async () =>
            {
                await Task.Delay(50);
                MachineOnRenderFrameCompleted(this,
                    new RenderFrameEventArgs(Vm.Machine.SpectrumVm.ScreenDevice.GetPixelBuffer()));
            });
        }

        /// <summary>
        /// Initialize the Spectrum virtual machine dependencies when the user control is loaded
        /// </summary>
        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            Vm = DataContext as EmulatorViewModel;
            if (Vm == null) return;

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
            Vm.VmStateChanged += OnVmStateChanged;
            Vm.KeyScanning += MachineOnKeyScanning;
            Vm.CpuFrameCompleted += MachineOnCpuFrameCompleted;
            Vm.RenderFrameCompleted += MachineOnRenderFrameCompleted;
            Vm.LeftSaveMode += MachineOnLeftSaveMode;
            Vm.ShadowScreenModeChanged += OnShadowScreenModeChanged;
            Vm.UlaIndicationModeChanged += OnUlaIndicationModeChanged;
            Vm.MachineInstanceChanged += OnMachineInstanceChanged;
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
                Vm.KeyScanning -= MachineOnKeyScanning;
                Vm.CpuFrameCompleted -= MachineOnCpuFrameCompleted;
                Vm.RenderFrameCompleted -= MachineOnRenderFrameCompleted;
                Vm.LeftSaveMode -= MachineOnLeftSaveMode;
                Vm.ShadowScreenModeChanged -= OnShadowScreenModeChanged;
                Vm.UlaIndicationModeChanged += OnUlaIndicationModeChanged;
                Vm.MachineInstanceChanged -= OnMachineInstanceChanged;
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
                            ShowUlaRaster();
                            break;
                        case VmState.Running:
                            Vm.Machine.BeeperProvider?.PlaySound();
                            Vm.FastLoadCompleted += OnFastLoadCompleted;
                            ShowUlaRaster();
                            StopShadowScreenRendering();
                            break;
                        case VmState.Paused:
                            Vm.Machine.BeeperProvider?.PauseSound();
                            MachineOnRenderFrameCompleted(this,
                                new RenderFrameEventArgs(Vm.Machine.SpectrumVm.ScreenDevice.GetPixelBuffer()));
                            StartShadowScreenRendering();
                            ShowUlaRaster();
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

        private void MachineOnCpuFrameCompleted(object sender, CancelEventArgs e)
        {
            // ReSharper disable once ExplicitCallerInfoArgument
            Vm.RaisePropertyChanged("Machine");
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
        /// The virtual machine has just saved a file.
        /// </summary>
        private void MachineOnLeftSaveMode(object sender, SaveModeEventArgs e)
        {
            Dispatcher.Invoke(() =>
                {
                    SpectrumProject.AddFileToProject(
                        SpectNetPackage.Default.Options.TapeFolder,
                        e.FileName,
                        INVALID_FOLDER_MESSAGE,
                        FILE_EXISTS_MESSAGE);
                });
        }

        /// <summary>
        /// Respond to the change of shadow screen mode
        /// </summary>
        private void OnShadowScreenModeChanged(object sender, EventArgs e)
        {
            if (Vm.MachineState != VmState.Paused) return;
            if (Vm.ShadowScreenEnabled)
            {
                StartShadowScreenRendering();
            }
            else
            {
                StopShadowScreenRendering();
            }
        }

        /// <summary>
        /// Responds to the change of ULA indication mode
        /// </summary>
        private void OnUlaIndicationModeChanged(object sender, EventArgs e)
        {
            ShowUlaRaster();
        }

        /// <summary>
        /// Responds to the event when machine instance changes.
        /// </summary>
        private void OnMachineInstanceChanged(object sender, MachineInstanceChangedEventArgs e)
        {
            Dispatcher.Invoke(() =>
                {
                    _lastBuffer = e.NewMachine.ScreenBitmap.GetPixelBuffer();
                    RefreshSpectrumScreen(_lastBuffer);
                },
                DispatcherPriority.Send
            );
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

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (Vm == null) return;
            ResizeFor(e.NewSize.Width, e.NewSize.Height);
            ShowUlaRaster();
        }

        private void OnLayoutUpdated(object sender, EventArgs e)
        {
            if (Vm == null) return;
            ResizeFor(ActualWidth, ActualHeight);
            ShowUlaRaster();
        }

        /// <summary>
        /// Starts rendering the shadow screen, provided it is turned on
        /// </summary>
        private async void StartShadowScreenRendering()
        {
            if (!Vm.ShadowScreenEnabled || _shadowRenderingTask != null || _cancellationSource != null) return;
            _savedBuffer = _lastBuffer.ToArray();
            _cancellationSource = new CancellationTokenSource();
            _shadowRenderingTask = Vm.Machine.RenderShadowScreen(_cancellationSource.Token);
            try
            {
                await _shadowRenderingTask;
            }
            catch
            {
                // --- This exception is intentionally ignored
            }
            _shadowRenderingTask = null;
        }

        /// <summary>
        /// Stops rendering the shadow screen
        /// </summary>
        private async void StopShadowScreenRendering()
        {
            if (_shadowRenderingTask == null || _cancellationSource == null) return;
            _cancellationSource.Cancel();
            try
            {
                await _shadowRenderingTask;
            }
            catch
            {
                // --- This exception is intentionally ignored
            }
            _shadowRenderingTask = null;
            _cancellationSource = null;
            Dispatcher.Invoke(() =>
                {
                    _lastBuffer = _savedBuffer.ToArray();
                    RefreshSpectrumScreen(_lastBuffer);
                },
                DispatcherPriority.Send
            );
        }

        private void ShowUlaRaster()
        {
            if (Vm.MachineState != VmState.Paused || !Vm.UlaIndicationEnabled)
            {
                // --- Hide ULA raster information
                RasterLine.Visibility = Visibility.Collapsed;
                PixelLine.Visibility = Visibility.Collapsed;
                SyncRectangle.Visibility = Visibility.Collapsed;
                NonVisibleRectangle.Visibility = Visibility.Collapsed;
                DisplayRectangle.Visibility = Visibility.Collapsed;
                BeamRectangle.Visibility = Visibility.Collapsed;
                return;
            }

            // --- Display ULA raster information
            var sc = Vm.Machine.ScreenConfiguration;
            var screenLines = sc.ScreenLines;
            var scale = Display.ActualHeight / screenLines;
            var ulaTacts = sc.ScreenRenderingFrameTactCount;
            var tact = Vm.Machine.CurrentFrameTact % ulaTacts;
            var line = tact / sc.ScreenLineTime;
            var pixel = tact % sc.ScreenLineTime * 2;
            var screenWidth = sc.ScreenLineTime * 2;

            RasterLine.X1 = (ActualWidth - Display.ActualWidth) / 2;
            RasterLine.X2 = RasterLine.X1 + screenWidth * scale;
            RasterLine.Y1 = RasterLine.Y2 = (ActualHeight - Display.ActualHeight) / 2 
            + (line - sc.NonVisibleBorderTopLines) * scale;
            RasterLine.StrokeThickness = scale;

            PixelLine.X1 = PixelLine.X2 = (ActualWidth - Display.ActualWidth) / 2 + pixel * scale;
            PixelLine.Y1 = RasterLine.Y1 - 4 * scale;
            PixelLine.Y2 = RasterLine.Y1 + 4 * scale;
            PixelLine.StrokeThickness = 2 * scale;

            BeamRectangle.Margin = new Thickness(
                (ActualWidth - Display.ActualWidth) / 2 + (pixel - 3) * scale,
                (ActualHeight - Display.ActualHeight) / 2 + (line - sc.NonVisibleBorderTopLines - 3) * scale,
                0,0);
            BeamRectangle.Width = BeamRectangle.Height = 6 * scale;
            BeamPosition.Text = $"({line}, {pixel / 2})";

            SyncRectangle.Margin = new Thickness(
                (ActualWidth - Display.ActualWidth) / 2,
                (ActualHeight - Display.ActualHeight) / 2
                - sc.NonVisibleBorderTopLines * scale, 4, 4);
            SyncRectangle.Width = screenWidth * scale;
            SyncRectangle.Height = sc.RasterLines * scale;

            NonVisibleRectangle.Margin = SyncRectangle.Margin;
            NonVisibleRectangle.Width = SyncRectangle.Width - sc.NonVisibleBorderRightTime * 2 * scale;
            NonVisibleRectangle.Height = SyncRectangle.Height - sc.NonVisibleBorderBottomLines * scale;

            DisplayRectangle.Margin = new Thickness(
                (ActualWidth - Display.ActualWidth) / 2 + sc.BorderLeftPixels * scale,
                (ActualHeight - Display.ActualHeight) / 2 + sc.BorderTopLines * scale, 0, 0);
            DisplayRectangle.Width = sc.DisplayWidth * scale;
            DisplayRectangle.Height = sc.DisplayLines * scale;

            RasterLine.Visibility = Visibility.Visible;
            PixelLine.Visibility = Visibility.Visible;
            SyncRectangle.Visibility = Visibility.Visible;
            NonVisibleRectangle.Visibility = Visibility.Visible;
            DisplayRectangle.Visibility = Visibility.Visible;
            BeamRectangle.Visibility = Visibility.Visible;
        }
    }
}
