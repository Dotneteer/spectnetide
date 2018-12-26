using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using Spect.Net.SpectrumEmu.Devices.Tape;

namespace Spect.Net.VsPackage.Z80Programs.Export
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

        /// <summary>
        /// Does not allow typing non-digit chars for a text box
        /// </summary>
        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
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
        private void OnSelectScreenFileClick(object sender, RoutedEventArgs e)
        {
            // --- Get the file name
            var dialog = new OpenFileDialog
            {
                Filter = "TZX files (*.tzx)|*.tzx|TAP files (*.tap)|*.tap",
                Title = "Select the screen file"
            };
            if (dialog.ShowDialog() != true)
            {
                return;
            }

            // --- Save and test for ZX Spectrum scrren file
            Vm.ScreenFile = dialog.FileName;
            if (!CommonTapeFilePlayer.CheckScreenFile(Vm.ScreenFile))
            {
                MessageBox.Show("The selected file is not a valid ZX Spectrum screen file.",
                    "Invalid Screen File", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
