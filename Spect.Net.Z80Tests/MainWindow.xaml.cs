using System.Windows;
using Spect.Net.Z80Tests.Disasm;
using Spect.Net.Z80Tests.Keyboard;

namespace Spect.Net.Z80Tests
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            MainView.Content = new KeyboardTestView();
        }

        private void OnRomDisassemblyClick(object sender, RoutedEventArgs e)
        {
            MainView.Content = new DisassemblyView();
        }
    }
}
