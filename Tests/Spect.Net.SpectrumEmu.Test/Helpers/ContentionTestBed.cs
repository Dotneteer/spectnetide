using System.Collections.Generic;
using System.Threading;
using Shouldly;
using Spect.Net.SpectrumEmu.Machine;

namespace Spect.Net.SpectrumEmu.Test.Helpers
{
    public class ContentionTestBed
    {
        protected static void ExecuteContentionTest(List<byte> ops, int codeAddress,
            int tactsFromFirstPixel, int expectedLength)
        {
            // --- Arrange
            var spectrum = CreateTestmachine();
            var fpTact = spectrum.ScreenConfiguration.FirstDisplayLine *
                         spectrum.ScreenConfiguration.ScreenLineTime +
                         spectrum.ScreenConfiguration.BorderLeftTime;

            var code = InitNopsForTact(fpTact + tactsFromFirstPixel, (ushort)codeAddress);
            spectrum.InitCode(code, 0xc000);
            for (var i = 0; i < ops.Count; i++)
            {
                spectrum.WriteSpectrumMemory((ushort)(codeAddress + i), ops[i]);
            }

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None,
                new ExecuteCycleOptions(EmulationMode.UntilExecutionPoint,
                terminationPoint: (ushort)(codeAddress + ops.Count)));

            // --- Assert
            var expectedTacts = fpTact + tactsFromFirstPixel + expectedLength;
            spectrum.Cpu.Tacts.ShouldBe(expectedTacts);
        }

        private static SpectrumAdvancedTestMachine CreateTestmachine()
        {
            var pixels = new TestPixelRenderer(SpectrumModels.ZxSpectrum48Pal.Screen);
            return new SpectrumAdvancedTestMachine(pixels);
        }

        private static List<byte> InitNopsForTact(int tactToReach, ushort codeAddr)
        {
            var code = new List<byte>
            {
                0xF3 // DI ; (4) 
            };
            var usedTacts = 4;
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