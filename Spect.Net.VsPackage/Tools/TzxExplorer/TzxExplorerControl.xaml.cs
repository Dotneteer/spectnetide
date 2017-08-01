namespace Spect.Net.VsPackage.Tools.TzxExplorer
{
    /// <summary>
    /// Interaction logic for TzxExplorerControl.xaml
    /// </summary>
    public partial class TzxExplorerControl
    {
        /// <summary>
        /// The view model behind this control
        /// </summary>
        public TzxExplorerViewModel Vm { get; }

        public TzxExplorerControl()
        {
            InitializeComponent();
            DataContext = Vm = new TzxExplorerViewModel();
        }

        /// <summary>
        /// Displays the selected TZX block information
        /// </summary>
        /// <remarks>
        /// Instead of handling this event, the event should be bound to the view model's 
        /// BlockSelectedCommand.
        /// </remarks>
        private void ListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            Vm.BlockSelectedCommand.Execute(null);
        }
    }
}
