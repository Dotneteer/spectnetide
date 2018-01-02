using Spect.Net.SpectrumEmu.Abstraction.Devices;

namespace Spect.Net.SpectrumEmu.Devices.Memory
{
    /// <summary>
    /// This device represents the Spectrum 48 memory device
    /// </summary>
    public sealed class Spectrum128MemoryDevice: IMemoryDevice
    {
        public const int PAGE_LENGTH = 0x4000;
        public const int RAMBANKS = 8;
        private IZ80Cpu _cpu;
        private IScreenDevice _screenDevice;
        private byte[][] _ramBanks;
        private byte[] _romPage0;
        private byte[] _romPage1;
        private byte[] _currentRomPage;
        private int _currentSlot3Bank;

        /// <summary>
        /// Provides access to the RAM banks
        /// </summary>
        public byte[][] RamBanks => _ramBanks;

        /// <summary>
        /// Provides access to the current ROM page
        /// </summary>
        public byte[] CurrentRom => _currentRomPage;

        /// <summary>
        /// Resets this device by filling the memory with 0xFF
        /// </summary>
        public void Reset()
        {
            for (var i = 0; i < PAGE_LENGTH; i++)
            {
                for (var j = 0; j < RAMBANKS; j++)
                {
                    _ramBanks[j][i] = 0xFF;
                }
            }
            _currentRomPage = _romPage0;
            _currentSlot3Bank = 0;
        }

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
            _cpu = hostVm?.Cpu;
            _screenDevice = hostVm?.ScreenDevice;

            // --- Create the ROM pages
            _romPage0 = new byte[PAGE_LENGTH];
            _romPage1 = new byte[PAGE_LENGTH];

            _ramBanks = new byte[8][];
            // --- Create RAM pages
            for (var i = 0; i < 8; i++)
            {
                _ramBanks[i] = new byte[PAGE_LENGTH];
            }

            _currentRomPage = _romPage0;
            _currentSlot3Bank = 0;
        }

        /// <summary>
        /// The addressable size of the memory
        /// </summary>
        public int AddressableSize => 0x1_0000;

        /// <summary>
        /// The size of a memory page
        /// </summary>
        public int PageSize => 0x4000;

        /// <summary>
        /// Reads the memory at the specified address
        /// </summary>
        /// <param name="addr">Memory address</param>
        /// <param name="noContention">Indicates non-contended read operation</param>
        /// <returns>Byte read from the memory</returns>
        public byte Read(ushort addr, bool noContention = false)
        {
            var memIndex = addr & 0x3FFF;
            byte memValue;
            switch (addr & 0xC000)
            {
                case 0x0000:
                    return _currentRomPage[memIndex];
                case 0x4000:
                    memValue = _ramBanks[5][memIndex];
                    if (noContention || _screenDevice == null) return memValue;
                    _cpu?.Delay(_screenDevice.GetContentionValue(HostVm.CurrentFrameTact));
                    return memValue;
                case 0x8000:
                    return _ramBanks[2][memIndex];
                default:
                    memValue = _ramBanks[_currentSlot3Bank][memIndex];
                    if ((_currentSlot3Bank & 0x01) == 0) return memValue;

                    // --- Bank 1, 3, 5, and 7 are contended
                    _cpu?.Delay(_screenDevice.GetContentionValue(HostVm.CurrentFrameTact));
                    return memValue;
            }
        }

        /// <summary>
        /// Sets the memory value at the specified address
        /// </summary>
        /// <param name="addr">Memory address</param>
        /// <param name="value">Memory value to write</param>
        /// <returns>Byte read from the memory</returns>
        public void Write(ushort addr, byte value)
        {
            // ReSharper disable once SwitchStatementMissingSomeCases
            var memIndex = addr & 0x3FFF;
            switch (addr & 0xC000)
            {
                case 0x0000:
                    // --- ROM cannot be overwritten
                    return;
                case 0x4000:
                    _cpu?.Delay(_screenDevice.GetContentionValue(HostVm.CurrentFrameTact));
                    _ramBanks[5][memIndex] = value;
                    break;
                case 0x8000:
                    _ramBanks[2][memIndex] = value;
                    break;
                default:
                    if ((_currentSlot3Bank & 0x01) != 0)
                    {
                        // --- Bank 1, 3, 5, and 7 are contended
                        _cpu?.Delay(_screenDevice.GetContentionValue(HostVm.CurrentFrameTact));
                    }
                    _ramBanks[_currentSlot3Bank][memIndex] = value;
                    break;
            }
        }

