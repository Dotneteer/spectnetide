using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.SpectrumEmu.Machine;
using Spect.Net.SpectrumEmu.Test.Helpers;

namespace Spect.Net.SpectrumEmu.Test.Devices.Memory
{
    [TestClass]
    public class SpectrumP3EAllRamTests
    {
        [TestMethod]
        public void AllRamMode0WorksAsExpected()
        {
            // --- Arrange
            var spectrum = new SpectrumP3EAdvancedTestMachine();
            var debugProvider = new TestDebugInfoProvider();
            spectrum.SetDebugInfoProvider(debugProvider);

            // --- We render the screen while the interrupt is disabled
            spectrum.InitCode(new byte[]
            {
                0x01, 0xFD, 0x1F, // LD BC,$1FFD
                0x3E, 0x01,       // LD A,$01
                0xED, 0x79,       // OUT (C),A
                0x76              // HALT
            });

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.UntilHalt));

            // --- Assert
            var md = spectrum.MemoryDevice;
            md.IsInAllRamMode.ShouldBeTrue();
            md.GetSelectedBankIndex(0).ShouldBe(0);
            md.GetSelectedBankIndex(1).ShouldBe(1);
            md.GetSelectedBankIndex(2).ShouldBe(2);
            md.GetSelectedBankIndex(3).ShouldBe(3);
        }

        [TestMethod]
        public void AllRamMode1WorksAsExpected()
        {
            // --- Arrange
            var spectrum = new SpectrumP3EAdvancedTestMachine();
            var debugProvider = new TestDebugInfoProvider();
            spectrum.SetDebugInfoProvider(debugProvider);

            // --- We render the screen while the interrupt is disabled
            spectrum.InitCode(new byte[]
            {
                0x01, 0xFD, 0x1F, // LD BC,$1FFD
                0x3E, 0x03,       // LD A,$03
                0xED, 0x79        // OUT (C),A
            });
            spectrum.DebugInfoProvider.Breakpoints.Add(0x8007, new MinimumBreakpointInfo());

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger));

            // --- Assert
            var md = spectrum.MemoryDevice;
            md.IsInAllRamMode.ShouldBeTrue();
            md.GetSelectedBankIndex(0).ShouldBe(4);
            md.GetSelectedBankIndex(1).ShouldBe(5);
            md.GetSelectedBankIndex(2).ShouldBe(6);
            md.GetSelectedBankIndex(3).ShouldBe(7);
        }

        [TestMethod]
        public void AllRamMode2WorksAsExpected()
        {
            // --- Arrange
            var spectrum = new SpectrumP3EAdvancedTestMachine();
            var debugProvider = new TestDebugInfoProvider();
            spectrum.SetDebugInfoProvider(debugProvider);

            // --- We render the screen while the interrupt is disabled
            spectrum.InitCode(new byte[]
            {
                0x01, 0xFD, 0x1F, // LD BC,$1FFD
                0x3E, 0x05,       // LD A,$05
                0xED, 0x79        // OUT (C),A
            });
            spectrum.DebugInfoProvider.Breakpoints.Add(0x8007, new MinimumBreakpointInfo());

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger));

            // --- Assert
            var md = spectrum.MemoryDevice;
            md.IsInAllRamMode.ShouldBeTrue();
            md.GetSelectedBankIndex(0).ShouldBe(4);
            md.GetSelectedBankIndex(1).ShouldBe(5);
            md.GetSelectedBankIndex(2).ShouldBe(6);
            md.GetSelectedBankIndex(3).ShouldBe(3);
        }

        [TestMethod]
        public void AllRamMode3WorksAsExpected()
        {
            // --- Arrange
            var spectrum = new SpectrumP3EAdvancedTestMachine();
            var debugProvider = new TestDebugInfoProvider();
            spectrum.SetDebugInfoProvider(debugProvider);

            // --- We render the screen while the interrupt is disabled
            spectrum.InitCode(new byte[]
            {
                0x01, 0xFD, 0x1F, // LD BC,$1FFD
                0x3E, 0x07,       // LD A,$07
                0xED, 0x79        // OUT (C),A
            });
            spectrum.DebugInfoProvider.Breakpoints.Add(0x8007, new MinimumBreakpointInfo());

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger));

            // --- Assert
            var md = spectrum.MemoryDevice;
            md.IsInAllRamMode.ShouldBeTrue();
            md.GetSelectedBankIndex(0).ShouldBe(4);
            md.GetSelectedBankIndex(1).ShouldBe(7);
            md.GetSelectedBankIndex(2).ShouldBe(6);
            md.GetSelectedBankIndex(3).ShouldBe(3);
        }
    }
}
