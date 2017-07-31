namespace Spect.Net.VsPackage.Tools.Memory
{
    /// <summary>
    /// Interaction logic for MemoryToolWindowControl.xaml
    /// </summary>
    public partial class MemoryToolWindowControl
    {
        public SpectrumMemoryViewModel Vm { get; }
        public MemoryToolWindowControl()
        {
            InitializeComponent();
            DataContext = Vm = new SpectrumMemoryViewModel();
        }
    }
}
