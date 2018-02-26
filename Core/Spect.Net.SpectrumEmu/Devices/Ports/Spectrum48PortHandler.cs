using Spect.Net.SpectrumEmu.Abstraction.Devices;

namespace Spect.Net.SpectrumEmu.Devices.Ports
{
    /// <summary>
    /// This class handles the standard spectrum port.
    /// </summary>
    public class Spectrum48PortHandler : PortHandlerBase
    {
        private IZ80Cpu _cpu;
        private IScreenDevice _screenDevice;
        private IBeeperDevice _beeperDevice;
        private IKeyboardDevice _keyboardDevice;
        private ITapeDevice _tapeDevice;

        /// <summary>
        /// Mask for partial port decoding
        /// </summary>
        public override ushort PortMask => 0b0000_0000_0000_0001;

        /// <summary>
        /// Port address after masking
        /// </summary>
        public override ushort Port => 0b0000_0000_0000_0000;

        /// <summary>
        /// Signs that the device has been attached to the Spectrum virtual machine
        /// </summary>
        public override void OnAttachedToVm(ISpectrumVm hostVm)
        {
            base.OnAttachedToVm(hostVm);
            _cpu = hostVm.Cpu;
            _screenDevice = hostVm.ScreenDevice;
            _beeperDevice = hostVm.BeeperDevice;
            _keyboardDevice = hostVm.KeyboardDevice;
            _tapeDevice = hostVm.TapeDevice;
        }

        /// <summary>
        /// Handles the read from the port
        /// </summary>
        /// <param name="addr">Full port address</param>
        /// <param name="readValue">The value read from the port</param>
        /// <returns>True, if read handled; otherwise, false</returns>
        public override bool HandleRead(ushort addr, out byte readValue)
        {
            readValue = _keyboardDevice.GetLineStatus((byte)(addr >> 8));
            var earBit = _tapeDevice.GetEarBit(_cpu.Tacts);
            if (!earBit)
            {
                readValue = (byte)(readValue & 0b1011_1111);
            }
            return true;
        }

        /// <summary>
        /// Writes the specified value to the port
        /// </summary>
        /// <param name="writeValue">Value to write to the port</param>
        public override void HandleWrite(byte writeValue)
        {
            _screenDevice.BorderColor = writeValue & 0x07;
            _beeperDevice.ProcessEarBitValue(false, (writeValue & 0x10) != 0);
            _tapeDevice.ProcessMicBit((writeValue & 0x08) != 0);
        }
    }
}