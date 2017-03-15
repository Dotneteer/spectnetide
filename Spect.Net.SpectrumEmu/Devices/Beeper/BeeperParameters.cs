namespace Spect.Net.SpectrumEmu.Devices.Beeper
{
    /// <summary>
    /// This class represents the parameters the Spectrum VM uses to render beeper
    /// and tape sound
    /// screen.
    /// </summary>
    public class BeeperParameters : IBeeperParameters
    {
        /// <summary>
        /// The audio sample rate used to generate sound wave form
        /// </summary>
        /// <remarks>
        /// One ULA frame contains 69888 Z80 clock pulses.
        /// 69888 = 2^8 * 3 * 91
        /// Using 546 samples per frame (546 = 2 * 3 *91) we have exactly
        /// 128 (2^7) Z80 tacts in each sample.
        /// </remarks>
        public int AudioSampleRate { get; }

        /// <summary>
        /// The number of samples per ULA video frame
        /// </summary>
        public int SamplesPerFrame { get; }

        /// <summary>
        /// The number of ULA tacts per audio sample
        /// </summary>
        public int UlaTactsPerSample { get; }

        /// <summary>
        /// The tact index we use to obtain a sample from the ULA tact within
        /// a sampling period
        /// </summary>
        public int SamplingOffset { get; }

        public BeeperParameters()
        {
            AudioSampleRate = 27300;
            SamplesPerFrame = 546;
            UlaTactsPerSample = 128;
            SamplingOffset = 64;
        }
    }
}