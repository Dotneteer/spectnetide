using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;

namespace Spect.Net.VsPackage.Z80Programs.ExportDisassembly
{
    /// <summary>
    /// Interaction logic for ExportDisassemblyDialog.xaml
    /// </summary>
    public partial class ExportDisassemblyDialog
    {
        /// <summary>
        /// The view model behind this dialog
        /// </summary>
        public ExportDisassemblyViewModel Vm { get; private set; }

        /// <summary>
        /// Sets the view model of this dialog
        /// </summary>
        /// <param name="vm"></param>
        public void SetVm(ExportDisassemblyViewModel vm)
        {
            DataContext = Vm = vm;
        }

        public ExportDisassemblyDialog()
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
        /// Selects the screen file for export
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSelectFileFolderClick(object sender, RoutedEventArgs e)
        {
            // --- Get the file name
            var dialog = new OpenFileDialog
            {
                Filter = "Z80ASM files (*.z80asm)|*.z80asm",
                Title = "Select the file to export"
            };
            if (dialog.ShowDialog() != true)
            {
                return;
            }
            Vm.Filename = dialog.FileName;
        }
    }
}
