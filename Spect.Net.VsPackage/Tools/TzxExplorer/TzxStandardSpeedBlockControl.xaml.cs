namespace Spect.Net.VsPackage.Tools.TzxExplorer
{
    /// <summary>
    /// Interaction logic for TzxStandardSpeedBlockControl.xaml
    /// </summary>
    public partial class TzxStandardSpeedBlockControl
    {
        public TzxStandardSpeedBlockViewModel Vm { get; }

        public TzxStandardSpeedBlockControl()
        {
            InitializeComponent();
        }

        public TzxStandardSpeedBlockControl(TzxStandardSpeedBlockViewModel vm) : this()
        {
            DataContext = Vm = vm;
        }
    }
}
