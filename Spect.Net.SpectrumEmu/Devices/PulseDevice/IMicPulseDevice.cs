using System.Collections.Generic;

namespace Spect.Net.SpectrumEmu.Devices.PulseDevice
{
    /// <summary>
    /// This interface represents the beeper device in the Spectrum VM
    /// </summary>
    public interface IMicPulseDevice : IUlaFrameBoundDevice
    {
        /// <summary>
        /// Pulse overflow to the next frame
        /// </summary>
        PulseData? Overflow { get; }

        /// <summary>
        /// The EAR bit pulses collected during the last frame
        /// </summary>
        List<PulseData> Pulses { get; }

        /// <summary>
        /// Gets the last value of the pulse bit
        /// </summary>
        bool LastPulseBit { get; }

        /// <summary>
        /// Gets the last pulse tact value
        /// </summary>
        int LastPulseTact { get; }

        /// <summary>
        /// Processes the change of the pulse bit value
        /// </summary>
        /// <param name="bit"></param>
        void ProcessBit(bool bit);
    }
}