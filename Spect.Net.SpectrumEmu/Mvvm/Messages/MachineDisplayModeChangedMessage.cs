using GalaSoft.MvvmLight.Messaging;

namespace Spect.Net.SpectrumEmu.Mvvm.Messages
{
    /// <summary>
    /// This message is raised whenever the display mode of the Spectrum virtual 
    /// machine changes
    /// </summary>
    public class MachineDisplayModeChangedMessage: MessageBase
    {
        /// <summary>
        /// The new state of the Spectrum virtual machine
        /// </summary>
        public SpectrumDisplayMode DisplayMode { get; }

        /// <summary>Initializes a new instance of the MessageBase class.</summary>
        public MachineDisplayModeChangedMessage(SpectrumDisplayMode displayMode)
        {
            DisplayMode = displayMode;
        }
    }
}