using GalaSoft.MvvmLight.Messaging;

namespace Spect.Net.SpectrumEmu.Mvvm.Messages
{
    /// <summary>
    /// This message is raised whenever the current Spectrum virtual machine debug
    /// session has been paused (paused, step-in, or step-over paused)
    /// </summary>
    public class MachineDebugPausedMessage: MessageBase
    {
        /// <summary>
        /// The new state of the Spectrum virtual machine
        /// </summary>
        public MachineViewModel MachineViewModel { get; }

        /// <summary>Initializes a new instance of the MessageBase class.</summary>
        public MachineDebugPausedMessage(MachineViewModel machineViewModel)
        {
            MachineViewModel = machineViewModel;
        }
    }
}