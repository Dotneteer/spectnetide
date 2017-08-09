using System.Collections.Generic;
using Spect.Net.SpectrumEmu.Devices.Beeper;

namespace Spect.Net.SpectrumEmu.Abstraction.Devices
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
        /// <param name="fromTape">
        /// False: EAR bit comes from an OUT instruction, 
        /// True: EAR bit comes from tape
        /// </param>
        /// <param name="earBit">EAR bit value</param>
        void ProcessEarBitValue(bool fromTape, bool earBit);

        /// <summary>
        /// This method signs that tape should override the OUT instruction's EAR bit
        /// </summary>
        /// <param name="useTape">
        /// True: Override the OUT instruction with the tape's EAR bit value
        /// </param>
        void SetTapeOverride(bool useTape);

        /// <summary>
        /// Starts playing the sound
        /// </summary>
        void PlaySound();

        /// <summary>
        /// Pauses playing the sound
        /// </summary>
        void PauseSound();

        /// <summary>
        /// Stops playing the sound
        /// </summary>
        void KillSound();
    }
}