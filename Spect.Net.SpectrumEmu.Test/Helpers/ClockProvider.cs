using System.Threading;
using Spect.Net.SpectrumEmu.Providers;

namespace Spect.Net.SpectrumEmu.Test.Helpers
{
    public class ClockProvider: IClockProvider
    {
        private long _frequency;

        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public ClockProvider()
        {
            Reset();
        }

        /// <summary>
        /// The component provider should be able to reset itself
        /// </summary>
        public void Reset()
        {
            KernelMethods.QueryPerformanceFrequency(out long frequency);
            _frequency = frequency;
        }

        /// <summary>
        /// Retrieves the frequency of the clock. This value shows new
        /// number of clock ticks per second.
        /// </summary>
        public long GetFrequency() => _frequency;

        /// <summary>
        /// Retrieves the current counter value of the clock.
        /// </summary>
        public long GetCounter()
        {
            KernelMethods.QueryPerformanceCounter(out long perfValue);
            return perfValue;
        }

        /// <summary>
        /// Waits until the specified counter value is reached
        /// </summary>
        /// <param name="counterValue">Counter value to reach</param>
        /// <param name="token">Token that can cancel the wait cycle</param>
        public void WaitUntil(long counterValue, CancellationToken token)
        {
            // --- Calculate the number of milliseconds to wait
            var millisec = _frequency / 1000;

            // --- Wait until we have up to 4 milliseconds left
            while (!token.IsCancellationRequested)
            {
                var millisecs = (counterValue - GetCounter()) / millisec;
                if (millisecs < 0)
                {
                    return;
                }
                if (millisecs < 4) break;
                Thread.Sleep(2);
            }

            // --- Use SpinWait
            while (!token.IsCancellationRequested)
            {
                if (counterValue < GetCounter()) break;
                Thread.SpinWait(1);
            }
        }
    }
}