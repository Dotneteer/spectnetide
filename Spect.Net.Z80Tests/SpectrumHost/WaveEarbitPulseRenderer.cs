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
        public const int FRAMES_BUFFERED = 10;

        /// <summary>
        /// We play the frames with a short delay to avoid lagging
        /// </summary>
        public const int FRAME_DELAY = 2;

        private readonly SoundParameters _soundPars;
        private readonly float[] _waveBuffer;
        private int _nextFrameIndex;
        private int _readIndex;
        private int _frameCount;

        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public WaveEarbitPulseRenderer(SoundParameters soundPars)
        {
            _soundPars = soundPars;
            _waveBuffer = new float[soundPars.SamplesPerFrame * FRAMES_BUFFERED];
            _nextFrameIndex = 0;
            _readIndex = 0;
            _frameCount = 0;
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
            if (_nextFrameIndex >= FRAMES_BUFFERED)
            {
                _nextFrameIndex = 0;
            }
            _frameCount++;
        }

        /// <summary>
        /// Gets the WaveFormat of this Sample Provider.
        /// </summary>
        /// <value>The wave format.</value>
        public WaveFormat WaveFormat { get; }

        /// <summary>
        /// Fill the specified buffer with 32 bit floating point samples
        /// </summary>
        /// <param name="buffer">The buffer to fill with samples.</param>
        /// <param name="offset">Offset into buffer</param>
        /// <param name="count">The number of samples to read</param>
        /// <returns>the number of samples written to the buffer.</returns>
        public int Read(float[] buffer, int offset, int count)
        {
            if (_frameCount < FRAME_DELAY)
            {
                for (var i = 0; i < count; i++)
                {
                    buffer[offset + i] = 0F;
                }
            }
            else
            {
                for (var i = 0; i < count; i++)
                {
                    buffer[offset + i] = _waveBuffer[_readIndex++];
                    if (_readIndex >= _waveBuffer.Length)
                    {
                        _readIndex = 0;
                    }
                }
            }
            return count;
        }
    }
}