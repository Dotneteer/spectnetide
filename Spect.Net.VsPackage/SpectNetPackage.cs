using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms.VisualStyles;
using Microsoft.VisualStudio.Shell;
using Spect.Net.SpectrumEmu.Abstraction.Devices;
using Spect.Net.VsPackage.SpectrumEmulator;
using Spect.Net.Wpf.Providers;
using Spect.Net.Wpf.SpectrumControl;

namespace Spect.Net.VsPackage
{
    /// <summary>
    /// This class provides a Visual Studio package for the Spect.NET IDE.
    /// </summary>
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)] // Info on this package for Help/About
    [Guid(PACKAGE_GUID_STRING)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideToolWindow(typeof(SpectrumEmulatorToolWindow), Style = VsDockStyle.MDI)]
    public sealed class SpectNetPackage : Package
    {
        /// <summary>
        /// SpectNetPackage GUID string.
        /// </summary>
        public const string PACKAGE_GUID_STRING = "1b214806-bc31-49bd-be5d-79ac4a189f3c";

        /// <summary>
        /// The ZX Spectrum virtual machine used within this package
        /// </summary>
        public SpectrumVmViewModel SpectrumVm { get; private set; }

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            // --- Initialize the commands
            SpectrumEmulatorToolWindowCommand.Initialize(this);
        }
    }
}
