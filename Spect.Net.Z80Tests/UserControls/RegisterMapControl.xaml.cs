using Spect.Net.Z80Tests.ViewModels;

namespace Spect.Net.Z80Tests.UserControls
{
    /// <summary>
    /// Interaction logic for RegisterMapControl.xaml
    /// </summary>
    public partial class RegisterMapControl
    {
        public RegisterMapControl()
        {
            InitializeComponent();
            DataContext = Regs = new RegistersViewModel();
            Loaded += (sender, args) =>
            {
                Regs.AF = 0xAACC;
                Regs.BC = 0x5555;
            };
        }

        public RegistersViewModel Regs { get; set; }
    }
}
