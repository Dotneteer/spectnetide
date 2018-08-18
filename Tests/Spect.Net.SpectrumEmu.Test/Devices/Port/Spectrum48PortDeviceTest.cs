using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.SpectrumEmu.Machine;
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
            var spectrum = new SpectrumAdvancedTestMachine();

            spectrum.InitCode(new byte[]
            {
                0x3E, 0x18,             // LD A,$18
                0xB7, 0xF8,             // OR A,$F8
                0xD3, 0xFE,             // OUT ($FE),A
                0x3E, 0x08,             // LD A,$08
                0xD3, 0xFE,             // OUT ($FE),A
                0x06, 0x06,             // LD B,6
                0xDD, 0x21, 0x00, 0x00, // LD IX,0
                0x10, 0xFA,             // DJNZ $-4
                0xDB, 0xFE,             // IN A,($FE)
                0x76                    // HALT
            });

            // --- Arrange
            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.UntilHalt));

            // --- Assert
            // === Only a part of the frame's tact time is used
            spectrum.Cpu.Registers.A.ShouldBe((byte)0xFF);
        }
    }
}
