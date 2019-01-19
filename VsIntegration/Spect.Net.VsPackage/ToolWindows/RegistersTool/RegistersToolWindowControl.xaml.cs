using Spect.Net.VsPackage.Vsx;

namespace Spect.Net.VsPackage.ToolWindows.RegistersTool
{
    /// <summary>
    /// Interaction logic for RegistersToolWindowControl.
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

        /// <summary>
        /// Initializes a new instance of the <see cref="RegistersToolWindowControl"/> class.
        /// </summary>
        public RegistersToolWindowControl()
        {
            InitializeComponent();
            Loaded += (s, e) => { Vm.Refresh(); };
            PreviewKeyDown += (s, arg) =>
            {
                Vm?.HandleDebugKeys(arg);
            };
        }
    }
}