using Spect.Net.VsPackage.Vsx;

namespace Spect.Net.VsPackage.Tools.BasicList
{
    /// <summary>
    /// Interaction logic for BasicListToolWindowControl.xaml
    /// </summary>
    public partial class BasicListToolWindowControl : ISupportsMvvm<BasicListToolWindowViewModel>
    {
        public BasicListToolWindowViewModel Vm { get; private set; }

        /// <summary>
        /// Sets the view model instance
        /// </summary>
        /// <param name="vm">View model instance to set</param>
        void ISupportsMvvm<BasicListToolWindowViewModel>.SetVm(BasicListToolWindowViewModel vm)
        {
            DataContext = Vm = vm;
        }

        public BasicListToolWindowControl()
        {
            InitializeComponent();
        }
    }
}
