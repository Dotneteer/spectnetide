using GalaSoft.MvvmLight.Messaging;
using Spect.Net.VsPackage.ToolWindows.Disassembly;
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
            DataContext = Vm = vm;
        }

        /// <summary>
        /// Initializes the view
        /// </summary>
        public StackToolWindowControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Override this message to respond to screen refresh events
        /// </summary>
        protected override void OnVmScreenRefreshed()
        {
            if (Vm.ScreenRefreshCount % 10 == 0)
            {
                Vm.Refresh();
            }
        }
    }
}
