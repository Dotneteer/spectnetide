using Spect.Net.SpectrumEmu.Abstraction.Devices;

namespace Spect.Net.SpectrumEmu.Devices.Memory
{
    /// <summary>
    /// This class represents the port device used by the Spectrum 128 virtual machine
    /// </summary>
    public class Spectrum128PortDevice: Spectrum48PortDevice
    {
        private IMemoryDevice _memoryDevice;
        private ISoundDevice _soundDevice;

        /// <summary>
        /// Indicates if paging is enabled or not
        /// </summary>
        public bool PagingEnabled { get; private set; }

        /// <summary>
        /// Signs that the device has been attached to the Spectrum virtual machine
        /// </summary>
        public override void OnAttachedToVm(ISpectrumVm hostVm)
        {
            base.OnAttachedToVm(hostVm);
            _memoryDevice = hostVm.MemoryDevice;
            _soundDevice = hostVm.SoundDevice;
        }

        /// <summary>
        /// Reads the port with the specified address
        /// </summary>
        /// <param name="addr">Port address</param>
        /// <returns>Byte read from the port</returns>
        public override byte OnReadPort(ushort addr)
        {
            // --- Handle contention + 0xFE port
            var result = base.OnReadPort(addr);
            return addr == 0xFFFD 
                ? _soundDevice.GetRegisterValue() 
                : result;
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
            base.OnWritePort(addr, data);

            // --- Carry out the I/O write operation (bit 15 and bit 1 reset)
            if ((addr & 0x8002) == 0)
            {
                // --- When paging is disabled, it will be enabled next time
                // --- only after reset
                if (PagingEnabled)
                {
                    // --- Choose the RAM bank for Slot 3 (0xc000-0xffff)
                    _memoryDevice.PageIn(3, data & 0x07);

                    // --- Choose screen (Bank 5 or 7)
                    _memoryDevice.UseShadowScreen = ((data >> 3) & 0x01) == 0x01;

                    // --- Choose ROM bank for Slot 0 (0x0000-0x3fff)
                    _memoryDevice.SelectRom((data >> 4) & 0x01);

                    // --- Enable/disable paging
                    PagingEnabled = (data & 0x20) == 0x00;
                }
            }
            else if (addr == 0xFFFD)
            {
                _soundDevice.SetRegisterIndex(data);
            }
            else if (addr == 0xBFFD || addr == 0xBEFD)
            {
                _soundDevice.SetRegisterValue(data);
            }
        }

        /// <summary>
        /// Resets this device
        /// </summary>
        public override void Reset()
        {
            base.Reset();
            PagingEnabled = true;
        }
    }
}