using System.Windows.Input;
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
        public SpectrumGenericToolWindowViewModel Vm { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SpectrumEmulatorToolWindowControl"/> class.
        /// </summary>
        public SpectrumEmulatorToolWindowControl()
        {
            InitializeComponent();
            DataContext = Vm = new SpectrumGenericToolWindowViewModel();

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

        private void OnPreviewKeyDown(object sender, KeyEventArgs args)
        {
            if (!Vm.VmPaused) return;

            if (args.Key == Key.F5 && Keyboard.Modifiers == ModifierKeys.None)
            {
                // --- Run
                Vm.MachineViewModel.StartDebugVmCommand.Execute(null);
                args.Handled = true;
                return;
            }

            if (args.Key == Key.F11 && Keyboard.Modifiers == ModifierKeys.None)
            {
                // --- Step into
                Vm.MachineViewModel.StepIntoCommand.Execute(null);
                args.Handled = true;
                return;
            }

            if (args.Key == Key.System && args.SystemKey == Key.F10 && Keyboard.Modifiers == ModifierKeys.None)
            {
                // --- Step over
                Vm.MachineViewModel.StepOverCommand.Execute(null);
                args.Handled = true;
            }
        }


    }
}