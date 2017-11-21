using System;
using GalaSoft.MvvmLight.Messaging;
using Spect.Net.SpectrumEmu.Machine;
using Spect.Net.VsPackage.Vsx;
using Spect.Net.Wpf.Mvvm.Messages;

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
            DataContext = Vm = vm;
        }

        public BasicListToolWindowControl()
        {
            InitializeComponent();
            Loaded += (s, e) => Messenger.Default.Register<VmStateChangedMessage>(this, OnVmStateChanged);
        }

        private void OnVmStateChanged(VmStateChangedMessage msg)
        {
            if (msg.NewState == VmState.Running)
            {
                Vm.MachineViewModel.SpectrumVm.TapeDevice.LoadCompleted += OnLoadCompleted;
            }
            else if (msg.NewState == VmState.Stopped)
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
            Dispatcher.InvokeAsync(() => Vm.RefreshBasicList());
        }
    }
}
