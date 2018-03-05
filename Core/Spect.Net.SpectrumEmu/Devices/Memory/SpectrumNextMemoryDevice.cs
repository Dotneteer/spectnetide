using System.Linq;
using Spect.Net.SpectrumEmu.Abstraction.Devices;

namespace Spect.Net.SpectrumEmu.Devices.Memory
{
    /// <summary>
    /// This class represents the Spectrum +3 memory device
    /// </summary>
    public class SpectrumNextMemoryDevice : ContendedMemoryDeviceBase
    {
        private INextFeatureSetDevice _nextDevice;

        private byte[][] _ramPages;
        private byte[][] _romPages;
        private int[] _slots16;
        private int[] _slots8;
        private int _selectedRomIndex;

        /// <summary>
        /// The number of RAM pages available
        /// </summary>
        public int RamPageCount { get; private set; }

        /// <summary>
        /// Indicates special mode: special RAM paging
        /// </summary>
        public bool IsInAllRamMode { get; private set; }

        /// <summary>
        /// Indicates if the device is in 8K mode
        /// </summary>
        public bool IsIn8KMode { get; private set; }

        /// <summary>
        /// Signs that the device has been attached to the Spectrum virtual machine
        /// </summary>
        public override void OnAttachedToVm(ISpectrumVm hostVm)
        {
            base.OnAttachedToVm(hostVm);
            _nextDevice = hostVm.NextDevice;

            // --- Create space for ROM pages (use 8 x 8K pages)
            _romPages = new byte[8][];
            for (var i = 0; i < 8; i++)
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

            IsIn8KMode = false;
        }

        /// <summary>
        /// Initialize memory
        /// </summary>
        public override void Reset()
        {
            // --- Set up initial RAM slot values
            _slots16 = new[] {0, 5, 2, 0};
            _slots8 = new[] {0xFF, 0xFF, 10, 11, 4, 5, 0, 1};

            // --- Set up modes
            IsInAllRamMode = false;
        }

        public override byte Read(ushort addr, bool noContention = false)
        {
            throw new System.NotImplementedException();
        }

        public override void Write(ushort addr, byte value)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Gets the buffer that holds memory data
        /// </summary>
        /// <returns></returns>
        public override byte[] CloneMemory()
        {
            var clone = new byte[0x10000];
            if (!IsIn8KMode || IsInAllRamMode)
            {
                // --- We use 16K slots
                for (var i = 0; i <= 3; i++)
                {
                    var cloneAddr = (ushort)(i * 0x4000);
                    var addrInfo = GetAddressLocation(cloneAddr);
                    if (addrInfo.IsInRom)
                    {
                        _romPages[addrInfo.Index * 2].CopyTo(clone, cloneAddr);
                        _romPages[addrInfo.Index * 2 + 1].CopyTo(clone, cloneAddr + 0x2000);
                    }
                    else
                    {
                        _ramPages[addrInfo.Index * 2].CopyTo(clone, cloneAddr);
                        _ramPages[addrInfo.Index * 2 + 1].CopyTo(clone, cloneAddr + 0x2000);
                    }
                }
            }
            else
            {
                // --- We use 8K slots
                for (var i = 0; i <= 7; i++)
                {
                    var cloneAddr = (ushort)(i * 0x2000);
                    var addrInfo = GetAddressLocation(cloneAddr);
                    if (addrInfo.IsInRom)
                    {
                        _romPages[_selectedRomIndex * 2 + i%2].CopyTo(clone, cloneAddr + 0x2000 * (i%2));
                    }
                    else
                    {
                        _ramPages[addrInfo.Index].CopyTo(clone, cloneAddr);
                    }
                }
            }
            return clone;
        }

        /// <summary>
        /// Fills up the memory from the specified buffer
        /// </summary>
        /// <param name="buffer">Contains the row data to fill up the memory</param>
        public override void CopyRom(byte[] buffer)
        {
            var firstPage = buffer.Take(0x2000).ToArray();
            var secondPage = buffer.Skip(0x2000).Take(0x2000).ToArray();
            firstPage.CopyTo(_romPages[_selectedRomIndex * 2], 0);
            secondPage.CopyTo(_romPages[_selectedRomIndex * 2 + 1], 0);
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
            if (romIndex >= 4)
            {
                romIndex = 3;
            }
            _selectedRomIndex = romIndex;
            IsInAllRamMode = false;
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
            IsIn8KMode = false;
            _slots16[slot & 0x03] = bank;
            if (slot != 3)
            {
                IsInAllRamMode = true;
            }
            _nextDevice.Sync16KSlot(slot, bank);
        }

        /// <summary>
        /// Gets the bank paged in to the specified slot
        /// </summary>
        /// <param name="slot">Slot index</param>
        /// <returns>
        /// The index of the bank that is pages into the slot
        /// </returns>
        public override int GetSelectedBankIndex(int slot) => _slots16[slot & 0x03];

        /// <summary>
        /// Gets the data for the specfied ROM page
        /// </summary>
        /// <param name="romIndex">Index of the ROM</param>
        /// <returns>
        /// The buffer that holds the binary data for the specified ROM page
        /// </returns>
        public override byte[] GetRomBuffer(int romIndex)
        {
            if (romIndex < 0)
            {
                romIndex = 0;
            }
            if (romIndex >= 4)
            {
                romIndex = 3;
            }
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
            if (bankIndex < 0)
            {
                bankIndex = 0;
            }
            if (bankIndex >= 4)
            {
                bankIndex = 3;
            }
            var ram = new byte[0x4000];
            _romPages[bankIndex * 2].CopyTo(ram, 0x0000);
            _romPages[bankIndex * 2 + 1].CopyTo(ram, 0x2000);
            return ram;
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

        public override bool IsRamBankPagedIn(int index, out ushort baseAddress)
        {
            throw new System.NotImplementedException();
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
    }
}