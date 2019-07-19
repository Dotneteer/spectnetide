using System;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Spect.Net.SpectrumEmu.Machine;

namespace Spect.Net.VsPackage.ToolWindows.SpectrumEmulator
{
    public class EmulatorViewModel: ViewModelBase, IDisposable
    {
        #region ViewModel properties

        /// <summary>
        /// The Spectrum machine to use in the emulator
        /// </summary>
        public SpectrumMachine Machine { get; }

        /// <summary>
        /// The current state of the virtual machine
        /// </summary>
        public VmState MachineState => Machine.MachineState;

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

        #endregion

        #region Life cycle methods

        /// <summary>
        /// Initializes the view model
        /// </summary>
        public EmulatorViewModel()
        {
            StartVmCommand = new RelayCommand(
                OnStartVm,
                () => MachineState != VmState.Running);
            PauseVmCommand = new RelayCommand(
                OnPauseVm,
                () => MachineState == VmState.Running);
            StopVmCommand = new RelayCommand(
                OnStopVm,
                () => MachineState == VmState.Running || MachineState == VmState.Paused);
            ResetVmCommand = new RelayCommand(
                OnResetVm,
                () => MachineState == VmState.Running || MachineState == VmState.Paused);
            Machine = SpectrumMachine.CreateSpectrum48Pal();
        }

        /// <summary>
        /// Initializes the view model with the specified machine
        /// </summary>
        /// <param name="machine"></param>
        public EmulatorViewModel(SpectrumMachine machine) : this()
        {
            Machine = machine;
            Machine.VmStateChanged += OnMachineStateChanged;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, 
        /// releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Machine.VmStateChanged -= OnMachineStateChanged;
        }

        #endregion

        #region Virtual machine command handlers

        /// <summary>
        /// Starts the Spectrum virtual machine
        /// </summary>
        protected virtual void OnStartVm()
        {
            Machine.FastTapeMode = SpectNetPackage.Default.Options.UseFastLoad;
            Machine.Start();
        }

        /// <summary>
        /// Pauses the Spectrum virtual machine
        /// </summary>
        protected virtual async void OnPauseVm()
        {
            await Machine.Pause();
        }

        /// <summary>
        /// Stops the Spectrum virtual machine
        /// </summary>
        protected virtual async void OnStopVm()
        {
            await Machine.Stop();
        }

        /// <summary>
        /// Resets the Spectrum virtual machine
        /// </summary>
        protected virtual async void OnResetVm()
        {
            await Machine.Stop();
            Machine.FastTapeMode = SpectNetPackage.Default.Options.UseFastLoad;
            Machine.Start();
        }

        #endregion

        #region Helpers

        private void OnMachineStateChanged(object sender, VmStateChangedEventArgs e)
        {
            RaisePropertyChanged(nameof(MachineState));
            StartVmCommand.RaiseCanExecuteChanged();
            PauseVmCommand.RaiseCanExecuteChanged();
            StopVmCommand.RaiseCanExecuteChanged();
            ResetVmCommand.RaiseCanExecuteChanged();
        }

        #endregion

    }
}