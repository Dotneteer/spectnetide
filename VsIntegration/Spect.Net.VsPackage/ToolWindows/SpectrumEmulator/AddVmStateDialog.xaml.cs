using System.Windows;

namespace Spect.Net.VsPackage.ToolWindows.SpectrumEmulator
{
    /// <summary>
    /// Interaction logic for ExportZ80ProgramDialog.xaml
    /// </summary>
    public partial class AddVmStateDialog
    {
        /// <summary>
        /// The view model behind this dialog
        /// </summary>
        public AddVmStateViewModel Vm { get; private set; }

        /// <summary>
        /// Create the dialog
        /// </summary>
        public AddVmStateDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Sets the view model of this dialog
        /// </summary>
        /// <param name="vm"></param>
        public void SetVm(AddVmStateViewModel vm)
        {
            DataContext = Vm = vm;
        }

        /// <summary>
        /// Cancels the operation
        /// </summary>
        private void OnCancelClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        /// <summary>
        /// Initiates the export operation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnExportClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}
