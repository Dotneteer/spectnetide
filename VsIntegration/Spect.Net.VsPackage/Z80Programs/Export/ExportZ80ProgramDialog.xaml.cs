using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

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

    }
}
