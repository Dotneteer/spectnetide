using System;
using System.Threading;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using Spect.Net.SpectrumEmu.Abstraction.Devices;
using Spect.Net.SpectrumEmu.Abstraction.Providers;
using Spect.Net.SpectrumEmu.Machine;
using Spect.Net.Wpf.Mvvm;

namespace Spect.Net.Wpf.SpectrumControl
{
    /// <summary>
    /// This view model represents the view that displays a run time Spectrum virtual machine
    /// </summary>
    public class SpectrumVmViewModel: EnhancedViewModelBase, IDisposable
    {
        private SpectrumVmState _vmState;
        private SpectrumDisplayMode _displayMode;
        private string _tapeSetName;
        private bool _runsInDebugMode;

        /// <summary>
        /// The ZX Spectrum virtual machine
        /// </summary>
        public ISpectrumVm SpectrumVm { get; private set; }

        /// <summary>
        /// The current state of the virtual machine
        /// </summary>
        public SpectrumVmState VmState
        {
            get => _vmState;
            set
            {
                var oldState = _vmState;
                if (!Set(ref _vmState, value)) return;

                UpdateCommandStates();
                MessengerInstance.Send(new SpectrumVmStateChangedMessage(oldState, value));
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
                MessengerInstance.Send(new SpectrumDisplayModeChangedMessage(value));
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
        /// The cancellation token source to suspend the virtual machine
        /// </summary>
        public CancellationTokenSource CancellationTokenSource { get; private set; }

        /// <summary>
        /// This task represents the ZX Spectrum execution cycle running in the background
        /// </summary>
        public Task<bool> RunnerTask { get; private set; }

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
        /// Initializes the view model
        /// </summary>
        public SpectrumVmViewModel()
        {
            VmState = SpectrumVmState.None;
            DisplayMode = SpectrumDisplayMode.Fit;
            StartVmCommand = new RelayCommand(
                OnStartVm, 
                () => VmState != SpectrumVmState.Running);
            PauseVmCommand = new RelayCommand(
                OnPauseVm, 
                () => VmState == SpectrumVmState.Running);
            StopVmCommand = new RelayCommand(
                OnStopVm, 
                () => VmState == SpectrumVmState.Running || VmState == SpectrumVmState.Paused);
            ResetVmCommand = new RelayCommand(
                OnResetVm, 
                () => VmState == SpectrumVmState.Running || VmState == SpectrumVmState.Paused);
            StartDebugVmCommand = new RelayCommand(
                OnStartDebugVm,
                () => VmState != SpectrumVmState.Running);
            StepIntoCommand = new RelayCommand(
                OnStepInto,
                () => VmState == SpectrumVmState.Paused);
            StepOverCommand = new RelayCommand(
                OnStepOver,
                () => VmState == SpectrumVmState.Paused);
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
            ContinueRun(new ExecuteCycleOptions());
        }

        /// <summary>
        /// Pauses the Spectrum virtual machine
        /// </summary>
        protected virtual void OnPauseVm()
        {
            if (CancellationTokenSource == null) return;

            SuspendVm();
            VmState = SpectrumVmState.Paused;
            if (RunsInDebugMode)
            {
                MessengerInstance.Send(new SpectrumDebugPausedMessage(this));
            }
        }

        /// <summary>
        /// Stops the Spectrum virtual machine
        /// </summary>
        protected virtual void OnStopVm()
        {
            if (CancellationTokenSource != null)
            {
                SuspendVm();
            }
            SpectrumVm = null;
            VmState = SpectrumVmState.Stopped;
        }

        /// <summary>
        /// Resets the Spectrum virtual machine
        /// </summary>
        protected virtual void OnResetVm()
        {
            if (VmState == SpectrumVmState.Paused)
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
            if (VmState == SpectrumVmState.Paused)
            {
                GoDebugMode(DebugStepMode.StepInto);
            }
        }

        /// <summary>
        /// Enters into the step-over debug mode
        /// </summary>
        private void OnStepOver()
        {
            if (VmState == SpectrumVmState.Paused)
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
        protected virtual void ContinueRun(ExecuteCycleOptions options)
        {
            CancellationTokenSource?.Dispose();
            CancellationTokenSource = new CancellationTokenSource();
            RunnerTask = Task.Run(() => SpectrumVm.ExecuteCycle(CancellationTokenSource.Token, options), 
                CancellationTokenSource.Token);
            VmState = SpectrumVmState.Running;
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
            CancellationTokenSource?.Cancel();
            CancellationTokenSource?.Dispose();
        }

        private void EnsureVirtualMachine()
        {
            if (VmState == SpectrumVmState.None
                || VmState == SpectrumVmState.Stopped)
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
            }
        }

        /// <summary>
        /// Suspends the Spectrum virtual machine
        /// </summary>
        private void SuspendVm()
        {
            // --- Let's cancel the run of the virtual machine and allow
            // --- two frame's time to complete the run
            CancellationTokenSource.Cancel();
            Thread.Sleep(40);

            // --- Sign successful suspension
            CancellationTokenSource.Dispose();
            CancellationTokenSource = null;
            RunnerTask = null;
        }

        /// <summary>
        /// Starts debug with the specified step mode
        /// </summary>
        private void GoDebugMode(DebugStepMode stepMode)
        {
            EnsureVirtualMachine();
            RunsInDebugMode = true;
            ContinueRun(new ExecuteCycleOptions(EmulationMode.Debugger, stepMode));
            RunnerTask?.GetAwaiter().OnCompleted(() =>
            {
                VmState = SpectrumVmState.Paused;
                MessengerInstance.Send(new SpectrumDebugPausedMessage(this));
            });
        }
    }
}