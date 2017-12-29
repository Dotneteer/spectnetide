using System;
using Spect.Net.SpectrumEmu.Abstraction.Configuration;
using Spect.Net.SpectrumEmu.Abstraction.Devices;
using Spect.Net.SpectrumEmu.Abstraction.Providers;

#pragma warning disable 67

namespace Spect.Net.SpectrumEmu.Devices.Sound
{
    public class SoundDevice: ISoundDevice
    {
        private ISoundProvider _soundProvider;
        private IAudioConfiguration _soundConfiguration;
        private long _frameBegins;
        private int _frameTacts;
        private int _tactsPerSample;

        /// <summary>
        /// The virtual machine that hosts the device
        /// </summary>
        public ISpectrumVm HostVm { get; private set; }

        /// <summary>
        /// Audio samples to build the audio stream
        /// </summary>
        public float[] AudioSamples { get; set; }

        /// <summary>
        /// Index of the next audio sample
        /// </summary>
        public int SamplesIndex { get; private set; }

        /// <summary>
        /// Signs that the device has been attached to the Spectrum virtual machine
        /// </summary>
        public void OnAttachedToVm(ISpectrumVm hostVm)
        {
            HostVm = hostVm;
            _soundConfiguration = hostVm.SoundConfiguration;
            _soundProvider = hostVm.SoundProvider;
            _frameTacts = hostVm.FrameTacts;
            _tactsPerSample = _soundConfiguration.TactsPerSample;
            Reset();
        }

        /// <summary>
        /// Resets this device
        /// </summary>
        public void Reset()
        {
            _frameBegins = HostVm.Cpu.Tacts;
            LastRegisterIndex = 0;
            PsgState = new PsgState(HostVm);
            for (var i = 0; i < 0x0F; i++)
            {
                PsgState[i] = 0;
            }
            FrameCount = 0;
            Overflow = 0;
            SetRegisterValue(0);
            _soundProvider?.Reset();
            InitializeSampling();
        }

        /// <summary>
        /// The last PSG state collected during the last frame
        /// </summary>
        public PsgState PsgState { get; private set; }

        /// <summary>
        /// The offset of the last recorded sample
        /// </summary>
        public long LastSampleTact { get; set; }

        /// <summary>
        /// #of frames rendered
        /// </summary>
        public int FrameCount { get; private set; }

        /// <summary>
        /// Overflow from the previous frame, given in #of tacts 
        /// </summary>
        public int Overflow { get; set; }

        /// <summary>
        /// Allow the device to react to the start of a new frame
        /// </summary>
        public void OnNewFrame()
        {
            FrameCount++;
            InitializeSampling();

            if (Overflow != 0)
            {
                // --- Managed overflown samples
                CreateSamples(_frameBegins + Overflow);
            }
            Overflow = 0;
        }

        /// <summary>
        /// Allow the device to react to the completion of a frame
        /// </summary>
        public void OnFrameCompleted()
        {
            if (LastSampleTact < _frameBegins + _frameTacts)
            {
                // --- Expand the samples till the end of the frame
                CreateSamples(_frameBegins + _frameTacts);
            }
            if (HostVm.Cpu.Tacts > _frameBegins + _frameTacts)
            {
                // --- Sign overflow tacts
                Overflow = (int)(HostVm.Cpu.Tacts - _frameBegins - _frameTacts);
            }
            _soundProvider?.AddSoundFrame(AudioSamples);
            _frameBegins += _frameTacts;
        }

        /// <summary>
        /// Allow external entities respond to frame completion
        /// </summary>
        public event EventHandler FrameCompleted;

        /// <summary>
        /// The index of the last addressed register
        /// </summary>
        public byte LastRegisterIndex { get; private set; }

        /// <summary>
        /// Sets the index of the PSG register
        /// </summary>
        /// <param name="index">Register index</param>
        public void SetRegisterIndex(byte index)
        {
            LastRegisterIndex = index;
        }

        /// <summary>
        /// Sets the value of the register according to the
        /// last register index
        /// </summary>
        /// <param name="value">Register value</param>
        public void SetRegisterValue(byte value)
        {
            CreateSamples(HostVm.Cpu.Tacts);
            PsgState[LastRegisterIndex] = value;
        }

        /// <summary>
        /// Gets the value of the register according to the
        /// last register index
        /// </summary>
        /// <returns>Register value</returns>
        public byte GetRegisterValue()
        {
            return PsgState[LastRegisterIndex];
        }

        private void InitializeSampling()
        {
            LastSampleTact = _frameBegins % _tactsPerSample == 0
                ? _frameBegins
                : _frameBegins + _tactsPerSample - (_frameBegins + _tactsPerSample) % _tactsPerSample;
            var samplesInFrame = (_frameBegins + _frameTacts - LastSampleTact - 1) / _tactsPerSample + 1;

            // --- Empty the samples array
            AudioSamples = new float[samplesInFrame];
            SamplesIndex = 0;
        }

        /// <summary>
        /// Create samples according the current PSG state
        /// </summary>
        /// <param name="cpuTacts"></param>
        private void CreateSamples(long cpuTacts)
        {
            var nextSampleOffset = LastSampleTact;
            if (cpuTacts > _frameBegins + _frameTacts)
            {
                cpuTacts = _frameBegins + _frameTacts;
            }
            while (nextSampleOffset < cpuTacts)
            {
                AudioSamples[SamplesIndex++] = CreateSampleFor(nextSampleOffset);
                nextSampleOffset += _tactsPerSample;
            }
            LastSampleTact = nextSampleOffset;

        }

        /// <summary>
        /// Create the PSG sample for the specified tact
        /// </summary>
        /// <param name="tact">CPU tact to create the sample for</param>
        /// <returns></returns>
        private float CreateSampleFor(long tact)
        {
            var noise = PsgState.GetNoiseSample(tact);
            var channelA = PsgState.GetChannelASample(tact) | (PsgState.NoiseAEnabled && noise) 
                ? PsgState.GetAplitudeA(tact) : 0.0f;
            var channelB = PsgState.GetChannelBSample(tact) | (PsgState.NoiseBEnabled && noise) 
                ? PsgState.GetAplitudeB(tact) : 0.0f;
            var channelC = PsgState.GetChannelCSample(tact) | (PsgState.NoiseCEnabled && noise) 
                ? PsgState.GetAplitudeC(tact) : 0.0f;
            return (channelA + channelB + channelC) / 3;
        }
    }
}

#pragma warning restore
