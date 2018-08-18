using System;
using Microsoft.VisualStudio.Shell;
using Spect.Net.SpectrumEmu.Machine;
using Spect.Net.VsPackage.Vsx;

namespace Spect.Net.VsPackage.ToolWindows.BasicList
{
    /// <summary>
    /// Interaction logic for BasicListToolWindowControl.xaml
    /// </summary>
    public partial class BasicListToolWindowControl : ISupportsMvvm<BasicListToolWindowViewModel>
    {
        public BasicListToolWindowViewModel Vm { get; private set; }

        /// <summary>
        /// Sets the view model instance
        /// </summary>
        /// <param name="vm">View model instance to set</param>
        void ISupportsMvvm<BasicListToolWindowViewModel>.SetVm(BasicListToolWindowViewModel vm)
        {
            if (Vm != null)
            {
                Vm.MachineViewModel.VmStateChanged -= OnVmStateChanged;
            }
            DataContext = Vm = vm;
            Vm.MachineViewModel.VmStateChanged += OnVmStateChanged;
        }

        public BasicListToolWindowControl()
        {
            InitializeComponent();
        }

        private void OnVmStateChanged(object sender, VmStateChangedEventArgs args)
        {
            if (args.NewState == VmState.Running)
            {
                Vm.MachineViewModel.SpectrumVm.TapeDevice.LoadCompleted += OnLoadCompleted;
            }
            else if (args.NewState == VmState.Stopped)
            {
                Vm.MachineViewModel.SpectrumVm.TapeDevice.LoadCompleted -= OnLoadCompleted;
            }
            RefreshBasicList();
        }

        private void OnLoadCompleted(object sender, EventArgs e)
        {
            RefreshBasicList();
        }

        private void RefreshBasicList()
        {
#pragma warning disable VSTHRD001 // Avoid legacy thread switching APIs
            Dispatcher.InvokeAsync(() => Vm.RefreshBasicList());
#pragma warning restore VSTHRD001 // Avoid legacy thread switching APIs
        }
    }
}
