namespace Spect.Net.VsPackage.ToolWindows.TzxExplorer
{
    /// <summary>
    /// Interaction logic for TzxExplorerControl.xaml
    /// </summary>
    public partial class TzxExplorerControl
    {
        /// <summary>
        /// The view model behind this control
        /// </summary>
        public TapeFileExplorerViewModel Vm { get; }

        public TzxExplorerControl()
        {
            InitializeComponent();
            DataContext = Vm = new TapeFileExplorerViewModel();
        }
    }
}
