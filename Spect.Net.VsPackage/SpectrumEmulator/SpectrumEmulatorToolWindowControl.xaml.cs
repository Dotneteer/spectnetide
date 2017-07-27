using System.Reflection;
using Spect.Net.Wpf.Providers;
using Spect.Net.Wpf.SpectrumControl;

namespace Spect.Net.VsPackage.SpectrumEmulator
{
    /// <summary>
    /// Interaction logic for SpectrumEmulatorToolWindowControl.
    /// </summary>
    public partial class SpectrumEmulatorToolWindowControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SpectrumEmulatorToolWindowControl"/> class.
        /// </summary>
        public SpectrumEmulatorToolWindowControl()
        {
            InitializeComponent();
            DataContext = new SpectrumVmViewModel();

            // --- We need to init the SpectrumControl's providers
            SpectrumControl.SetupDefaultProviders();

            // --- We use a different LoadContentProvider
            SpectrumControl.TzxLoadContentProvider = 
                new TzxEmbeddedResourceLoadContentProvider(Assembly.GetExecutingAssembly());

            SpectrumControl.SetupDisplay();
            SpectrumControl.SetupSound();
        }
    }
}