using System;
using Spect.Net.SpectrumEmu.Abstraction.Configuration;
using Spect.Net.SpectrumEmu.Abstraction.Devices;
using Spect.Net.SpectrumEmu.Abstraction.Providers;

#pragma warning disable 67

namespace Spect.Net.SpectrumEmu.Devices.Beeper
{
    public class BeeperDevice : IBeeperDevice
    {
        private IBeeperProvider _beeperProvider;
        private IAudioConfiguration _audioConfiguration;
        private long _frameBegins;
        private int _frameTacts;
        private int _tactsPerSample;
        private bool _useTapeMode;

        /// <summary>
        /// Audio samples to build the audio stream
        /// </summary>
        public float[] AudioSamples { get; private set; }

        /// <summary>
        /// Index of the next audio sample
        /// </summary>
        public int SamplesIndex { get; private set; }

        /// <summary>
        /// The virtual machine that hosts the device
        /// </summary>
        public ISpectrumVm HostVm { get; set; }

        /// <summary>
        /// Signs that the device has been attached to the Spectrum virtual machine
        /// </summary>
        public void OnAttachedToVm(ISpectrumVm hostVm)
        {
            HostVm = hostVm;
            _audioConfiguration = hostVm.AudioConfiguration;
            _beeperProvider = hostVm.BeeperProvider;
            _frameTacts = hostVm.FrameTacts;
            _tactsPerSample = _audioConfiguration.TactsPerSample;
            Reset();
        }

        /// <summary>
        /// Resets this device
        /// </summary>
        public void Reset()
        {
            _frameBegins = HostVm.Cpu.Tacts;
            LastEarBit = true;
            FrameCount = 0;
            Overflow = 0;
            _useTapeMode = false;
            _beeperProvider?.Reset();
            InitializeSampling();
        }

        /// <summary>
        /// Gets the last value of the EAR bit
        /// </summary>
        public bool LastEarBit { get; set; }

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
                Overflow = (int) (HostVm.Cpu.Tacts - _frameBegins - _frameTacts);
            }
            _beeperProvider?.AddSoundFrame(AudioSamples);
            _frameBegins += _frameTacts;
        }

        /// <summary>
        /// Allow external entities respond to frame completion
        /// </summary>
        public event EventHandler FrameCompleted;

        /// <summary>
        /// Processes the change of the EAR bit value
        /// </summary>
        /// <param name="fromTape">
        /// False: EAR bit comes from an OUT instruction, 
        /// True: EAR bit comes from tape
        /// </param>
        /// <param name="earBit">EAR bit value</param>
        public void ProcessEarBitValue(bool fromTape, bool earBit)
        {
            if (!fromTape && _useTapeMode)
            {
                // --- The EAR bit comes from and OUT instruction, but now we're in tape mode
                return;
            }
            if (earBit == LastEarBit)
            {
                // --- The earbit has not changed
                return;
            }

            CreateSamples(HostVm.Cpu.Tacts);
            LastEarBit = earBit;
        }

        /// <summary>
        /// This method signs that tape should override the OUT instruction's EAR bit
        /// </summary>
        /// <param name="useTape">
        /// True: Override the OUT instruction with the tape's EAR bit value
        /// </param>
        public void SetTapeOverride(bool useTape)
        {
            _useTapeMode = useTape;
        }

        /// <summary>
        /// Sets up sampling ionformation for the forthcoming frame
        /// </summary>
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
        /// Create samples according the specified ear bit
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
                AudioSamples[SamplesIndex++] = LastEarBit ? 1.0f : 0.0f;
                nextSampleOffset += _tactsPerSample;
            }
            LastSampleTact = nextSampleOffset;
        }
    }
}

#pragma warning restore
