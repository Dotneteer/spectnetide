using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.Spectrum.Keyboard;

namespace Spect.Net.Spectrum.Test.Keyboard
{
    [TestClass]
    public class KeyboardStatusTests
    {
        [TestMethod]
        public void NoKeyIsDownAfterInstantiation()
        {
            // --- Act
            var status = new KeyboardStatus();

            // --- Assert
            status.GetLineStatus(0xFF).ShouldBe((byte)0xFF);
        }

        [TestMethod]
        public void InputLine0WorksAsExpected()
        {
            TestKey(SpectrumKeyCode.CShift, 0xFE, 0xFE);
            TestKey(SpectrumKeyCode.Z, 0xFE, 0xFD);
            TestKey(SpectrumKeyCode.X, 0xFE, 0xFB);
            TestKey(SpectrumKeyCode.C, 0xFE, 0xF7);
            TestKey(SpectrumKeyCode.V, 0xFE, 0xEF);
        }

        [TestMethod]
        public void InputLine1WorksAsExpected()
        {
            TestKey(SpectrumKeyCode.A, 0xFD, 0xFE);
            TestKey(SpectrumKeyCode.S, 0xFD, 0xFD);
            TestKey(SpectrumKeyCode.D, 0xFD, 0xFB);
            TestKey(SpectrumKeyCode.F, 0xFD, 0xF7);
            TestKey(SpectrumKeyCode.G, 0xFD, 0xEF);
        }

        [TestMethod]
        public void InputLine2WorksAsExpected()
        {
            TestKey(SpectrumKeyCode.Q, 0xFB, 0xFE);
            TestKey(SpectrumKeyCode.W, 0xFB, 0xFD);
            TestKey(SpectrumKeyCode.E, 0xFB, 0xFB);
            TestKey(SpectrumKeyCode.R, 0xFB, 0xF7);
            TestKey(SpectrumKeyCode.T, 0xFB, 0xEF);
        }

        [TestMethod]
        public void InputLine3WorksAsExpected()
        {
            TestKey(SpectrumKeyCode.N1, 0xF7, 0xFE);
            TestKey(SpectrumKeyCode.N2, 0xF7, 0xFD);
            TestKey(SpectrumKeyCode.N3, 0xF7, 0xFB);
            TestKey(SpectrumKeyCode.N4, 0xF7, 0xF7);
            TestKey(SpectrumKeyCode.N5, 0xF7, 0xEF);
        }

        [TestMethod]
        public void InputLine4WorksAsExpected()
        {
            TestKey(SpectrumKeyCode.N0, 0xEF, 0xFE);
            TestKey(SpectrumKeyCode.N9, 0xEF, 0xFD);
            TestKey(SpectrumKeyCode.N8, 0xEF, 0xFB);
            TestKey(SpectrumKeyCode.N7, 0xEF, 0xF7);
            TestKey(SpectrumKeyCode.N6, 0xEF, 0xEF);
        }

        [TestMethod]
        public void InputLine5WorksAsExpected()
        {
            TestKey(SpectrumKeyCode.P, 0xDF, 0xFE);
            TestKey(SpectrumKeyCode.O, 0xDF, 0xFD);
            TestKey(SpectrumKeyCode.I, 0xDF, 0xFB);
            TestKey(SpectrumKeyCode.U, 0xDF, 0xF7);
            TestKey(SpectrumKeyCode.Y, 0xDF, 0xEF);
        }

        [TestMethod]
        public void InputLine6WorksAsExpected()
        {
            TestKey(SpectrumKeyCode.Enter, 0xBF, 0xFE);
            TestKey(SpectrumKeyCode.L, 0xBF, 0xFD);
            TestKey(SpectrumKeyCode.K, 0xBF, 0xFB);
            TestKey(SpectrumKeyCode.J, 0xBF, 0xF7);
            TestKey(SpectrumKeyCode.H, 0xBF, 0xEF);
        }

        [TestMethod]
        public void InputLine7WorksAsExpected()
        {
            TestKey(SpectrumKeyCode.Space, 0x7F, 0xFE);
            TestKey(SpectrumKeyCode.SShift, 0x7F, 0xFD);
            TestKey(SpectrumKeyCode.M, 0x7F, 0xFB);
            TestKey(SpectrumKeyCode.N, 0x7F, 0xF7);
            TestKey(SpectrumKeyCode.B, 0x7F, 0xEF);
        }

        private void TestKey(SpectrumKeyCode key, byte address, byte expectedInput)
        {
            // --- Arrange
            var status = new KeyboardStatus();
            status.SetStatus(key, false);

            // --- Act
            var before = status.GetStatus(key);
            var inputBefore = status.GetLineStatus(address);
            status.SetStatus(key, true);
            var after = status.GetStatus(key);
            var inputAfter = status.GetLineStatus(address);

            // --- Assert
            before.ShouldBeFalse();
            after.ShouldBeTrue();
            inputBefore.ShouldBe((byte)0xFF);
            inputAfter.ShouldBe(expectedInput);
        }
    }
}
