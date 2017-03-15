using System;
using System.Linq;
using System.Threading;
using GalaSoft.MvvmLight.Command;
using Spect.Net.SpectrumEmu.Devices;
using Spect.Net.SpectrumEmu.Keyboard;
using Spect.Net.SpectrumEmu.Machine;
using Spect.Net.SpectrumEmu.Providers;
using Spect.Net.Z80Tests.Mvvm.Navigation;

namespace Spect.Net.Z80Tests.ViewModels.SpectrumEmu
{
    /// <summary>
    /// This view model represents the debug view
    /// </summary>
    public class SpectrumDebugViewModel: NavigableViewModelBase, IDisposable
    {
        private CancellationTokenSource _cancellationSource;
        private VmState _vmState;
        private DisassemblyViewModel _disassembly;
        private RegistersViewModel _registers;

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
        public ITzxTapeContentProvider TapeContentProvider { get; set; }

        /// <summary>
        /// The disassembly for this VM
        /// </summary>
        public DisassemblyViewModel Disassembly
        {
            get { return _disassembly; }
            set { Set(ref _disassembly, value); }
        }

        /// <summary>
        /// The view model that represents the registers of the Z80 CPU
        /// </summary>
        public RegistersViewModel Registers
        {
            get { return _registers; }
            set { Set(ref _registers, value); }
        }

        /// <summary>
        /// Provides debug information for the VM
        /// </summary>
        public DebugInfoProvider DebugInfoProvider { get; }

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
        public SpectrumDebugViewModel()
        {
            StartVmCommand = new RelayCommand(OnStartVm, () => VmState != VmState.Running);
            PauseVmCommand = new RelayCommand(OnPauseVm, () => VmState == VmState.Running);
            StopVmCommand = new RelayCommand(OnStopVm, () => VmState == VmState.Running || VmState == VmState.Paused);
            ResetVmCommand = new RelayCommand(OnResetVm, () => VmState == VmState.Running || VmState == VmState.Paused);
            StepIntoCommand = new RelayCommand(OnStepInto, () => VmState == VmState.Paused);
            StepOverCommand = new RelayCommand(OnStepOver, () => VmState == VmState.Paused);
            EmulationMode = EmulationMode.Continuous;
            DebugStepMode = DebugStepMode.StopAtBreakpoint;
            Disassembly = new DisassemblyViewModel(this);
            Registers = new RegistersViewModel();
            EmulationMode = EmulationMode.Debugger;
            DebugStepMode = DebugStepMode.StopAtBreakpoint;
            DebugInfoProvider = new DebugInfoProvider();
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
            var beforeItem = Disassembly?.DisassemblyItems
                .FirstOrDefault(di => di.Item.Address == SpectrumVm.Cpu.Registers.PC);
            var result = SpectrumVm.ExecuteCycle(_cancellationSource.Token, emulationMode, debugStepMode);
            beforeItem?.RaisePropertyChanged(nameof(DisassemblyItemViewModel.IsCurrentInstruction));
            var afterItem = Disassembly?.DisassemblyItems
                .FirstOrDefault(di => di.Item.Address == SpectrumVm.Cpu.Registers.PC);
            afterItem?.RaisePropertyChanged(nameof(DisassemblyItemViewModel.IsCurrentInstruction));
            Registers.Bind(SpectrumVm.Cpu.Registers);
            return result;
        }

        /// <summary>
        /// Responds to the Init command
        /// </summary>
        protected void OnStartVm()
        {
            if (VmState == VmState.None || VmState == VmState.Stopped)
            {
                SpectrumVm = new Spectrum48(
                    RomProvider,
                    ClockProvider,
                    ScreenPixelRenderer,
                    SoundProcessor,
                    TapeContentProvider);
                ScreenPixelRenderer?.Reset();
                SoundProcessor?.Reset();
                Disassembly.Disassemble();
                SpectrumVm.SetDebugInfoProvider(DebugInfoProvider);
            }
            EmulationMode = EmulationMode.Debugger;
            DebugStepMode = DebugStepMode.StopAtBreakpoint;
            ContinueRun();
        }

        /// <summary>
        /// Continues running the VM from the current point
        /// </summary>
        protected virtual void ContinueRun()
        {
            if (VmState == VmState.Paused)
            {
                var startDisasmItem = Disassembly.DisassemblyItems
                    .FirstOrDefault(di => di.Item.Address == SpectrumVm?.Cpu.Registers.PC);
                startDisasmItem?.RaisePropertyChanged(nameof(DisassemblyItemViewModel.IsCurrentInstruction));
            }
            DebugInfoProvider.ImminentBreakpoint = null;
            _cancellationSource?.Dispose();
            _cancellationSource = new CancellationTokenSource();
            VmState = VmState.Running;
            MessengerInstance.Send(new SpectrumVmPreparedToRunMessage(EmulationMode, DebugStepMode));
        }

        /// <summary>
        /// Responds to the pause command
        /// </summary>
        protected virtual void OnPauseVm()
        {
            if (_cancellationSource == null) return;

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
            else
            {
                SpectrumVm?.Reset();
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
            StepIntoCommand.RaiseCanExecuteChanged();
            StepOverCommand.RaiseCanExecuteChanged();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or 
        /// resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            _cancellationSource?.Dispose();
        }

        /// <summary>
        /// Executes the next instruction in Step-Into mode
        /// </summary>
        public RelayCommand StepIntoCommand { get; set; }

        /// <summary>
        /// Executes the next instruction in Step-Into mode
        /// </summary>
        public RelayCommand StepOverCommand { get; set; }

        private void OnStepInto()
        {
            EmulationMode = EmulationMode.Debugger;
            DebugStepMode = DebugStepMode.StepInto;
            ContinueRun();
        }

        private void OnStepOver()
        {
            EmulationMode = EmulationMode.Debugger;
            DebugStepMode = DebugStepMode.StepOver;
            ContinueRun();
        }
    }
}