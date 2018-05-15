using System;
using Spect.Net.SpectrumEmu.Abstraction.Devices;
// ReSharper disable ArgumentsStyleLiteral

namespace Spect.Net.SpectrumEmu.Devices.Ports
{
    /// <summary>
    /// This class handles the extended memory paging port of Spectrum +3E
    /// </summary>
    public class SpectrumP3ExtMemoryPagePortHandler : PortHandlerBase
    {
        private IMemoryDevice _memoryDevice;
        private byte _selectRomHigh;

        private const ushort PORTMASK = 0b1111_0000_0000_0010;
        private const ushort PORT = 0b0001_0000_0000_0000;

        /// <summary>
        /// Initializes a new port handler with the specified attributes.
        /// </summary>
        /// <param name="parent">Parent device</param>
        public SpectrumP3ExtMemoryPagePortHandler(IPortDevice parent) : base(parent, PORTMASK, PORT, canRead: false)
        {
        }

        /// <summary>
        /// Indicates the paging mode
        /// </summary>
        /// <remarks>
        /// Port 0x1FFD, Bit 0: 
        /// False - normal mode, 
        /// True - special mode
        /// </remarks>
        public bool PagingMode { get; set; }

        /// <summary>
        /// Indicates the special configuration
        /// </summary>
        /// <remarks>
        /// Port 0x1FFD, Bit 1-2:
        /// Used only is special paging mode.
        /// 0: RAM banks 0, 1, 2, and 3 are active for pages 0...3
        /// 1: RAM banks 4, 5, 6, and 7 are active for pages 0...3
        /// 2: RAM banks 4, 5, 6, and 3 are active for pages 0...3
        /// 3: RAM banks 4, 7, 6, and 3 are active for pages 0...3
        /// </remarks>
        public byte SpecialConfig { get; set; }

        /// <summary>
        /// Indicates the low bit of ROM selection
        /// </summary>
        /// <remarks>
        /// Port 0x7FFD, Bit 4:
        /// Used in normal paging mode
        /// 0: Bit is 0
        /// 1: Bit is 1
        /// </remarks>
        public byte SelectRomLow { get; set; }

        /// <summary>
        /// Indicates the high bit of ROM selection
        /// </summary>
        /// <remarks>
        /// Port 0x1FFD, Bit 2:
        /// Used in normal paging mode
        /// 0: Bit is 0
        /// 2: Bit is 1
        /// </remarks>
        public byte SelectRomHigh
        {
            get => _selectRomHigh;
            set
            {
                _selectRomHigh = value;
                RomHighSelectionChanged?.Invoke(value, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Raised when the SelectRomLow property changes
        /// </summary>
        public event EventHandler RomHighSelectionChanged;


        /// <summary>
        /// Indicates the state of the disk motor
        /// </summary>
        /// <remarks>
        /// Port 0x1FFD, Bit 3:
        /// Used in every paging mode
        /// False: motor off
        /// True: motor on
        /// </remarks>
        public bool DiskMotorState { get; set; }

        /// <summary>
        /// Indicates the printer port strobe value
        /// </summary>
        /// <remarks>
        /// Port 0x1FFD, Bit 4: Used in every paging mode
        /// </remarks>
        public bool PrinterPortStrobe { get; set; }

        /// <summary>
        /// The last slot 3 index set
        /// </summary>
        public int LastSlot3Index { get; set; }

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
            PagingMode = false;
            SpecialConfig = 0;
            SelectRomLow = 0;
            SelectRomHigh = 0;
            DiskMotorState = false;
            PrinterPortStrobe = false;
        }

        /// <summary>
        /// Writes the specified value to the port
        /// </summary>
        /// <param name="addr">Full port address</param>
        /// <param name="writeValue">Value to write to the port</param>
        public override void HandleWrite(ushort addr, byte writeValue)
        {
            PagingMode = (writeValue & 0x01) != 0;
            SpecialConfig = (byte)((writeValue >> 1) & 0x03);
            DiskMotorState = (writeValue & 0x08) != 0;
            PrinterPortStrobe = (writeValue & 0x10) != 0;
            SelectRomHigh = (byte)((writeValue >> 1) & 0x02);

            // --- Page the ROM according to current settings
            if (PagingMode)
            {
                // --- Special paging mode
                switch (SpecialConfig)
                {
                    case 0:
                        _memoryDevice.PageIn(0, 0);
                        _memoryDevice.PageIn(1, 1);
                        _memoryDevice.PageIn(2, 2);
                        _memoryDevice.PageIn(3, 3);
                        break;

                    case 1:
                        _memoryDevice.PageIn(0, 4);
                        _memoryDevice.PageIn(1, 5);
                        _memoryDevice.PageIn(2, 6);
                        _memoryDevice.PageIn(3, 7);
                        break;

                    case 2:
                        _memoryDevice.PageIn(0, 4);
                        _memoryDevice.PageIn(1, 5);
                        _memoryDevice.PageIn(2, 6);
                        _memoryDevice.PageIn(3, 3);
                        break;

                    case 3:
                        _memoryDevice.PageIn(0, 4);
                        _memoryDevice.PageIn(1, 7);
                        _memoryDevice.PageIn(2, 6);
                        _memoryDevice.PageIn(3, 3);
                        break;
                }
            }
            else
            {
                // --- Normal mode
                _memoryDevice.PageIn(1, 5);
                _memoryDevice.PageIn(2, 2);
                _memoryDevice.PageIn(3, LastSlot3Index);
                _memoryDevice.SelectRom(SelectRomHigh | SelectRomLow);
            }
        }
    }
}