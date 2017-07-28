using GalaSoft.MvvmLight.Messaging;

namespace Spect.Net.Wpf.SpectrumControl
{
    /// <summary>
    /// This message signs that a SpectrumControl instance is fully loaded.
    /// </summary>
    public class SpectrumControlFullyLoaded: MessageBase
    {
        public SpectrumDisplayControl SpectrumControl { get; }

        /// <summary>Initializes a new instance of the MessageBase class.</summary>
        public SpectrumControlFullyLoaded(SpectrumDisplayControl spectrumControl)
        {
            SpectrumControl = spectrumControl;
        }
    }
}