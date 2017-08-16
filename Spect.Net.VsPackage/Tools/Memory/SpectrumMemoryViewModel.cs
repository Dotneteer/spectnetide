using System.Collections.ObjectModel;
using Spect.Net.Wpf.SpectrumControl;

namespace Spect.Net.VsPackage.Tools.Memory
{
    /// <summary>
    /// This view model represents the ZX Spectrum memory
    /// </summary>
    public class SpectrumMemoryViewModel: SpectrumGenericToolWindowViewModel
    {
        public ObservableCollection<MemoryLineViewModel> MemoryLines { get; } = new ObservableCollection<MemoryLineViewModel>();

        /// <summary>
        /// Instantiates this view model
        /// </summary>
        public SpectrumMemoryViewModel()
        {
            if (IsInDesignMode) return;

            InitMemoryLines();
            RefreshMemoryLines();
        }

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
                RefreshMemoryLines();
            }
            // --- ... stopped, or paused
            else if (VmStopped || VmPaused)
            {
                RefreshMemoryLines();
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

        private void InitMemoryLines()
        {
            for (var i = 0; i < 4096; i++)
            {
                MemoryLines.Add(new MemoryLineViewModel());
            }
        }

        /// <summary>
        /// Refreshes all memory lines
        /// </summary>
        public void RefreshMemoryLines()
        {
            for (var addr = 0x0000; addr < 0x10000; addr += 16)
            {
                RefreshMemoryLine((ushort)addr);
            }
        }
    }
}