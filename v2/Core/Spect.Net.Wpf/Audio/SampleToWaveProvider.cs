using System;

namespace Spect.Net.Wpf.Audio
{
    /// <summary>
    /// Helper class for when you need to convert back to an IWaveProvider from
    /// an ISampleProvider. Keeps it as IEEE float
    /// </summary>
    public class SampleToWaveProvider : IWaveProvider
    {
        private readonly ISampleProvider _source;

        /// <summary>
        /// Initializes a new instance of the WaveProviderFloatToWaveProvider class
        /// </summary>
        /// <param name="source">Source wave provider</param>
        public SampleToWaveProvider(ISampleProvider source)
        {
            if (source.WaveFormat.Encoding != WaveFormatEncoding.IeeeFloat)
            {
                throw new ArgumentException("Must be already floating point");
            }
            _source = source;
        }

        /// <summary>
        /// Reads from this provider
        /// </summary>
        public int Read(byte[] buffer, int offset, int count)
        {
            var samplesNeeded = count / 4;
            var wb = new WaveBuffer(buffer);
            var samplesRead = _source.Read(wb.FloatBuffer, offset / 4, samplesNeeded);
            return samplesRead * 4;
        }

        /// <summary>
        /// The waveformat of this WaveProvider (same as the source)
        /// </summary>
        public WaveFormat WaveFormat => _source.WaveFormat;
    }

}