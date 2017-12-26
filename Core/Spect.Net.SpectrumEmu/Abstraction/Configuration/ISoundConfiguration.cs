namespace Spect.Net.SpectrumEmu.Abstraction.Configuration
{
    /// <summary>
    /// This interface represents the configuration of a sound device
    /// </summary>
    public interface ISoundConfiguration: IDeviceConfiguration
    {
        /// <summary>
        /// Base frequency of the PSG chip
        /// </summary>
        int BaseFrequency { get; }

        /// <summary>
        /// The audio sample rate used to generate sound wave form
        /// </summary>
        int AudioSampleRate { get; }

        /// <summary>
        /// The number of CPU tacts per audio sample
        /// </summary>
        int TactsPerSample { get; }
    }
}