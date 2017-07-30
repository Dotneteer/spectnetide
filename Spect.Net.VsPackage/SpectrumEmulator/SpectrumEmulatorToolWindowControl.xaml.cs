using System.Reflection;
using System.Windows.Threading;
using GalaSoft.MvvmLight.Messaging;
using Spect.Net.VsPackage.Messages;
using Spect.Net.VsPackage.Vsx;
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
        /// The view model behind this control
        /// </summary>
        public SpectrumVmViewModel ViewModel { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SpectrumEmulatorToolWindowControl"/> class.
        /// </summary>
        public SpectrumEmulatorToolWindowControl()
        {
            InitializeComponent();
            DataContext = ViewModel = VsxPackage.GetPackage<SpectNetPackage>().SpectrumVmViewModel;

            // --- We need to init the SpectrumControl's providers
            SpectrumControl.SetupDefaultProviders();

            // --- We use a different LoadContentProvider
            SpectrumControl.TzxLoadContentProvider = 
                new TzxEmbeddedResourceLoadContentProvider(Assembly.GetExecutingAssembly());

            // --- We automatically start the machine when the ZX Spectrum control
            // --- is fully loaded and prepared, but not before
            Messenger.Default.Register(this, (SpectrumControlFullyLoaded msg) =>
            {
                // msg.SpectrumControl.StartVm();
            });

            // --- Prepare to handle the shutdown message
            Messenger.Default.Register(this, (PackageShutdownMessage msg) =>
            {
                Dispatcher.Invoke(() =>
                {
                    SpectrumControl.StopSound();
                },
                DispatcherPriority.Normal);
            });
        }
    }
}