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
        }
    }
}
