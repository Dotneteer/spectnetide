namespace Spect.Net.SpectrumEmu.Abstraction.Configuration
{
    /// <summary>
    /// This interface represents the configuration of the beeper device
    /// </summary>
    public interface IAudioConfiguration: IDeviceConfiguration
    {
        /// <summary>
        /// The audio sample rate used to generate sound wave form
        /// </summary>
        int AudioSampleRate { get; }

        /// <summary>
        /// The number of samples per ULA video frame
        /// </summary>
        int SamplesPerFrame { get; }

        /// <summary>
        /// The number of ULA tacts per audio sample
        /// </summary>
        int TactsPerSample { get; }
    }
}