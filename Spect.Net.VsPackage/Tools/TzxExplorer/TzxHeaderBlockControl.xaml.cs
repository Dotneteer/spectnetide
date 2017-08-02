namespace Spect.Net.VsPackage.Tools.TzxExplorer
{
    /// <summary>
    /// Interaction logic for TzxHeaderBlockControl.xaml
    /// </summary>
    public partial class TzxHeaderBlockControl
    {
        public TzxHeaderBlockViewModel Vm { get; }

        public TzxHeaderBlockControl()
        {
            InitializeComponent();
        }

        public TzxHeaderBlockControl(TzxHeaderBlockViewModel vm): this()
        {
            DataContext = Vm = vm;
        }
    }
}
