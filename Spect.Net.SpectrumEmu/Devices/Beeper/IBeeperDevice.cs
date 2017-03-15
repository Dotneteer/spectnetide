using System.Collections.Generic;

namespace Spect.Net.SpectrumEmu.Devices.Beeper
{
    /// <summary>
    /// This interface represents the beeper device in the Spectrum VM
    /// </summary>
    public interface IBeeperDevice : IFrameBoundDevice
    {
        /// <summary>
        /// The EAR bit pulses collected during the last frame
        /// </summary>
        List<EarBitPulse> Pulses { get; }

        /// <summary>
        /// Gets the last value of the EAR bit
        /// </summary>
        bool LastEarBit { get; }

        /// <summary>
        /// Count of beeper frames since initialization
        /// </summary>
        int FrameCount { get; }

        /// <summary>
        /// Gets the last pulse tact value
        /// </summary>
        int LastPulseTact { get; }

        /// <summary>
        /// Processes the change of the EAR bit value
        /// </summary>
        /// <param name="earBit"></param>
        void ProcessEarBitValue(bool earBit);
    }
}