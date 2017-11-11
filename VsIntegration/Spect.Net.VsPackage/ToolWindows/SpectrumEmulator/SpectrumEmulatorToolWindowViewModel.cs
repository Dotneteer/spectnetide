using Spect.Net.SpectrumEmu.Machine;
using Spect.Net.Wpf.Mvvm.Messages;

namespace Spect.Net.VsPackage.ToolWindows.SpectrumEmulator
{
    /// <summary>
    /// This class represents the view model of the Spectrum Emulator tool window
    /// </summary>
    public class SpectrumEmulatorToolWindowViewModel : SpectrumGenericToolWindowViewModel
    {
        /// <summary>
        /// Notifies the views listening to VmStateChangedMessage
        /// </summary>
        protected override void OnVmStateChanged(object sender, VmStateChangedEventArgs args)
        {
            MessengerInstance.Send(new VmStateChangedMessage(args.OldState, args.NewState));
        }
    }
}