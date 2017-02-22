using System.Windows;
using Spect.Net.Z80Tests.Disasm;
using Spect.Net.Z80Tests.UserControls;

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

        private void OnRomDisassemblyClick(object sender, RoutedEventArgs e)
        {
            MainView.Content = new DisassemblyView();
        }

        private void DisplayClicked(object sender, RoutedEventArgs e)
        {
            MainView.Content = new SpectrumDisplayControl();
        }
    }
}
