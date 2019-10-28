using System.Threading;

namespace Spect.Net.SpectrumEmu.Abstraction.Providers
{
    /// <summary>
    /// This provider describes a  high resolution clock
    /// that can be used for timing tasks.
    /// </summary>
    public interface IClockProvider: IVmComponentProvider
    {
        /// <summary>
        /// Retrieves the frequency of the clock. This value shows new
        /// number of clock ticks per second.
        /// </summary>
        long GetFrequency();

        /// <summary>
        /// Retrieves the current counter value of the clock.
        /// </summary>
        long GetCounter();
    }
}