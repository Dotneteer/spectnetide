namespace Spect.Net.SpectrumEmu.Abstraction.Configuration
{
    /// <summary>
    /// This class represents the configuration of a sound device
    /// </summary>
    public class SoundConfigurationData : ISoundConfiguration
    {
        /// <summary>
        /// Base frequency of the PSG chip
        /// </summary>
        public int BaseFrequency { get; set; }

        /// <summary>
        /// The audio sample rate used to generate sound wave form
        /// </summary>
        public int AudioSampleRate { get; set; }

        /// <summary>
        /// The number of CPU tacts per audio sample
        /// </summary>
        public int TactsPerSample { get; set; }

        /// <summary>
        /// Returns a clone of this instance
        /// </summary>
        /// <returns>A clone of this instance</returns>
        public SoundConfigurationData Clone()
        {
            return new SoundConfigurationData
            {
                BaseFrequency = BaseFrequency,
                AudioSampleRate = AudioSampleRate,
                TactsPerSample = TactsPerSample
            };
        }
    }
}