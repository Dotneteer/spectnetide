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

            PreviewKeyDown += (sender, args) => SpectrumControl.ProcessKeyDown(args);
            PreviewKeyUp += (sender, args) => SpectrumControl.ProcessKeyUp(args);

            Application.Current.Exit += (sender, obj) => SpectrumControl.StopSound();
        }
    }
}
