namespace Spect.Net.SpectrumEmu.Abstraction.Devices
{
    /// <summary>
    /// This interface represents the Spectrum's memory device
    /// </summary>
    public interface IMemoryDevice : ISpectrumBoundDevice
    {
        /// <summary>
        /// Reads the memory at the specified address
        /// </summary>
        /// <param name="addr">Memory address</param>
        /// <param name="noContention">
        /// Indicates non-contended read operation
        /// </param>
        /// <returns>Byte read from the memory</returns>
        byte Read(ushort addr, bool noContention = false);

        /// <summary>
        /// Sets the memory value at the specified address
        /// </summary>
        /// <param name="addr">Memory address</param>
        /// <param name="value">Memory value to write</param>
        /// <returns>Byte read from the memory</returns>
        void Write(ushort addr, byte value);

        /// <summary>
        /// Emulates memory contention
        /// </summary>
        /// <param name="addr">Contention address</param>
        void ContentionWait(ushort addr);

        /// <summary>
        /// Gets the buffer that holds memory data
        /// </summary>
        /// <returns></returns>
        byte[] CloneMemory();

        /// <summary>
        /// Fills up the contents of the ROM pointed by 
        /// <see cref="SelectRom"/> from the specified buffer
        /// </summary>
        /// <param name="buffer">
        /// Contains the row data to fill up the memory
        /// </param>
        void CopyRom(byte[] buffer);

        /// <summary>
        /// Selects the ROM with the specified index
        /// </summary>
        /// <param name="romIndex">Index of the ROM</param>
        void SelectRom(int romIndex);

        /// <summary>
        /// Retrieves the index of the selected ROM
        /// </summary>
        /// <returns>The index of the selected ROM</returns>
        int GetSelectedRomIndex();

        /// <summary>
        /// Pages in the selected bank into the specified slot
        /// </summary>
        /// <param name="slot">Index of the slot</param>
        /// <param name="bank">Index of the bank to page in</param>
        /// <param name="bank16Mode">
        /// True: 16K banks; False: 8K banks
        /// </param>
        void PageIn(int slot, int bank, bool bank16Mode = true);

        /// <summary>
        /// Gets the bank paged in to the specified slot
        /// </summary>
        /// <param name="slot">Slot index</param>
        /// <returns>
        /// The index of the bank that is pages into the slot
        /// </returns>
        int GetSelectedBankIndex(int slot);

        /// <summary>
        /// Indicates of shadow screen should be used
        /// </summary>
        bool UseShadowScreen { get; set; }

        /// <summary>
        /// Gets the data for the specfied ROM page
        /// </summary>
        /// <param name="romIndex">Index of the ROM</param>
        /// <returns>
        /// The buffer that holds the binary data for the specified ROM page
        /// </returns>
        byte[] GetRomBuffer(int romIndex);

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
        byte[] GetRamBank(int bankIndex, bool bank16Mode = true);

        /// <summary>
        /// Gets the location of the address
        /// </summary>
        /// <param name="addr">Address to check the location</param>
        /// <returns>
        /// IsInRom: true, if the address is in ROM
        /// Index: ROM/RAM bank index
        /// Address: Index within the bank
        /// </returns>
        (bool IsInRom, int Index, ushort Address) GetAddressLocation(ushort addr);

        /// <summary>
        /// Checks if the RAM bank with the specified index is paged in
        /// </summary>
        /// <param name="index">RAM bank index</param>
        /// <param name="baseAddress">Base memory address, provided the bank is paged in</param>
        /// <returns>True, if the bank is paged in; otherwise, false</returns>
        bool IsRamBankPagedIn(int index, out ushort baseAddress);
    }
}