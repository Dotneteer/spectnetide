using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.Spectrum.Machine;
using Spect.Net.Spectrum.Test.Helpers;

namespace Spect.Net.Spectrum.Test.Ula
{
    [TestClass]
    public class UlaInterruptDeviceTests
    {
        [TestMethod]
        public void DisabledInterruptIsNotRaised()
        {
            // --- Arrange
            var spectrum = new SpectrumAdvancedTestMachine();

            // --- We render the screen while the interrupt is disabled
            spectrum.InitCode(new byte[]
            {
                0xED, 0x56,       // IM 1
                0xF3,             // DI
                0x3E, 0x05,       // LD A,$05
                0xD3, 0xFE,       // OUT ($FE),A
                0x01, 0x00, 0x0A, // LD BC,$0A00
                0x0B,             // DECLB: DEC BC
                0x78,             // LD A,B
                0xB1,             // OR C
                0x20, 0xFB,       // JR NZ,DECLB
                0x76              // HALT
            });
            var pixels = new TestPixelRenderer(spectrum.DisplayPars);
            spectrum.ScreenDevice.SetScreenPixelRenderer(pixels);
            var startTime = spectrum.Clock.GetNativeCounter();

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, EmulationMode.UntilHalt);

            // === Display some extra information about the duration of the frame execution
            var duration = (spectrum.Clock.GetNativeCounter() - startTime)
                / (double)spectrum.Clock.Frequency;
            Console.WriteLine("Frame execution time: {0} second", duration);

            // --- Assert
            var regs = spectrum.Cpu.Registers;
            regs.PC.ShouldBe((ushort)0x8010);

            spectrum.Cpu.Ticks.ShouldBeGreaterThanOrEqualTo(66599ul);
        }

        [TestMethod]
        public void EnabledInterruptIsRaised()
        {
            // --- Arrange
            var spectrum = new SpectrumAdvancedTestMachine();

            // --- We render the screen while the interrupt is disabled
            spectrum.InitCode(new byte[]
            {
                0xED, 0x56,       // IM 1
                0xFB,             // EI
                0x3E, 0x05,       // LD A,$05
                0xD3, 0xFE,       // OUT ($FE),A
                0x01, 0x00, 0x0A, // LD BC,$0A00
                0x0B,             // DECLB: DEC BC
                0x78,             // LD A,B
                0xB1,             // OR C
                0x20, 0xFB,       // JR NZ,DECLB
                0x76              // HALT
            });
            var pixels = new TestPixelRenderer(spectrum.DisplayPars);
            spectrum.ScreenDevice.SetScreenPixelRenderer(pixels);
            var startTime = spectrum.Clock.GetNativeCounter();

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, EmulationMode.UntilHalt);

            // === Display some extra information about the duration of the frame execution
            var duration = (spectrum.Clock.GetNativeCounter() - startTime)
                / (double)spectrum.Clock.Frequency;
            Console.WriteLine("Frame execution time: {0} second", duration);

            // --- Assert
            var regs = spectrum.Cpu.Registers;
            regs.PC.ShouldBe((ushort)0x8010);

            // --- The instructions above take 66599 tacts while reaching the HALT operation
            // --- However, an interrupt is generated, and because of IM 1, the RST 38 is
            // --- invoked. It checks to keyboard status in 1034 tacts.
            // --- When HALT is reached, the CPU tact count is 67633.
            spectrum.Cpu.Ticks.ShouldBeGreaterThanOrEqualTo(67633ul);
        }
    }
}
