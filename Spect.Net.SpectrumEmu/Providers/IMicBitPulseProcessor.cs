using System.Collections;
using System.Collections.Generic;
using Spect.Net.SpectrumEmu.Devices.PulseDevice;

namespace Spect.Net.SpectrumEmu.Providers
{
    /// <summary>
    /// Processes a list of pulses when the frame ends
    /// </summary>
    public interface IMicBitPulseProcessor: IVmComponentProvider
    {
        /// <summary>
        /// Processes the specified list of pulses
        /// </summary>
        /// <param name="pulses">A set of pulses to process</param>
        void ProcessPulses(IList<PulseData> pulses);
    }
}