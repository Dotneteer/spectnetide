using System.Linq;
using Spect.Net.SpectrumEmu.Abstraction.Configuration;
using Spect.Net.SpectrumEmu.Abstraction.Devices;

namespace Spect.Net.SpectrumEmu.Devices.Memory
{
    /// <summary>
    /// This class represents the Spectrum +3 memory device
    /// </summary>
    public class SpectrumNextMemoryDevice : 
        ContendedMemoryDeviceBase,
        IMemoryDivIdeTestSupport
    {
        private const int DIVIDE_ROM_PAGE_INDEX = 4 * 2;

        private IRomConfiguration _romConfig;
        private INextFeatureSetDevice _nextDevice;
        private IDivIdeDevice _divIdeDevice;

        private byte[][] _ramPages;
        private byte[][] _romPages;
        private byte[][] _divIdeBanks;
        private int[] _slots16;
        private int[] _slots8;
        private int _selectedRomIndex;
        private bool _isInAllRamMode;
        private bool _isIn8KMode;

        /// <summary>
        /// Indicates special mode: special RAM paging
        /// </summary>
        public override bool IsInAllRamMode => _isInAllRamMode;

        /// <summary>
        /// Indicates if the device is in 8K mode
        /// </summary>
        public override bool IsIn8KMode => _isIn8KMode;

        /// <summary>
        /// The number of RAM pages available
        /// </summary>
        public int RamPageCount { get; private set; }

        /// <summary>
        /// Signs that the device has been attached to the Spectrum virtual machine
        /// </summary>
        public override void OnAttachedToVm(ISpectrumVm hostVm)
        {
            base.OnAttachedToVm(hostVm);
            _romConfig = hostVm.RomConfiguration;
            _nextDevice = hostVm.NextDevice;
            _divIdeDevice = hostVm.DivIdeDevice;

            // --- Create space for ROM pages (use 10 x 8K pages)
            var romCount = _romConfig.NumberOfRoms * 2;
            _romPages = new byte[romCount][];
            for (var i = 0; i < romCount; i++)
            {
                _romPages[i] = new byte[0x2000];
            }

            // --- Obtain Next memory size
            var memSize = HostVm.MemoryConfiguration.NextMemorySize;
            if (memSize < 1024)
            {
                memSize = 512;
            }
            else if (memSize >= 1024 && memSize < 1536)
            {
                memSize = 1024;
            }
            else if (memSize >= 1536 && memSize < 2048)
            {
                memSize = 1536;
            }
            else
            {
                memSize = 2048;
            }

            // --- Calculate number of RAM pages
            RamPageCount = 16 + 64 * (memSize - 512) / 512;

            // --- Allocate RAM
            _ramPages = new byte[RamPageCount][];
            for (var i = 0; i < RamPageCount; i++)
            {
                _ramPages[i] = new byte[0x2000];
            }

            // --- Create DivIDE RAM
            _divIdeBanks = new byte[4][];
            for (var i = 0; i < 4; i++)
            {
                _divIdeBanks[i] = new byte[0x2000];
            }
            Reset();
        }

        /// <summary>
        /// Initialize memory
        /// </summary>
        public override void Reset()
        {
            // --- Set up initial RAM slot values
            _slots16 = new[] { 0, 5, 2, 0 };
            _slots8 = new[] { 0xFF, 0xFF, 10, 11, 4, 5, 0, 1 };

            // --- Set up modes
            _isInAllRamMode = false;
            _isIn8KMode = false;
            _selectedRomIndex = 0;
        }

        /// <summary>
        /// Reads the memory at the specified address
        /// </summary>
        /// <param name="addr">Memory address</param>
        /// <param name="suppressContention">Indicates non-contended read operation</param>
        /// <returns>Byte read from the memory</returns>
        public override byte Read(ushort addr, bool suppressContention = false)
        {
            var memIndex = addr & 0x1FFF;
            var slotOffset = (addr >> 13) & 0x01;
            int slotIndex;
            var romIndex = _selectedRomIndex * 2 + slotOffset;
            var isRam = IsInAllRamMode;

            if (IsInAllRamMode || !IsIn8KMode)
            {
                // --- We use 16K banks
                slotIndex = 2 * _slots16[(byte)(addr >> 14)] + slotOffset;
            }
            else
            {
                // --- We use 8K banks
                slotIndex = _slots8[(byte)(addr >> 13)];
                isRam = slotIndex != 0xFF;
            }
            switch (addr & 0xE000)
            {
                // --- 8K Bank #0
                case 0x0000:
                    // --- Check for DivIDE ROM page
                    if (_divIdeDevice?.IsDivIdePagedIn ?? false)
                    {
                        // --- Check for DivIDE RAM page
                        if (_divIdeDevice?.MapRam ?? false)
                        {
                            // --- MAPRAM flag od DivIDE is set, page in RAM 3
                            return _divIdeBanks[3][memIndex];
                        }

                        return _romPages[DIVIDE_ROM_PAGE_INDEX][memIndex];
                    }

                    // --- Check 8K RAM mode
                    if (isRam)
                    {
                        return slotIndex >= _ramPages.Length
                            ? (byte) 0xFF
                            : _ramPages[slotIndex][memIndex];
                    }

                    // --- The selected ROM is paged in
                    return _romPages[romIndex][memIndex];

                // --- 8K Bank #1
                case 0x02000:
                    // --- DivIDE Ram is page in
                    if (_divIdeDevice?.IsDivIdePagedIn ?? false)
                    {
                        return _divIdeBanks[_divIdeDevice.Bank][memIndex];
                    }

                    // --- A 8K RAM page is used
                    if (isRam)
                    {
                        return slotIndex >= _ramPages.Length
                            ? (byte) 0xFF
                            : _ramPages[slotIndex][memIndex];
                    }

                    // --- The selected ROM is used
                    return _romPages[romIndex][memIndex];

                // --- 8K Bank #2, #3
                case 0x4000:
                case 0x0600:
                    if (!suppressContention && ScreenDevice != null)
                    {
                        ApplyDelay();
                    }
                    if (slotIndex >= _ramPages.Length) return 0xFF;
                    return _ramPages[slotIndex][memIndex];

                // --- 8K Bank #4, #5
                case 0x8000:
                case 0xA000:
                    if (slotIndex >= _ramPages.Length) return 0xFF;
                    return _ramPages[slotIndex][memIndex];

                // --- 8K Bank #6, #7
                default:
                    // --- Bank 4, 5, 6, and 7 are contended
                    if (_slots16[3] >= 4 && ScreenDevice != null)
                    {
                        ApplyDelay();
                    }
                    if (slotIndex >= _ramPages.Length) return 0xFF;
                    return _ramPages[slotIndex][memIndex];
            }
        }

        /// <summary>
        /// Sets the memory value at the specified address
        /// </summary>
        /// <param name="addr">Memory address</param>
        /// <param name="value">Memory value to write</param>
        /// <param name="supressContention">
        /// Indicates non-contended write operation
        /// </param>
        public override void Write(ushort addr, byte value, bool supressContention = false)
        {
            var memIndex = addr & 0x1FFF;
            var slotOffset = (addr >> 13) & 0x01;
            int slotIndex;
            var isRam = IsInAllRamMode;

            if (IsInAllRamMode || !IsIn8KMode)
            {
                // --- We use 16K banks
                slotIndex = 2 * _slots16[(byte)(addr >> 14)] + slotOffset;
            }
            else
            {
                // --- We use 8K banks
                slotIndex = _slots8[(byte)(addr >> 13)];
                isRam = slotIndex != 0xFF;
            }

            switch (addr & 0xE000)
            {
                // --- 8K Bank #0
                case 0x0000:

                    if (isRam && slotIndex < _ramPages.Length)
                    {
                        _ramPages[slotIndex][memIndex] = value;
                    }
                    return;

                // --- 8K Bank #1
                case 0x2000:
                    // --- Check for DivIDE paged in
                    if (_divIdeDevice?.IsDivIdePagedIn ?? false)
                    {
                        _divIdeBanks[_divIdeDevice.Bank][memIndex] = value;
                        return;
                    }

                    // --- Check for 8K RAM mode
                    if (isRam && slotIndex < _ramPages.Length)
                    {
                        _ramPages[slotIndex][memIndex] = value;
                    }
                    return;

                // --- 8K Bank #2, #3
                case 0x4000:
                case 0x6000:
                    if (!supressContention)
                    {
                        ApplyDelay();
                    }
                    if (slotIndex < _ramPages.Length)
                    {
                        _ramPages[slotIndex][memIndex] = value;
                    }
                    return;

                // --- 8K Bank #4, #5
                case 0x8000:
                case 0xA000:
                    if (slotIndex < _ramPages.Length)
                    {
                        _ramPages[slotIndex][memIndex] = value;
                    }
                    return;

                // --- 8K Bank #6, #7
                default:
                    // --- Bank 4, 5, 6, and 7 are contended
                    if (_slots16[3] >= 4 && ScreenDevice != null)
                    {
                        if (!supressContention)
                        {
                            ApplyDelay();
                        }
                    }
                    if (slotIndex < _ramPages.Length)
                    {
                        _ramPages[slotIndex][memIndex] = value;
                    }
                    return;
            }
        }

        /// <summary>
        /// Gets the buffer that holds memory data
        /// </summary>
        /// <returns></returns>
        public override byte[] CloneMemory()
        {
            var clone = new byte[0x10000];

            // --- Iterate through all 8K memory slices
            for (var addr = 0x0000; addr < 0x10000; addr += 0x2000)
            {
                // --- Prepare slice information
                byte[] toCopy = null;
                var slotOffset = (addr >> 13) & 0x01;
                int slotIndex;
                var romIndex = _selectedRomIndex * 2 + slotOffset;
                var isRam = IsInAllRamMode;

                if (IsInAllRamMode || !IsIn8KMode)
                {
                    // --- We use 16K banks
                    slotIndex = 2 * _slots16[(byte)(addr >> 14)] + slotOffset;
                }
                else
                {
                    // --- We use 8K banks
                    slotIndex = _slots8[(byte)(addr >> 13)];
                    isRam = slotIndex != 0xFF;
                }

                // --- Determine the slice to copy for each 8K page
                switch (addr & 0xE000)
                {
                    // --- 8K Bank #0
                    case 0x0000:
                        // --- Check for DivIDE ROM page
                        if (_divIdeDevice?.IsDivIdePagedIn ?? false)
                        {
                            // --- Check for DivIDE RAM page
                            if (_divIdeDevice?.MapRam ?? false)
                            {
                                // --- MAPRAM flag od DivIDE is set, page in RAM 3
                                toCopy = _divIdeBanks[3];
                            }
                            else
                            {
                                toCopy = _romPages[DIVIDE_ROM_PAGE_INDEX];
                            }
                        }
                        else if (isRam)
                        {
                            // --- Check 8K RAM mode
                            if (slotIndex < _ramPages.Length)
                            {
                                toCopy = _ramPages[slotIndex];
                            }
                        }
                        else
                        {
                            // --- The selected ROM is paged in
                            toCopy = _romPages[romIndex];
                        }
                        break;

                    // --- 8K Bank #1
                    case 0x02000:
                        if (_divIdeDevice?.IsDivIdePagedIn ?? false)
                        {
                            // --- DivIDE Ram is page in
                            toCopy = _divIdeBanks[_divIdeDevice.Bank];
                        }
                        else if (isRam)
                        {
                            // --- A 8K RAM page is used
                            if (slotIndex < _ramPages.Length)
                            {
                                toCopy = _ramPages[slotIndex];
                            }
                        }
                        else
                        {
                            // --- The selected ROM is used
                            toCopy = _romPages[romIndex];
                        }
                        break;

                    // --- 8K Bank #2, #3, #4, #5, #6, #7
                    default:
                        if (slotIndex < _ramPages.Length)
                        {
                            toCopy = _ramPages[slotIndex];
                        }
                        break;
                }

                if (toCopy == null)
                {
                    // --- Non-existing memory page
                    for (var i = 0; i < 0x2000; i++)
                    {
                        clone[addr + i] = 0xFF;
                    }
                }
                else
                {
                    toCopy.CopyTo(clone, addr);
                }
            }
            return clone;
        }

        ///// <summary>
        ///// Gets the buffer that holds memory data
        ///// </summary>
        ///// <returns></returns>
        //public byte[] CloneMemoryOld()
        //{
        //    var clone = new byte[0x10000];
        //    if (!IsIn8KMode || IsInAllRamMode)
        //    {
        //        // --- We use 16K slots
        //        for (var i = 0; i <= 3; i++)
        //        {
        //            var cloneAddr = (ushort)(i * 0x4000);
        //            var addrInfo = GetAddressLocation(cloneAddr);
        //            if (addrInfo.IsInRom)
        //            {
        //                if (_divIdeDevice?.IsDivIdePagedIn ?? false)
        //                {
        //                    _romPages[DIVIDE_ROM_PAGE_INDEX].CopyTo(clone, cloneAddr);
        //                }
        //                else
        //                {
        //                    _romPages[addrInfo.Index * 2].CopyTo(clone, cloneAddr);
        //                }
        //                _romPages[addrInfo.Index * 2 + 1].CopyTo(clone, cloneAddr + 0x2000);
        //            }
        //            else
        //            {
        //                _ramPages[addrInfo.Index * 2].CopyTo(clone, cloneAddr);
        //                _ramPages[addrInfo.Index * 2 + 1].CopyTo(clone, cloneAddr + 0x2000);
        //            }
        //        }
        //    }
        //    else
        //    {
        //        // --- We use 8K slots
        //        for (var i = 0; i <= 7; i++)
        //        {
        //            var cloneAddr = (ushort)(i * 0x2000);
        //            var addrInfo = GetAddressLocation(cloneAddr);
        //            if (addrInfo.IsInRom)
        //            {
        //                _romPages[_selectedRomIndex * 2 + i % 2].CopyTo(clone, cloneAddr + 0x2000 * (i % 2));
        //            }
        //            else
        //            {
        //                if (addrInfo.Index >= _ramPages.Length)
        //                {
        //                    // --- RAM page is unavailable
        //                    for (var j = 0; j < 0x2000; j++)
        //                    {
        //                        clone[cloneAddr + j] = 0xFF;
        //                    }
        //                }
        //                else
        //                {
        //                    _ramPages[addrInfo.Index].CopyTo(clone, cloneAddr);
        //                }
        //            }
        //        }
        //    }
        //    return clone;
        //}

        /// <summary>
        /// Fills up the memory from the specified buffer
        /// </summary>
        /// <param name="buffer">Contains the row data to fill up the memory</param>
        public override void CopyRom(byte[] buffer)
        {
            var firstPage = buffer.Take(0x2000).ToArray();
            var secondPage = buffer.Skip(0x2000).Take(0x2000).ToArray();
            firstPage.CopyTo(_romPages[_selectedRomIndex * 2], 0);
            if (buffer.Length > 0x2000)
            {
                secondPage.CopyTo(_romPages[_selectedRomIndex * 2 + 1], 0);
            }
        }

        /// <summary>
        /// Selects the ROM with the specified index
        /// </summary>
        /// <param name="romIndex">Index of the ROM</param>
        public override void SelectRom(int romIndex)
        {
            if (romIndex < 0)
            {
                romIndex = 0;
            }
            if (romIndex >= _romConfig.NumberOfRoms)
            {
                romIndex = _romConfig.NumberOfRoms - 1;
            }
            _selectedRomIndex = romIndex;
            _isInAllRamMode = false;
        }

        /// <summary>
        /// Retrieves the index of the selected ROM
        /// </summary>
        /// <returns>The index of the selected ROM</returns>
        public override int GetSelectedRomIndex() => _selectedRomIndex;

        /// <summary>
        /// Pages in the selected bank into the specified slot
        /// </summary>
        /// <param name="slot">Index of the slot</param>
        /// <param name="bank">Index of the bank to page in</param>
        /// <param name="bank16Mode">
        /// True: 16K banks; False: 8K banks
        /// </param>
        public override void PageIn(int slot, int bank, bool bank16Mode = true)
        {
            if (bank16Mode)
            {
                _isIn8KMode = false;
                slot &= 0x03;
                _slots16[slot] = bank;
                if (slot != 3)
                {
                    _isInAllRamMode = true;
                }
                _slots8[slot * 2] = bank * 2;
                _slots8[slot * 2 + 1] = bank * 2 + 1;
                _nextDevice.Sync16KSlot(slot, bank);
            }
            else
            {
                _isIn8KMode = true;
                slot &= 0x07;
                _slots8[slot] = bank;
            }
        }

        /// <summary>
        /// Gets the bank paged in to the specified slot
        /// </summary>
        /// <param name="slot">Slot index</param>
        /// <param name="bank16Mode">
        /// True: 16K banks; False: 8K banks
        /// </param>
        /// <returns>
        /// The index of the bank that is pages into the slot
        /// </returns>
        public override int GetSelectedBankIndex(int slot, bool bank16Mode = true)
            => bank16Mode ? _slots16[slot & 0x03] : _slots8[slot & 0x07];

        /// <summary>
        /// Gets the data for the specfied ROM page
        /// </summary>
        /// <param name="romIndex">Index of the ROM</param>
        /// <returns>
        /// The buffer that holds the binary data for the specified ROM page
        /// </returns>
        public override byte[] GetRomBuffer(int romIndex)
        {
            if (romIndex < 0) romIndex = 0;
            else if (romIndex >= _romConfig.NumberOfRoms) romIndex = _romConfig.NumberOfRoms - 1;

            var rom = new byte[0x4000];
            _romPages[romIndex * 2].CopyTo(rom, 0x0000);
            _romPages[romIndex * 2 + 1].CopyTo(rom, 0x2000);
            return rom;
        }

        /// <summary>
        /// Gets the data for the specfied RAM bank
        /// </summary>
        /// <param name="bankIndex">Index of the RAM bank</param>
        /// <param name="bank16Mode">
        /// True: 16K banks; False: 8K banks
        /// </param>
        /// <returns>
        /// The buffer that holds the binary data for the specified RAM bank
        /// </returns>
        public override byte[] GetRamBank(int bankIndex, bool bank16Mode = true)
        {
            if (bank16Mode)
            {
                if (bankIndex < 0) bankIndex = 0;
                else if (bankIndex >= 4) bankIndex = 3;

                var ram = new byte[0x4000];
                _ramPages[bankIndex * 2].CopyTo(ram, 0x0000);
                _ramPages[bankIndex * 2 + 1].CopyTo(ram, 0x2000);
                return ram;
            }
            else
            {
                if (bankIndex < 0) bankIndex = 0;
                var ram = new byte[0x2000];
                if (bankIndex < _ramPages.Length)
                {
                    _ramPages[bankIndex].CopyTo(ram, 0x0000);
                }
                else
                {
                    // --- Non-existing RAM pages returns 0xFFs
                    for (var i = 0; i < 0x2000; i++)
                    {
                        ram[i] = 0xFF;
                    }
                }
                return ram;
            }
        }

        /// <summary>
        /// Gets the location of the address
        /// </summary>
        /// <param name="addr">Address to check the location</param>
        /// <returns>
        /// IsInRom: true, if the address is in ROM
        /// Index: ROM/RAM bank index
        /// Address: Index within the bank
        /// </returns>
        public override (bool IsInRom, int Index, ushort Address) GetAddressLocation(ushort addr)
        {
            if (!IsIn8KMode || IsInAllRamMode)
            {
                // --- We use 16K banks
                var bankAddr = (ushort)(addr & 0x3FFF);
                switch (addr & 0xC000)
                {
                    case 0x0000:
                        return IsInAllRamMode
                            ? (false, _slots16[0], addr)
                            : (true, _selectedRomIndex, addr);
                    case 0x4000:
                        return (false, _slots16[1], bankAddr);
                    case 0x8000:
                        return (false, _slots16[2], bankAddr);
                    default:
                        return (false, _slots16[3], bankAddr);
                }
            }
            else
            {
                // --- We use 8K banks
                var bankAddr = (ushort)(addr & 0x1FFF);
                var slotIndex = addr >> 13;
                var bankIndex = _slots8[slotIndex];
                return bankIndex == 0xFF
                    ? (true, _selectedRomIndex, bankAddr)
                    : (false, bankIndex, bankAddr);
            }
        }

        /// <summary>
        /// Checks if the RAM bank with the specified index is paged in
        /// </summary>
        /// <param name="index">RAM bank index</param>
        /// <param name="baseAddress">Base memory address, provided the bank is paged in</param>
        /// <returns>True, if the bank is paged in; otherwise, false</returns>
        public override bool IsRamBankPagedIn(int index, out ushort baseAddress)
        {
            baseAddress = 0x0000;
            if (IsInAllRamMode || !IsIn8KMode)
            {
                if (IsInAllRamMode && _slots16[0] == index)
                {
                    return true;
                }
                if (_slots16[1] == index)
                {
                    baseAddress = 0x4000;
                    return true;
                }
                if (_slots16[2] == index)
                {
                    baseAddress = 0x8000;
                    return true;
                }
                if (_slots16[3] == index)
                {
                    baseAddress = 0xC000;
                    return true;
                }
                baseAddress = 0;
                return false;
            }

            // --- We're in 8K mode
            for (var i = 0; i < 8; i++)
            {
                if (_slots8[i] != index) continue;

                baseAddress = (ushort)(0x2000 * i);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Gets the state of the device so that the state can be saved
        /// </summary>
        /// <returns>The object that describes the state of the device</returns>
        public override IDeviceState GetState() => null;

        /// <summary>
        /// Sets the state of the device from the specified object
        /// </summary>
        /// <param name="state">Device state</param>
        public override void RestoreState(IDeviceState state)
        {
        }

        #region Test helpers

        /// <summary>
        /// Fills each 8K RAM page with the same value as the page index
        /// </summary>
        /// <remarks>Use this method for testing purposes</remarks>
        public void FillRamWithTestPattern()
        {
            for (var i = 0; i < _ramPages.Length; i++)
            {
                for (var j = 0; j < 0x2000; j++)
                {
                    _ramPages[i][j] = (byte)i;
                }
            }
        }

        /// <summary>
        /// Allows access to the DivIde RAM memory
        /// </summary>
        /// <param name="bank">Bank index</param>
        /// <param name="addr">Memory address</param>
        /// <returns>RAM byte</returns>
        byte IMemoryDivIdeTestSupport.this[int bank, ushort addr]
        {
            get => _divIdeBanks[bank & 0x03][(ushort)(addr & 0x1FFF)];
            set => _divIdeBanks[bank & 0x03][(ushort)(addr & 0x1FFF)] = value;
        }

        /// <summary>
        /// Allows reading the DivIDE ROM
        /// </summary>
        /// <param name="addr">Memory address</param>
        /// <returns>ROM byte</returns>
        byte IMemoryDivIdeTestSupport.this[ushort addr] 
            => _romPages[DIVIDE_ROM_PAGE_INDEX][(ushort)(addr & 0x1FFF)];

        #endregion


    }
}