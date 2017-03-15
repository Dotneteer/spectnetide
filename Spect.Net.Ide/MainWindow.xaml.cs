using Spect.Net.Ide.ViewModels;

namespace Spect.Net.Ide
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new IdeWorkspace();
        }
    }
}
