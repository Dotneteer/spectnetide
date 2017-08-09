namespace Spect.Net.VsPackage.Tools.Disassembly
{
    /// <summary>
    /// Interaction logic for DisassemblyToolWindowControl.xaml
    /// </summary>
    public partial class DisassemblyToolWindowControl
    {
        public DisassemblyViewModel Vm { get; }

        public DisassemblyToolWindowControl()
        {
            InitializeComponent();
            DataContext = Vm = new DisassemblyViewModel();
            Loaded += (s, e) =>
            {
                Vm.Disassemble();
            };
        }
    }
}
