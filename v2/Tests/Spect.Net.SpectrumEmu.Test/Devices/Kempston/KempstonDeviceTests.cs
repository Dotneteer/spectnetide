using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.SpectrumEmu.Abstraction.Machine;
using Spect.Net.SpectrumEmu.Test.Helpers;

namespace Spect.Net.SpectrumEmu.Test.Devices.Kempston
{
    [TestClass]
    public class KempstonDeviceTests
    {
        [TestMethod]
        public void NoKempstonDeviceWorks()
        {
            // --- Arrange
            var spectrum = new SpectrumAdvancedTestMachine();
            var provider = spectrum.KempstonProvider as KempstonTestProvider;
            // ReSharper disable once PossibleNullReferenceException
            provider.Reset();
            provider.IsPresent = false;

            // --- We render the screen while the interrupt is disabled
            spectrum.InitCode(new byte[]
            {
                0x01, 0x1F, 0x00, // LD BC,001FH
                0xED, 0x78,       // IN A,(C)
                0x76              // HALT
            });

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.UntilHalt));

            // --- Assert
            var regs = spectrum.Cpu.Registers;
            regs.A.ShouldBe((byte)0xFF);
        }

        [TestMethod]
        public void KempstonDeviceWithNoKeyPressWorks()
        {
            // --- Arrange
            var spectrum = new SpectrumAdvancedTestMachine();
            var provider = spectrum.KempstonProvider as KempstonTestProvider;
            // ReSharper disable once PossibleNullReferenceException
            provider.Reset();
            provider.IsPresent = true;

            // --- We render the screen while the interrupt is disabled
            spectrum.InitCode(new byte[]
            {
                0x01, 0x1F, 0x00, // LD BC,001FH
                0xED, 0x78,       // IN A,(C)
                0x76              // HALT
            });

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.UntilHalt));

            // --- Assert
            var regs = spectrum.Cpu.Registers;
            regs.A.ShouldBe((byte)0x00);
        }

        [TestMethod]
        [DataRow(false, false, false, false, false, 0x00)]
        [DataRow(true, false, false, false, false, 0x02)]
        [DataRow(true, false, false, false, true, 0x12)]
        [DataRow(true, false, true, false, false, 0x0A)]
        [DataRow(true, false, false, true, false, 0x06)]
        [DataRow(true, false, true, false, true, 0x1A)]
        [DataRow(true, false, false, true, true, 0x16)]
        [DataRow(false, true, false, false, false, 0x01)]
        [DataRow(false, true, false, false, true, 0x11)]
        [DataRow(false, true, true, false, false, 0x09)]
        [DataRow(false, true, false, true, false, 0x05)]
        [DataRow(false, true, true, false, true, 0x19)]
        [DataRow(false, true, false, true, true, 0x15)]
        [DataRow(false, false, true, false, false, 0x08)]
        [DataRow(false, false, true, false, true, 0x18)]
        [DataRow(false, false, false, true, false, 0x04)]
        [DataRow(false, false, false, true, true, 0x14)]
        public void KempstonDeviceWithKeyPressWorks(bool left, bool right, bool up, bool down, bool fire, int expected)
        {
            // --- Arrange
            var spectrum = new SpectrumAdvancedTestMachine();
            var provider = spectrum.KempstonProvider as KempstonTestProvider;

            // ReSharper disable once PossibleNullReferenceException
            provider.Reset();
            provider.IsPresent = true;
            provider.LeftPressed = left;
            provider.RightPressed = right;
            provider.DownPressed = down;
            provider.UpPressed = up;
            provider.FirePressed = fire;

            // --- We render the screen while the interrupt is disabled
            spectrum.InitCode(new byte[]
            {
                0x01, 0x1F, 0x00, // LD BC,001FH
                0xED, 0x78,       // IN A,(C)
                0x76              // HALT
            });

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.UntilHalt));

            // --- Assert
            var regs = spectrum.Cpu.Registers;
            regs.A.ShouldBe((byte)expected);
        }
    }
}