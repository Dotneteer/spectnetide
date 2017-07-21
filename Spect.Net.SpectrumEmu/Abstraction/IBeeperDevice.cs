using System.Collections.Generic;
using Spect.Net.SpectrumEmu.Devices.Beeper;

namespace Spect.Net.SpectrumEmu.Abstraction
{
    /// <summary>
    /// This interface represents the beeper device in the Spectrum VM
    /// </summary>
    public interface IBeeperDevice: IFrameBoundDevice, ISpectrumBoundDevice
    {
        /// <summary>
        /// Get the beeper parameters
        /// </summary>
        BeeperConfiguration BeeperConfiguration { get; }

        /// <summary>
        /// The EAR bit pulses collected during the last frame
        /// </summary>
        List<EarBitPulse> Pulses { get; }

        /// <summary>
        /// Gets the last value of the EAR bit
        /// </summary>
        bool LastEarBit { get; }

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