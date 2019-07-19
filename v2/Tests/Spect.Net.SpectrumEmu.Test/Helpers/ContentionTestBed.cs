using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Shouldly;
using Spect.Net.SpectrumEmu.Abstraction.Machine;

namespace Spect.Net.SpectrumEmu.Test.Helpers
{
    public class ContentionTestBed
    {
        private static TestPixelRenderer s_PixelRenderer;
        private static SpectrumAdvancedTestMachine s_Spectrum;

        protected void ExecuteContentionTest(List<byte> ops, int codeAddress,
            int tactsFromFirstPixel, int expectedLength, Action<SpectrumAdvancedTestMachine> initAction = null)
        {
            // --- Arrange
            var sw = new Stopwatch();
            sw.Start();
            var spectrum = CreateTestmachine();
            sw.Stop();
            Console.WriteLine(sw.ElapsedMilliseconds);

            var fpTact = spectrum.ScreenConfiguration.FirstDisplayLine *
                         spectrum.ScreenConfiguration.ScreenLineTime +
                         spectrum.ScreenConfiguration.BorderLeftTime - 128;
            var baseTacts = fpTact / 4 * 4;

            var code = InitNopsForTact(baseTacts, fpTact + 128 + tactsFromFirstPixel, (ushort)codeAddress);
            spectrum.InitCode(code, 0xc000);
            for (var i = 0; i < ops.Count; i++)
            {
                spectrum.WriteSpectrumMemory((ushort)(codeAddress + i), ops[i]);
            }
            spectrum.SetUlaFrameTact(baseTacts);
            initAction?.Invoke(spectrum);

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None,
                new ExecuteCycleOptions(EmulationMode.UntilExecutionPoint,
                terminationPoint: (ushort)(codeAddress + ops.Count)));

            // --- Assert
            var expectedTacts = fpTact + 128 + tactsFromFirstPixel + expectedLength;
            spectrum.Cpu.Tacts.ShouldBe(expectedTacts);
        }

        private SpectrumAdvancedTestMachine CreateTestmachine()
        {
            if (s_PixelRenderer == null)
            {
                s_PixelRenderer = new TestPixelRenderer(SpectrumModels.ZxSpectrum48Pal.Screen);
            }
            return s_Spectrum ?? (s_Spectrum = new SpectrumAdvancedTestMachine(s_PixelRenderer));
        }

        private List<byte> InitNopsForTact(int baseTacts, int tactToReach, ushort codeAddr)
        {
            var code = new List<byte>
            {
                0xF3 // DI ; (4) 
            };
            var usedTacts = 4;
            tactToReach -= baseTacts;
            var remainder = tactToReach % 4;
            switch (remainder)
            {
                case 0:
                    code.Add(0x03); // INC BC (6)
                    usedTacts += 6;
                    break;
                case 1:
                    code.Add(0x3E); // LD A,0 (7)
                    code.Add(0x00);
                    usedTacts += 7;
                    break;
                case 2:
                    break;
                default:
                    code.Add(0xED); // LD A,I (9)
                    code.Add(0x57);
                    usedTacts += 9;
                    break;
            }

            usedTacts += 10;
            var extraNops = (tactToReach - usedTacts) / 4;
            for (var i = 0; i < extraNops; i++)
            {
                code.Add(0x00); // NOP
            }
            code.AddRange(new byte[]
            {
                0xC3,                   // JP <codeAddr>
                (byte)codeAddr,
                (byte)(codeAddr >> 8)
            });
            return code;
        }

    }
}