using System.Collections.Generic;
using Spect.Net.SpectrumEmu.Machine;

namespace Spect.Net.SpectrumEmu.Devices
{
    /// <summary>
    /// This class represents the beeper device in ZX Spectrum
    /// </summary>
    public class BeeperDevice: IFrameBoundDevice
    {
        private readonly Spectrum48 _hostVm;
        private readonly int _ulaFrameTactCount;
        private readonly IEarBitPulseRenderer _pulseRenderer;

        /// <summary>
        /// The EAR bit pulses collected during the last frame
        /// </summary>
        public List<EarBitPulse> Pulses { get; }

        /// <summary>
        /// Gets the last value of the EAR bit
        /// </summary>
        public bool LastEarBit { get; private set; }

        /// <summary>
        /// Count of beeper frames since initialization
        /// </summary>
        public int FrameCount { get; private set; }

        /// <summary>
        /// Gets the last pulse tact value
        /// </summary>
        public int LastPulseTact { get; private set; }

        public BeeperDevice(Spectrum48 hostVm, IEarBitPulseRenderer pulseRenderer)
        {
            _hostVm = hostVm;
            _ulaFrameTactCount = hostVm.DisplayPars.UlaFrameTactCount;
            _pulseRenderer = pulseRenderer;
            Pulses = new List<EarBitPulse>(1000);
            Reset();
        }

        /// <summary>
        /// Resets this device
        /// </summary>
        public void Reset()
        {
            Pulses.Clear();
            LastPulseTact = 0;
            LastEarBit = true;
            FrameCount = 0;
            _pulseRenderer?.Reset();
        }

        /// <summary>
        /// Announdec that the device should start a new frame
        /// </summary>
        public void StartNewFrame()
        {
            Pulses.Clear();
            LastPulseTact = 0;
            FrameCount++;
        }

        /// <summary>
        /// Signs that the current frame has been completed
        /// </summary>
        public void SignFrameCompleted()
        {
            if (LastPulseTact == 0 && LastEarBit)
            {
                // --- We do not store a pulse if EAR bit has not changed
                // --- during the entire frame.
                return;
            }

            if (LastPulseTact < _ulaFrameTactCount -1)
            {
                // --- We have to store the last pulse information
                Pulses.Add(new EarBitPulse
                {
                    EarBit = LastEarBit,
                    Lenght = _ulaFrameTactCount - LastPulseTact
                });
            }

            _pulseRenderer?.AddSoundFrame(Pulses);
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
            var currentTact = _hostVm.CurrentFrameTact;
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
    }
}