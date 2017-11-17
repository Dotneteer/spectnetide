using System;
using GalaSoft.MvvmLight.Command;
using Spect.Net.SpectrumEmu.Abstraction.Devices;
using Spect.Net.SpectrumEmu.Abstraction.Discovery;
using Spect.Net.SpectrumEmu.Abstraction.Models;
using Spect.Net.SpectrumEmu.Abstraction.Providers;
using Spect.Net.SpectrumEmu.Machine;
using Spect.Net.Wpf.Mvvm;
using Spect.Net.Wpf.Mvvm.Messages;

namespace Spect.Net.VsPackage.ToolWindows.SpectrumEmulator
{
    /// <summary>
    /// This view model represents the view that displays a run time Spectrum virtual machine
    /// </summary>
    public class MachineViewModel: EnhancedViewModelBase, IDisposable
    {
        private VmState _vmState;
        private SpectrumDisplayMode _displayMode;
        private string _tapeSetName;
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
                _controller.VmStateChanged += OnControllerOnVmStateChanged;
                _controller.VmScreenRefreshed += OnControllerOnVmScreenRefreshed;
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
        public VmState VmState
        {
            get => _vmState;
            set
            {
                var oldState = _vmState;
                if (!Set(ref _vmState, value)) return;

                UpdateCommandStates();
                VmStateChanged?.Invoke(this, new VmStateChangedEventArgs(oldState, value));
            }
        }

        /// <summary>
        /// Signs that the state of the virtual machine has been changed
        /// </summary>
        public event EventHandler<VmStateChangedEventArgs> VmStateChanged;

        /// <summary>
        /// Sign that the screen of the virtual machnine has been refresehd
        /// </summary>
        public event EventHandler VmScreenRefreshed;

        public int RefreshInvocations => VmScreenRefreshed?.GetInvocationList().Length ?? 0;

        /// <summary>
        /// The current display mode of the Spectrum control
        /// </summary>
        public SpectrumDisplayMode DisplayMode
        {
            get => _displayMode;
            set
            {
                if (!Set(ref _displayMode, value)) return;
                MessengerInstance.Send(new MachineDisplayModeChangedMessage(value));
            }
        }

        /// <summary>
        /// The name of the tapeset that is to be used with the next LOAD command
        /// </summary>
        public string TapeSetName
        {
            get => _tapeSetName;
            set
            {
                if (!Set(ref _tapeSetName, value)) return;
                if (LoadContentProvider != null)
                {
                    LoadContentProvider.TapeSetName = _tapeSetName;
                }
            }
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
        /// Initializes the ZX Spectrum virtual machine
        /// </summary>
        public RelayCommand StartVmCommand { get; set; }

        /// <summary>
        /// Initializes the ZX Spectrum virtual machine
        /// and prepares it to run injected code
        /// </summary>
        public RelayCommand StartVmWithCodeCommand { get; set; }

        /// <summary>
        /// Pauses the virtual machine
        /// </summary>
        public RelayCommand PauseVmCommand { get; set; }

        /// <summary>
        /// Stops the ZX Spectrum virtual machine
        /// </summary>
        public RelayCommand StopVmCommand { get; set; }

        /// <summary>
        /// Resets the ZX Spectrum virtual machine
        /// </summary>
        public RelayCommand ResetVmCommand { get; set; }

        /// <summary>
        /// Starts the ZX Spectrum virtual machine in debug mode
        /// </summary>
        public RelayCommand StartDebugVmCommand { get; set; }

        /// <summary>
        /// Steps into the next instruction
        /// </summary>
        public RelayCommand StepIntoCommand { get; set; }

        /// <summary>
        /// Steps ove the next instruction
        /// </summary>
        public RelayCommand StepOverCommand { get; set; }

        /// <summary>
        /// Assigns the tape set name to the load content provider
        /// </summary>
        public RelayCommand<string> AssignTapeSetName { get; set; }

        /// <summary>
        /// The ROM provider to use with the VM
        /// </summary>
        public IRomProvider RomProvider { get; set; }

        /// <summary>
        /// The clock provider to use with the VM
        /// </summary>
        public IClockProvider ClockProvider { get; set; }

        /// <summary>
        /// The pixel renderer to use with the VM
        /// </summary>
        public IScreenFrameProvider ScreenFrameProvider { get; set; }

        /// <summary>
        /// The renderer that creates the beeper and tape sound
        /// </summary>
        public IEarBitFrameProvider EarBitFrameProvider { get; set; }

        /// <summary>
        /// The TZX content provider for the tape device
        /// </summary>
        public ITapeContentProvider LoadContentProvider { get; set; }

        /// <summary>
        /// TZX Save provider for the tape device
        /// </summary>
        public ISaveToTapeProvider SaveToTapeProvider { get; set; }

        /// <summary>
        /// The provider for the keyboard
        /// </summary>
        public IKeyboardProvider KeyboardProvider { get; set; }

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
            VmState = VmState.None;
            DisplayMode = SpectrumDisplayMode.Fit;
            StartVmCommand = new RelayCommand(
                OnStartVm, 
                () => VmState != VmState.Running);
            PauseVmCommand = new RelayCommand(
                OnPauseVm, 
                () => VmState == VmState.Running);
            StopVmCommand = new RelayCommand(
                OnStopVm, 
                () => VmState == VmState.Running || VmState == VmState.Paused);
            ResetVmCommand = new RelayCommand(
                OnResetVm, 
                () => VmState == VmState.Running || VmState == VmState.Paused);
            StartDebugVmCommand = new RelayCommand(
                OnStartDebugVm,
                () => VmState != VmState.Running);
            StepIntoCommand = new RelayCommand(
                OnStepInto,
                () => VmState == VmState.Paused);
            StepOverCommand = new RelayCommand(
                OnStepOver,
                () => VmState == VmState.Paused);
            StartVmWithCodeCommand = new RelayCommand(
                OnStartVmWithCode);
            AssignTapeSetName = new RelayCommand<string>(OnAssignTapeSet);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, 
        /// releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            MachineController?.Dispose();
            if (_configPrepared && _controller != null)
            {
                _controller.VmStateChanged -= OnControllerOnVmStateChanged;
                _controller.VmScreenRefreshed -= OnControllerOnVmScreenRefreshed;
            }
        }

