using System.Collections.ObjectModel;
using Spect.Net.Wpf.Mvvm;
using Spect.Net.Wpf.Mvvm.Messages;

namespace Spect.Net.VsPackage.Tools.Memory
{
    /// <summary>
    /// This view model represents the ZX Spectrum memory
    /// </summary>
    public class SpectrumMemoryViewModel: SpectrumGenericToolWindowViewModel
    {
        public ObservableCollection<MemoryLineViewModel> MemoryLines { get; } = 
            new ObservableCollection<MemoryLineViewModel>();

        /// <summary>
        /// Instantiates this view model
        /// </summary>
        public SpectrumMemoryViewModel()
        {
            if (IsInDesignMode) return;

            EvaluateState();
            if (VmNotStopped)
            {
                InitMemoryLines();
                RefreshMemoryLines();
            }
        }

        /// <summary>
        /// Set the machnine status
        /// </summary>
        protected override void OnVmStateChanged(MachineStateChangedMessage msg)
        {
            base.OnVmStateChanged(msg);

            // --- We refresh all lines whenever the machnine is newly started...
            if ((msg.OldState == VmState.None || msg.OldState == VmState.Stopped)
                && msg.NewState == VmState.Running)
            {
                InitMemoryLines();
                RefreshMemoryLines();
            }
            // --- ... or paused.
            else if (VmPaused)
            {
                RefreshMemoryLines();
            }
            // --- We clear the memory contents as the virtual machine is stopped.
            else if (VmStopped)
            {
                MemoryLines.Clear();
            }
        }

        /// <summary>
        /// Refreshes the specified memory line
        /// </summary>
        /// <param name="addr">Address of the memory line</param>
        public void RefreshMemoryLine(int addr)
        {
            var memory = VmStopped ? null : MachineViewModel.SpectrumVm.MemoryDevice.GetMemoryBuffer();
            var memLine = new MemoryLineViewModel(addr);
            memLine.BindTo(memory);
            MemoryLines[addr >> 4] = memLine;
        }

        /// <summary>
        /// Initializes the memory lines with empty values
        /// </summary>
        private void InitMemoryLines()
        {
            var memorySize = MachineViewModel.SpectrumVm.MemoryDevice.GetMemoryBuffer().Length;
            for (var i = 0; i < (memorySize + 1)/16; i++)
            {
                MemoryLines.Add(new MemoryLineViewModel());
            }
        }

        /// <summary>
        /// Refreshes all memory lines
        /// </summary>
        private void RefreshMemoryLines()
        {
            var memorySize = MachineViewModel.SpectrumVm.MemoryDevice.GetMemoryBuffer().Length;
            for (var addr = 0x0000; addr < memorySize + 1; addr += 16)
            {
                RefreshMemoryLine((ushort)addr);
            }
        }
    }
}