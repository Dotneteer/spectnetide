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
    }
}
