using Spect.Net.SpectrumEmu.Abstraction.Devices;

namespace Spect.Net.SpectrumEmu.Devices.Memory
{
    /// <summary>
    /// This device represents the Spectrum 48 memory device
    /// </summary>
    public sealed class Spectrum48MemoryDevice: IMemoryDevice
    {
        private IZ80Cpu _cpu;
        private IScreenDevice _screenDevice;
        private byte[] _memory;

        /// <summary>
        /// Resets this device by filling the memory with 0xFF
        /// </summary>
        public void Reset()
        {
            for (var i = 0; i < _memory.Length; i++)
            {
                Write((ushort)i, 0xFF);
            }
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
            _cpu = hostVm.Cpu;
            _screenDevice = hostVm.ScreenDevice;
            _memory = new byte[0x10000];
        }

        /// <summary>
        /// The addressable size of the memory
        /// </summary>
        public int AddressableSize => 0x1_0000;

        /// <summary>
        /// The size of a memory page
        /// </summary>
        /// <remarks>
        /// Though Spectrum 48K does not use a paging, this size defines the 
        /// virtual ROM page size
        /// </remarks>
        public int PageSize => 0x4000;

        /// <summary>
        /// Reads the memory at the specified address
        /// </summary>
        /// <param name="addr">Memory address</param>
        /// <returns>Byte read from the memory</returns>
        public byte Read(ushort addr)
        {
            var value = _memory[addr];
            if ((addr & 0xC000) == 0x4000)
            {
                _cpu.Delay(_screenDevice.GetContentionValue(HostVm.CurrentFrameTact));
            }
            return value;
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
            switch (addr & 0xC000)
            {
                case 0x0000:
                    // --- ROM cannot be overwritten
                    return;
                case 0x4000:
                    _cpu.Delay(_screenDevice.GetContentionValue(HostVm.CurrentFrameTact));
                    break;
            }
            _memory[addr] = value;
        }

        /// <summary>
        /// Gets the buffer that holds memory data
        /// </summary>
        /// <returns></returns>
        public byte[] CloneMemory()
        {
            var clone = new byte[AddressableSize];
            _memory.CopyTo(clone, 0);
            return clone;
        }

        /// <summary>
        /// The ULA reads the memory at the specified address
        /// </summary>
        /// <param name="addr">Memory address</param>
        /// <returns>Byte read from the memory</returns>
        /// <remarks>
        /// We need this device to emulate the contention for the screen memory
        /// between the CPU and the ULA.
        /// </remarks>
        public byte UlaRead(ushort addr)
        {
            var value = _memory[(addr & 0x3FFF) + 0x4000];
            return value;
        }

        /// <summary>
        /// Fills up the memory from the specified buffer
        /// </summary>
        /// <param name="buffer">Contains the row data to fill up the memory</param>
        public void CopyRom(byte[] buffer)
        {
            buffer?.CopyTo(_memory, 0);
        }

        /// <summary>
        /// Selects the ROM with the specified index
        /// </summary>
        /// <param name="romIndex">Index of the ROM</param>
        public void SelectRom(int romIndex)
        {
            // --- Spectrum 48 does not support banks, we do nothing
        }

        /// <summary>
        /// Retrieves the index of the selected ROM
        /// </summary>
        /// <returns>
        /// As Spectrum 48K does not support paging, 
        /// this method always return 0
        /// </returns>
        public int GetSelectedRomIndex() => 0;

        /// <summary>
        /// Pages in the selected bank into the specified slot
        /// </summary>
        /// <param name="slot">Index of the slot</param>
        /// <param name="bank">Index of the bank to page in</param>
        public void PageIn(int slot, int bank)
        {
            // --- Spectrum 48 does not support banks, we do nothing
        }

        /// <summary>
        /// Gets the bank paged in to the specified slot
        /// </summary>
        /// <param name="slot">Slot index</param>
        /// <returns>
        /// As Spectrum 48K does not support paging, 
        /// this method always return 0
        /// </returns>
        public int GetSelectedBankIndex(int slot) => 0;

        /// <summary>
        /// Indicates of shadow screen should be used
        /// </summary>
        /// <remarks>
        /// Spectrum 48K does not use this flag
        /// </remarks>
        public bool UseShadowScreen { get; set; }

        /// <summary>
        /// Gets the data for the specfied ROM page
        /// </summary>
        /// <param name="romIndex">Index of the ROM</param>
        /// <returns>
        /// The buffer that holds the binary data for the specified ROM page
        /// </returns>
        public byte[] GetRomBuffer(int romIndex)
        {
            var rom = new byte[0x4000];
            for (var i = 0; i < 0x4000; i++) rom[i] = _memory[i];
            return rom;
        }

        /// <summary>
        /// Gets the data for the specfied RAM bank
        /// </summary>
        /// <param name="bankIndex">Index of the RAM bank</param>
        /// <returns>
        /// The buffer that holds the binary data for the specified RAM bank
        /// </returns>
        public byte[] GetRamBank(int bankIndex)
        {
            var ram = new byte[0xC000];
            for (var i = 0; i < 0xC000; i++) ram[i] = _memory[i+0x4000];
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
        public (bool IsInRom, int Index, ushort Address) GetAddressLocation(ushort addr)
        {
            return addr < 0x4000 
                ? (true, 0, addr) 
                : (false, 0, (ushort)(addr - 0x4000));
        }

        /// <summary>
        /// Checks if the RAM bank with the specified index is paged in
        /// </summary>
        /// <param name="index">RAM bank index</param>
        /// <param name="baseAddress">0x4000</param>
        /// <returns>True</returns>
        /// <remarks>The Single RAM bank of Spectrum 48 is always paged in</remarks>
        public bool IsRamBankPagedIn(int index, out ushort baseAddress)
        {
            baseAddress = 0x4000;
            return false;
        }
    }
}