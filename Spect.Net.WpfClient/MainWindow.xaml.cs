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

            Application.Current.Exit += (sender, obj) => SpectrumControl.StopSound();
        }
    }
}
