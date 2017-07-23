using System.Threading;
using Spect.Net.SpectrumEmu.Abstraction.Providers;

namespace Spect.Net.Wpf
{
    /// <summary>
    /// This class implements a clock provider that allows access to the 
    /// high resoultion system clock.
    /// </summary>
    public class ClockProvider : IClockProvider
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
            QueryPerformanceFrequency(out long frequency);
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
            QueryPerformanceCounter(out long perfValue);
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

        /// <summary>
        /// QueryPerformanceCounter function
        /// Retrieves the current value of the performance counter, which is 
        /// a high resolution (less than 1us) time stamp that can be used for 
        /// time-interval measurements.
        /// </summary>
        /// <param name="lpPerformanceCount">
        /// A pointer to a variable that receives the current performance-
        /// counter value, in counts.
        /// </param>
        /// <returns>
        /// If the function succeeds, the return value is nonzero. On systems 
        /// that run Windows XP or later, the function will always succeed and 
        /// will thus never return zero.
        /// </returns>
        [System.Runtime.InteropServices.DllImport("Kernel32.dll")]
        public static extern bool QueryPerformanceCounter(
            out long lpPerformanceCount);

        /// <summary>
        /// Retrieves the frequency of the performance counter. The frequency 
        /// of the performance counter is fixed at system boot and is consistent 
        /// across all processors. Therefore, the frequency need only be queried 
        /// upon application initialization, and the result can be cached.
        /// </summary>
        /// <param name="lpFrequency">
        /// A pointer to a variable that receives the current performance-counter 
        /// frequency, in counts per second. If the installed hardware doesn't 
        /// support a high-resolution performance counter, this parameter can be 
        /// zero (this will not occur on systems that run Windows XP or later).
        /// </param>
        /// <returns>
        /// If the installed hardware supports a high-resolution performance 
        /// counter, the return value is nonzero. On systems that run Windows XP 
        /// or later, the function will always succeed and will thus never 
        /// return zero.
        /// </returns>
        [System.Runtime.InteropServices.DllImport("Kernel32.dll")]
        public static extern bool QueryPerformanceFrequency(
            out long lpFrequency);
    }
}