using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.SpectrumEmu.Keyboard;

namespace Spect.Net.SpectrumEmu.Test.Keyboard
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

        [TestMethod]
        public void SingleKeyPressOnLine0IsSended()
        {
            TestSingleKeyPress(SpectrumKeyCode.CShift, 0x27);
            TestSingleKeyPress(SpectrumKeyCode.Z, 0x1F);
            TestSingleKeyPress(SpectrumKeyCode.X, 0x17);
            TestSingleKeyPress(SpectrumKeyCode.C, 0x0F);
            TestSingleKeyPress(SpectrumKeyCode.V, 0x07);
        }

        [TestMethod]
        public void SingleKeyPressOnLine1IsSended()
        {
            TestSingleKeyPress(SpectrumKeyCode.A, 0x26);
            TestSingleKeyPress(SpectrumKeyCode.S, 0x1E);
            TestSingleKeyPress(SpectrumKeyCode.D, 0x16);
            TestSingleKeyPress(SpectrumKeyCode.F, 0x0E);
            TestSingleKeyPress(SpectrumKeyCode.G, 0x06);
        }

        [TestMethod]
        public void SingleKeyPressOnLine2IsSended()
        {
            TestSingleKeyPress(SpectrumKeyCode.Q, 0x25);
            TestSingleKeyPress(SpectrumKeyCode.W, 0x1D);
            TestSingleKeyPress(SpectrumKeyCode.E, 0x15);
            TestSingleKeyPress(SpectrumKeyCode.R, 0x0D);
            TestSingleKeyPress(SpectrumKeyCode.T, 0x05);
        }

        [TestMethod]
        public void SingleKeyPressOnLine3IsSended()
        {
            TestSingleKeyPress(SpectrumKeyCode.N1, 0x24);
            TestSingleKeyPress(SpectrumKeyCode.N2, 0x1C);
            TestSingleKeyPress(SpectrumKeyCode.N3, 0x14);
            TestSingleKeyPress(SpectrumKeyCode.N4, 0x0C);
            TestSingleKeyPress(SpectrumKeyCode.N5, 0x04);
        }

        [TestMethod]
        public void SingleKeyPressOnLine4IsSended()
        {
            TestSingleKeyPress(SpectrumKeyCode.N0, 0x23);
            TestSingleKeyPress(SpectrumKeyCode.N9, 0x1B);
            TestSingleKeyPress(SpectrumKeyCode.N8, 0x13);
            TestSingleKeyPress(SpectrumKeyCode.N7, 0x0B);
            TestSingleKeyPress(SpectrumKeyCode.N6, 0x03);
        }

        [TestMethod]
        public void SingleKeyPressOnLine5IsSended()
        {
            TestSingleKeyPress(SpectrumKeyCode.P, 0x22);
            TestSingleKeyPress(SpectrumKeyCode.O, 0x1A);
            TestSingleKeyPress(SpectrumKeyCode.I, 0x12);
            TestSingleKeyPress(SpectrumKeyCode.U, 0x0A);
            TestSingleKeyPress(SpectrumKeyCode.Y, 0x02);
        }

        [TestMethod]
        public void SingleKeyPressOnLine6IsSended()
        {
            TestSingleKeyPress(SpectrumKeyCode.Enter, 0x21);
            TestSingleKeyPress(SpectrumKeyCode.L, 0x19);
            TestSingleKeyPress(SpectrumKeyCode.K, 0x11);
            TestSingleKeyPress(SpectrumKeyCode.J, 0x09);
            TestSingleKeyPress(SpectrumKeyCode.H, 0x01);
        }

        [TestMethod]
        public void SingleKeyPressOnLine7IsSended()
        {
            TestSingleKeyPress(SpectrumKeyCode.Space, 0x20);
            TestSingleKeyPress(SpectrumKeyCode.SShift, 0x18);
            TestSingleKeyPress(SpectrumKeyCode.M, 0x10);
            TestSingleKeyPress(SpectrumKeyCode.N, 0x08);
            TestSingleKeyPress(SpectrumKeyCode.B, 0x00);
        }

        private void TestSingleKeyPress(SpectrumKeyCode key, byte romCode)
        {
            // --- Arrange
            var machine = new SpectrumKeyboardTestMachine();
            machine.KeyboardStatus.SetStatus(key, true);

            // --- Act
            machine.CallIntoRom(SpectrumKeyboardTestMachine.KEY_SCAN);

            // --- Assert
            var regs = machine.Cpu.Registers;
            regs.D.ShouldBe((byte)0xFF);
            regs.E.ShouldBe(romCode);
            regs.ZFlag.ShouldBeTrue();
        }
    }
}
