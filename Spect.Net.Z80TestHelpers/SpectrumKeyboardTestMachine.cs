using Spect.Net.Spectrum.Keyboard;

namespace Spect.Net.Z80TestHelpers
{
    /// <summary>
    /// This virtual machine can be used to test the behavior of the Spectrum keyboard
    /// </summary>
    public class SpectrumKeyboardTestMachine : SpectrumTestMachine
    {
        public const ushort KEY_SCAN = 0x028E;

        public readonly KeyboardStatus KeyboardStatus = new KeyboardStatus();

        protected override byte ReadPort(ushort addr)
        {
            return (addr & 0xFF) != 0xFE
                ? (byte)0xFF
                : KeyboardStatus.GetLineStatus((byte)(addr >> 8));
        }
    }
}