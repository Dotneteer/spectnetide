using System.Collections.ObjectModel;
using Spect.Net.Wpf.SpectrumControl;

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
        /// Set the machnine status
        /// </summary>
        protected override void OnVmStateChanged(SpectrumVmStateChangedMessage msg)
        {
            base.OnVmStateChanged(msg);

            // --- We refresh all lines whenever the machnine is newly started...
            if ((msg.OldState == SpectrumVmState.None || msg.OldState == SpectrumVmState.Stopped)
                && msg.NewState == SpectrumVmState.Running)
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
            var memory = VmStopped ? null : SpectrumVmViewModel.SpectrumVm.MemoryDevice.GetMemoryBuffer();
            var memLine = new MemoryLineViewModel(addr);
            memLine.BindTo(memory);
            MemoryLines[addr >> 4] = memLine;
        }

        /// <summary>
        /// Initializes the memory lines with empty values
        /// </summary>
        private void InitMemoryLines()
        {
            var memorySize = SpectrumVmViewModel.SpectrumVm.MemoryDevice.GetMemoryBuffer().Length;
            for (var i = 0; i < (memorySize + 1)/8; i++)
            {
                MemoryLines.Add(new MemoryLineViewModel());
            }
        }

        /// <summary>
        /// Refreshes all memory lines
        /// </summary>
        private void RefreshMemoryLines()
        {
            var memorySize = SpectrumVmViewModel.SpectrumVm.MemoryDevice.GetMemoryBuffer().Length;
            for (var addr = 0x0000; addr < memorySize + 1; addr += 16)
            {
                RefreshMemoryLine((ushort)addr);
            }
        }
    }
}