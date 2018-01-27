namespace Spect.Net.SpectrumEmu.Devices.Memory
{
    /// <summary>
    /// This class represents the port device used by the Spectrum +3 virtual machine
    /// </summary>
    public class SpectrumP3PortDevice : Spectrum128PortDevice
    {
        /// <summary>
        /// Indicates the paging mode
        /// </summary>
        /// <remarks>
        /// Port 0x1FFD, Bit 0: 
        /// False - normal mode, 
        /// True - special mode
        /// </remarks>
        public bool PagingMode { get; private set; }

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
        public byte SpecialConfig { get; private set; }

        /// <summary>
        /// Indicates the low bit of ROM selection
        /// </summary>
        /// <remarks>
        /// Port 0x7FFD, Bit 4:
        /// Used in normal paging mode
        /// 0: Bit is 0
        /// 1: Bit is 1
        /// </remarks>
        public byte SelectRomLow { get; private set; }

        /// <summary>
        /// Indicates the high bit of ROM selection
        /// </summary>
        /// <remarks>
        /// Port 0x1FFD, Bit 2:
        /// Used in normal paging mode
        /// 0: Bit is 0
        /// 2: Bit is 1
        /// </remarks>
        public byte SelectRomHigh { get; private set; }

        /// <summary>
        /// Indicates the state of the disk motor
        /// </summary>
        /// <remarks>
        /// Port 0x1FFD, Bit 3:
        /// Used in every paging mode
        /// False: motor off
        /// True: motor on
        /// </remarks>
        public bool DiskMotorState { get; private set; }

        /// <summary>
        /// Indicates the printer port strobe value
        /// </summary>
        /// <remarks>
        /// Port 0x1FFD, Bit 4: Used in every paging mode
        /// </remarks>
        public bool PrinterPortStrobe { get; private set; }

        /// <summary>
        /// The last slot 3 index set
        /// </summary>
        public int LastSlot3Index { get; private set; }
        
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
        /// Sends a byte to the port with the specified address
        /// </summary>
        /// <param name="addr">Port address</param>
        /// <param name="data">Data to write to the port</param>
        /// <returns>Byte read from the memory</returns>
        public override void WritePort(ushort addr, byte data)
        {
            HandleSpectrum48PortWrites(addr, data);

            // --- Port 0x7FFD, bit 14 set, bit 15 and bit 1 reset
            if ((addr & 0xC002) == 0x4000)
            {
                // --- When paging is disabled, it will be enabled next time
                // --- only after reset
                if (PagingEnabled)
                {
                    // --- Choose the RAM bank for Slot 3 (0xc000-0xffff)
                    LastSlot3Index = data & 0x07;
                    MemoryDevice.PageIn(3, LastSlot3Index);

                    // --- Choose screen (Bank 5 or 7)
                    MemoryDevice.UseShadowScreen = ((data >> 3) & 0x01) == 0x01;

                    // --- Choose ROM bank for Slot 0 (0x0000-0x3fff)
                    SelectRomLow = (byte) ((data >> 4) & 0x01);

                    // --- Enable/disable paging
                    PagingEnabled = (data & 0x20) == 0x00;

                    // --- Page the ROM according to current settings
                    MemoryDevice.SelectRom(SelectRomHigh | SelectRomLow);
                }
            }
            // --- Port 0x1FFD, bit 12 set, bit 1, 13, 14 and 15 reset
            else if ((addr & 0xF002) == 0x1000)
            {
                PagingMode = (data & 0x01) != 0;
                SpecialConfig = (byte) ((data >> 1) & 0x03);
                DiskMotorState = (data & 0x08) != 0;
                PrinterPortStrobe = (data & 0x10) != 0;
                SelectRomHigh = (byte)((data >> 1) & 0x02);

                // --- Page the ROM according to current settings
                if (PagingMode)
                {
                    // --- Special paging mode
                    switch (SpecialConfig)
                    {
                        case 0:
                            MemoryDevice.PageIn(0, 0);
                            MemoryDevice.PageIn(1, 1);
                            MemoryDevice.PageIn(2, 2);
                            MemoryDevice.PageIn(3, 3);
                            break;

                        case 1:
                            MemoryDevice.PageIn(0, 4);
                            MemoryDevice.PageIn(1, 5);
                            MemoryDevice.PageIn(2, 6);
                            MemoryDevice.PageIn(3, 7);
                            break;

                        case 2:
                            MemoryDevice.PageIn(0, 4);
                            MemoryDevice.PageIn(1, 5);
                            MemoryDevice.PageIn(2, 6);
                            MemoryDevice.PageIn(3, 3);
                            break;

                        case 3:
                            MemoryDevice.PageIn(0, 4);
                            MemoryDevice.PageIn(1, 7);
                            MemoryDevice.PageIn(2, 6);
                            MemoryDevice.PageIn(3, 3);
                            break;
                    }
                }
                else
                {
                    // --- Normal mode
                    MemoryDevice.PageIn(1, 5);
                    MemoryDevice.PageIn(2, 2);
                    MemoryDevice.PageIn(3, LastSlot3Index);
                    MemoryDevice.SelectRom(SelectRomHigh | SelectRomLow);
                }
            }
            else if (addr == 0xFFFD)
            {
                SoundDevice.SetRegisterIndex(data);
            }
            else if (addr == 0xBFFD || addr == 0xBEFD)
            {
                SoundDevice.SetRegisterValue(data);
            }
        }

        /// <summary>
        /// Ports are not contended.
        /// </summary>
        /// <param name="addr">Contention address</param>
        /// <remarks>
        /// Standard N:4 delay
        /// </remarks>
        public override void ContentionWait(ushort addr)
        {
            Cpu.Delay(4);
        }
    }
}