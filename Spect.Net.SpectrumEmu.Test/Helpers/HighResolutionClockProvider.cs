using System;
using System.Threading;
using Spect.Net.SpectrumEmu.Ula;

namespace Spect.Net.SpectrumEmu.Test.Helpers
{
    public class HighResolutionClockProvider: IHighResolutionClockProvider
    {
        private long? _frequency;

        /// <summary>
        /// Retrieves the frequency of the clock. This value shows new
        /// number of clock ticks per second.
        /// </summary>
        public long GetFrequency()
        {
            long frequency;
            if (!KernelMethods.QueryPerformanceFrequency(out frequency))
            {
                throw new InvalidOperationException("Performance frequecy cannot be queried.");
            }
            return frequency;
        }

        /// <summary>
        /// Retrieves the current counter value of the clock.
        /// </summary>
        public long GetCounter()
        {
            long perfValue;
            KernelMethods.QueryPerformanceCounter(out perfValue);
            return perfValue;
        }

        /// <summary>
        /// Waits until the specified counter value is reached
        /// </summary>
        /// <param name="counterValue">Counter value to reach</param>
        /// <param name="token">Token that can cancel the wait cycle</param>
        public void WaitUntil(long counterValue, CancellationToken token)
        {
            if (_frequency == null)
            {
                _frequency = GetFrequency();
            }

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