        /// <summary>
        /// Gets the buffer that holds memory data
        /// </summary>
        /// <returns></returns>
        public byte[] CloneMemory()
        {
            var clone = new byte[AddressableSize];
            _currentRomPage.CopyTo(clone, 0x0000);
            _ramBanks[5].CopyTo(clone, 0x4000);
            _ramBanks[2].CopyTo(clone, 0x8000);
            _ramBanks[_currentSlot3Bank].CopyTo(clone, 0xC000);
            return clone;
        }

        /// <summary>
        /// Fills up the memory from the specified buffer
        /// </summary>
        /// <param name="buffer">Contains the row data to fill up the memory</param>
        public void CopyRom(byte[] buffer)
        {
            buffer?.CopyTo(_currentRomPage, 0);
        }

        /// <summary>
        /// Selects the ROM with the specified index
        /// </summary>
        /// <param name="romIndex">Index of the ROM</param>
        public void SelectRom(int romIndex)
        {
            _currentRomPage = romIndex == 0
                ? _romPage0
                : _romPage1;
        }

        /// <summary>
        /// Retrieves the index of the selected ROM
        /// </summary>
        /// <returns>The index of the selected ROM</returns>
        public int GetSelectedRomIndex() => 
            _currentRomPage == _romPage0 ? 0 : 1;

        /// <summary>
        /// Pages in the selected bank into the specified slot
        /// </summary>
        /// <param name="slot">Index of the slot</param>
        /// <param name="bank">Index of the bank to page in</param>
        public void PageIn(int slot, int bank)
        {
            if (slot != 3) return;
            _currentSlot3Bank = bank & 0x07;
        }

        /// <summary>
        /// Gets the bank paged in to the specified slot
        /// </summary>
        /// <param name="slot">Slot index</param>
        /// <returns>
        /// The index of the bank that is pages into the slot
        /// </returns>
        public int GetSelectedBankIndex(int slot)
        {
            switch (slot & 0x03)
            {
                case 0: return 0;
                case 1: return 5;
                case 2: return 2;
                default: return _currentSlot3Bank;
            }
        }

        /// <summary>
        /// Indicates of shadow screen should be used
        /// </summary>
        public bool UseShadowScreen { get; set; }

        /// <summary>
        /// Gets the data for the specfied ROM page
        /// </summary>
        /// <param name="romIndex">Index of the ROM</param>
        /// <returns>
        /// The buffer that holds the binary data for the specified ROM page
        /// </returns>
        public byte[] GetRomBuffer(int romIndex) => romIndex == 0 ? _romPage0 : _romPage1;

        /// <summary>
        /// Gets the data for the specfied RAM bank
        /// </summary>
        /// <param name="bankIndex">Index of the RAM bank</param>
        /// <returns>
        /// The buffer that holds the binary data for the specified RAM bank
        /// </returns>
        public byte[] GetRamBank(int bankIndex) => _ramBanks[bankIndex & 0x07];

        /// <summary>
        /// Gets the location of the address
        /// </summary>
        /// <param name="addr">Address to check the location</param>
        /// <returns>
        /// IsInRom: true, if the address is in ROM
        /// Index: ROM/RAM bank index
        /// Address: Index within the bank
        /// </returns>
        public (bool IsInRom, int Index, ushort Address) GetAddressLocation(ushort addr)
        {
            var bankAddr = (ushort)(addr & 0x3FFF);
            switch (addr & 0xC000)
            {
                case 0x0000:
                    return (true, _currentRomPage == _romPage0 ? 0 : 1, addr);
                case 0x4000:
                    return (false, 5, bankAddr);
                case 0x8000:
                    return (false, 2, bankAddr);
                default:
                    return (false, _currentSlot3Bank, bankAddr);
            }
        }

        /// <summary>
        /// Checks if the RAM bank with the specified index is paged in
        /// </summary>
        /// <param name="index">RAM bank index</param>
        /// <param name="baseAddress">Base memory address, provided the bank is paged in</param>
        /// <returns>True, if the bank is paged in; otherwise, false</returns>
        public bool IsRamBankPagedIn(int index, out ushort baseAddress)
        {
            if (index == 2)
            {
                baseAddress = 0x8000;
                return true;
            }
            if (index == 5)
            {
                baseAddress = 0x4000;
                return true;
            }
            if (index == _currentSlot3Bank)
            {
                baseAddress = 0xC000;
                return true;
            }
            baseAddress = 0;
            return false;
        }
    }
}