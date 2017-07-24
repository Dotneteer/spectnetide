using System;
using System.Threading;
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
        /// The cancellation token source to suspend the virtual machine
        /// </summary>
        public CancellationTokenSource CancellationTokenSource { get; private set; }

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
        public IScreenPixelRenderer ScreenPixelRenderer { get; set; }

        /// <summary>
        /// The renderer that creates the beeper and tape sound
        /// </summary>
        public IEarBitPulseProcessor SoundProcessor { get; set; }

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
            SetZoomCommand = new RelayCommand<SpectrumDisplayMode>(OnZoomSet);
            AssignTapeSetName = new RelayCommand<string>(OnAssignTapeSet);
        }

        /// <summary>
        /// Runs the execution cycle of the Spectrum virtual machine
        /// </summary>
        /// <returns>
        /// True, if the cycle completed; false, if it has been cancelled
        /// </returns>
        /// <remarks>
        /// This method should be invoked from a worker thread
        /// </remarks>
        public virtual bool RunVm()
        {
            return SpectrumVm.ExecuteCycle(CancellationTokenSource.Token, new ExecuteCycleOptions());
        }

        /// <summary>
        /// Starts the Spectrum virtual machine
        /// </summary>
        protected virtual void OnStartVm()
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
                    ScreenPixelRenderer,
                    SoundProcessor,
                    LoadContentProvider,
                    SaveContentProvider);

                // --- Let's reset all providers
                RomProvider?.Reset();
                ClockProvider?.Reset();
                KeyboardProvider?.Reset();
                ScreenPixelRenderer?.Reset();
                SoundProcessor?.Reset();
                LoadContentProvider?.Reset();
                SaveContentProvider?.Reset();
            }
            ContinueRun();
        }

        /// <summary>
        /// Pauses the Spectrum virtual machine
        /// </summary>
        protected virtual void OnPauseVm()
        {
            if (CancellationTokenSource == null) return;

            SuspendVm();
            VmState = SpectrumVmState.Paused;
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
        protected virtual void ContinueRun()
        {
            CancellationTokenSource?.Dispose();
            CancellationTokenSource = new CancellationTokenSource();
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
            CancellationTokenSource?.Dispose();
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
        }
    }
}