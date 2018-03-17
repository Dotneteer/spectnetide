using Spect.Net.SpectrumEmu.Abstraction.Devices;
// ReSharper disable ArgumentsStyleLiteral

namespace Spect.Net.SpectrumEmu.Devices.Ports
{
    /// <summary>
    /// This class handles the memory paging port of Spectrum 128.
    /// </summary>
    public class Spectrum128MemoryPagePortHandler : PortHandlerBase
    {
        private IMemoryDevice _memoryDevice;

        private const ushort PORTMASK = 0b1100_0000_0000_0010;
        private const ushort PORT = 0b0100_0000_0000_0000;

        /// <summary>
        /// Initializes a new port handler with the specified attributes.
        /// </summary>
        public Spectrum128MemoryPagePortHandler() : base(PORTMASK, PORT, canRead: false)
        {
        }

        /// <summary>
        /// Signs that the device has been attached to the Spectrum virtual machine
        /// </summary>
        public override void OnAttachedToVm(ISpectrumVm hostVm)
        {
            base.OnAttachedToVm(hostVm);
            _memoryDevice = hostVm.MemoryDevice;
        }

        /// <summary>
        /// Resets this device
        /// </summary>
        public override void Reset()
        {
            base.Reset();
            _memoryDevice.PagingEnabled = true;
        }

        /// <summary>
        /// Writes the specified value to the port
        /// </summary>
        /// <param name="addr">Full port address</param>
        /// <param name="writeValue">Value to write to the port</param>
        public override void HandleWrite(ushort addr, byte writeValue)
        {
            // --- When paging is disabled, it will be enabled next time
            // --- only after reset
            if (!_memoryDevice.PagingEnabled) return;

            // --- Choose the RAM bank for Slot 3 (0xc000-0xffff)
            _memoryDevice.PageIn(3, writeValue & 0x07);

            // --- Choose screen (Bank 5 or 7)
            _memoryDevice.UseShadowScreen = ((writeValue >> 3) & 0x01) == 0x01;

            // --- Choose ROM bank for Slot 0 (0x0000-0x3fff)
            _memoryDevice.SelectRom((writeValue >> 4) & 0x01);

            // --- Enable/disable paging
            _memoryDevice.PagingEnabled = (writeValue & 0x20) == 0x00;
        }
    }
}