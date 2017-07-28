using GalaSoft.MvvmLight.Messaging;

namespace Spect.Net.Wpf.SpectrumControl
{
    /// <summary>
    /// This message is raised whenever the current Spectrum virtual machine debug
    /// session has been paused (paused, step-in, or step-over paused)
    /// </summary>
    public class SpectrumDebugPausedMessage: MessageBase
    {
        /// <summary>
        /// The new state of the Spectrum virtual machine
        /// </summary>
        public SpectrumVmViewModel SpectrumVmViewModel { get; }

        /// <summary>Initializes a new instance of the MessageBase class.</summary>
        public SpectrumDebugPausedMessage(SpectrumVmViewModel spectrumVmViewModel)
        {
            SpectrumVmViewModel = spectrumVmViewModel;
        }
    }
}