using Spect.Net.SpectrumEmu.Abstraction.Devices;

namespace Spect.Net.SpectrumEmu.Devices.Memory
{
    /// <summary>
    /// This class represents the port device used by the Spectrum 48 virtual machine
    /// </summary>
    public class Spectrum48PortDevice: ContendedPortDeviceBase
    {
        private IScreenDevice _screenDevice;
        private IBeeperDevice _beeperDevice;
        private IKeyboardDevice _keyboardDevice;
        private ITapeDevice _tapeDevice;

        /// <summary>
        /// Signs that the device has been attached to the Spectrum virtual machine
        /// </summary>
        public override void OnAttachedToVm(ISpectrumVm hostVm)
        {
            base.OnAttachedToVm(hostVm);
            _screenDevice = hostVm.ScreenDevice;
            _beeperDevice = hostVm.BeeperDevice;
            _keyboardDevice = hostVm.KeyboardDevice;
            _tapeDevice = hostVm.TapeDevice;
        }

        /// <summary>
        /// Reads the port with the specified address
        /// </summary>
        /// <param name="addr">Port address</param>
        /// <returns>Byte read from the port</returns>
        public override byte ReadPort(ushort addr)
        {
            // --- Handle I/O contention
            ContentionWait(addr);

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
        /// Handles Spectrum 48 ports
        /// </summary>
        /// <param name="addr">Port address</param>
        /// <param name="data">Data to write to the port</param>
        public void HandleSpectrum48PortWrites(ushort addr, byte data)
        {
            // --- Handle I/O contention
            ContentionWait(addr);

            // --- Carry out the I/O write operation
            if ((addr & 0x0001) != 0) return;

            _screenDevice.BorderColor = data & 0x07;
            _beeperDevice.ProcessEarBitValue(false, (data & 0x10) != 0);
            _tapeDevice.ProcessMicBit((data & 0x08) != 0);
        }

        /// <summary>
        /// Sends a byte to the port with the specified address
        /// </summary>
        /// <param name="addr">Port address</param>
        /// <param name="data">Data to write to the port</param>
        public override void WritePort(ushort addr, byte data)
        {
            HandleSpectrum48PortWrites(addr, data);
        }
    }
}