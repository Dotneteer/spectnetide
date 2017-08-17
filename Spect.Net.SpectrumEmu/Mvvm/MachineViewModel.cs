using System;
using System.Threading;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using Spect.Net.SpectrumEmu.Abstraction.Devices;
using Spect.Net.SpectrumEmu.Abstraction.Providers;
using Spect.Net.SpectrumEmu.Devices.Screen;
using Spect.Net.SpectrumEmu.Machine;
using Spect.Net.SpectrumEmu.Mvvm.Messages;

namespace Spect.Net.SpectrumEmu.Mvvm
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

        public ISpectrumDebugInfoProvider DebugInfoProvider { get; set; }

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
        public ITzxLoadContentProvider LoadContentProvider { get; set; }

        /// <summary>
        /// TZX Save provider for the tape device
        /// </summary>
        public ITzxSaveContentProvider SaveContentProvider { get; set; }

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
            SetZoomCommand = new RelayCommand<SpectrumDisplayMode>(OnZoomSet);
            AssignTapeSetName = new RelayCommand<string>(OnAssignTapeSet);
        }

        /// <summary>
        /// Starts the Spectrum virtual machine
        /// </summary>
        protected virtual void OnStartVm()
        {
            EnsureVirtualMachine();
            RunsInDebugMode = false;
            ContinueRun(new ExecuteCycleOptions(fastTapeMode: true));
        }

        /// <summary>
        /// Pauses the Spectrum virtual machine
        /// </summary>
        protected virtual void OnPauseVm()
        {
            if (CancellationTokenSource == null) return;

            // --- Initiate stopping the machine execution cycle
            CancellationTokenSource.Cancel();
            VmState = VmState.Paused;
            //if (RunsInDebugMode)
            //{
            //    MessengerInstance.Send(new MachineDebugPausedMessage(this));
            //}
        }

        /// <summary>
        /// Stops the Spectrum virtual machine
        /// </summary>
        protected virtual void OnStopVm()
        {
            CancellationTokenSource?.Cancel();
            SpectrumVm = null;
            VmState = VmState.Stopped;
        }

        /// <summary>
        /// Resets the Spectrum virtual machine
        /// </summary>
        protected virtual void OnResetVm()
        {
            if (VmState == VmState.Paused)
            {
                OnStartVm();
            }
            SpectrumVm?.Reset();
        }

        /// <summary>
        /// Starts the ZX Spectrum virtual machine in debug mode
        /// </summary>
        private void OnStartDebugVm()
        {
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

        /// <summary>
        /// Continues running the VM from the current point
        /// </summary>
        protected virtual void ContinueRun(ExecuteCycleOptions options, Action afterMachineStops = null)
        {
            CancellationTokenSource?.Dispose();
            CancellationTokenSource = new CancellationTokenSource();
            _vmBackgroundTaskCompletionSource = new TaskCompletionSource<bool>();

            Task.Run(() =>
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
                    _vmBackgroundTaskCompletionSource.SetException(ex);
                }
            }, CancellationTokenSource.Token).ContinueWith(ContinuationFunction, null);

            VmState = VmState.Running;

            void ContinuationFunction(Task t, object o)
            {
                // --- Excute the appropriate continuation
                afterMachineStops?.Invoke();

                // --- Clean up cancellation related resources
                CancellationTokenSource?.Dispose();
                CancellationTokenSource = null;

                // --- Sign that the machine is in a new state, ready to stop
                // --- or run again
                _vmBackgroundTaskCompletionSource.SetResult(true);
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
        /// Performs application-defined tasks associated with freeing, 
        /// releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            _vmBackgroundTaskCompletionSource = null;
            CancellationTokenSource?.Cancel();
            CancellationTokenSource?.Dispose();
        }

        private void EnsureVirtualMachine()
        {
            if (VmState == VmState.None
                || VmState == VmState.Stopped)
            {
                // --- In this modes we need to initialize a new instance of the Spectrum
                // --- virtual machine
                SpectrumVm = new Spectrum48(
                    RomProvider,
                    ClockProvider,
                    KeyboardProvider,
                    ScreenFrameProvider,
                    EarBitFrameProvider,
                    LoadContentProvider,
                    SaveContentProvider);

                // --- Let's reset all providers
                RomProvider?.Reset();
                ClockProvider?.Reset();
                KeyboardProvider?.Reset();
                ScreenFrameProvider?.Reset();
                EarBitFrameProvider?.Reset();
                LoadContentProvider?.Reset();
                SaveContentProvider?.Reset();

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
                DebugInfoProvider.Reset();
            }
        }

        /// <summary>
        /// Starts debug with the specified step mode
        /// </summary>
        private void GoDebugMode(DebugStepMode stepMode)
        {
            EnsureVirtualMachine();
            RunsInDebugMode = true;
            ContinueRun(new ExecuteCycleOptions(EmulationMode.Debugger, stepMode), () =>
            {
                VmState = VmState.Paused;
                MessengerInstance.Send(new MachineDebugPausedMessage(this));
            });
        }
    }
}