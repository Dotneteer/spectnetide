namespace Spect.Net.SpectrumEmu.Abstraction.Devices
{
    /// <summary>
    /// This interface describes a device that works with audio samples
    /// </summary>
    public interface IAudioSamplesDevice
    {
        /// <summary>
        /// Audio samples to build the audio stream
        /// </summary>
        float[] AudioSamples { get; }

        /// <summary>
        /// Index of the next audio sample
        /// </summary>
        int NextSampleIndex { get; }
    }
}