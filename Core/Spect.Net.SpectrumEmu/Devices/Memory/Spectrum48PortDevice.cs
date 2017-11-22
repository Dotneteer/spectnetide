using Spect.Net.SpectrumEmu.Abstraction.Devices;

namespace Spect.Net.SpectrumEmu.Devices.Memory
{
    /// <summary>
    /// This class represents the port device used by the Spectrum 48 virtual machine
    /// </summary>
    public class Spectrum48PortDevice: SpectrumPortDeviceBase
    {
        private IBorderDevice _borderDevice;
        private IBeeperDevice _beeperDevice;
        private IKeyboardDevice _keyboardDevice;
        private ITapeDevice _tapeDevice;

        /// <summary>
        /// Signs that the device has been attached to the Spectrum virtual machine
        /// </summary>
        public override void OnAttachedToVm(ISpectrumVm hostVm)
        {
            base.OnAttachedToVm(hostVm);
            _borderDevice = hostVm.BorderDevice;
            _beeperDevice = hostVm.BeeperDevice;
            _keyboardDevice = hostVm.KeyboardDevice;
            _tapeDevice = hostVm.TapeDevice;
        }

        /// <summary>
        /// Reads the port with the specified address
        /// </summary>
        /// <param name="addr">Port address</param>
        /// <returns>Byte read from the port</returns>
        public override byte OnReadPort(ushort addr)
        {
            // --- Handle I/O contention
            base.OnReadPort(addr);

            // --- Carry out I/O read operation
            if ((addr & 0x0001) != 0) return 0xFF;

            var portBits = _keyboardDevice.GetLineStatus((byte)(addr >> 8));
            var earBit = _tapeDevice.GetEarBit(Cpu.Tacts);
            if (!earBit)
            {
                portBits = (byte)(portBits & 0b1011_1111);
            }
            return portBits;
        }

        /// <summary>
        /// Sends a byte to the port with the specified address
        /// </summary>
        /// <param name="addr">Port address</param>
        /// <param name="data">Data to write to the port</param>
        /// <returns>Byte read from the memory</returns>
        public override void OnWritePort(ushort addr, byte data)
        {
            // --- Handle I/O contention
            base.OnReadPort(addr);

            // --- Carry out the I/O write operation
            if ((addr & 0x0001) != 0) return;

            _borderDevice.BorderColor = data & 0x07;
            _beeperDevice.ProcessEarBitValue(false, (data & 0x10) != 0);
            _tapeDevice.ProcessMicBit((data & 0x08) != 0);
        }
    }
}