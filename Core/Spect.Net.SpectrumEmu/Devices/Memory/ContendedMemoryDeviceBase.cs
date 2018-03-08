using Spect.Net.SpectrumEmu.Abstraction.Devices;

namespace Spect.Net.SpectrumEmu.Devices.Memory
{
    /// <summary>
    /// This class implements an abstract memory device that handles
    /// contention
    /// </summary>
    public abstract class ContendedMemoryDeviceBase : IMemoryDevice
    {
        protected IZ80Cpu Cpu;
        protected IScreenDevice ScreenDevice;

        /// <summary>
        /// Resets this device
        /// </summary>
        public abstract void Reset();

        /// <summary>
        /// Gets the state of the device so that the state can be saved
        /// </summary>
        /// <returns>The object that describes the state of the device</returns>
        public abstract IDeviceState GetState();

        /// <summary>
        /// Sets the state of the device from the specified object
        /// </summary>
        /// <param name="state">Device state</param>
        public abstract void RestoreState(IDeviceState state);

        /// <summary>
        /// The virtual machine that hosts the device
        /// </summary>
        public ISpectrumVm HostVm { get; private set; }

        /// <summary>
        /// Signs that the device has been attached to the Spectrum virtual machine
        /// </summary>
        public virtual void OnAttachedToVm(ISpectrumVm hostVm)
        {
            HostVm = hostVm;
            Cpu = hostVm?.Cpu;
            ScreenDevice = hostVm?.ScreenDevice;
        }

        /// <summary>
        /// Reads the memory at the specified address
        /// </summary>
        /// <param name="addr">Memory address</param>
        /// <param name="noContention">Indicates non-contended read operation</param>
        /// <returns>Byte read from the memory</returns>
        public abstract byte Read(ushort addr, bool noContention = false);

        /// <summary>
        /// Sets the memory value at the specified address
        /// </summary>
        /// <param name="addr">Memory address</param>
        /// <param name="value">Memory value to write</param>
        /// <returns>Byte read from the memory</returns>
        public abstract void Write(ushort addr, byte value);

        /// <summary>
        /// Emulates memory contention
        /// </summary>
        /// <param name="addr">Contention address</param>
        public virtual void ContentionWait(ushort addr)
        {
            if ((addr & 0xC000) == 0x4000)
            {
                var delay = ScreenDevice.GetContentionValue(HostVm.CurrentFrameTact);
                Cpu?.Delay(delay);
            }
        }

        /// <summary>
        /// Gets the buffer that holds memory data
        /// </summary>
        /// <returns></returns>
        public abstract byte[] CloneMemory();

        /// <summary>
        /// Fills up the memory from the specified buffer
        /// </summary>
        /// <param name="buffer">Contains the row data to fill up the memory</param>
        public abstract void CopyRom(byte[] buffer);

        /// <summary>
        /// Selects the ROM with the specified index
        /// </summary>
        /// <param name="romIndex">Index of the ROM</param>
        public abstract void SelectRom(int romIndex);

        /// <summary>
        /// Retrieves the index of the selected ROM
        /// </summary>
        /// <returns>The index of the selected ROM</returns>
        public abstract int GetSelectedRomIndex();

        /// <summary>
        /// Pages in the selected bank into the specified slot
        /// </summary>
        /// <param name="slot">Index of the slot</param>
        /// <param name="bank">Index of the bank to page in</param>
        /// <param name="bank16Mode">
        /// True: 16K banks; False: 8K banks
        /// </param>
        public abstract void PageIn(int slot, int bank, bool bank16Mode = true);

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
        public abstract int GetSelectedBankIndex(int slot, bool bank16Mode = true);

        /// <summary>
        /// Indicates of shadow screen should be used
        /// </summary>
        public bool UseShadowScreen { get; set; }

        /// <summary>
        /// Indicates special mode: special RAM paging
        /// </summary>
        public virtual bool IsInAllRamMode => false;

        /// <summary>
        /// Indicates if the device is in 8K mode
        /// </summary>
        public virtual bool IsIn8KMode => false;

        /// <summary>
        /// Gets the data for the specfied ROM page
        /// </summary>
        /// <param name="romIndex">Index of the ROM</param>
        /// <returns>
        /// The buffer that holds the binary data for the specified ROM page
        /// </returns>
        public abstract byte[] GetRomBuffer(int romIndex);

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
        public abstract byte[] GetRamBank(int bankIndex, bool bank16Mode = true);

        /// <summary>
        /// Gets the location of the address
        /// </summary>
        /// <param name="addr">Address to check the location</param>
        /// <returns>
        /// IsInRom: true, if the address is in ROM
        /// Index: ROM/RAM bank index
        /// Address: Index within the bank
        /// </returns>
        public abstract (bool IsInRom, int Index, ushort Address) GetAddressLocation(ushort addr);

        /// <summary>
        /// Checks if the RAM bank with the specified index is paged in
        /// </summary>
        /// <param name="index">RAM bank index</param>
        /// <param name="baseAddress">Base memory address, provided the bank is paged in</param>
        /// <returns>True, if the bank is paged in; otherwise, false</returns>
        public abstract bool IsRamBankPagedIn(int index, out ushort baseAddress);
    }
}