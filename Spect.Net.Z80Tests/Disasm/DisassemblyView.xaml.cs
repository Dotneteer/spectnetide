using Spect.Net.Z80Tests.ViewModels;

namespace Spect.Net.Z80Tests.Disasm
{
    /// <summary>
    /// Interaction logic for DisassemblyView.xaml
    /// </summary>
    public partial class DisassemblyView
    {
        public DisassemblyView()
        {
            InitializeComponent();
            Vm = new DisassemblyViewModel();
            DataContext = Vm;
            Loaded += (sender, args) =>
            {
                Vm.DisassemblyCommand.Execute(null);
            };
        }

        public DisassemblyViewModel Vm { get; }
    }
}
