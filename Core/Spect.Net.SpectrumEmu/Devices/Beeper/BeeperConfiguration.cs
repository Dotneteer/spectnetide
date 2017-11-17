﻿using Spect.Net.SpectrumEmu.Abstraction.Configuration;

namespace Spect.Net.SpectrumEmu.Devices.Beeper
{
    /// <summary>
    /// This class represents the parameters the Spectrum VM uses to render beeper
    /// and tape sound
    /// screen.
    /// </summary>
    public class BeeperConfiguration: IBeeperConfiguration
    {
        /// <summary>
        /// The audio sample rate used to generate sound wave form
        /// </summary>
        public int AudioSampleRate { get; set; }

        /// <summary>
        /// The number of samples per ULA video frame
        /// </summary>
        public int SamplesPerFrame { get; set; }

        /// <summary>
        /// The number of ULA tacts per audio sample
        /// </summary>
        public int TactsPerSample { get; set; }

        public BeeperConfiguration()
        {
            // TODO: Remove this initial setup
            AudioSampleRate = 35000;
            SamplesPerFrame = 699;
            TactsPerSample = 100;
        }

        /// <summary>
        /// Returns a clone of this instance
        /// </summary>
        /// <returns>A clone of this instance</returns>
        public BeeperConfiguration Clone()
        {
            return new BeeperConfiguration
            {
                AudioSampleRate = AudioSampleRate,
                SamplesPerFrame = SamplesPerFrame,
                TactsPerSample = TactsPerSample
            };
        }
    }
}