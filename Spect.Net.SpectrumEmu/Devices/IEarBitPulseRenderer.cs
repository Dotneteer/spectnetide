using System.Collections.Generic;

namespace Spect.Net.SpectrumEmu.Devices
{
    /// <summary>
    /// This interface represents a device that can render beeper pulses
    /// into sound
    /// </summary>
    public interface IEarBitPulseRenderer
    {
        /// <summary>
        /// Resets the renderer device
        /// </summary>
        void Reset();

        /// <summary>
        /// Adds the specified set of pulses to the sound
        /// </summary>
        /// <param name="pulses"></param>
        void AddSoundFrame(IList<EarBitPulse> pulses);
    }
}