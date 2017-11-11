using System.Collections.ObjectModel;
using Spect.Net.SpectrumEmu.Machine;

namespace Spect.Net.VsPackage.ToolWindows.Memory
{
    /// <summary>
    /// This view model represents the ZX Spectrum memory
    /// </summary>
    public class MemoryToolWindowViewModel: SpectrumGenericToolWindowViewModel
    {
        public ObservableCollection<MemoryLineViewModel> MemoryLines { get; } = 
            new ObservableCollection<MemoryLineViewModel>();

        /// <summary>
        /// Instantiates this view model
        /// </summary>
        public MemoryToolWindowViewModel()
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
        protected override void OnVmStateChanged(object sender, VmStateChangedEventArgs args)
        {
            if (VmRuns)
            {
                if (MachineViewModel.IsFirstStart)
                {
                    // --- We have just started the virtual machine
                    InitMemoryLines();
                }
                RefreshMemoryLines();
            }

            // --- ... or paused.
            else if (VmPaused)
            {
                MessengerInstance.Send(new RefreshMemoryViewMessage());
            }

            // --- We clear the memory contents as the virtual machine is stopped.
            else if (VmStopped)
            {
                MemoryLines.Clear();
            }
        }

        /// <summary>
        /// We refresh the memory view for every 25th screen refresh
        /// </summary>
        protected override void OnScreenRefreshed()
        {
            if (ScreenRefreshCount % 25 == 0)
            {
                MessengerInstance.Send(new RefreshMemoryViewMessage());
            }
        }

        /// <summary>
        /// Refreshes the specified memory line
        /// </summary>
        /// <param name="addr">Address of the memory line</param>
        public void RefreshMemoryLine(int addr)
        {
            var memory = VmStopped ? null : MachineViewModel?.SpectrumVm?.MemoryDevice?.GetMemoryBuffer();
            if (memory == null) return;
            var memLine = new MemoryLineViewModel(addr);
            memLine.BindTo(memory);
            var lineNo = addr >> 4;
            if (lineNo < MemoryLines.Count)
            {
                MemoryLines[addr >> 4] = memLine;
            }
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