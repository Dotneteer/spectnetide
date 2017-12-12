namespace Spect.Net.VsPackage.CustomEditors.SpConfEditor
{
    /// <summary>
    /// Interaction logic for SpConfEditorControl.xaml
    /// </summary>
    public partial class SpConfEditorControl
    {
        private SpConfEditorViewModel _vm;

        /// <summary>
        /// The view model that represents the ROM
        /// </summary>
        public SpConfEditorViewModel Vm
        {
            get => _vm;
            set => DataContext = _vm = value;
        }

        public SpConfEditorControl()
        {
            InitializeComponent();
        }
    }
}
