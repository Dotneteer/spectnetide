namespace Spect.Net.VsPackage.ToolWindows.TapeFileExplorer
{
    /// <summary>
    /// Interaction logic for TapeFileExplorerControl.xaml
    /// </summary>
    public partial class TapeFileExplorerControl
    {
        /// <summary>
        /// The view model behind this control
        /// </summary>
        public TapeFileExplorerViewModel Vm { get; }

        public TapeFileExplorerControl()
        {
            InitializeComponent();
            DataContext = Vm = new TapeFileExplorerViewModel();
        }
    }
}
