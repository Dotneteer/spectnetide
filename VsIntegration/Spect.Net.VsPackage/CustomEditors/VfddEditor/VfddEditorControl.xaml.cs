namespace Spect.Net.VsPackage.CustomEditors.VfddEditor
{
    /// <summary>
    /// Interaction logic for VfddEditorControl.xaml
    /// </summary>
    public partial class VfddEditorControl
    {
        private VfddEditorViewModel _vm;

        /// <summary>
        /// The view model that represents the ROM
        /// </summary>
        public VfddEditorViewModel Vm
        {
            get => _vm;
            set => DataContext = _vm = value;
        }

        public VfddEditorControl()
        {
            InitializeComponent();
        }
    }
}
