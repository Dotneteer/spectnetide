using Spect.Net.SpectrumEmu.Cpu;
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
        }

        public RegistersViewModel Regs { get; set; }

        public void Bind(Registers regs)
        {
            Regs.Bind(regs);
        }
    }
}
