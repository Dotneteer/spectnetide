using Spect.Net.VsPackage.Vsx;

namespace Spect.Net.VsPackage.ToolWindows.CompilerOutput
{
    /// <summary>
    /// Interaction logic for AssemblerOutputToolWindowControl.xaml
    /// </summary>
    public partial class AssemblerOutputToolWindowControl : ISupportsMvvm<AssemblerOutputToolWindowViewModel>
    {
        /// <summary>
        /// The view model of Z80 Assembler output
        /// </summary>
        public AssemblerOutputToolWindowViewModel Vm { get; set; }

        /// <summary>
        /// Sets the view model instance
        /// </summary>
        /// <param name="vm">View model instance to set</param>
        public void SetVm(AssemblerOutputToolWindowViewModel vm)
        {
            DataContext = Vm = vm;
        }

        public AssemblerOutputToolWindowControl()
        {
            InitializeComponent();
        }
    }
}
