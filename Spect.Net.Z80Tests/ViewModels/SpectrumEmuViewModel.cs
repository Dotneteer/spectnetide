using System;
using System.Threading;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Spect.Net.SpectrumEmu.Machine;
using Spect.Net.SpectrumEmu.Ula;

namespace Spect.Net.Z80Tests.ViewModels
{
    public class SpectrumEmuViewModel: ViewModelBase, IDisposable
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
        public RelayCommand InitVmCommand { get; set; }

        /// <summary>
        /// Runs the ZX Spectrum virtual machine
        /// </summary>
        public RelayCommand RunVmCommand { get; set; }

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
            InitVmCommand = new RelayCommand(OnInitVm, () => !IsVmRunning);
            RunVmCommand = new RelayCommand(OnRunVm, () => !IsVmRunning);
            StopVmCommand = new RelayCommand(OnStopVm, () => IsVmRunning);
            ResetVmCommand = new RelayCommand(OnResetVm, () => IsVmRunning);
        }

        /// <summary>
        /// Responds to the Init command
        /// </summary>
        private void OnInitVm()
        {
            SpectrumVm = new Spectrum48(
                RomProvider,
                ClockProvider,
                ScreenPixelRenderer);
            _cancellationSource?.Dispose();
            _cancellationSource = new CancellationTokenSource();
            UpdateCommandStates();
        }

        /// <summary>
        /// Responds to the Run command
        /// </summary>
        private void OnRunVm()
        {
            if (SpectrumVm == null)
            {
                return;
            }
            SpectrumVm.ExecuteCycle(_cancellationSource.Token);
            IsVmRunning = true;
            UpdateCommandStates();
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
            OnStopVm();
            OnInitVm();
        }

        /// <summary>
        /// Updates command state changes
        /// </summary>
        private void UpdateCommandStates()
        {
            InitVmCommand.RaiseCanExecuteChanged();
            RunVmCommand.RaiseCanExecuteChanged();
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