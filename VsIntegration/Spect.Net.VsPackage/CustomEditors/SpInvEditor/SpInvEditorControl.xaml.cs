namespace Spect.Net.VsPackage.CustomEditors.SpInvEditor
{
    /// <summary>
    /// Interaction logic for SpInvEditorControl.xaml
    /// </summary>
    public partial class SpInvEditorControl
    {
        private SpInvEditorViewModel _vm;

        /// <summary>
        /// The view model that represents the ROM
        /// </summary>
        public SpInvEditorViewModel Vm
        {
            get => _vm;
            set => DataContext = _vm = value;
        }

        public SpInvEditorControl()
        {
            InitializeComponent();
        }
    }
}
