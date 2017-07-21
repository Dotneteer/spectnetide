using Spect.Net.SpectrumEmu.Abstraction;

namespace Spect.Net.SpectrumEmu.Devices.Memory
{
    /// <summary>
    /// This class represents the port device used by the Spectrum 48 virtual machine
    /// </summary>
    public class Spectrum48PortDevice: ISpectrumPortDevice
    {
        private IZ80Cpu _cpu;
        private IBorderDevice _borderDevice;
        private IBeeperDevice _beeperDevice;
        private IKeyboardDevice _keyboardDevice;
        private ITapeDevice _tapeDevice;

        /// <summary>
        /// The virtual machine that hosts the device
        /// </summary>
        public ISpectrumVm HostVm { get; private set; }

        /// <summary>
        /// Signs that the device has been attached to the Spectrum virtual machine
        /// </summary>
        public void OnAttachedToVm(ISpectrumVm hostVm)
        {
            HostVm = hostVm;
            _cpu = hostVm.Cpu;
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
        public byte OnReadPort(ushort addr)
        {
            if ((addr & 0x0001) != 0) return 0xFF;

            var portBits = _keyboardDevice.GetLineStatus((byte)(addr >> 8));
            var earBit = _tapeDevice.GetEarBit(_cpu.Tacts);
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
        public void OnWritePort(ushort addr, byte data)
        {
            if ((addr & 0x0001) == 0)
            {
                _borderDevice.BorderColor = data & 0x07;
                _beeperDevice.ProcessEarBitValue((data & 0x10) != 0);
                _tapeDevice.ProcessMicBitValue((data & 0x08) != 0);
            }
        }

        /// <summary>
        /// Resets this device
        /// </summary>
        public void Reset()
        {
        }
    }
}