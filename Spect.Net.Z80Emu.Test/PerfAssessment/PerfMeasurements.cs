using System;
using System.Runtime.CompilerServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Spect.Net.Z80Emu.Core;
// ReSharper disable InconsistentNaming

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


        private byte AluADC(byte left, byte right, bool cf, out byte flags)
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

            return (byte)result;
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
