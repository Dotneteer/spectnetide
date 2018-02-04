using Spect.Net.SpectrumEmu.Abstraction.Devices;

namespace Spect.Net.SpectrumEmu.Devices.Memory
{
    /// <summary>
    /// This device represents the Spectrum 48 memory device
    /// </summary>
    public sealed class Spectrum48MemoryDevice: ContendedMemoryDeviceBase
    {
        private byte[] _memory;

        /// <summary>
        /// Resets this device by filling the memory with 0xFF
        /// </summary>
        public override void Reset()
        {
            for (var i = 0; i < _memory.Length; i++)
            {
                Write((ushort)i, 0xFF);
            }
        }

        /// <summary>
        /// Gets the state of the device so that the state can be saved
        /// </summary>
        /// <returns>The object that describes the state of the device</returns>
        public override IDeviceState GetState() => new Spectrum48MemoryDeviceState(this);

        /// <summary>
        /// Sets the state of the device from the specified object
        /// </summary>
        /// <param name="state">Device state</param>
        public override void RestoreState(IDeviceState state) => state.RestoreDeviceState(this);

        /// <summary>
        /// Signs that the device has been attached to the Spectrum virtual machine
        /// </summary>
        public override void OnAttachedToVm(ISpectrumVm hostVm)
        {
            base.OnAttachedToVm(hostVm);
            _memory = new byte[AddressableSize];
        }

        /// <summary>
        /// Reads the memory at the specified address
        /// </summary>
        /// <param name="addr">Memory address</param>
        /// <param name="noContention">Indicates non-contended read operation</param>
        /// <returns>Byte read from the memory</returns>
        public override byte Read(ushort addr, bool noContention = false)
        {
            var value = _memory[addr];
            if (noContention) return value;

            ContentionWait(addr);
            return value;
        }

        /// <summary>
        /// Sets the memory value at the specified address
        /// </summary>
        /// <param name="addr">Memory address</param>
        /// <param name="value">Memory value to write</param>
        /// <returns>Byte read from the memory</returns>
        public override void Write(ushort addr, byte value)
        {
            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (addr & 0xC000)
            {
                case 0x0000:
                    // --- ROM cannot be overwritten
                    return;
                case 0x4000:
                    ContentionWait(addr);
                    break;
            }
            _memory[addr] = value;
        }

        /// <summary>
        /// Gets the buffer that holds memory data
        /// </summary>
        /// <returns></returns>
        public override byte[] CloneMemory()
        {
            var clone = new byte[AddressableSize];
            _memory.CopyTo(clone, 0);
            return clone;
        }

        /// <summary>
        /// Fills up the memory from the specified buffer
        /// </summary>
        /// <param name="buffer">Contains the row data to fill up the memory</param>
        public override void CopyRom(byte[] buffer)
        {
            buffer?.CopyTo(_memory, 0);
        }

        /// <summary>
        /// Selects the ROM with the specified index
        /// </summary>
        /// <param name="romIndex">Index of the ROM</param>
        public override void SelectRom(int romIndex)
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
        public override int GetSelectedRomIndex() => 0;

        /// <summary>
        /// Pages in the selected bank into the specified slot
        /// </summary>
        /// <param name="slot">Index of the slot</param>
        /// <param name="bank">Index of the bank to page in</param>
        public override void PageIn(int slot, int bank)
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
        public override int GetSelectedBankIndex(int slot) => 0;

        /// <summary>
        /// Gets the data for the specfied ROM page
        /// </summary>
        /// <param name="romIndex">Index of the ROM</param>
        /// <returns>
        /// The buffer that holds the binary data for the specified ROM page
        /// </returns>
        public override byte[] GetRomBuffer(int romIndex)
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
        public override byte[] GetRamBank(int bankIndex)
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
        public override (bool IsInRom, int Index, ushort Address) GetAddressLocation(ushort addr)
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
        public override bool IsRamBankPagedIn(int index, out ushort baseAddress)
        {
            baseAddress = 0x4000;
            return false;
        }

        /// <summary>
        /// Spectrum 48 memory device state
        /// </summary>
        public class Spectrum48MemoryDeviceState : IDeviceState
        {
            public byte[] Memory { get; set; }

            public Spectrum48MemoryDeviceState()
            {
            }

            public Spectrum48MemoryDeviceState(Spectrum48MemoryDevice device)
            {
                Memory = device._memory;
            }

            /// <summary>
            /// Restores the dvice state from this state object
            /// </summary>
            /// <param name="device">Device instance</param>
            public void RestoreDeviceState(IDevice device)
            {
                if (!(device is Spectrum48MemoryDevice sp48)) return;

                sp48._memory = Memory;
            }
        }
    }
}