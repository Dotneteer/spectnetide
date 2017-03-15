using System.Collections.Generic;
using NAudio.Wave;
using Spect.Net.SpectrumEmu.Devices;
using Spect.Net.SpectrumEmu.Devices.Beeper;
using Spect.Net.SpectrumEmu.Providers;

namespace Spect.Net.Z80Tests.SpectrumHost
{
    /// <summary>
    /// This renderer renders the ear bit pulses into a wave form that can be 
    /// played through NAudio
    /// </summary>
    public class WaveEarbitPulseProcessor: IEarBitPulseProcessor, ISampleProvider

    {
        /// <summary>
        /// Number of sound frames buffered
        /// </summary>
        public const int FRAMES_BUFFERED = 10;

        /// <summary>
        /// We play the frames with a short delay to avoid lagging
        /// </summary>
        public const int FRAME_DELAY = 2;

        private readonly BeeperParameters _beeperPars;
        private readonly float[] _waveBuffer;
        private int _nextFrameIndex;
        private int _frameCount;
        private int _bufferLength;
        private ulong _writeCounter;
        private ulong _readCounter;


        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public WaveEarbitPulseProcessor(BeeperParameters beeperPars)
        {
            _beeperPars = beeperPars;
            _bufferLength = beeperPars.SamplesPerFrame * FRAMES_BUFFERED;
            _waveBuffer = new float[_bufferLength];
            _nextFrameIndex = 0;
            _frameCount = 0;
            _writeCounter = 0;
            _readCounter = 0;
            WaveFormat = WaveFormat.CreateIeeeFloatWaveFormat(beeperPars.AudioSampleRate, 1);
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
            BeeperDevice.RenderFloat(pulses, _beeperPars, _waveBuffer, _nextFrameIndex*_beeperPars.SamplesPerFrame);
            _nextFrameIndex++;
            if (_nextFrameIndex >= FRAMES_BUFFERED)
            {
                _nextFrameIndex = 0;
            }
            _frameCount++;
            _writeCounter += (ulong)_beeperPars.SamplesPerFrame;
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
                    if (_readCounter > _writeCounter)
                    {
                        buffer[offset + i] = 0F;
                    }
                    else
                    {
                        _readCounter++;
                        var readIndex = (int)(_readCounter % (ulong)_bufferLength);
                        buffer[offset + i] = _waveBuffer[readIndex];
                    }
                }
            }
            return count;
        }
    }
}