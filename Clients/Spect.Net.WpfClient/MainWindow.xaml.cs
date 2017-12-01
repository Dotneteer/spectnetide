using System.Windows;

namespace Spect.Net.WpfClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = AppViewModel.Default;

            // --- We need to stop playing sound whenever the app closes
            Application.Current.Exit += (sender, obj) => 
                SpectrumControl.Vm.SpectrumVm.BeeperProvider.KillSound();
        }
    }
}
