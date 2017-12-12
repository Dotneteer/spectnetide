using System;
using Spect.Net.SpectrumEmu.Machine;
using Spect.Net.VsPackage.Vsx;

namespace Spect.Net.VsPackage.ToolWindows.StackTool
{
    /// <summary>
    /// Interaction logic for StackToolWindowControl.xaml
    /// </summary>
    public partial class StackToolWindowControl : ISupportsMvvm<StackToolWindowViewModel>
    {
        /// <summary>
        /// Gets the view model instance
        /// </summary>
        public StackToolWindowViewModel Vm { get; private set; }

        /// <summary>
        /// Sets the view model instance
        /// </summary>
        /// <param name="vm">View model instance to set</param>
        void ISupportsMvvm<StackToolWindowViewModel>.SetVm(StackToolWindowViewModel vm)
        {
            if (Vm != null)
            {
                Vm.ScreenRefreshed -= OnScreenRefreshed;
                Vm.MachineViewModel.VmStateChanged -= OnVmStateChanged;
            }
            DataContext = Vm = vm;
            Vm.ScreenRefreshed += OnScreenRefreshed;
            Vm.MachineViewModel.VmStateChanged += OnVmStateChanged;
        }

        /// <summary>
        /// Initializes the view
        /// </summary>
        public StackToolWindowControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Responds to the event when the screen has been refreshed
        /// </summary>
        private void OnScreenRefreshed(object sender, EventArgs eventArgs)
        {
            if (IsControlLoaded && Vm.ScreenRefreshCount % 25 == 0)
            {
                DispatchOnUiThread(() => Vm.Refresh());
            }
        }

        /// <summary>
        /// Refrehs the stack view when the machine is paused.
        /// </summary>
        private void OnVmStateChanged(object sender, VmStateChangedEventArgs args)
        {
            if (args.NewState == VmState.Paused)
            {
                DispatchOnUiThread(() => Vm.Refresh());
            }
        }
    }
}
