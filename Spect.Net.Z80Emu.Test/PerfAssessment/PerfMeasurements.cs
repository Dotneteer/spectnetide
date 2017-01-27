using System;
using System.Runtime.CompilerServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Spect.Net.Z80Emu.Test.PerfAssessment
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
