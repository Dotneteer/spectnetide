using GalaSoft.MvvmLight.Messaging;

namespace Spect.Net.SpectrumEmu.Mvvm.Messages
{
    /// <summary>
    /// This message is raised whenever the state of the Spectrum virtual 
    /// machine changes
    /// </summary>
    public class VmStateChangedMessage: MessageBase
    {
        /// <summary>
        /// The old state of the Spectrum virtual machine
        /// </summary>
        public VmState OldState { get; }
        
        /// <summary>
        /// The new state of the Spectrum virtual machine
        /// </summary>
        public VmState NewState { get; }

        /// <summary>Initializes a new instance of the MessageBase class.</summary>
        public VmStateChangedMessage(VmState oldState, VmState newState)
        {
            OldState = oldState;
            NewState = newState;
        }
    }
}