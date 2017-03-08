using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.SpectrumEmu.Devices;

namespace Spect.Net.SpectrumEmu.Test.Devices
{
    [TestClass]
    public class UlaClockTest
    {
        [TestMethod]
        public void UlaClockInitializationWorksAsExpected()
        {
            // --- Arrange
            var fakeClock = new FakeClockProvider();
            fakeClock.SetFrequency(1000000);
            fakeClock.SetCounter(100000);

            // --- Act
            var clock = new UlaClock(fakeClock);

            // --- Assert
            clock.Frequency.ShouldBe(1000000);
            clock.TicksAtStart.ShouldBe(100000);
            Math.Abs(clock.Z80ClockTick - 1000000 / (double)3500000).ShouldBeLessThan(1e-14);
            Math.Abs(clock.Z80NopTick - 4 * 1000000 / (double)3500000).ShouldBeLessThan(1e-14);
            clock.Z80TickCount.ShouldBe(0);
        }

        [TestMethod]
        public void Z80TickCountIsCalculatedProperly()
        {
            // --- Arrange
            var fakeClock = new FakeClockProvider();
            fakeClock.SetFrequency(1000000);
            fakeClock.SetCounter(100000);
            var clock = new UlaClock(fakeClock);

            // --- Act
            fakeClock.SetCounter(110000);
            var z80Tick1 = clock.Z80TickCount;
            fakeClock.SetCounter(200000);
            var z80Tick2 = clock.Z80TickCount;

            // --- Assert
            z80Tick1.ShouldBe((long)(10000 / clock.Z80ClockTick));
            z80Tick2.ShouldBe((long)(100000 / clock.Z80ClockTick));
        }

        private class FakeClockProvider : IHighResolutionClockProvider
        {
            private long _frequency;
            private long _counter;

            public void SetFrequency(long frequency)
            {
                _frequency = frequency;
            }

            public void SetCounter(long counter)
            {
                _counter = counter;
            }

            /// <summary>
            /// Retrieves the frequency of the clock. This value shows new
            /// number of clock ticks per second.
            /// </summary>
            public long GetFrequency()
            {
                return _frequency;
            }

            /// <summary>
            /// Retrieves the current counter value of the clock.
            /// </summary>
            public long GetCounter()
            {
                return _counter;
            }

            /// <summary>
            /// Waits until the specified counter value is reached
            /// </summary>
            /// <param name="counterValue">Counter value to reach</param>
            /// <param name="token">Token that can cancel the wait cycle</param>
            public void WaitUntil(long counterValue, CancellationToken token)
            {
                throw new NotImplementedException();
            }
        }
    }
}
