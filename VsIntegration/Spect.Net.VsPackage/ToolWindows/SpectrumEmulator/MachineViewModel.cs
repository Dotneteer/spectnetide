using System;
using Spect.Net.SpectrumEmu.Abstraction.Configuration;
using Spect.Net.SpectrumEmu.Abstraction.Devices;
using Spect.Net.SpectrumEmu.Abstraction.Discovery;
using Spect.Net.SpectrumEmu.Abstraction.Providers;
using Spect.Net.SpectrumEmu.Devices.Rom;
using Spect.Net.SpectrumEmu.Machine;
using Spect.Net.Wpf.Mvvm;

namespace Spect.Net.VsPackage.ToolWindows.SpectrumEmulator
{
    /// <summary>
    /// This view model represents the view that displays a run time Spectrum virtual machine
    /// </summary>
    public class MachineViewModel: EnhancedViewModelBase, IDisposable
    {
        private SpectrumDisplayMode _displayMode;
        private bool _runsInDebugMode;
        private SpectrumVmControllerBase _controller;
        private bool _configPrepared;

        #region ViewModel properties

        /// <summary>
        /// The controller that provides machine operations
        /// </summary>
        public SpectrumVmControllerBase MachineController
        {
            get => _controller;
            set
            {
                if (_configPrepared)
                {
                    throw new InvalidOperationException(
                        "Machine is prepared to run, you cannot change its controller.");
                }
                _controller = value;
            }
        }

        /// <summary>
        /// The Spectrum virtual machine
        /// </summary>
        public ISpectrumVm SpectrumVm => _controller.SpectrumVm;

        /// <summary>
        /// Signs that this is the very first start of the
        /// virtual machine 
        /// </summary>
        public bool IsFirstStart => _controller.IsFirstStart;

        /// <summary>
        /// Signs that this is the very first paused state
        /// of the virtual machine
        /// </summary>
        public bool IsFirstPause => _controller.IsFirstPause;

        /// <summary>
        /// The current state of the virtual machine
        /// </summary>
        public VmState VmState => _controller?.VmState ?? VmState.None;

        /// <summary>
        /// Signs that the state of the virtual machine has been changed
        /// </summary>
        public event EventHandler<VmStateChangedEventArgs> VmStateChanged
        {
            add => _controller.VmStateChanged += value;
            remove => _controller.VmStateChanged -= value;
        }

        /// <summary>
        /// Sign that the screen of the virtual machnine has been refresehd
        /// </summary>
        public event EventHandler<VmScreenRefreshedEventArgs> VmScreenRefreshed
        {
            add => _controller.VmScreenRefreshed += value;
            remove => _controller.VmScreenRefreshed -= value;
        }

        /// <summary>
        /// The current display mode of the Spectrum control
        /// </summary>
        public SpectrumDisplayMode DisplayMode
        {
            get => _displayMode;
            set => Set(ref _displayMode, value);
        }

        /// <summary>
        /// Indicates if the virtual machine runs in debug mode
        /// </summary>
        public bool RunsInDebugMode
        {
            get => _runsInDebugMode;
            set => Set(ref _runsInDebugMode, value);
        }

        /// <summary>
        /// Provider to manage debug information
        /// </summary>
        public ISpectrumDebugInfoProvider DebugInfoProvider { get; set; }

        /// <summary>
        /// Stack debug provider
        /// </summary>
        public IStackDebugSupport StackDebugSupport { get; set; }

        /// <summary>
        /// Device information for startup
        /// </summary>
        public DeviceInfoCollection DeviceData { get; set; }

        /// <summary>
        /// Signs if keyboard scan is allowed or disabled
        /// </summary>
        public bool AllowKeyboardScan { get; set; }

        /// <summary>
        /// Gets the screen configuration
        /// </summary>
        public IScreenConfiguration ScreenConfiguration { get; set; }

        /// <summary>
        /// Gets the flag that indicates if fast load mode is allowed
        /// </summary>
        public bool FastTapeMode { get; set; }

