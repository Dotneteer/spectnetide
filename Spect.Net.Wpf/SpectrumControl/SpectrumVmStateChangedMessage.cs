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
        /// The new state of the Spectrum virtual machine
        /// </summary>
        public SpectrumVmState State { get; }

        /// <summary>Initializes a new instance of the MessageBase class.</summary>
        public SpectrumVmStateChangedMessage(SpectrumVmState state)
        {
            State = state;
        }
    }
}