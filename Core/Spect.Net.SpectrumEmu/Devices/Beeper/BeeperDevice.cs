using System;
using System.Collections.Generic;
using Spect.Net.SpectrumEmu.Abstraction.Configuration;
using Spect.Net.SpectrumEmu.Abstraction.Devices;
using Spect.Net.SpectrumEmu.Abstraction.Providers;

#pragma warning disable 67

namespace Spect.Net.SpectrumEmu.Devices.Beeper
{
    /// <summary>
    /// This class represents the beeper device in ZX Spectrum
    /// </summary>
    public class BeeperDevice: IBeeperDevice
    {
        private readonly IEarBitFrameProvider _earBitFrameProvider;
        private long _frameBegins;
        private int _frameTacts;
        private int _tactsPerSample;
        private bool _useTapeMode;

        /// <summary>
        /// The virtual machine that hosts the device
        /// </summary>
        public ISpectrumVm HostVm { get; private set; }

        /// <summary>
        /// Signs that the device has been attached to the Spectrum virtual machine
        /// </summary>
        public void OnAttachedToVm(ISpectrumVm hostVm)
        {
            HostVm = hostVm;
            BeeperConfiguration = new BeeperConfigurationData();
            _frameTacts = hostVm.FrameTacts;
            _tactsPerSample = BeeperConfiguration.TactsPerSample;
            Pulses = new List<EarBitPulse>(1000);
            Reset();
        }

        public BeeperDevice(IEarBitFrameProvider earBitFrameProvider = null)
        {
            _earBitFrameProvider = earBitFrameProvider;
        }

        /// <summary>
        /// Get the beeper parameters
        /// </summary>
        public BeeperConfigurationData BeeperConfiguration { get; private set; }

        /// <summary>
        /// The EAR bit pulses collected during the last frame
        /// </summary>
        public List<EarBitPulse> Pulses { get; private set; }

        /// <summary>
        /// Gets the last value of the EAR bit
        /// </summary>
        public bool LastEarBit { get; private set; }

        /// <summary>
        /// Count of beeper frames since initialization
        /// </summary>
        public int FrameCount { get; private set; }

        /// <summary>
        /// Overflow from the previous frame, given in #of tacts 
        /// </summary>
        public int Overflow { get; set; }

        /// <summary>
        /// Gets the last pulse tact value
        /// </summary>
        public int LastPulseTact { get; private set; }

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

            LastEarBit = earBit;
            var currentHostTact = HostVm.CurrentFrameTact;
            var currentTact = currentHostTact <= _frameTacts ? currentHostTact : _frameTacts;
            var length = currentTact - LastPulseTact;

            // --- If the first tact changes the pulse, we do
            // --- not add it
            if (length > 0)
            {
                Pulses.Add(new EarBitPulse
                {
                    EarBit = !earBit,
                    Lenght = length
                });
            }
            LastPulseTact = currentTact;
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
        /// Starts playing the sound
        /// </summary>
        public void PlaySound()
        {
            _earBitFrameProvider?.PlaySound();
        }

        /// <summary>
        /// Pauses playing the sound
        /// </summary>
        public void PauseSound()
        {
            _earBitFrameProvider?.PauseSound();
        }

        /// <summary>
        /// Stops playing the sound
        /// </summary>
        public void KillSound()
        {
            _earBitFrameProvider?.KillSound();
        }

        /// <summary>
        /// Allow the device to react to the start of a new frame
        /// </summary>
        public void OnNewFrame()
        {
            Pulses.Clear();
            LastPulseTact = 0;
            FrameCount++;
        }

        /// <summary>
        /// Allow the device to react to the completion of a frame
        /// </summary>
        public void OnFrameCompleted()
        {
            if (LastPulseTact <= _frameTacts - 1)
            {
                // --- We have to store the last pulse information
                Pulses.Add(new EarBitPulse
                {
                    EarBit = LastEarBit,
                    Lenght = _frameTacts - LastPulseTact
                });
            }

            // --- Create the array for the samples
            var firstSampleOffset = _frameBegins % _tactsPerSample == 0
                ? 0
                : _tactsPerSample - (_frameBegins + _tactsPerSample) % _tactsPerSample;
            var samplesInFrame = (_frameTacts - firstSampleOffset - 1) / _tactsPerSample + 1;
            var samples = new float[samplesInFrame];

            // --- Convert pulses to samples
            var sampleIndex = 0;
            var currentEnd = _frameBegins;

            foreach (var pulse in Pulses)
            {
                var firstSample = currentEnd % _tactsPerSample == 0
                    ? currentEnd
                    : currentEnd + _tactsPerSample - currentEnd % _tactsPerSample;
                for (var i = firstSample; i < currentEnd + pulse.Lenght; i += _tactsPerSample)
                {
                    samples[sampleIndex++] = pulse.EarBit ? 1.0F : 0.0F;
                }
                currentEnd += pulse.Lenght;
            }
            _earBitFrameProvider?.AddSoundFrame(samples);
            _frameBegins += _frameTacts;
        }

        /// <summary>
        /// Allow external entities respond to frame completion
        /// </summary>
        public event EventHandler FrameCompleted;

        /// <summary>
        /// Resets this device
        /// </summary>
        public void Reset()
        {
            Pulses.Clear();
            LastPulseTact = 0;
            LastEarBit = true;
            FrameCount = 0;
            _frameBegins = 0;
            _useTapeMode = false;
            _earBitFrameProvider?.Reset();
        }
    }

#pragma warning restore

}