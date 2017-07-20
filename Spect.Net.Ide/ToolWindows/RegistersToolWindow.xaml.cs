using Spect.Net.Ide.ViewModels;
using Spect.Net.SpectrumEmu.Cpu;

namespace Spect.Net.Ide.ToolWindows
{
    /// <summary>
    /// Interaction logic for RegistersToolWindow.xaml
    /// </summary>
    public partial class RegistersToolWindow
    {
        public RegistersToolWindow()
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
