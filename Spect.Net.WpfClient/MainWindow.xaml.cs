using System.Net.Mime;
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

            // --- We need to init the SpectrumControl's providers
            SpectrumControl.SetupDefaultProviders();
            SpectrumControl.SetupDisplay();
            SpectrumControl.SetupSound();

            Application.Current.Exit += (sender, obj) => SpectrumControl.StopSound();
        }
    }
}
