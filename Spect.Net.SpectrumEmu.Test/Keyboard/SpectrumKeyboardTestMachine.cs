using Spect.Net.SpectrumEmu.Devices.Keyboard;
using Spect.Net.SpectrumEmu.Test.Helpers;

namespace Spect.Net.SpectrumEmu.Test.Keyboard
{
    /// <summary>
    /// This virtual machine can be used to test the behavior of the Spectrum keyboard
    /// </summary>
    public class SpectrumKeyboardTestMachine : SpectrumSimpleTestMachine
    {
        public const ushort KEY_SCAN = 0x028E;

        public readonly KeyboardDevice KeyboardDevice = new KeyboardDevice(null);

        protected override byte ReadPort(ushort addr)
        {
            return (addr & 0xFF) != 0xFE
                ? (byte)0xFF
                : KeyboardDevice.GetLineStatus((byte)(addr >> 8));
        }
    }
}