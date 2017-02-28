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
        private bool _isVmRunning;
        private CancellationTokenSource _cancellationSource;

        /// <summary>
        /// The ZX Spectrum virtual machine
        /// </summary>
        public Spectrum48 SpectrumVm { get; private set; }

        /// <summary>
        /// Tests if the VM is currently running
        /// </summary>
        public bool IsVmRunning
        {
            get { return _isVmRunning; }
            set { Set(ref _isVmRunning, value); }
        }

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
            StartVmCommand = new RelayCommand(OnStartVm, () => !IsVmRunning);
            StopVmCommand = new RelayCommand(OnStopVm, () => IsVmRunning);
            ResetVmCommand = new RelayCommand(OnResetVm, () => IsVmRunning);
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
        /// <remarks>Call this method from a BackgroundWorker</remarks>
        public void RunVm()
        {
            SpectrumVm.ExecuteCycle(_cancellationSource.Token);
        }

        /// <summary>
        /// Responds to the Init command
        /// </summary>
        private void OnStartVm()
        {
            SpectrumVm = new Spectrum48(
                RomProvider,
                ClockProvider,
                ScreenPixelRenderer);
            _cancellationSource?.Dispose();
            _cancellationSource = new CancellationTokenSource();
            UpdateCommandStates();
            IsVmRunning = true;
            MessengerInstance.Send(new SpectrumVmInitializedMessage());
        }

        /// <summary>
        /// Responds to the Stop command
        /// </summary>
        private void OnStopVm()
        {
            if (SpectrumVm == null )
            {
                return;
            }
            _cancellationSource.Cancel();
            Thread.Sleep(10);
            _cancellationSource.Dispose();
            SpectrumVm = null;
            IsVmRunning = false;
            UpdateCommandStates();
        }

        /// <summary>
        /// Responds to the Reset command
        /// </summary>
        private void OnResetVm()
        {
            SpectrumVm?.Reset();
            UpdateCommandStates();
        }

        /// <summary>
        /// Updates command state changes
        /// </summary>
        public void UpdateCommandStates()
        {
            StartVmCommand.RaiseCanExecuteChanged();
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