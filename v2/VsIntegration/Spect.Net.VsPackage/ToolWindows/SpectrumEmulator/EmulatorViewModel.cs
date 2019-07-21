using System;
using System.ComponentModel;
using Spect.Net.SpectrumEmu.Machine;
using Spect.Net.Wpf.Mvvm;

namespace Spect.Net.VsPackage.ToolWindows.SpectrumEmulator
{
    public class EmulatorViewModel: ViewModelBase, IDisposable
    {
        #region ViewModel properties

        /// <summary>
        /// The Spectrum machine to use in the emulator
        /// </summary>
        public SpectrumMachine Machine { get; private set; }

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

        /// <summary>
        /// Starts the ZX Spectrum virtual machine in debug mode
        /// </summary>
        public RelayCommand DebugVmCommand { get; set; }

        /// <summary>
        /// Runs the ZX Spectrum virtual machine in Step-Into mode
        /// </summary>
        public RelayCommand StepIntoCommand { get; set; }

        /// <summary>
        /// Runs the ZX Spectrum virtual machine in Step-Over mode
        /// </summary>
        public RelayCommand StepOverCommand { get; set; }

        /// <summary>
        /// Runs the ZX Spectrum virtual machine in Step-Out mode
        /// </summary>
        public RelayCommand StepOutCommand { get; set; }

        /// <summary>
        /// This event is raised when the virtual machine instance has changed.
        /// </summary>
        /// <remarks>
        /// The old virtual machine is paused when this event is raised.
        /// </remarks>
        public event EventHandler<MachineInstanceChangedEventArgs> MachineInstanceChanged;

        /// <summary>
        /// This event is raised when the state of the virtual machine changes
        /// </summary>
        public event EventHandler<VmStateChangedEventArgs> VmStateChanged;

        /// <summary>
        /// This event is executed when it is time to scan the ZX Spectrum keyboard.
        /// </summary>
        public event EventHandler<KeyStatusEventArgs> KeyScanning;

        /// <summary>
        /// This event is executed whenever the CPU frame has been completed.
        /// </summary>
        public event EventHandler<CancelEventArgs> CpuFrameCompleted;

        /// <summary>
        /// This event is executed whenever the render frame has been completed.
        /// </summary>
        public event EventHandler<RenderFrameEventArgs> RenderFrameCompleted;

        /// <summary>
        /// This event is raised when a fast load operation has been completed.
        /// </summary>
        public event EventHandler FastLoadCompleted;

        /// <summary>
        /// This event fires when the virtual machine engine raised an exception.
        /// </summary>
        public event EventHandler<VmExceptionArgs> ExceptionRaised;
        
        #endregion

        #region Life cycle methods

        /// <summary>
        /// Initializes the view model
        /// </summary>
        public EmulatorViewModel()
        {
            StartVmCommand = new RelayCommand(OnStartVm);
            PauseVmCommand = new RelayCommand(OnPauseVm);
            StopVmCommand = new RelayCommand(OnStopVm);
            ResetVmCommand = new RelayCommand(OnResetVm);
            DebugVmCommand = new RelayCommand(OnDebugVm);
            StepIntoCommand = new RelayCommand(OnStepInto);
            StepOverCommand = new RelayCommand(OnStepOver);
            StepOutCommand = new RelayCommand(OnStepOut);
        }

        /// <summary>
        /// Sets the ZX Spectrum virtual machine
        /// </summary>
        /// <param name="machine">ZX Spectrum VM to use</param>
        public void SetMachine(SpectrumMachine machine)
        {
            var oldMachine = Machine;
            if (Machine != null)
            {
                Machine.VmStateChanged -= OnVmStateChanged;
                Machine.KeyScanning -= OnKeyScanning;
                Machine.CpuFrameCompleted -= OnCpuFrameCompleted;
                Machine.RenderFrameCompleted -= OnRenderFrameCompleted;
                Machine.FastLoadCompleted -= OnFastLoadCompleted;
                Machine.ExceptionRaised -= OnExceptionRaised;
            }
            Machine = machine;
            if (Machine != null)
            {
                Machine.VmStateChanged += OnVmStateChanged;
                Machine.KeyScanning += OnKeyScanning;
                Machine.CpuFrameCompleted += OnCpuFrameCompleted;
                Machine.RenderFrameCompleted += OnRenderFrameCompleted;
                Machine.FastLoadCompleted += OnFastLoadCompleted;
                Machine.ExceptionRaised += OnExceptionRaised;
            }
            MachineInstanceChanged?.Invoke(this, 
                new MachineInstanceChangedEventArgs(oldMachine, Machine));
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, 
        /// releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (Machine != null)
            {
                Machine.VmStateChanged -= OnVmStateChanged;
                Machine.KeyScanning -= OnKeyScanning;
                Machine.CpuFrameCompleted -= OnCpuFrameCompleted;
                Machine.RenderFrameCompleted -= OnRenderFrameCompleted;
                Machine.FastLoadCompleted -= OnFastLoadCompleted;
                Machine.ExceptionRaised -= OnExceptionRaised;
            }
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

        /// <summary>
        /// Starts the Spectrum virtual machine in Debug mode
        /// </summary>
        protected virtual void OnDebugVm()
        {
            Machine.FastTapeMode = SpectNetPackage.Default.Options.UseFastLoad;
            Machine.StartDebug();
        }

        /// <summary>
        /// Starts the Spectrum virtual machine in Step-Into mode
        /// </summary>
        protected virtual async void OnStepInto()
        {
            Machine.FastTapeMode = SpectNetPackage.Default.Options.UseFastLoad;
            await Machine.StepInto();
        }

        /// <summary>
        /// Starts the Spectrum virtual machine in Step-Over mode
        /// </summary>
        protected virtual async void OnStepOver()
        {
            Machine.FastTapeMode = SpectNetPackage.Default.Options.UseFastLoad;
            await Machine.StepOver();
        }

        /// <summary>
        /// Starts the Spectrum virtual machine in Step-Out mode
        /// </summary>
        protected virtual async void OnStepOut()
        {
            Machine.FastTapeMode = SpectNetPackage.Default.Options.UseFastLoad;
            await Machine.StepOut();
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Forwards the VmStateChanged event to subscribers
        /// </summary>
        private void OnVmStateChanged(object sender, VmStateChangedEventArgs e)
        {
            VmStateChanged?.Invoke(sender, e);
        }

        /// <summary>
        /// Forwards the KeyScanning event to subscribers
        /// </summary>
        private void OnKeyScanning(object sender, KeyStatusEventArgs e)
        {
            KeyScanning?.Invoke(sender, e);
        }

        /// <summary>
        /// Forwards the CpuFrameCompleted event to subscribers
        /// </summary>
        private void OnCpuFrameCompleted(object sender, CancelEventArgs e)
        {
            CpuFrameCompleted?.Invoke(sender, e);
        }

        /// <summary>
        /// Forwards the RenderFrameCompleted event to subscribers
        /// </summary>
        private void OnRenderFrameCompleted(object sender, RenderFrameEventArgs e)
        {
            RenderFrameCompleted?.Invoke(sender, e);
        }

        /// <summary>
        /// Forwards the FastLoadCompleted event to subscribers
        /// </summary>
        private void OnFastLoadCompleted(object sender, EventArgs e)
        {
            FastLoadCompleted?.Invoke(sender, e);
        }

        /// <summary>
        /// Forwards the ExceptionRaise event to subscribers
        /// </summary>
        private void OnExceptionRaised(object sender, VmExceptionArgs e)
        {
            ExceptionRaised?.Invoke(sender, e);
        }

        #endregion
    }
}