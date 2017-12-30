using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Spect.Net.SpectrumEmu.Cpu;

// ReSharper disable InconsistentNaming

namespace Spect.Net.SpectrumEmu.Test.PerfAssessment
{
    [TestClass]
    public class PerfMeasurements
    {
        [System.Runtime.InteropServices.DllImport("Kernel32.dll")]
        private static extern bool QueryPerformanceCounter(
            out long lpPerformanceCount);

        [System.Runtime.InteropServices.DllImport("Kernel32.dll")]
        private static extern bool QueryPerformanceFrequency(
            out long lpFrequency);

        private static long s_Frequency;
        private ulong _clock;


        [ClassInitialize]
        public static void InitClass(TestContext context)
        {
            if (QueryPerformanceFrequency(out s_Frequency) == false)
            {
                // --- High-performance counter not supported
                throw new System.ComponentModel.Win32Exception();
            }
        }

        [TestMethod]
        public void MeasureNormalExecution()
        {
            const int CYCLES = 1000000;
            long startTick;
            long endTick;

            _clock = 0;
            QueryPerformanceCounter(out startTick);
            for (var i = 0; i < CYCLES; i++)
            {
                ClockP3();
            }
            QueryPerformanceCounter(out endTick);
            Console.WriteLine(_clock);
            Console.WriteLine($"Frequency: {s_Frequency}");
            Console.WriteLine($"Start    : {startTick}");
            Console.WriteLine($"End      : {endTick}");
            Console.WriteLine((endTick - startTick)/(double)s_Frequency);
        }

        [TestMethod]
        public void MeasureInlinedExecution()
        {
            const int CYCLES = 1000000;
            long startTick;
            long endTick;

            _clock = 0;
            QueryPerformanceCounter(out startTick);
            for (var i = 0; i < CYCLES; i++)
            {
                ClockP3Inline();
            }
            QueryPerformanceCounter(out endTick);
            Console.WriteLine(_clock);
            Console.WriteLine($"Frequency: {s_Frequency}");
            Console.WriteLine($"Start    : {startTick}");
            Console.WriteLine($"End      : {endTick}");
            Console.WriteLine((endTick - startTick) / (double)s_Frequency);
        }

        [TestMethod]
        public void MeasureFlatExecution()
        {
            const int CYCLES = 1000000;
            long startTick;
            long endTick;

            _clock = 0;
            QueryPerformanceCounter(out startTick);
            for (var i = 0; i < CYCLES; i++)
            {
                _clock += 3;
            }
            QueryPerformanceCounter(out endTick);
            Console.WriteLine(_clock);
            Console.WriteLine($"Frequency: {s_Frequency}");
            Console.WriteLine($"Start    : {startTick}");
            Console.WriteLine($"End      : {endTick}");
            Console.WriteLine((endTick - startTick) / (double)s_Frequency);
        }

        [TestMethod]
        public void MeasureAluADCExecution()
        {
            const int CYCLES = 1000000;
            long startTick;
            long endTick;

            _clock = 0;
            QueryPerformanceCounter(out startTick);
            for (var i = 0; i < CYCLES; i++)
            {
                byte flags;
                AluADC(0x34, 0xA7, true, out flags);
            }
            QueryPerformanceCounter(out endTick);
            Console.WriteLine(_clock);
            Console.WriteLine($"Frequency: {s_Frequency}");
            Console.WriteLine($"Start    : {startTick}");
            Console.WriteLine($"End      : {endTick}");
            Console.WriteLine((endTick - startTick) / (double)s_Frequency);
        }

        [TestMethod]
        public void MeasureTimerSlot()
        {
            const int CYCLES = 1000000;
            const double Z80T = 1.1428E-6;
            var ticks = new long[CYCLES];

            _clock = 0;
            for (var i = 0; i < CYCLES; i++)
            {
                long tick;
                QueryPerformanceCounter(out tick);
                ticks[i] = tick;
            }
            var min = 0.0;
            var max = 0.0;
            var avg = 0.0;
            for (var i = 0; i < CYCLES - 1; i++)
            {
                var slot = (ticks[i + 1] - ticks[i])/(double) s_Frequency;
                if (slot < min) min = slot;
                if (slot > max) max = slot;
                avg += (slot/CYCLES);
            }
            var overZ80 = 0;
            var overAvg = 0;
            var overAvg2 = 0;
            var overAvg10 = 0;
            var overAvg100 = 0;
            var currentPeriod = 0;
            var longestPeriod = 0;
            var currentSlot = 0.0;
            var longestSlot = 0.0;
            var tooLong = false;
            for (var i = 0; i < CYCLES - 1; i++)
            {
                var slot = (ticks[i + 1] - ticks[i]) / (double)s_Frequency;
                if (slot > Z80T)
                {
                    overZ80++;
                }
                if (slot > avg)
                {
                    tooLong = true;
                    overAvg++;
                    currentPeriod++;
                    currentSlot += slot;
                }
                else if (tooLong)
                {
                    tooLong = false;
                    if (currentPeriod > longestPeriod) longestPeriod = currentPeriod;
                    if (currentSlot > longestSlot) longestSlot = currentSlot;
                    currentPeriod = 0;
                    currentSlot = 0.0;
                }

                if (slot > avg * 2) overAvg2++;
                if (slot > avg * 10) overAvg10++;
                if (slot > avg * 100) overAvg100++;
            }
            Console.WriteLine($"Frequency: {s_Frequency}");
            Console.WriteLine($"Min      : {min}");
            Console.WriteLine($"Max      : {max}");
            Console.WriteLine($"Avg      : {avg}");
            Console.WriteLine($">Z80T    : {overZ80}");
            Console.WriteLine($">Avg     : {overAvg}");
            Console.WriteLine($">Avg*2   : {overAvg2}");
            Console.WriteLine($">Avg*10  : {overAvg10}");
            Console.WriteLine($">Avg*100 : {overAvg100}");
            Console.WriteLine($"LongestNo: {longestPeriod}");
            Console.WriteLine($"LongestSl: {longestSlot}");
        }

