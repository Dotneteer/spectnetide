using Spect.Net.Z80Tests.ViewModels;

namespace Spect.Net.Z80Tests.Views
{
    /// <summary>
    /// Interaction logic for DisassemblyView.xaml
    /// </summary>
    public partial class DisassemblyView
    {
        public DisassemblyViewModel Vm { get; set; }

        public DisassemblyView()
        {
            InitializeComponent();
            Vm = new DisassemblyViewModel();
            DataContext = Vm;
            Loaded += (sender, args) =>
            {
                Vm = DataContext as DisassemblyViewModel;
                Vm?.DisassemblyCommand.Execute(null);
            };
        }
    }
}
