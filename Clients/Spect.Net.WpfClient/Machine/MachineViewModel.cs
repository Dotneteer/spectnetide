using System;
using GalaSoft.MvvmLight.Command;
using Spect.Net.SpectrumEmu.Abstraction.Configuration;
using Spect.Net.SpectrumEmu.Abstraction.Devices;
using Spect.Net.SpectrumEmu.Abstraction.Discovery;
using Spect.Net.SpectrumEmu.Abstraction.Providers;
using Spect.Net.SpectrumEmu.Machine;
using Spect.Net.Wpf.Mvvm;
using Spect.Net.Wpf.Mvvm.Messages;

namespace Spect.Net.WpfClient.Machine
{
    /// <summary>
    /// This view model represents the view that displays a run time Spectrum virtual machine
    /// </summary>
    public class MachineViewModel: EnhancedViewModelBase, IDisposable
    {
        private VmState _vmState;
        private SpectrumDisplayMode _displayMode;
        private string _tapeSetName;
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
            }
        }


        /// <summary>
        /// The Spectrum virtual machine
        /// </summary>
        public ISpectrumVm SpectrumVm => MachineController.SpectrumVm;

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
                VmStateChanged?.Invoke(this,new VmStateChangedEventArgs(oldState, value));
            }
        }

        /// <summary>
        /// Signs that the state of the virtual machine has been changed
        /// </summary>
        public event EventHandler<VmStateChangedEventArgs> VmStateChanged;

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
        /// Gets the screen configuration
        /// </summary>
        public IScreenConfiguration ScreenConfiguration { get; set; }

        /// <summary>
        /// The name of the tapeset that is to be used with the next LOAD command
        /// </summary>
        public string TapeSetName
        {
            get => _tapeSetName;
            set
            {
                if (!Set(ref _tapeSetName, value)) return;
                if (TapeProvider != null)
                {
                    TapeProvider.TapeSetName = _tapeSetName;
                }
            }
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
        /// Device data to use
        /// </summary>
        public DeviceInfoCollection DeviceData { get; set; }

        /// <summary>
        /// TZX Save provider for the tape device
        /// </summary>
        public ITapeProvider TapeProvider { get; set; }

        /// <summary>
        /// Signs if keyboard scan is allowed or disabled
        /// </summary>
        public bool AllowKeyboardScan { get; set; }

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
            _configPrepared = false;
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
            SetZoomCommand = new RelayCommand<SpectrumDisplayMode>(OnZoomSet);
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
            _controller.StartVm(new ExecuteCycleOptions(fastTapeMode: FastTapeMode));
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
        /// Prepares the startup configuration of the machine
        /// </summary>
        private void PrepareStartupConfig()
        {
            _controller.StartupConfiguration = new MachineStartupConfiguration
            {
                DeviceData = DeviceData,
                DebugInfoProvider = DebugInfoProvider,
                StackDebugSupport = StackDebugSupport
            };
            _configPrepared = true;
        }

        /// <summary>
        /// Respond to the events when the state of the underlying controller changes
        /// </summary>
        private void OnControllerOnVmStateChanged(object sender, VmStateChangedEventArgs args)
        {
            VmState = args.NewState;
        }

        #endregion
    }
}