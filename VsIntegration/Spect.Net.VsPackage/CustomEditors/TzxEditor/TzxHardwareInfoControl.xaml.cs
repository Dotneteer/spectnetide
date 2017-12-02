namespace Spect.Net.VsPackage.CustomEditors.TzxEditor
{
    /// <summary>
    /// Interaction logic for TzxHardwareInfoControl.xaml
    /// </summary>
    public partial class TzxHardwareInfoControl
    {
        public TzxHardwareInfoBlockViewModel Vm { get; }

        public TzxHardwareInfoControl()
        {
            InitializeComponent();
        }

        public TzxHardwareInfoControl(TzxHardwareInfoBlockViewModel vm): this()
        {
            DataContext = Vm = vm;
        }
    }
}
