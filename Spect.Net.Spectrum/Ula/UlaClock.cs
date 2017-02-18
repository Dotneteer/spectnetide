using System;
using Spect.Net.Spectrum.Utilities;
// ReSharper disable ConvertToAutoProperty
// ReSharper disable ConvertToAutoPropertyWithPrivateSetter

namespace Spect.Net.Spectrum.Ula
{
    /// <summary>
    /// This class represents the ULA clock that drives both the ULA chip and Z80
    /// in the Spectrum 48K model
    /// </summary>
    public class UlaClock
    {
        private double _z80ClockTick;
        private double _z80NopTick;
        private long _performanceFrequency;
        private long _performanceCountAtStart;

        /// <summary>
        /// The CPU uses 3.5 MHz clock
        /// </summary>
        public const double CPU_FREQ_MHZ = 3.5;

        /// <summary>
        /// The number of performance counter ticks for a single Z80 clock
        /// signal
        /// </summary>
        public double Z80ClockTick => _z80ClockTick;

        /// <summary>
        /// The number of performance counter ticks for a single Z80 NOP
        /// instruction
        /// </summary>
        public double Z80NopTick => _z80NopTick;

        /// <summary>
        /// Retrieves the frequency of the performance counter that measures
        /// the system time with less than 1us prcision.
        /// </summary>
        public long PerformanceFrequency => _performanceFrequency;

        /// <summary>
        /// Gets the initial performance counter value when the ULA clock
        /// has been reset last time
        /// </summary>
        public long PerformanceCountAtStart => _performanceCountAtStart;

        /// <summary>
        /// Resets the clock (just as if the ULA chip was reset)
        /// </summary>
        public void Reset()
        {
            long freq;
            if (!KernelMethods.QueryPerformanceFrequency(out freq))
            {
                throw new InvalidOperationException("Performance frequecy cannot be queried.");
            }
            _performanceFrequency = freq;
            _z80ClockTick = freq/CPU_FREQ_MHZ/1000000;
            _z80NopTick = 4*_z80ClockTick;
            long counter;
            if (!KernelMethods.QueryPerformanceCounter(out counter))
            {
                throw new InvalidOperationException("Performance counter cannot be queried.");
            }
            _performanceCountAtStart = counter;
        }

        /// <summary>
        /// Gets the number of ticks from the last reset
        /// </summary>
        public long Z80TickCount
        {
            get
            {
                long perfValue;
                KernelMethods.QueryPerformanceCounter(out perfValue);
                return (long)((perfValue - _performanceCountAtStart)/_z80ClockTick);
            }
        }
    }
}