        /// <summary>
        /// Signs if the instructions within the maskable interrupt 
        /// routine should be skipped
        /// </summary>
        public bool SkipInterruptRoutine { get; set; }

        #endregion

        #region Life cycle methods

        /// <summary>
        /// Initializes the view model
        /// </summary>
        public MachineViewModel()
        {
            DeviceData = new DeviceInfoCollection();
            DisplayMode = SpectrumDisplayMode.Fit;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, 
        /// releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            MachineController?.Dispose();
        }

        #endregion

        #region Virtual machine command handlers

        /// <summary>
        /// Starts the Spectrum virtual machine
        /// </summary>
        public void StartVm()
        {
            PrepareStartupConfig();
            RunsInDebugMode = false;
            _controller.StartVm(new ExecuteCycleOptions(fastTapeMode: FastTapeMode));
        }

        /// <summary>
        /// Starts the Spectrum virtual machine and prepares
        /// it to run injected code
        /// </summary>
        public void RunVmToTerminationPoint(int terminationRom, ushort terminationPoint)
        {
            PrepareStartupConfig();
            RunsInDebugMode = false;
            _controller.StartVm(new ExecuteCycleOptions(EmulationMode.UntilExecutionPoint,
                terminationRom: terminationRom,
                terminationPoint: terminationPoint,
                fastTapeMode: FastTapeMode));
        }

        /// <summary>
        /// Restarts the Spectrum virtual machine and prepares
        /// it to run injected code
        /// </summary>
        public void RestartVmAndRunToTerminationPoint(int terminationRom, ushort terminationPoint)
        {
            PrepareStartupConfig();
            _controller.EnsureMachine();
            RunsInDebugMode = false;
            _controller.StartVm(new ExecuteCycleOptions(EmulationMode.UntilExecutionPoint,
                terminationRom: terminationRom,
                terminationPoint: terminationPoint,
                fastTapeMode: FastTapeMode));
        }

        /// <summary>
        /// Pauses the Spectrum virtual machine
        /// </summary>
        public void PauseVm()
        {
            _controller.PauseVm();
        }

        /// <summary>
        /// Stops the Spectrum virtual machine
        /// </summary>
        public void StopVm()
        {
            _controller.StopVm();
        }

        /// <summary>
        /// Resets the Spectrum virtual machine
        /// </summary>
        public async void ResetVm()
        {
            StopVm();
            await _controller.CompletionTask;
            StartVm();
        }

        /// <summary>
        /// Starts the ZX Spectrum virtual machine in debug mode
        /// </summary>
        public void StartDebugVm()
        {
            PrepareStartupConfig();
            RunsInDebugMode = true;
            _controller.StartVm(new ExecuteCycleOptions(EmulationMode.Debugger, 
                fastTapeMode: FastTapeMode,
                skipInterruptRoutine: SkipInterruptRoutine));
        }

        /// <summary>
        /// Enters into the step-in debug mode
        /// </summary>
        public void StepInto()
        {
            if (VmState != VmState.Paused) return;

            PrepareStartupConfig();
            RunsInDebugMode = true;
            _controller.StartVm(new ExecuteCycleOptions(EmulationMode.Debugger,
                DebugStepMode.StepInto, FastTapeMode,
                    skipInterruptRoutine: SkipInterruptRoutine));
        }

        /// <summary>
        /// Enters into the step-over debug mode
        /// </summary>
        public void StepOver()
        {
            if (VmState != VmState.Paused) return;

            PrepareStartupConfig();
            RunsInDebugMode = true;
            _controller.StartVm(new ExecuteCycleOptions(EmulationMode.Debugger,
                    DebugStepMode.StepOver, FastTapeMode,
                    skipInterruptRoutine: SkipInterruptRoutine));
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Prepares the startup configuration of the machine
        /// </summary>
        public void PrepareStartupConfig()
        {
            _controller.StartupConfiguration = new MachineStartupConfiguration
            {
                DeviceData = DeviceData,
                DebugInfoProvider = DebugInfoProvider,
                StackDebugSupport = StackDebugSupport
            };
            _configPrepared = true;
        }

        #endregion
    }
}