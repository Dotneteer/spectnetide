using System.Windows.Controls;

namespace Spect.Net.VsPackage.CustomEditors.DisannEditor
{
    /// <summary>
    /// Interaction logic for DisAnnEditorControl.xaml
    /// </summary>
    public partial class DisAnnEditorControl
    {
        private DisAnnEditorViewModel _vm;

        /// <summary>
        /// The view model that represents the ROM
        /// </summary>
        public DisAnnEditorViewModel Vm
        {
            get => _vm;
            set => DataContext = _vm = value;
        }

        public DisAnnEditorControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Display the selected bank's contents
        /// </summary>
        private void OnSelectedBankChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!(BanksList.SelectedItem is int selectedIndex)) return;
            if (Vm.Annotations.TryGetValue(selectedIndex, out var item))
            {
                Vm.SelectedBank = item;
            }
        }
    }
}
