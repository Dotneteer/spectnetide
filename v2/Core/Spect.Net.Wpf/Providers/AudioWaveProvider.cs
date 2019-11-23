using Spect.Net.SpectrumEmu.Abstraction.Configuration;
using Spect.Net.SpectrumEmu.Abstraction.Devices;
using Spect.Net.SpectrumEmu.Abstraction.Providers;
using Spect.Net.Wpf.Audio;

namespace Spect.Net.Wpf.Providers
{
    /// <summary>
    /// Type of the audio provider
    /// </summary>
    public enum AudioProviderType
    {
        Beeper = 1,
        Psg 
    }

    /// <summary>
    /// This renderer renders the ear bit pulses into an MME wave form
    /// </summary>
    public class AudioWaveProvider: VmComponentProviderBase, ISoundProvider, ISampleProvider
    {
        /// <summary>
        /// Number of sound frames buffered
        /// </summary>
        public const int FRAMES_BUFFERED = 50;
        public const int FRAMES_DELAYED = 2;

        private IAudioConfiguration _audioPars;
        private float[] _waveBuffer;
        private int _bufferLength;
        private int _frameCount;
        private long _writeIndex;
        private long _readIndex;
        private IWavePlayer _waveOut;

        /// <summary>
        /// Type of the audio provider
        /// </summary>
        public AudioProviderType Type { get; }

        /// <summary>
        /// Initializes the provider with the specified name
        /// </summary>
        public AudioWaveProvider(AudioProviderType type = AudioProviderType.Beeper)
        {
            Type = type;
        }

        /// <summary>
        /// Signs that the provider has been attached to the Spectrum virtual machine
        /// </summary>
        public override void OnAttachedToVm(ISpectrumVm hostVm)
        {
            base.OnAttachedToVm(hostVm);
            _audioPars = Type == AudioProviderType.Beeper 
                ? hostVm.AudioConfiguration : 
                hostVm.SoundConfiguration;
            WaveFormat = WaveFormat.CreateIeeeFloatWaveFormat(_audioPars.AudioSampleRate, 1);
            Reset();
        }

        /// <summary>
        /// Resets the renderer device
        /// </summary>
        public override void Reset()
        {
            try
            {
                _waveOut?.Dispose();
            }
            catch
            {
                // --- We ignore this exception deliberately
            }
            _waveOut = null;
            _bufferLength = (_audioPars.SamplesPerFrame + 1) * FRAMES_BUFFERED;
            _waveBuffer = new float[_bufferLength];
            _frameCount = 0;
            _writeIndex = 0;
            _readIndex = 0;
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
        public WaveFormat WaveFormat { get; private set; }

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
            if (_frameCount <= FRAMES_DELAYED)
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

            _waveOut.Volume = 1.0F;
            _waveOut?.Play();
            _waveOut?.Pause();
            _waveOut?.Play();
        }

        public void PauseSound()
        {
            _waveOut?.Pause();            
        }

        public void KillSound()
        {
            if (_waveOut == null) return;

            _waveOut.Volume = 0.0F;
            _waveOut.Stop();
            _waveOut.Dispose();
            _waveOut = null;
        }

        private void SetupSound()
        {
            _waveOut = new WaveOut
            {
                DesiredLatency = 100,
            };
            _waveOut.Init(this);
            _waveOut.Volume = 1.0F;
        }
    }
}