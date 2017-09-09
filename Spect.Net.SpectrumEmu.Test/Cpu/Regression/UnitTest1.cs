using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Spect.Net.SpectrumEmu.Test.Helpers;

namespace Spect.Net.SpectrumEmu.Test.Cpu.Regression
{
    [TestClass]
    public class RldRegressionTests
    {
        [TestMethod]
        public void DaaRegression()
        {
            for (var inA = 0; inA < 256; inA++)
            {
                ExecuteDaa((byte) inA, out var af);
                ExecuteZmakDaa((byte) inA, out var zmakAf);

                if ((af & 0xFFD7) != (zmakAf &0xFFD7))
                {
                    Console.WriteLine($"A: {inA}, AF: {af}, zmakAF: {zmakAf}");
                }
            }
        }

        private void ExecuteDaa(byte inA, out ushort af)
        {
            var m = new Z80TestMachine(RunMode.UntilHalt);
            m.InitCode(new byte[]
            {
                0x27, // DAA
                0x76
            });
            var regs = m.Cpu.Registers;
            regs.A = inA;
            m.Run();

            af = regs.AF;
        }

        private void ExecuteZmakDaa(byte inA, out ushort af)
        {
            var m = new ZmakTestMachine(RunMode.UntilHalt);
            m.InitCode(new byte[]
            {
                0x27, // Daa
                0x76
            });
            var regs = m.Cpu.regs;
            regs.A = inA;
            m.Run();

            af = regs.AF;
        }

    }
}
