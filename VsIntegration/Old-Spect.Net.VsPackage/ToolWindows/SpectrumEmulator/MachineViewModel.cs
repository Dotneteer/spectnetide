using System;
using System.Threading.Tasks;
using Spect.Net.SpectrumEmu.Abstraction.Configuration;
using Spect.Net.SpectrumEmu.Abstraction.Devices;
using Spect.Net.SpectrumEmu.Abstraction.Discovery;
using Spect.Net.SpectrumEmu.Abstraction.Providers;
using Spect.Net.SpectrumEmu.Machine;
using Spect.Net.SpectrumEmu.Scripting;
using Spect.Net.Wpf.Mvvm;

namespace Spect.Net.VsPackage.ToolWindows.SpectrumEmulator
{
    /// <summary>
    /// This view model represents the view that displays a run time Spectrum virtual machine
    /// </summary>
    public class MachineViewModel: EnhancedViewModelBase, ISpectrumVmController, IDisposable
    {
        private SpectrumDisplayMode _displayMode;
        private bool _runsInDebugMode;
        private ushort _memViewPoint;
        private ushort _disAssViewPoint;

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
        public VmState MachineState => Machine.VmState;

        /// <summary>
        /// Signs that the state of the virtual machine has been changed
        /// </summary>
        public event EventHandler<VmStateChangedEventArgs> VmStateChanged
        {
            add => Machine.VmStateChanged += value;
            remove => Machine.VmStateChanged -= value;
        }

        /// <summary>
        /// Sign that the screen of the virtual machine has been refreshed
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
        /// The task that represents the completion of the execution cycle
        /// </summary>
        public Task CompletionTask => Machine.CompletionTask;

        /// <summary>
        /// Provider to manage debug information
        /// </summary>
        public ISpectrumDebugInfoProvider DebugInfoProvider { get; set; }

        /// <summary>
        /// Stack debug provider
        /// </summary>
        public IStackDebugSupport StackDebugSupport
        {
            get => Machine?.SpectrumVm?.Cpu?.StackDebugSupport;
            set
            {
                if (Machine?.SpectrumVm?.Cpu != null)
                {
                    Machine.SpectrumVm.Cpu.StackDebugSupport = value;
                }
            }
        }

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

        /// <summary>
        /// Sets the memory address to navigate to when showing the ZX Spectrum Memory window
        /// </summary>
        public ushort MemViewPoint
        {
            get => _memViewPoint;
            set
            {
                Set(ref _memViewPoint, value);
                MemViewPointChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Sign that the screen of the virtual machine has been refreshed
        /// </summary>
        public event EventHandler MemViewPointChanged;

        /// <summary>
        /// Sets the memory address to navigate to when showing the Z80 Disassembly window
        /// </summary>
        public ushort DisAssViewPoint
        {
            get => _disAssViewPoint;
            set
            {
                Set(ref _disAssViewPoint, value);
                DisAssViewPointChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public event EventHandler DisAssViewPointChanged;


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
        public void Start()
        {
            RunsInDebugMode = false;
            Machine.Start(new ExecuteCycleOptions(fastTapeMode: FastTapeMode));
        }

        /// <summary>
        /// Sets the debug mode
        /// </summary>
        /// <param name="mode">True, if the machine should run in debug mode</param>
        void ISpectrumVmController.SetDebugMode(bool mode)
        {
            RunsInDebugMode = mode;
        }

        /// <summary>
        /// Pauses the Spectrum virtual machine
        /// </summary>
        public async Task Pause()
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
        public async Task Stop()
        {
            await Machine.Stop();
            JustRestoredState = false;
            NoToolRefreshMode = false;
        }

        /// <summary>
        /// Starts the machine in a background thread.
        /// </summary>
        /// <param name="options">Options to start the machine with.</param>
        /// <remarks>
        /// Reports completion when the machine starts executing its cycles. The machine can
        /// go into Paused or Stopped state, if the execution options allow, for example, 
        /// when it runs to a predefined breakpoint.
        /// </remarks>
        void ISpectrumVmController.Start(ExecuteCycleOptions options) => Machine.Start(options);

        /// <summary>
        /// Forces the machine into Paused state
        /// </summary>
        void ISpectrumVmController.ForcePausedState()
        {
            Machine.ForcePausedState();
        }

        /// <summary>
        /// Resets the Spectrum virtual machine
        /// </summary>
        public async Task ResetAsync()
        {
            await Stop();
            Start();
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
            if (MachineState != VmState.Paused) return;

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
            if (MachineState != VmState.Paused) return;

            RunsInDebugMode = true;
            Machine.Start(new ExecuteCycleOptions(EmulationMode.Debugger,
                    DebugStepMode.StepOver, FastTapeMode,
                    skipInterruptRoutine: SkipInterruptRoutine));
        }

        /// <summary>
        /// Enters into the step-out debug mode
        /// </summary>
        public void StepOut()
        {
            if (MachineState != VmState.Paused) return;

            RunsInDebugMode = true;
            Machine.Start(new ExecuteCycleOptions(EmulationMode.Debugger,
                DebugStepMode.StepOut, FastTapeMode,
                skipInterruptRoutine: SkipInterruptRoutine));
        }

        #endregion
    }
}