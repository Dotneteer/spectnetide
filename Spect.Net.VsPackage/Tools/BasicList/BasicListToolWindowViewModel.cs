using System.Linq;
using Spect.Net.SpectrumEmu.Devices.Tape;
using Spect.Net.SpectrumEmu.Machine;
using Spect.Net.SpectrumEmu.Mvvm.Messages;

namespace Spect.Net.VsPackage.Tools.BasicList
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
            MessengerInstance.Register<VmFastLoadCompletedMessage>(this, OnFastLoadCompleted);
            if (!IsInDesignMode) return;

            List = new BasicListViewModel();
        }

        /// <summary>
        /// Refresh the list after the load completed
        /// </summary>
        /// <param name="msg"></param>
        private void OnFastLoadCompleted(VmFastLoadCompletedMessage msg)
        {
            RefreshBasicList();
        }

        /// <summary>
        /// Refreshes the BASIC list
        /// </summary>
        public void RefreshBasicList()
        {
            if (VmStopped)
            {
                return;
            }
            var memory = MachineViewModel.SpectrumVm.MemoryDevice.GetMemoryBuffer();
            var prog = SystemVariables.Variables.FirstOrDefault(v => v.Name == "PROG")?.Address;
            if (prog == null) return;
            var progStart = (ushort)(memory[(ushort)prog] + memory[(ushort)(prog + 1)] * 0x100);
            var vars = SystemVariables.Variables.FirstOrDefault(v => v.Name == "VARS")?.Address;
            if (vars == null) return;
            var progEnd = (ushort)(memory[(ushort)vars] + memory[(ushort)(vars + 1)] * 0x100);
            List = new BasicListViewModel(memory, progStart, progEnd);
            List.DecodeBasicProgram();
        }

        /// <summary>
        /// Set the machnine status and notify controls
        /// </summary>
        protected override void OnVmStateChanged(MachineStateChangedMessage msg)
        {
            base.OnVmStateChanged(msg);
            RefreshBasicList();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, 
        /// or resetting unmanaged resources.
        /// </summary>
        public override void Dispose()
        {
            MessengerInstance.Unregister<VmFastLoadCompletedMessage>(this);
            base.Dispose();
        }
    }
}