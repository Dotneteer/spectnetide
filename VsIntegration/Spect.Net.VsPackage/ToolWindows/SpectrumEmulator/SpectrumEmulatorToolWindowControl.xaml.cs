using System.Windows.Threading;
using Spect.Net.VsPackage.Vsx;

namespace Spect.Net.VsPackage.ToolWindows.SpectrumEmulator
{
    /// <summary>
    /// Interaction logic for SpectrumEmulatorToolWindowControl.
    /// </summary>
    public partial class SpectrumEmulatorToolWindowControl : ISupportsMvvm<SpectrumEmulatorToolWindowViewModel>
    {
        /// <summary>
        /// The view model behind this control
        /// </summary>
        public SpectrumEmulatorToolWindowViewModel Vm { get; private set; }

        /// <summary>
        /// Sets the view model instance
        /// </summary>
        /// <param name="vm">View model instance to set</param>
        void ISupportsMvvm<SpectrumEmulatorToolWindowViewModel>.SetVm(SpectrumEmulatorToolWindowViewModel vm)
        {
            DataContext = Vm = vm;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SpectrumEmulatorToolWindowViewModel"/> class.
        /// </summary>
        public SpectrumEmulatorToolWindowControl()
        {
            InitializeComponent();

            // --- Prepare to handle the shutdown message
            SpectNetPackage.Default.PackageClosing += (s, e) =>
            {
                Dispatcher.Invoke(() =>
                    {
                        SpectrumControl.Vm.SpectrumVm.BeeperProvider.KillSound();
                    },
                    DispatcherPriority.Normal);
            };
            PreviewKeyDown += (s, arg) => Vm.HandleDebugKeys(arg);
        }
    }
}