        #endregion

        #region Virtual machine command handlers

        /// <summary>
        /// Starts the Spectrum virtual machine
        /// </summary>
        protected virtual void OnStartVm()
        {
            PrepareStartupConfig();
            RunsInDebugMode = false;
            _controller.StartVm(new ExecuteCycleOptions(fastTapeMode: FastTapeMode));
        }

        /// <summary>
        /// Starts the Spectrum virtual machine and prepares
        /// it to run injected code
        /// </summary>
        private void OnStartVmWithCode()
        {
            PrepareStartupConfig();
            _controller.EnsureMachine();
            RunsInDebugMode = false;
            _controller.StartVm(new ExecuteCycleOptions(EmulationMode.UntilExecutionPoint,
                terminationPoint: SpectrumVm.RomInfo.MainExecAddress,
                fastTapeMode: FastTapeMode));
        }

        /// <summary>
        /// Pauses the Spectrum virtual machine
        /// </summary>
        protected virtual void OnPauseVm()
        {
            _controller.PauseVm();
        }

        /// <summary>
        /// Stops the Spectrum virtual machine
        /// </summary>
        protected virtual void OnStopVm()
        {
            _controller.StopVm();
        }

        /// <summary>
        /// Resets the Spectrum virtual machine
        /// </summary>
        protected virtual async void OnResetVm()
        {
            OnStopVm();
            await _controller.CompletionTask;
            OnStartVm();
        }

        /// <summary>
        /// Starts the ZX Spectrum virtual machine in debug mode
        /// </summary>
        private void OnStartDebugVm()
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
        private void OnStepInto()
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
        private void OnStepOver()
        {
            if (VmState != VmState.Paused) return;

            PrepareStartupConfig();
            RunsInDebugMode = true;
            _controller.StartVm(new ExecuteCycleOptions(EmulationMode.Debugger,
                    DebugStepMode.StepOver, FastTapeMode,
                    skipInterruptRoutine: SkipInterruptRoutine));
        }

        /// <summary>
        /// Assigns the specified tape set name to the load content provider
        /// </summary>
        /// <param name="tapeSetName"></param>
        protected virtual void OnAssignTapeSet(string tapeSetName)
        {
            TapeSetName = tapeSetName;
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Continues running the VM from the current point
        /// </summary>
        /// <summary>
        /// Updates command state changes
        /// </summary>
        public virtual void UpdateCommandStates()
        {
            StartVmCommand.RaiseCanExecuteChanged();
            StartDebugVmCommand.RaiseCanExecuteChanged();
            PauseVmCommand.RaiseCanExecuteChanged();
            StopVmCommand.RaiseCanExecuteChanged();
            StepIntoCommand.RaiseCanExecuteChanged();
            StepOverCommand.RaiseCanExecuteChanged();
            ResetVmCommand.RaiseCanExecuteChanged();
        }

        /// <summary>
        /// Prepares the startup configuration of the machine
        /// </summary>
        private void PrepareStartupConfig()
        {
            _controller.StartupConfiguration = new MachineStartupConfiguration
            {
                DebugInfoProvider = DebugInfoProvider,
                ClockProvider = ClockProvider,
                EarBitFrameProvider = EarBitFrameProvider,
                KeyboardProvider = KeyboardProvider,
                LoadContentProvider = LoadContentProvider,
                RomProvider = RomProvider,
                SaveToTapeProvider = SaveToTapeProvider,
                ScreenConfiguration = ScreenConfiguration,
                ScreenFrameProvider = ScreenFrameProvider,
                StackDebugSupport = StackDebugSupport
            };
            _configPrepared = true;
        }

        /// <summary>
        /// Responds to vm state changes
        /// </summary>
        private void OnControllerOnVmScreenRefreshed(object s, EventArgs e)
        {
            VmScreenRefreshed?.Invoke(s, e);
        }

        /// <summary>
        /// Responds to screen refresh events
        /// </summary>
        private void OnControllerOnVmStateChanged(object s, VmStateChangedEventArgs e)
        {
            VmState = e.NewState;
        }

        #endregion
    }
}