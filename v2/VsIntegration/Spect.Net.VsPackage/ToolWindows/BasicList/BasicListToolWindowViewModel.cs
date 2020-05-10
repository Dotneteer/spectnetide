using Spect.Net.SpectrumEmu.Machine;

namespace Spect.Net.VsPackage.ToolWindows.BasicList
{
    /// <summary>
    /// This view model represent the ZX Spectrum BASIC List
    /// </summary>
    public class BasicListToolWindowViewModel : SpectrumToolWindowViewModelBase
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
        /// Should mimic ZX BASIC?
        /// </summary>
        public bool MimicZxBasic { get; set; } = false;

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
            List = CreateBasicListViewModel();
            List.MimicZxBasic = MimicZxBasic;
            List.DecodeBasicProgram();
        }

        /// <summary>
        /// Creates a view model from the current state in memory
        /// </summary>
        /// <returns>View model that represents the current state</returns>
        public BasicListViewModel CreateBasicListViewModel()
        {
            if (MachineState == VmState.Stopped || SpectrumVm == null)
            {
                return new BasicListViewModel();
            }
            var memory = SpectrumVm.MemoryDevice.CloneMemory();
            var prog = SystemVariables.Get("PROG")?.Address;
            if (prog == null) return new BasicListViewModel();
            var progStart = (ushort)(memory[(ushort)prog] + memory[(ushort)(prog + 1)] * 0x100);
            var vars = SystemVariables.Get("VARS")?.Address;
            if (vars == null) return new BasicListViewModel();
            var progEnd = (ushort)(memory[(ushort)vars] + memory[(ushort)(vars + 1)] * 0x100);
            return new BasicListViewModel(memory, progStart, progEnd);
        }
    }
}
