using System.Collections.Generic;
using Spect.Net.SpectrumEmu.Machine;
using Spect.Net.SpectrumEmu.Providers;

namespace Spect.Net.SpectrumEmu.Devices.PulseDevice
{
    /// <summary>
    /// Implements a device that collects pulses and processes them at the end of the frame.
    /// </summary>
    public class MicPulseDevice: IMicPulseDevice
    {
        private readonly Spectrum48 _hostVm;
        private readonly int _ulaFrameTactCount;
        private readonly IMicBitPulseProcessor _micBitPulseProcessor;

        /// <summary>
        /// Pulse overflow to the next frame
        /// </summary>
        public PulseData? Overflow { get; private set; }

        /// <summary>
        /// The EAR bit pulses collected during the last frame
        /// </summary>
        public List<PulseData> Pulses { get; }

        /// <summary>
        /// Gets the last value of the pulse bit
        /// </summary>
        public bool LastPulseBit { get; private set; }

        /// <summary>
        /// Gets the last pulse tact value
        /// </summary>
        public int LastPulseTact { get; private set; }

        /// <summary>
        /// Initializes the device with the specified Spectrum VM and pulse processor
        /// </summary>
        /// <param name="hostVm">Host Spectrum VM</param>
        /// <param name="micBitPulseProcessor">Pulse processor instance</param>
        public MicPulseDevice(Spectrum48 hostVm, IMicBitPulseProcessor micBitPulseProcessor)
        {
            _hostVm = hostVm;
            _micBitPulseProcessor = micBitPulseProcessor;
            _ulaFrameTactCount = hostVm.FrameTacts;
            Pulses = new List<PulseData>(1000);
            Overflow = null;
            Reset();
        }

        /// <summary>
        /// Resets this device
        /// </summary>
        public void Reset()
        {
            Pulses.Clear();
            LastPulseTact = 0;
            LastPulseBit = true;
            _micBitPulseProcessor?.Reset();
        }

        /// <summary>
        /// Announces that the device should start a new frame
        /// </summary>
        public void StartNewFrame()
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Signs that the current frame has been completed
        /// </summary>
        public void SignFrameCompleted()
        {
            if (LastPulseTact == 0 && LastPulseBit)
            {
                // --- We do not store a pulse if the bit has not changed
                // --- during the entire frame.
                return;
            }

            if (LastPulseTact < _ulaFrameTactCount - 1)
            {
                // --- We have to store the last pulse information
                Pulses.Add(new PulseData
                {
                    PulseBit = LastPulseBit,
                    Lenght = _ulaFrameTactCount - LastPulseTact
                });
            }
            else
            {
                // --- There is an overflow pulse to add to the next frame
                Overflow = new PulseData
                {
                    PulseBit = LastPulseBit,
                    Lenght = _ulaFrameTactCount - LastPulseTact
                };
            }

            _micBitPulseProcessor?.ProcessPulses(Pulses);
        }


        /// <summary>
        /// Processes the change of the pulse bit value
        /// </summary>
        /// <param name="bit"></param>
        public void ProcessBit(bool bit)
        {
            if (bit == LastPulseBit)
            {
                // --- The bit has not changed
                return;
            }

            LastPulseBit = bit;
            var currentTact = _hostVm.CurrentFrameTact;
            var length = currentTact - LastPulseTact;

            // --- If the first tact changes the pulse, we do
            // --- not add it
            if (length > 0)
            {
                Pulses.Add(new PulseData
                {
                    PulseBit = !bit,
                    Lenght = length
                });
            }
            LastPulseTact = currentTact;
        }
    }
}