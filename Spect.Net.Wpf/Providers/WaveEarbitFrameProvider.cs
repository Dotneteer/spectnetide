using System;
using System.Collections.Generic;
using Spect.Net.SpectrumEmu.Abstraction.Providers;
using Spect.Net.SpectrumEmu.Devices.Beeper;
using Spect.Net.Wpf.Audio;

namespace Spect.Net.Wpf.Providers
{
    /// <summary>
    /// This renderer renders the ear bit pulses into an MME wave form
    /// </summary>
    public class WaveEarbitFrameProvider: IEarBitFrameProvider, ISampleProvider
    {
        /// <summary>
        /// Number of sound frames buffered
        /// </summary>
        public const int FRAMES_BUFFERED = 50;

        private readonly float[] _waveBuffer;
        private readonly int _bufferLength;
        private int _frameCount;
        private long _writeIndex;
        private long _readIndex;
        private IWavePlayer _waveOut;

        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public WaveEarbitFrameProvider(BeeperConfiguration beeperPars)
        {
            _bufferLength = (beeperPars.SamplesPerFrame + 1) * FRAMES_BUFFERED;
            _waveBuffer = new float[_bufferLength];
            _frameCount = 0;
            _writeIndex = 0;
            _readIndex = 0;
            WaveFormat = WaveFormat.CreateIeeeFloatWaveFormat(beeperPars.AudioSampleRate, 1);
        }

        /// <summary>
        /// Resets the renderer device
        /// </summary>
        public void Reset()
        {
            _waveOut?.Dispose();
        }

        /// <summary>
        /// Adds the specified set of pulse samples to the sound
        /// </summary>
        /// <param name="samples">
        /// Array of sound samples (values between 0.0F and 1.0F)
        /// </param>
        public void AddSoundFrame(float[] samples)
        {
            foreach (var sample in samples)
            {
                _waveBuffer[_writeIndex++] = sample;
                if (_writeIndex >= _bufferLength) _writeIndex = 0;
            }
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
            // --- We set up the initial buffer content for desired latency
            if (_frameCount == 0)
            {
                for (var i = 0; i < count; i++)
                {
                    buffer[offset++] = 0.0F;
                }
            }
            else
            {
                // --- We use the real samples
                for (var i = 0; i < count; i++)
                {
                    buffer[offset++] = _waveBuffer[_readIndex++];
                    if (_readIndex >= _bufferLength) _readIndex = 0;
                }
            }
            _frameCount++;
            return count;
        }

        public void PlaySound()
        {
            if (_waveOut == null)
            {
                SetupSound();
            }
            _waveOut?.Play();
        }

        public void PauseSound()
        {
            _waveOut?.Pause();            
        }

        public void KillSound()
        {
            _waveOut?.Dispose();
            _waveOut = null;
        }

        private void SetupSound()
        {
            _waveOut = new WaveOut
            {
                DesiredLatency = 100
            };
            _waveOut.Init(this);
        }
    }
}