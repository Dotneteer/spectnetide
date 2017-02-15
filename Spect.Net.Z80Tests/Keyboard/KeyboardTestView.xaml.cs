using System.Windows;
using Spect.Net.Z80Emu.Spectrum;
using Spect.Net.Z80TestHelpers;

namespace Spect.Net.Z80Tests.Keyboard
{
    /// <summary>
    /// Interaction logic for KeyboardTestView.xaml
    /// </summary>
    public partial class KeyboardTestView
    {
        public const ushort KEY_SCAN = 0x028E;
        public KeyboardTestView()
        {
            InitializeComponent();
        }

        private void StartTestClicked(object sender, RoutedEventArgs e)
        {
            var machine = new SpectrumKeyboardTestMachine();
            //machine.KeyboardStatus.SetStatus(SpectrumKeyCode.Z, true);
            machine.KeyboardStatus.SetStatus(SpectrumKeyCode.Z, true);
            machine.CallIntoRom(KEY_SCAN);
            Registers.Bind(machine.Cpu.Registers);
        }
    }
}
