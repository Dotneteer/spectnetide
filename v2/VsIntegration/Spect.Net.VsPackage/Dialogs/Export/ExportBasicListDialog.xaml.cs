using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;

namespace Spect.Net.VsPackage.Dialogs.Export
{
    /// <summary>
    /// Interaction logic for ExportDisassemblyDialog.xaml
    /// </summary>
    public partial class ExportBasicListDialog
    {
        /// <summary>
        /// The view model behind this dialog
        /// </summary>
        public ExportBasicListViewModel Vm { get; private set; }

        /// <summary>
        /// Sets the view model of this dialog
        /// </summary>
        /// <param name="vm"></param>
        public void SetVm(ExportBasicListViewModel vm)
        {
            DataContext = Vm = vm;
        }

        public ExportBasicListDialog()
        {
            InitializeComponent();
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

        /// <summary>
        /// Does not allow typing non-digit chars for a text box
        /// </summary>
        private void PreviewDigitOnlyTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !ContainsOnlyDigits(e.Text);
        }

        /// <summary>
        /// Check for digit-only pattern
        /// </summary>
        private static bool ContainsOnlyDigits(string text)
        {
            var regex = new Regex("[0-9]+");
            return regex.IsMatch(text);
        }

        /// <summary>
        /// Selects the folder to export
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSelectFileFolderClick(object sender, RoutedEventArgs e)
        {
            // --- Get the file name
            var dialog = new FolderBrowserDialog
            {
                SelectedPath = ExportBasicListViewModel.LatestFolder
                    ?? "C:\\Temp",
                Description = "Select the folder to export to"
            };
            if (dialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
            {
                return;
            }

            var filename = Path.GetFileName(Vm.Filename);
            Vm.Filename = Path.Combine(dialog.SelectedPath, filename ?? "");
        }
    }
}
