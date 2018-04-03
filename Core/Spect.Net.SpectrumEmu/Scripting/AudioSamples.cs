using Spect.Net.SpectrumEmu.Abstraction.Devices;

namespace Spect.Net.SpectrumEmu.Scripting
{
    /// <summary>
    /// Represents the sound samples of the current frame
    /// </summary>
    public sealed class AudioSamples
    {
        private readonly IAudioSamplesDevice _samplesDevice;

        /// <summary>
        /// Initializes samples for a sound device
        /// </summary>
        /// <param name="samplesDevice">Device to get audio samples from</param>
        public AudioSamples(IAudioSamplesDevice samplesDevice)
        {
            _samplesDevice = samplesDevice;
        }

        /// <summary>
        /// Get the number of samples
        /// </summary>
        public int Count => _samplesDevice.AudioSamples.Length;

        /// <summary>
        /// Gets the sample with the specified index
        /// </summary>
        /// <param name="index">Sample index</param>
        /// <returns>Sample value</returns>
        public float this[int index] => _samplesDevice.AudioSamples[index];
    }
}