namespace Spect.Net.VsPackage.Tools.BasicList
{
    /// <summary>
    /// Interaction logic for BasicListToolWindowControl.xaml
    /// </summary>
    public partial class BasicListToolWindowControl
    {
        public BasicListToolWindowViewModel Vm { get; }

        public BasicListToolWindowControl()
        {
            InitializeComponent();
            DataContext = Vm = new BasicListToolWindowViewModel();
        }
    }
}
