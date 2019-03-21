namespace Spect.Net.VsPackage.CustomEditors.TzxEditor
{
    /// <summary>
    /// Interaction logic for TzxArchiveInfoControl.xaml
    /// </summary>
    public partial class TzxArchiveInfoControl
    {
        public TzxArchiveInfoViewModel Vm { get; }

        public TzxArchiveInfoControl()
        {
            InitializeComponent();
        }

        public TzxArchiveInfoControl(TzxArchiveInfoViewModel vm): this()
        {
            DataContext = Vm = vm;
        }
    }
}
