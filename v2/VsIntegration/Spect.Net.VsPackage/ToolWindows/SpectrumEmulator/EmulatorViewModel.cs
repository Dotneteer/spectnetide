using System;
using System.ComponentModel;
using Spect.Net.SpectrumEmu.Abstraction.Devices.Tape;
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

        /// <summary>
        /// This event fires when the virtual machine left the save mode.
        /// </summary>
        public event EventHandler<SaveModeEventArgs> LeftSaveMode;

        #endregion

        #region Life cycle methods

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
                Machine.LeftSaveMode -= OnLeftSaveMode;
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
                Machine.LeftSaveMode += OnLeftSaveMode;
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
                Machine.LeftSaveMode -= OnLeftSaveMode;
            }
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

        /// <summary>
        /// Forwards the LeftSaveMode event to subscribers
        /// </summary>
        private void OnLeftSaveMode(object sender, SaveModeEventArgs e)
        {
            LeftSaveMode?.Invoke(sender, e);
        }

        #endregion
    }
}