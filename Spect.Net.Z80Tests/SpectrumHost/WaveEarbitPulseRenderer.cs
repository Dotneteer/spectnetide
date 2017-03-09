using System.Collections.Generic;
using NAudio.Wave;
using Spect.Net.SpectrumEmu.Devices;

namespace Spect.Net.Z80Tests.SpectrumHost
{
    /// <summary>
    /// This renderer renders the ear bit pulses into a wave form that can be 
    /// played through NAudio
    /// </summary>
    public class WaveEarbitPulseRenderer: IEarBitPulseRenderer, ISampleProvider

    {
        /// <summary>
        /// Number of sound frames buffered
        /// </summary>
        public const int FRAMES_BUFFRERED = 10;

        /// <summary>
        /// We play the frames with a short delay to avoid lagging
        /// </summary>
        public const int FRAME_DELAY = 2;

        private SoundParameters _soundPars;
        private readonly float[] _waveBuffer;
        private int _nextFrameIndex;
        private int _readIndex;

        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public WaveEarbitPulseRenderer(SoundParameters soundPars)
        {
            _soundPars = soundPars;
            _waveBuffer = new float[soundPars.SamplesPerFrame * FRAMES_BUFFRERED];
            _nextFrameIndex = 0;
            _readIndex = 0;
            WaveFormat = WaveFormat.CreateIeeeFloatWaveFormat(soundPars.AudioSampleRate, 1);
        }

        /// <summary>
        /// Resets the renderer device
        /// </summary>
        public void Reset()
        {
        }

        /// <summary>
        /// Adds the specified set of pulses to the sound
        /// </summary>
        /// <param name="pulses"></param>
        public void AddSoundFrame(IList<EarBitPulse> pulses)
        {
            BeeperDevice.RenderFloat(pulses, _soundPars, _waveBuffer, _nextFrameIndex*_soundPars.SamplesPerFrame);
            _nextFrameIndex++;
            if (_nextFrameIndex >= FRAMES_BUFFRERED)
            {
                _nextFrameIndex = 0;
            }
        }

        /// <summary>
        /// Gets the WaveFormat of this Sample Provider.
        /// </summary>
        /// <value>The wave format.</value>
        public WaveFormat WaveFormat { get; }

        private int _val;

        /// <summary>
        /// Fill the specified buffer with 32 bit floating point samples
        /// </summary>
        /// <param name="buffer">The buffer to fill with samples.</param>
        /// <param name="offset">Offset into buffer</param>
        /// <param name="count">The number of samples to read</param>
        /// <returns>the number of samples written to the buffer.</returns>
        public int Read(float[] buffer, int offset, int count)
        {
            for (var i = 0; i < count; i++)
            {
                buffer[offset + i] = _val % 80 < 40 ? 0F : 0.5F;
                _val++;
            }
            return count;
        }
    }
}