        [TestMethod]
        public void MeasureTimerSlotWithPriority()
        {
            const int CYCLES = 1000000;
            var ticks = new long[CYCLES];

            Thread.CurrentThread.Priority = ThreadPriority.Highest;
            _clock = 0;
            for (var i = 0; i < CYCLES; i++)
            {
                long tick;
                QueryPerformanceCounter(out tick);
                ticks[i] = tick;
            }
            var min = 0.0;
            var max = 0.0;
            var avg = 0.0;
            for (var i = 0; i < CYCLES - 1; i++)
            {
                var slot = (ticks[i + 1] - ticks[i]) / (double)s_Frequency;
                if (slot < min) min = slot;
                if (slot > max) max = slot;
                avg += (slot / CYCLES);
            }
            var overAvg = 0;
            var overAvg2 = 0;
            var overAvg10 = 0;
            var overAvg100 = 0;
            for (var i = 0; i < CYCLES - 1; i++)
            {
                var slot = (ticks[i + 1] - ticks[i]) / (double)s_Frequency;
                if (slot > avg) overAvg++;
                if (slot > avg * 2) overAvg2++;
                if (slot > avg * 10) overAvg10++;
                if (slot > avg * 100) overAvg100++;
            }
            Console.WriteLine($"Frequency: {s_Frequency}");
            Console.WriteLine($"Min      : {min}");
            Console.WriteLine($"Max      : {max}");
            Console.WriteLine($"Avg      : {avg}");
            Console.WriteLine($">Avg     : {overAvg}");
            Console.WriteLine($">Avg*2   : {overAvg2}");
            Console.WriteLine($">Avg*10  : {overAvg10}");
            Console.WriteLine($">Avg*100 : {overAvg100}");
        }

        [TestMethod]
        public void MeasureTimerSlotWithSleep()
        {
            const int CYCLES = 1000000;
            var ticks = new long[CYCLES];

            _clock = 0;
            for (var i = 0; i < CYCLES; i++)
            {
                long tick;
                QueryPerformanceCounter(out tick);
                Thread.Sleep(0);
                ticks[i] = tick;
            }
            var min = 0.0;
            var max = 0.0;
            var avg = 0.0;
            for (var i = 0; i < CYCLES - 1; i++)
            {
                var slot = (ticks[i + 1] - ticks[i]) / (double)s_Frequency;
                if (slot < min) min = slot;
                if (slot > max) max = slot;
                avg += (slot / CYCLES);
            }
            var overAvg = 0;
            var overAvg2 = 0;
            var overAvg10 = 0;
            var overAvg100 = 0;
            for (var i = 0; i < CYCLES - 1; i++)
            {
                var slot = (ticks[i + 1] - ticks[i]) / (double)s_Frequency;
                if (slot > avg) overAvg++;
                if (slot > avg * 2) overAvg2++;
                if (slot > avg * 10) overAvg10++;
                if (slot > avg * 100) overAvg100++;
            }
            Console.WriteLine($"Frequency: {s_Frequency}");
            Console.WriteLine($"Min      : {min}");
            Console.WriteLine($"Max      : {max}");
            Console.WriteLine($"Avg      : {avg}");
            Console.WriteLine($">Avg     : {overAvg}");
            Console.WriteLine($">Avg*2   : {overAvg2}");
            Console.WriteLine($">Avg*10  : {overAvg10}");
            Console.WriteLine($">Avg*100 : {overAvg100}");
        }

        [TestMethod]
        public void GenerateSinTable()
        {
            for (var i = 0; i < 16; i++)
            {
                var value = Math.Sin(2 * Math.PI / 16 * i)*0.5 + 1.0;
                Console.WriteLine(value);

            }
        }




        private static void AluADC(byte left, byte right, bool cf, out byte flags)
        {
            var c = cf ? 1 : 0;
            var result = left + right + c;
            var signed = (sbyte)left + (sbyte)right + c;
            var lNibble = ((left & 0x0F) + (right & 0x0F) + c) & 0x10;

            flags = (byte)(result & (FlagsSetMask.S | FlagsSetMask.R5 | FlagsSetMask.R3));
            if ((result & 0xFF) == 0) flags |= FlagsSetMask.Z;
            if (result >= 0x100) flags |= FlagsSetMask.C;
            if (lNibble != 0) flags |= FlagsSetMask.H;
            if (signed >= 0x80 || signed <= -0x81) flags |= FlagsSetMask.PV;
        }


        private void ClockP3()
        {
            _clock += 3;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ClockP3Inline()
        {
            _clock += 3;
        }
    }
}
