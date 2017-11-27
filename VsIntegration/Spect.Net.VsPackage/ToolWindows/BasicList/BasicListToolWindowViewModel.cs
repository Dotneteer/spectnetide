using Spect.Net.SpectrumEmu.Machine;

namespace Spect.Net.VsPackage.ToolWindows.BasicList
{
    /// <summary>
    /// This view model represent the ZX Spectrum BASIC List
    /// </summary>
    public class BasicListToolWindowViewModel: SpectrumGenericToolWindowViewModel
    {
        private BasicListViewModel _basicListViewModel;

        /// <summary>
        /// The view model that represents the BASIC List
        /// </summary>
        public BasicListViewModel List
        {
            get => _basicListViewModel;
            set => Set(ref _basicListViewModel, value);
        }

        /// <summary>
        /// Instantiates this view model
        /// </summary>
        public BasicListToolWindowViewModel()
        {
            if (!IsInDesignMode) return;

            List = new BasicListViewModel();
        }

        /// <summary>
        /// Refreshes the BASIC list
        /// </summary>
        public void RefreshBasicList()
        {
            if (VmStopped || MachineViewModel?.SpectrumVm == null)
            {
                return;
            }
            var memory = MachineViewModel.SpectrumVm.MemoryDevice.CloneMemory();
            var prog = SystemVariables.Get("PROG")?.Address;
            if (prog == null) return;
            var progStart = (ushort)(memory[(ushort)prog] + memory[(ushort)(prog + 1)] * 0x100);
            var vars = SystemVariables.Get("VARS")?.Address;
            if (vars == null) return;
            var progEnd = (ushort)(memory[(ushort)vars] + memory[(ushort)(vars + 1)] * 0x100);
            List = new BasicListViewModel(memory, progStart, progEnd);
            List.DecodeBasicProgram();
        }
    }
}