using System;
using System.Threading.Tasks;
using Spect.Net.SpectrumEmu.Abstraction.Configuration;
using Spect.Net.SpectrumEmu.Abstraction.Devices;
using Spect.Net.SpectrumEmu.Abstraction.Discovery;
using Spect.Net.SpectrumEmu.Abstraction.Providers;
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

        #region ViewModel properties

        /// <summary>
        /// The Spectrum machine to use in the emulator
        /// </summary>
        public SpectrumMachine Machine { get; }

        /// <summary>
        /// The Spectrum virtual machine
        /// </summary>
        public ISpectrumVm SpectrumVm => Machine.SpectrumVm;

        /// <summary>
        /// Signs that this is the very first start of the
        /// virtual machine 
        /// </summary>
        public bool IsFirstStart => Machine.IsFirstStart;

        /// <summary>
        /// Signs that this is the very first paused state
        /// of the virtual machine
        /// </summary>
        public bool IsFirstPause => Machine.IsFirstPause;

        /// <summary>
        /// Indicates that state has just been restored
        /// </summary>
        public bool JustRestoredState { get; set; }

        /// <summary>
        /// Indicates that tool windows should not be refreshed while the VM runs
        /// </summary>
        public bool NoToolRefreshMode { get; set; }

        /// <summary>
        /// The current state of the virtual machine
        /// </summary>
        public VmState VmState => Machine.VmState;

        /// <summary>
        /// Signs that the state of the virtual machine has been changed
        /// </summary>
        public event EventHandler<VmStateChangedEventArgs> VmStateChanged
        {
            add => Machine.VmStateChanged += value;
            remove => Machine.VmStateChanged -= value;
        }

        /// <summary>
        /// Sign that the screen of the virtual machnine has been refresehd
        /// </summary>
        public event EventHandler<VmScreenRefreshedEventArgs> VmScreenRefreshed
        {
            add => Machine.VmScreenRefreshed += value;
            remove => Machine.VmScreenRefreshed -= value;
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
        public IScreenConfiguration ScreenConfiguration => Machine.SpectrumVm.ScreenConfiguration;

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
        /// Initializes the view model with the specified machine
        /// </summary>
        /// <param name="machine"></param>
        public MachineViewModel(SpectrumMachine machine) : this()
        {
            Machine = machine;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, 
        /// releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Machine.Dispose();
        }

        #endregion

        #region Virtual machine command handlers

        /// <summary>
        /// Starts the Spectrum virtual machine
        /// </summary>
        public void StartVm()
        {
            RunsInDebugMode = false;
            Machine.Start(new ExecuteCycleOptions(fastTapeMode: FastTapeMode));
        }

        /// <summary>
        /// Starts the Spectrum virtual machine and prepares
        /// it to run injected code
        /// </summary>
        public void RunVmToTerminationPoint(int terminationRom, ushort terminationPoint)
        {
            RunsInDebugMode = false;
            Machine.Start(new ExecuteCycleOptions(EmulationMode.UntilExecutionPoint,
                terminationRom: terminationRom,
                terminationPoint: terminationPoint,
                fastTapeMode: FastTapeMode,
                hiddenMode: true));
        }

        /// <summary>
        /// Restarts the Spectrum virtual machine and prepares
        /// it to run injected code
        /// </summary>
        public void RestartVmToRunToTerminationPoint(int terminationRom, ushort terminationPoint)
        {
            RunsInDebugMode = false;
            Machine.Start(new ExecuteCycleOptions(EmulationMode.UntilExecutionPoint,
                terminationRom: terminationRom,
                terminationPoint: terminationPoint,
                fastTapeMode: FastTapeMode,
                hiddenMode: true));
        }

        /// <summary>
        /// Pauses the Spectrum virtual machine
        /// </summary>
        public async Task PauseVm()
        {
            await Machine.Pause();
            JustRestoredState = false;
            NoToolRefreshMode = false;
        }

        /// <summary>
        /// Forces the VM to get paused state
        /// </summary>
        public void ForcePauseVmAfterStateRestore()
        {
            Machine.ForcePausedState();
            JustRestoredState = true;
            NoToolRefreshMode = false;
        }

        /// <summary>
        /// Stops the Spectrum virtual machine
        /// </summary>
        public async Task StopVm()
        {
            await Machine.Stop();
            JustRestoredState = false;
            NoToolRefreshMode = false;
        }

        /// <summary>
        /// Resets the Spectrum virtual machine
        /// </summary>
        public async Task ResetVm()
        {
            await StopVm();
            StartVm();
        }

        /// <summary>
        /// Starts the ZX Spectrum virtual machine in debug mode
        /// </summary>
        public void StartDebugVm()
        {
            RunsInDebugMode = true;
            Machine.Start(new ExecuteCycleOptions(EmulationMode.Debugger, 
                fastTapeMode: FastTapeMode,
                skipInterruptRoutine: SkipInterruptRoutine));
        }

        /// <summary>
        /// Enters into the step-in debug mode
        /// </summary>
        public void StepInto()
        {
            if (VmState != VmState.Paused) return;

            RunsInDebugMode = true;
            Machine.Start(new ExecuteCycleOptions(EmulationMode.Debugger,
                DebugStepMode.StepInto, FastTapeMode,
                    skipInterruptRoutine: SkipInterruptRoutine));
        }

        /// <summary>
        /// Enters into the step-over debug mode
        /// </summary>
        public void StepOver()
        {
            if (VmState != VmState.Paused) return;

            RunsInDebugMode = true;
            Machine.Start(new ExecuteCycleOptions(EmulationMode.Debugger,
                    DebugStepMode.StepOver, FastTapeMode,
                    skipInterruptRoutine: SkipInterruptRoutine));
        }

        #endregion
    }
}