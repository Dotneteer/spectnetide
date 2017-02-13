using Spect.Net.Z80Emu.Spectrum;

namespace Spect.Net.Z80TestHelpers
{
    /// <summary>
    /// This virtual machine can be used to test the behavior of the Spectrum keyboard
    /// </summary>
    public class SpectrumKeyboardTestMachine : SpectrumTestMachine
    {
        public const ushort KEY_SCAN = 0x028E;

        private readonly KeyboardStatus _keyboardStatus = new KeyboardStatus();

        protected override byte ReadPort(ushort addr)
        {
            return (addr & 0xFF) != 0xFE
                ? (byte)0xFF
                : _keyboardStatus.GetLineStatus((byte)(addr >> 8));
        }
    }
}