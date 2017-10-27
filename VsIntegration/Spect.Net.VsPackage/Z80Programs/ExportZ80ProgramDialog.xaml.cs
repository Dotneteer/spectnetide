using System.Windows;

namespace Spect.Net.VsPackage.Z80Programs
{
    /// <summary>
    /// Interaction logic for ExportZ80ProgramDialog.xaml
    /// </summary>
    public partial class ExportZ80ProgramDialog
    {
        /// <summary>
        /// The view model behind this dialog
        /// </summary>
        public ExportZ80ProgramViewModel Vm { get; private set; }

        /// <summary>
        /// Create the dialog
        /// </summary>
        public ExportZ80ProgramDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Sets the view model of this dialog
        /// </summary>
        /// <param name="vm"></param>
        public void SetVm(ExportZ80ProgramViewModel vm)
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
