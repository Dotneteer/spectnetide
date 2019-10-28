namespace Spect.Net.VsPackage.CustomEditors.TzxEditor
{
    /// <summary>
    /// Interaction logic for TzxTextDescriptionControl.xaml
    /// </summary>
    public partial class TzxTextDescriptionControl
    {
        public TzxTextDescriptionBlockViewModel Vm { get; }

        public TzxTextDescriptionControl()
        {
            InitializeComponent();
        }

        public TzxTextDescriptionControl(TzxTextDescriptionBlockViewModel vm) : this()
        {
            DataContext = Vm = vm;
        }
    }
}
