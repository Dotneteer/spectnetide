using System.Windows.Threading;
using GalaSoft.MvvmLight.Messaging;
using Spect.Net.VsPackage.Messages;

namespace Spect.Net.VsPackage.Tools.SpectrumEmulator
{
    /// <summary>
    /// Interaction logic for SpectrumEmulatorToolWindowControl.
    /// </summary>
    public partial class SpectrumEmulatorToolWindowControl
    {
        /// <summary>
        /// The view model behind this control
        /// </summary>
        public SpectrumGenericToolWindowViewModel ViewModel { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SpectrumEmulatorToolWindowControl"/> class.
        /// </summary>
        public SpectrumEmulatorToolWindowControl()
        {
            InitializeComponent();
            DataContext = ViewModel = new SpectrumGenericToolWindowViewModel();

            // --- Prepare to handle the shutdown message
            Messenger.Default.Register(this, (PackageShutdownMessage msg) =>
            {
                Dispatcher.Invoke(() =>
                {
                    SpectrumControl.Vm.EarBitFrameProvider.KillSound();
                },
                DispatcherPriority.Normal);
            });
        }
    }
}