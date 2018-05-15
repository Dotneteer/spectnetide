using System;
using Spect.Net.SpectrumEmu.Abstraction.Devices;
// ReSharper disable ArgumentsStyleLiteral

namespace Spect.Net.SpectrumEmu.Devices.Ports
{
    /// <summary>
    /// This class handles the memory paging port of Spectrum +3E
    /// </summary>
    public class SpectrumP3MemoryPagePortHandler : PortHandlerBase
    {
        private IMemoryDevice _memoryDevice;
        private byte _selectRomLow;
        private int _lastSlot3Index;

        private const ushort PORTMASK = 0b1100_0000_0000_0010;
        private const ushort PORT = 0b0100_0000_0000_0000;

        /// <summary>
        /// Initializes a new port handler with the specified attributes.
        /// </summary>
        /// <param name="parent">Parent device</param>
        public SpectrumP3MemoryPagePortHandler(IPortDevice parent) : base(parent, PORTMASK, PORT, canRead: false)
        {
        }

        /// <summary>
        /// Indicates if paging is enabled or not
        /// </summary>
        /// <remarks>
        /// Port 0x7FFD, Bit 5: 
        /// False - paging is enables
        /// True - paging is not enabled and further output to the port
        /// is ignored until the computer is reset
        /// </remarks>
        public bool PagingEnabled { get; set; }

        /// <summary>
        /// Indicates the low bit of ROM selection
        /// </summary>
        /// <remarks>
        /// Port 0x7FFD, Bit 4:
        /// Used in normal paging mode
        /// 0: Bit is 0
        /// 1: Bit is 1
        /// </remarks>
        public byte SelectRomLow
        {
            get => _selectRomLow;
            set
            {
                _selectRomLow = value;
                RomLowSelectionChanged?.Invoke(value, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Raised when the SelectRomLow property changes
        /// </summary>
        public event EventHandler RomLowSelectionChanged;

        /// <summary>
        /// Indicates the high bit of ROM selection
        /// </summary>
        /// <remarks>
        /// Port 0x1FFD, Bit 2:
        /// Used in normal paging mode
        /// 0: Bit is 0
        /// 2: Bit is 1
        /// </remarks>
        public byte SelectRomHigh { get; set; }

        /// <summary>
        /// The last slot 3 index set
        /// </summary>
        public int LastSlot3Index
        {
            get => _lastSlot3Index;
            set
            {
                _lastSlot3Index = value;
                LastSlot3IndexChanged?.Invoke(value, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Raised when the LastSlot3Index property changes
        /// </summary>
        public event EventHandler LastSlot3IndexChanged;

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
            SelectRomLow = 0;
            SelectRomHigh = 0;
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
            LastSlot3Index = writeValue & 0x07;
            _memoryDevice.PageIn(3, LastSlot3Index);

            // --- Choose screen (Bank 5 or 7)
            _memoryDevice.UsesShadowScreen = ((writeValue >> 3) & 0x01) == 0x01;

            // --- Choose ROM bank for Slot 0 (0x0000-0x3fff)
            SelectRomLow = (byte)((writeValue >> 4) & 0x01);

            // --- Enable/disable paging
            _memoryDevice.PagingEnabled = (writeValue & 0x20) == 0x00;

            // --- Page the ROM according to current settings
            _memoryDevice.SelectRom(SelectRomHigh | SelectRomLow);
        }
    }
}