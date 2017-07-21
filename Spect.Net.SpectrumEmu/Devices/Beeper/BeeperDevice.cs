using System.Collections.Generic;
using Spect.Net.SpectrumEmu.Abstraction;
using Spect.Net.SpectrumEmu.Providers;

namespace Spect.Net.SpectrumEmu.Devices.Beeper
{
    /// <summary>
    /// This class represents the beeper device in ZX Spectrum
    /// </summary>
    public class BeeperDevice: IBeeperDevice
    {
        private readonly IEarBitPulseProcessor _earBitPulseProcessor;
        private int _frameTacts;

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
            BeeperConfiguration = new BeeperConfiguration();
            _frameTacts = hostVm.FrameTacts;
            Pulses = new List<EarBitPulse>(1000);
            Reset();
        }

        public BeeperDevice(IEarBitPulseProcessor earBitPulseProcessor = null)
        {
            _earBitPulseProcessor = earBitPulseProcessor;
        }

        /// <summary>
        /// Get the beeper parameters
        /// </summary>
        public BeeperConfiguration BeeperConfiguration { get; private set; }

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
        /// The current tact within the frame
        /// </summary>
        public int CurrentFrameTact { get; }

        /// <summary>
        /// Overflow from the previous frame, given in #of tacts 
        /// </summary>
        public int Overflow { get; }

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
            if (LastPulseTact == 0 && LastEarBit)
            {
                // --- We do not store a pulse if EAR bit has not changed
                // --- during the entire frame.
                return;
            }

            if (LastPulseTact < _frameTacts - 1)
            {
                // --- We have to store the last pulse information
                Pulses.Add(new EarBitPulse
                {
                    EarBit = LastEarBit,
                    Lenght = _frameTacts - LastPulseTact
                });
            }
            //else if (LastPulseTact > _frameTacts - 1)
            //{
            //    // --- We have to modify the part of the last pulse
            //    // --- within this frame
            //    var overflow = LastPulseTact - _frameTacts + 1;
            //    var lastPulseIndex = Pulses.Count - 1;
            //    if (lastPulseIndex >= 0)
            //    {
            //        var lastPulse = Pulses[lastPulseIndex];
            //        lastPulse.Lenght -= overflow;
            //        Pulses[lastPulseIndex] = lastPulse;
            //    }
            //    Pulses.Add(new EarBitPulse
            //    {
            //        EarBit = LastEarBit,
            //        Lenght = _frameTacts - LastPulseTact
            //    });

            //}

            _earBitPulseProcessor?.AddSoundFrame(Pulses);
        }

        /// <summary>
        /// Gets the last pulse tact value
        /// </summary>
        public int LastPulseTact { get; private set; }

        /// <summary>
        /// Resets this device
        /// </summary>
        public void Reset()
        {
            Pulses.Clear();
            LastPulseTact = 0;
            LastEarBit = true;
            FrameCount = 0;
            _earBitPulseProcessor?.Reset();
        }

        /// <summary>
        /// Processes the change of the EAR bit value
        /// </summary>
        /// <param name="earBit"></param>
        public void ProcessEarBitValue(bool earBit)
        {
            if (earBit == LastEarBit)
            {
                // --- The earbit has not changed
                return;
            }

            LastEarBit = earBit;
            var currentTact = HostVm.CurrentFrameTact;
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
        /// Renders the provided pulses into the specified buffer as float volume numbers
        /// </summary>
        /// <param name="pulses">Pulses to convert</param>
        /// <param name="beeperPars">Sound parameters</param>
        /// <param name="buffer">Pulse sample buffer</param>
        /// <param name="offset">Buffer offset</param>
        /// <param name="volumeLow">Low volume value</param>
        /// <param name="volumeHigh">High volume value</param>
        public static void RenderFloat(IList<EarBitPulse> pulses, BeeperConfiguration beeperPars, 
            float[] buffer, int offset, 
            float volumeLow = 0F, float volumeHigh = 1F)
        {
            if (pulses.Count == 0)
            {
                pulses = new List<EarBitPulse>
                {
                    new EarBitPulse
                    {
                        EarBit = true,
                        Lenght = beeperPars.SamplesPerFrame * beeperPars.UlaTactsPerSample
                    }
                };
            }
            var currentEnd = 0;
            var sampleOffset = beeperPars.SamplingOffset;
            var tactsInSample = beeperPars.UlaTactsPerSample;
            foreach (var pulse in pulses)
            {
                var firstSample = (currentEnd + sampleOffset) / tactsInSample;
                var lastSample = (currentEnd + pulse.Lenght + sampleOffset) / tactsInSample;
                for (var i = firstSample; i < lastSample; i++)
                {
                    buffer[offset + i] = pulse.EarBit ? volumeHigh : volumeLow;
                }
                currentEnd += pulse.Lenght;
            }
        }
    }
}