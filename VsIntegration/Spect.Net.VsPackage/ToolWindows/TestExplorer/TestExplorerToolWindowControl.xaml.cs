using Spect.Net.VsPackage.Vsx;

namespace Spect.Net.VsPackage.ToolWindows.TestExplorer
{
    /// <summary>
    /// Interaction logic for TestExplorerToolWindowControl.xaml
    /// </summary>
    public partial class TestExplorerToolWindowControl : ISupportsMvvm<TestExplorerToolWindowViewModel>
    {
        /// <summary>
        /// Gets the view model instance
        /// </summary>
        public TestExplorerToolWindowViewModel Vm { get; private set; }

        /// <summary>
        /// Sets the view model instance
        /// </summary>
        /// <param name="vm">View model instance to set</param>
        public void SetVm(TestExplorerToolWindowViewModel vm)
        {
            DataContext = Vm = vm;
        }

        public TestExplorerToolWindowControl()
        {
            InitializeComponent();
        }
    }
}
