namespace Spect.Net.VsPackage.Tools.RegistersTool
{
    /// <summary>
    /// Interaction logic for RegistersToolWindowControl.
    /// </summary>
    public partial class RegistersToolWindowControl
    {
        /// <summary>
        /// The ZX Spectrum virtual machine view model utilized by this user control
        /// </summary>
        public Z80RegistersViewModel Vm { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RegistersToolWindowControl"/> class.
        /// </summary>
        public RegistersToolWindowControl()
        {
            InitializeComponent();
            DataContext = Vm = new Z80RegistersViewModel();
        }
    }
}