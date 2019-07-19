using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.SpectrumEmu.Abstraction.Machine;
using Spect.Net.SpectrumEmu.Test.Helpers;

namespace Spect.Net.SpectrumEmu.Test.Devices.Port
{
    [TestClass]
    public class Spectrum48PortDeviceTest
    {
        [TestMethod]
        public void PortWithIssue2UlaAndLowTimingWorks()
        {
            // --- Arrange
            var spectrum = new SpectrumAdvancedTestMachine(ulaIssue: "2");

            spectrum.InitCode(new byte[]
            {
                0x3E, 0x18,             // LD A,$18
                0xF6, 0xF8,             // OR A,$F8
                0xD3, 0xFE,             // OUT ($FE),A
                0x3E, 0x08,             // LD A,$08
                0xF6, 0xE8,             // OR A,$E8
                0xD3, 0xFE,             // OUT ($FE),A
                0x06, 0x06,             // LD B,6
                0xDD, 0x21, 0x00, 0x00, // LD IX,0
                0x10, 0xFA,             // DJNZ $-4
                0xDB, 0xFE,             // IN A,($FE)
                0x76                    // HALT
            });

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.UntilHalt));

            // --- Assert
            spectrum.Cpu.Registers.A.ShouldBe((byte)0xFF);
        }

        [TestMethod]
        public void PortWithIssue2UlaAndHighTimingWorks()
        {
            // --- Arrange
            var spectrum = new SpectrumAdvancedTestMachine(ulaIssue: "2");

            spectrum.InitCode(new byte[]
            {
                0x3E, 0x18,             // LD A,$18
                0xF6, 0xF8,             // OR A,$F8
                0xD3, 0xFE,             // OUT ($FE),A
                0x3E, 0x08,             // LD A,$08
                0xF6, 0xE8,             // OR A,$E8
                0xD3, 0xFE,             // OUT ($FE),A
                0x06, 0x08,             // LD B,8
                0xDD, 0x21, 0x00, 0x00, // LD IX,0
                0x10, 0xFA,             // DJNZ $-4
                0xDB, 0xFE,             // IN A,($FE)
                0x76                    // HALT
            });

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.UntilHalt));

            // --- Assert
            spectrum.Cpu.Registers.A.ShouldBe((byte)0xFF);
        }

        [TestMethod]
        public void PortWithIssue3UlaAndLowTimingWorks()
        {
            // --- Arrange
            var spectrum = new SpectrumAdvancedTestMachine();

            spectrum.InitCode(new byte[]
            {
                0x3E, 0x18,             // LD A,$18
                0xF6, 0xF8,             // OR A,$F8
                0xD3, 0xFE,             // OUT ($FE),A
                0x3E, 0x08,             // LD A,$08
                0xF6, 0xE8,             // OR A,$E8
                0xD3, 0xFE,             // OUT ($FE),A
                0x06, 0x06,             // LD B,6
                0xDD, 0x21, 0x00, 0x00, // LD IX,0
                0x10, 0xFA,             // DJNZ $-4
                0xDB, 0xFE,             // IN A,($FE)
                0x76                    // HALT
            });

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.UntilHalt));

            // --- Assert
            spectrum.Cpu.Registers.A.ShouldBe((byte)0xFF);
        }

        [TestMethod]
        public void PortWithIssue3UlaAndHighTimingWorks()
        {
            // --- Arrange
            var spectrum = new SpectrumAdvancedTestMachine();

            spectrum.InitCode(new byte[]
            {
                0x3E, 0x18,             // LD A,$18
                0xF6, 0xF8,             // OR A,$F8
                0xD3, 0xFE,             // OUT ($FE),A
                0x3E, 0x08,             // LD A,$08
                0xF6, 0xE8,             // OR A,$E8
                0xD3, 0xFE,             // OUT ($FE),A
                0x06, 0x08,             // LD B,8
                0xDD, 0x21, 0x00, 0x00, // LD IX,0
                0x10, 0xFA,             // DJNZ $-4
                0xDB, 0xFE,             // IN A,($FE)
                0x76                    // HALT
            });

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.UntilHalt));

            // --- Assert
            spectrum.Cpu.Registers.A.ShouldBe((byte)0xBF);
        }

        [TestMethod]
        [DataRow(0xF8, "2", 0xFF)]
        [DataRow(0xF8, "3", 0xFF)]
        [DataRow(0xF0, "2", 0xFF)]
        [DataRow(0xF0, "3", 0xFF)]
        [DataRow(0xE8, "2", 0xFF)]
        [DataRow(0xE8, "3", 0xBF)]
        [DataRow(0xE0, "2", 0xBF)]
        [DataRow(0xE0, "3", 0xBF)]
        public void Bit6ValueIsHandledProperly(int portValue, string ulaIssue, int expectedValue)
        {
            // --- Arrange
            var spectrum = new SpectrumAdvancedTestMachine(ulaIssue: ulaIssue);

            spectrum.InitCode(new byte[]
            {
                0x3E, (byte) portValue, // LD A,$18
                0xD3, 0xFE,             // OUT ($FE),A
                0x06, 0x06,             // LD B,8
                0xDD, 0x21, 0x00, 0x00, // LD IX,0
                0x10, 0xFA,             // DJNZ $-4
                0xDB, 0xFE,             // IN A,($FE)
                0x76                    // HALT
            });

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.UntilHalt));

            // --- Assert
            spectrum.Cpu.Registers.A.ShouldBe((byte)expectedValue);
        }

        [TestMethod]
        public void Bint4ImmediatelyGoesBackTo1WithIssue3Ula()
        {
            // --- Arrange
            var spectrum = new SpectrumAdvancedTestMachine();

            spectrum.InitCode(new byte[]
            {
                0x3E, 0x18,             // LD A,$18
                0xF6, 0xF8,             // OR A,$F8
                0xD3, 0xFE,             // OUT ($FE),A
                0x3E, 0x08,             // LD A,$08
                0xF6, 0xE8,             // OR A,$E8
                0xD3, 0xFE,             // OUT ($FE),A
                0x06, 0x06,             // LD B,6
                0xDD, 0x21, 0x00, 0x00, // LD IX,0
                0x10, 0xFA,             // DJNZ $-4
                0xDB, 0xFE,             // IN A,($FE)
                0x3E, 0xF8,             // LD A,$F8
                0xD3, 0xFE,             // OUT ($FE),A
                0xDB, 0xFE,             // IN A,($FE)
                0x76                    // HALT
            });

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.UntilHalt));

            // --- Assert
            spectrum.Cpu.Registers.A.ShouldBe((byte)0xFF);
        }
    }
}
