using System.Collections.Generic;
using Spect.Net.SpectrumEmu.Devices;

namespace Spect.Net.SpectrumEmu.Providers
{
    /// <summary>
    /// This interface represents a device that can render beeper pulses
    /// into sound
    /// </summary>
    public interface IEarBitPulseProcessor: IVmComponentProvider
    {
        /// <summary>
        /// Adds the specified set of pulses to the sound
        /// </summary>
        /// <param name="pulses"></param>
        void AddSoundFrame(IList<EarBitPulse> pulses);
    }
}