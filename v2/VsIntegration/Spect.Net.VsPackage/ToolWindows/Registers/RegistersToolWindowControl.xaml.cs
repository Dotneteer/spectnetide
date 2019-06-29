using Spect.Net.VsPackage.VsxLibrary.ToolWindow;

namespace Spect.Net.VsPackage.ToolWindows.Registers
{
    /// <summary>
    /// Interaction logic for RegistersToolWindowControl.xaml
    /// </summary>
    public partial class RegistersToolWindowControl : ISupportsMvvm<RegistersToolWindowViewModel>
    {
        /// <summary>
        /// The ZX Spectrum virtual machine view model utilized by this user control
        /// </summary>
        public RegistersToolWindowViewModel Vm { get; private set; }

        /// <summary>
        /// Sets the view model instance
        /// </summary>
        /// <param name="vm">View model instance to set</param>
        void ISupportsMvvm<RegistersToolWindowViewModel>.SetVm(RegistersToolWindowViewModel vm)
        {
            DataContext = Vm = vm;
        }

        public RegistersToolWindowControl()
        {
            InitializeComponent();
        }
    }
}
