using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.Z80TestHelpers;

namespace Spect.Net.Z80Emu.Test.Spectrum
{
    [TestClass]
    public class RomKeyboardTest
    {
        [TestMethod]
        public void NoKeyPressedIsSendedBySpectrumRom()
        {
            // --- Arrange
            var machine = new SpectrumKeyboardTestMachine();

            // --- Act
            machine.CallIntoRom(SpectrumKeyboardTestMachine.KEY_SCAN);

            // --- Assert
            machine.Cpu.Registers.DE.ShouldBe((ushort)0xFFFF);
        }
    }
}
