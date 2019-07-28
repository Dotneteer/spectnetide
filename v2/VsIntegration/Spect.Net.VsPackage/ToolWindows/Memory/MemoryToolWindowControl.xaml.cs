using Spect.Net.VsPackage.VsxLibrary.ToolWindow;

namespace Spect.Net.VsPackage.ToolWindows.Memory
{
    /// <summary>
    /// Interaction logic for MemoryToolWindowControl.xaml
    /// </summary>
    public partial class MemoryToolWindowControl : ISupportsMvvm<MemoryToolWindowViewModel>
    {
        public MemoryToolWindowViewModel Vm { get; private set; }

        /// <summary>
        /// Sets the view model instance
        /// </summary>
        /// <param name="vm">View model instance to set</param>
        void ISupportsMvvm<MemoryToolWindowViewModel>.SetVm(MemoryToolWindowViewModel vm)
        {
            DataContext = Vm = vm;
        }

        public MemoryToolWindowControl()
        {
            InitializeComponent();
        }
    }
}
