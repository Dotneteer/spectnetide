using Spect.Net.VsPackage.Vsx;

namespace Spect.Net.VsPackage.Tools.RegistersTool
{
    /// <summary>
    /// Interaction logic for RegistersToolWindowControl.
    /// </summary>
    public partial class RegistersToolWindowControl : ISupportsMvvm<Z80RegistersViewModel>
    {
        /// <summary>
        /// The ZX Spectrum virtual machine view model utilized by this user control
        /// </summary>
        public Z80RegistersViewModel Vm { get; private set; }

        /// <summary>
        /// Sets the view model instance
        /// </summary>
        /// <param name="vm">View model instance to set</param>
        void ISupportsMvvm<Z80RegistersViewModel>.SetVm(Z80RegistersViewModel vm)
        {
            DataContext = Vm = vm;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RegistersToolWindowControl"/> class.
        /// </summary>
        public RegistersToolWindowControl()
        {
            InitializeComponent();
            Loaded += (s, e) => Vm.Refresh();
        }
    }
}