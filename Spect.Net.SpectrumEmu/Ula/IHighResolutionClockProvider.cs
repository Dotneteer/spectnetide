using System.Threading;

namespace Spect.Net.SpectrumEmu.Ula
{
    /// <summary>
    /// This interface defines the behavior of a high resolution clock provider
    /// that can be used for timing tasks.
    /// </summary>
    public interface IHighResolutionClockProvider
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

        /// <summary>
        /// Waits until the specified counter value is reached
        /// </summary>
        /// <param name="counterValue">Counter value to reach</param>
        /// <param name="token">Token that can cancel the wait cycle</param>
        void WaitUntil(long counterValue, CancellationToken token);
    }
}