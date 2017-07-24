using GalaSoft.MvvmLight.Messaging;

namespace Spect.Net.Wpf.SpectrumControl
{
    /// <summary>
    /// This message is raised whenever the state of the Spectrum virtual 
    /// machine changes
    /// </summary>
    public class SpectrumVmStateChangedMessage: MessageBase
    {
        /// <summary>
        /// The old state of the Spectrum virtual machine
        /// </summary>
        public SpectrumVmState OldState { get; }
        
        /// <summary>
        /// The new state of the Spectrum virtual machine
        /// </summary>
        public SpectrumVmState NewState { get; }

        /// <summary>Initializes a new instance of the MessageBase class.</summary>
        public SpectrumVmStateChangedMessage(SpectrumVmState oldState, SpectrumVmState newState)
        {
            OldState = oldState;
            NewState = newState;
        }
    }
}