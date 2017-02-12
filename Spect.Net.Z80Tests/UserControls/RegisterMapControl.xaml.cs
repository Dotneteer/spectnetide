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
                Regs.DE = 0xEEEE;
                Regs.HL = 0x88CC;
                Regs.PC = 0x0000;
                Regs.SP = 0xFFFF;
                Regs.IX = 0x8000;
                Regs.IY = 0xC000;
            };
        }

        public RegistersViewModel Regs { get; set; }
    }
}
