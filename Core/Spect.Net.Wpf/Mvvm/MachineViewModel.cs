using System;
using System.Threading;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using Spect.Net.SpectrumEmu.Abstraction.Devices;
using Spect.Net.SpectrumEmu.Abstraction.Discovery;
using Spect.Net.SpectrumEmu.Abstraction.Providers;
using Spect.Net.SpectrumEmu.Devices.Screen;
using Spect.Net.SpectrumEmu.Machine;
using Spect.Net.Wpf.Mvvm.Messages;

namespace Spect.Net.Wpf.Mvvm
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
        private TaskCompletionSource<bool> _vmBackgroundTaskCompletionSource;

        // --- Stores the VM state expected after ExceuteCycle()
        private VmState _stateAfterExecuteCycle;

        #region ViewModel properties

        /// <summary>
        /// The ZX Spectrum virtual machine
        /// </summary>
        public ISpectrumVm SpectrumVm { get; private set; }

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
                MessengerInstance.Send(new MachineStateChangedMessage(oldState, value));
            }
        }

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
        /// The cancellation token source to suspend the virtual machine
        /// </summary>
        public CancellationTokenSource CancellationTokenSource { get; private set; }

        /// <summary>
        /// This task represents the ZX Spectrum execution cycle running in the background
        /// </summary>
        public Task<bool> RunnerTask => _vmBackgroundTaskCompletionSource?.Task;

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
        /// Sets the zoom according to the specified string
        /// </summary>
        public RelayCommand<SpectrumDisplayMode> SetZoomCommand { get; set; }

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
        public ScreenConfiguration ScreenConfiguration { get; }

        /// <summary>
        /// Gets the flag that indicates if fast load mode is allowed
        /// </summary>
        public bool FastTapeMode { get; set; }

        #endregion

        #region Life cycle methods

        /// <summary>
        /// Initializes the view model
        /// </summary>
        public MachineViewModel()
        {
            VmState = VmState.None;
            DisplayMode = SpectrumDisplayMode.Fit;
            ScreenConfiguration = new ScreenConfiguration();
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
            SetZoomCommand = new RelayCommand<SpectrumDisplayMode>(OnZoomSet);
            AssignTapeSetName = new RelayCommand<string>(OnAssignTapeSet);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, 
        /// releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            _vmBackgroundTaskCompletionSource = null;
            CancellationTokenSource?.Cancel();
            CancellationTokenSource?.Dispose();
        }

        #endregion

        #region Virtual machine command handlers

        /// <summary>
        /// Starts the Spectrum virtual machine
        /// </summary>
        protected virtual void OnStartVm()
        {
            if (VmState == VmState.Running)
            {
                // --- No need to start the machine, as it runs.
                return;
            }

            if (VmState == VmState.None || VmState == VmState.Stopped)
            {
                // --- Take care the machine is created
                PrepareSpectrumVmToStart();
            }

            // --- Just go on with the execution
            ContinueRun(new ExecuteCycleOptions(fastTapeMode: FastTapeMode));
        }

        /// <summary>
        /// Starts the Spectrum virtual machine and prepares
        /// it to run injected code
        /// </summary>
        private void OnStartVmWithCode()
        {
            if (VmState == VmState.Running)
            {
                // --- No need to start the machine, as it runs.
                return;
            }

            if (VmState == VmState.None || VmState == VmState.Stopped)
            {
                // --- Take care the machine is created
                PrepareSpectrumVmToStart();
            }

            // --- Just go on with the execution
            _stateAfterExecuteCycle = VmState.Paused;
            ContinueRun(new ExecuteCycleOptions(EmulationMode.UntilExecutionPoint,
                terminationPoint: SpectrumVm.RomInfo.MainExecAddress,
                fastTapeMode: FastTapeMode));
        }

        /// <summary>
        /// Pauses the Spectrum virtual machine
        /// </summary>
        protected virtual void OnPauseVm()
        {
            if (VmState != VmState.Running)
            {
                // --- The machine does not run, thus it cannot be paused
                return;
            }

            // --- Initiate stopping the machine execution cycle
            _stateAfterExecuteCycle = VmState.Paused;
            CancellationTokenSource.Cancel();
        }

        /// <summary>
        /// Stops the Spectrum virtual machine
        /// </summary>
        protected virtual void OnStopVm()
        {
            if (VmState == VmState.Running)
            {
                // --- The machine runs, we can stop it.
                _stateAfterExecuteCycle = VmState.Stopped;
                CancellationTokenSource?.Cancel();
                return;
            }

            if (VmState == VmState.Paused)
            {
                // --- The machine is paused, stopping it is easy
                VmState = VmState.Stopped;
            }
        }

        /// <summary>
        /// Resets the Spectrum virtual machine
        /// </summary>
        protected virtual void OnResetVm()
        {
            if (VmState == VmState.Paused)
            {
                // --- Let's mimic the machine is stopped
                OnStopVm();
                OnStartVm();
            }
            else
            {
                SpectrumVm.Reset();
            }
        }

        /// <summary>
        /// Starts the ZX Spectrum virtual machine in debug mode
        /// </summary>
        private void OnStartDebugVm()
        {
            if (VmState == VmState.Running)
            {
                // --- The machine runs, no need to start it again
                return;
            }

            if (VmState == VmState.None || VmState == VmState.Stopped)
            {
                PrepareSpectrumVmToStart();
            }
            GoDebugMode(DebugStepMode.StopAtBreakpoint);
        }

        /// <summary>
        /// Enters into the step-in debug mode
        /// </summary>
        private void OnStepInto()
        {
            if (VmState == VmState.Paused)
            {
                GoDebugMode(DebugStepMode.StepInto);
            }
        }

        /// <summary>
        /// Enters into the step-over debug mode
        /// </summary>
        private void OnStepOver()
        {
            if (VmState == VmState.Paused)
            {
                GoDebugMode(DebugStepMode.StepOver);
            }
        }

        /// <summary>
        /// Sets the zoom mode of the virtual machine display
        /// </summary>
        /// <param name="zoom"></param>
        protected virtual void OnZoomSet(SpectrumDisplayMode zoom)
        {
            DisplayMode = zoom;
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
        protected virtual void ContinueRun(ExecuteCycleOptions options, Action afterMachineStops = null)
        {
            // --- Dispose the previous cancellation token, and create a new one
            CancellationTokenSource?.Dispose();
            CancellationTokenSource = new CancellationTokenSource();

            // --- We use the completion source to sign that the VM's execution cycle is done
            _vmBackgroundTaskCompletionSource = new TaskCompletionSource<bool>();

            // --- It is time to run the machine asycnronously
            VmState = VmState.Running;

            // --- Set the appropriate debug mode
            RunsInDebugMode = options.EmulationMode == EmulationMode.Debugger;

            // --- Let's save the current context
            var uiContext = TaskScheduler.FromCurrentSynchronizationContext();

            // --- No exception is caught
            Exception exDuringRun = null;

            Task.Factory.StartNew(ExecutionAction,
                    CancellationTokenSource.Token,
                    TaskCreationOptions.LongRunning,
                    TaskScheduler.Current)

                // --- When execution cycle is completed, finish the task
                .ContinueWith(FinishVmExecutionCycle, null, uiContext);

            // --- Executes the machine cycle
            void ExecutionAction()
            {
                try
                {
                    SpectrumVm.ExecuteCycle(CancellationTokenSource.Token, options);
                }
                catch (TaskCanceledException)
                {
                }
                catch (Exception ex)
                {
                    exDuringRun = ex;
                }
            }

            // --- Completes the machine cycle
            void FinishVmExecutionCycle(Task task, object state)
            {
                // --- Excute the appropriate continuation
                try
                {
                    afterMachineStops?.Invoke();
                }
                catch
                {
                    // --- We ignore this exception intentionally
                }

                // --- Clean up cancellation related resources
                CancellationTokenSource?.Dispose();
                CancellationTokenSource = null;

                // --- Now, we can finally settle the new state
                VmState = exDuringRun == null
                    ? _stateAfterExecuteCycle
                    : VmState.Stopped;

                if (exDuringRun == null)
                {
                    // --- Sign that the machine is in a new state, ready to stop
                    // --- or run again
                    _vmBackgroundTaskCompletionSource?.SetResult(true);
                }
                else
                {
                    _vmBackgroundTaskCompletionSource?.SetException(exDuringRun);
                }
            }
        }

        /// <summary>
        /// Updates command state changes
        /// </summary>
        public virtual void UpdateCommandStates()
        {
            StartVmCommand.RaiseCanExecuteChanged();
            PauseVmCommand.RaiseCanExecuteChanged();
            StopVmCommand.RaiseCanExecuteChanged();
            ResetVmCommand.RaiseCanExecuteChanged();
        }

        /// <summary>
        /// Prepares the SpectrumVm to run as if it has just been turned on.
        /// </summary>
        private void PrepareSpectrumVmToStart()
        {
            if (SpectrumVm == null)
            {
                // --- Create the machine on first start
                SpectrumVm = new Spectrum48(
                    RomProvider,
                    ClockProvider,
                    KeyboardProvider,
                    ScreenFrameProvider,
                    EarBitFrameProvider,
                    LoadContentProvider,
                    SaveToTapeProvider);

            }

            // --- We either provider out DebugInfoProvider, or use
            // --- the default one
            if (DebugInfoProvider == null)
            {
                DebugInfoProvider = SpectrumVm.DebugInfoProvider;
            }
            else
            {
                SpectrumVm.DebugInfoProvider = DebugInfoProvider;
            }
            // --- Set up stack debug support
            if (StackDebugSupport != null)
            {
                SpectrumVm.Cpu.StackDebugSupport = StackDebugSupport;
                StackDebugSupport.Reset();
            }

            // --- At this point we have a Spectrum VM.
            // --- Let's reset it
            SpectrumVm.Reset();
        }

        /// <summary>
        /// Starts debug with the specified step mode
        /// </summary>
        private void GoDebugMode(DebugStepMode stepMode)
        {
            _stateAfterExecuteCycle = VmState.Paused;
            SpectrumVm.DebugInfoProvider.PrepareBreakpoints();
            ContinueRun(new ExecuteCycleOptions(EmulationMode.Debugger, stepMode), () =>
            {
                MessengerInstance.Send(new MachineDebugPausedMessage(this));
            });
        }

        #endregion
    }
}