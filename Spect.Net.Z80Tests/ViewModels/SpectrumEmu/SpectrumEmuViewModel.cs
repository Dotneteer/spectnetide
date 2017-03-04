using System;
using System.Threading;
using GalaSoft.MvvmLight.Command;
using Spect.Net.SpectrumEmu.Keyboard;
using Spect.Net.SpectrumEmu.Machine;
using Spect.Net.SpectrumEmu.Ula;
using Spect.Net.Z80Tests.Mvvm.Navigation;

namespace Spect.Net.Z80Tests.ViewModels.SpectrumEmu
{
    public class SpectrumEmuViewModel: NavigableViewModelBase, IDisposable
    {
        private CancellationTokenSource _cancellationSource;
        private VmState _vmState;

        /// <summary>
        /// The ZX Spectrum virtual machine
        /// </summary>
        public Spectrum48 SpectrumVm { get; private set; }

        /// <summary>
        /// The current state of the virtual machine
        /// </summary>
        public VmState VmState
        {
            get { return _vmState; }
            set
            {
                Set(ref _vmState, value);
                UpdateCommandStates();
            }
        }

        /// <summary>
        /// The emulation mode to start the VM with
        /// </summary>
        public EmulationMode EmulationMode { get; set; }

        /// <summary>
        /// The debug step mode to start the VM with
        /// </summary>
        public DebugStepMode DebugStepMode { get; set; }

        /// <summary>
        /// The ROM provider to use with the VM
        /// </summary>
        public IRomProvider RomProvider { get; set; }

        /// <summary>
        /// The clock provider to use with the VM
        /// </summary>
        public IHighResolutionClockProvider ClockProvider { get; set; }

        /// <summary>
        /// The pixel renderer to use with the VM
        /// </summary>
        public IScreenPixelRenderer ScreenPixelRenderer { get; set; }

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
        /// Resets the ZS Spectrum virtual machine
        /// </summary>
        public RelayCommand ResetVmCommand { get; set; }

        /// <summary>
        /// Initializes a new instance of the ViewModelBase class.
        /// </summary>
        public SpectrumEmuViewModel()
        {
            StartVmCommand = new RelayCommand(OnStartVm, () => VmState != VmState.Running);
            PauseVmCommand = new RelayCommand(OnPauseVm, () => VmState == VmState.Running);
            StopVmCommand = new RelayCommand(OnStopVm, () => VmState == VmState.Running || VmState == VmState.Paused);
            ResetVmCommand = new RelayCommand(OnResetVm, () => VmState == VmState.Running || VmState == VmState.Paused);
            EmulationMode = EmulationMode.Continuous;
            DebugStepMode = DebugStepMode.StopAtBreakpoint;
        }

        /// <summary>
        /// Sets the status of the specified key
        /// </summary>
        /// <param name="keyCode"></param>
        /// <param name="isDown"></param>
        public void SetKeyStatus(SpectrumKeyCode keyCode, bool isDown)
        {
            SpectrumVm?.KeyboardStatus?.SetStatus(keyCode, isDown);
        }

        /// <summary>
        /// This method is called after the view model has been removed from the navigation
        /// stack.
        /// </summary>
        /// <remarks>
        /// There's no way a view model can prevent closing it. Nonetheless, the view model
        /// initiates closing itself.
        /// </remarks>
        public override void OnViewModelClosed()
        {
            base.OnViewModelClosed();
            StopVmCommand.Execute(null);
        }

        /// <summary>
        /// This method is called after the view model has been navigated away.
        /// </summary>
        /// <remarks>
        /// There's no way to prevent navigation away from the view model
        /// </remarks>
        public override void OnViewModelLeft()
        {
            base.OnViewModelLeft();
            OnStopVm();
        }

        /// <summary>
        /// Runs the virtual machine
        /// </summary>
        /// <param name="emulationMode">Emulation mode to use</param>
        /// <param name="debugStepMode">Debug step mode to use</param>
        /// <remarks>Call this method from a BackgroundWorker</remarks>
        public virtual bool RunVm(EmulationMode emulationMode, DebugStepMode debugStepMode)
        {
            return SpectrumVm.ExecuteCycle(_cancellationSource.Token, emulationMode, debugStepMode);
        }

        /// <summary>
        /// Responds to the Init command
        /// </summary>
        protected virtual void OnStartVm()
        {
            if (VmState == VmState.None || VmState == VmState.Stopped)
            {
                SpectrumVm = new Spectrum48(
                    RomProvider,
                    ClockProvider,
                    ScreenPixelRenderer);
                ScreenPixelRenderer?.Reset();
                OnVmCreated();
            }
            _cancellationSource?.Dispose();
            _cancellationSource = new CancellationTokenSource();
            VmState = VmState.Running;
            MessengerInstance.Send(new SpectrumVmPreparedToRunMessage(EmulationMode, DebugStepMode));
        }

        /// <summary>
        /// Override this method to set up the VM
        /// </summary>
        protected virtual void OnVmCreated()
        {
        }

        /// <summary>
        /// Responds to the pause command
        /// </summary>
        protected virtual void OnPauseVm()
        {
            _cancellationSource.Cancel();
            Thread.Sleep(10);
            _cancellationSource.Dispose();
            _cancellationSource = null;
            VmState = VmState.Paused;
        }

        /// <summary>
        /// Responds to the Stop command
        /// </summary>
        protected virtual void OnStopVm()
        {
            if (_cancellationSource != null)
            {
                _cancellationSource.Cancel();
                Thread.Sleep(10);
                _cancellationSource.Dispose();
                _cancellationSource = null;
            }
            SpectrumVm = null;
            VmState = VmState.Stopped;
        }

        /// <summary>
        /// Responds to the Reset command
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
        /// Performs application-defined tasks associated with freeing, releasing, or 
        /// resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            _cancellationSource?.Dispose();
        }
    }
}