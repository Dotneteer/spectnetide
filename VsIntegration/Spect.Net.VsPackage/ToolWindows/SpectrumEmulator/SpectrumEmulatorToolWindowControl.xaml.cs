using System.Windows.Threading;
using GalaSoft.MvvmLight.Messaging;
using Spect.Net.VsPackage.Vsx;

namespace Spect.Net.VsPackage.ToolWindows.SpectrumEmulator
{
    /// <summary>
    /// Interaction logic for SpectrumEmulatorToolWindowControl.
    /// </summary>
    public partial class SpectrumEmulatorToolWindowControl : ISupportsMvvm<SpectrumGenericToolWindowViewModel>
    {
        /// <summary>
        /// The view model behind this control
        /// </summary>
        public SpectrumGenericToolWindowViewModel Vm { get; private set; }

        /// <summary>
        /// Sets the view model instance
        /// </summary>
        /// <param name="vm">View model instance to set</param>
        void ISupportsMvvm<SpectrumGenericToolWindowViewModel>.SetVm(SpectrumGenericToolWindowViewModel vm)
        {
            DataContext = Vm = vm;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SpectrumEmulatorToolWindowControl"/> class.
        /// </summary>
        public SpectrumEmulatorToolWindowControl()
        {
            InitializeComponent();

            // --- Prepare to handle the shutdown message
            Messenger.Default.Register(this, (SpectNetPackage.PackageShutdownMessage msg) =>
            {
                Dispatcher.Invoke(() =>
                {
                    SpectrumControl.Vm.EarBitFrameProvider.KillSound();
                },
                DispatcherPriority.Normal);
            });
            PreviewKeyDown += (s, arg) => Vm.HandleDebugKeys(arg);
        }
    }
}