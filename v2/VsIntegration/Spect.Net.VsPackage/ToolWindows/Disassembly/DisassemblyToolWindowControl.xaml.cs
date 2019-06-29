using Spect.Net.VsPackage.VsxLibrary.ToolWindow;

namespace Spect.Net.VsPackage.ToolWindows.Disassembly
{
    /// <summary>
    /// Interaction logic for DisassemblyToolWindowControl.xaml
    /// </summary>
    public partial class DisassemblyToolWindowControl : ISupportsMvvm<DisassemblyToolWindowViewModel>
    {
        public DisassemblyToolWindowViewModel Vm { get; private set; }

        /// <summary>
        /// Sets the view model instance
        /// </summary>
        /// <param name="vm">View model instance to set</param>
        void ISupportsMvvm<DisassemblyToolWindowViewModel>.SetVm(DisassemblyToolWindowViewModel vm)
        {
            DataContext = Vm = vm;
        }

        public DisassemblyToolWindowControl()
        {
            InitializeComponent();
        }
    }
}
