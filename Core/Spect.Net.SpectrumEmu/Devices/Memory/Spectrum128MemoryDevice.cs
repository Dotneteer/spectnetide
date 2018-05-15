using Spect.Net.SpectrumEmu.Abstraction.Devices;

namespace Spect.Net.SpectrumEmu.Devices.Memory
{
    /// <summary>
    /// This device represents the Spectrum 128 memory device
    /// </summary>
    public sealed class Spectrum128MemoryDevice : BankedMemoryDeviceBase
    {
        private int _currentSlot3Bank;

        /// <summary>
        /// Initializes the device
        /// </summary>
        public Spectrum128MemoryDevice() : base(2, 8)
        {
        }

        /// <summary>
        /// Resets this device by filling the memory with 0xFF
        /// </summary>
        public override void Reset()
        {
            base.Reset();
            _currentSlot3Bank = 0;
        }

        /// <summary>
        /// Gets the state of the device so that the state can be saved
        /// </summary>
        /// <returns>The object that describes the state of the device</returns>
        public override IDeviceState GetState() => new Spectrum128MemoryDeviceState(this);

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
            _currentSlot3Bank = 0;
        }

        /// <summary>
        /// Reads the memory at the specified address
        /// </summary>
        /// <param name="addr">Memory address</param>
        /// <param name="suppressContention">Indicates non-contended read operation</param>
        /// <returns>Byte read from the memory</returns>
        public override byte Read(ushort addr, bool suppressContention = false)
        {
            var memIndex = addr & 0x3FFF;
            byte memValue;
            switch (addr & 0xC000)
            {
                case 0x0000:
                    return Roms[SelectedRomIndex][memIndex];
                case 0x4000:
                    memValue = RamBanks[5][memIndex];
                    if (suppressContention || ScreenDevice == null) return memValue;
                    ApplyDelay();
                    return memValue;
                case 0x8000:
                    return RamBanks[2][memIndex];
                default:
                    memValue = RamBanks[_currentSlot3Bank][memIndex];
                    if ((_currentSlot3Bank & 0x01) == 0) return memValue;

                    // --- Bank 1, 3, 5, and 7 are contended
                    ApplyDelay();
                    return memValue;
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
            // ReSharper disable once SwitchStatementMissingSomeCases
            var memIndex = addr & 0x3FFF;
            switch (addr & 0xC000)
            {
                case 0x0000:
                    // --- ROM cannot be overwritten
                    return;
                case 0x4000:
                    if (!supressContention)
                    {
                        ApplyDelay();
                    }
                    RamBanks[5][memIndex] = value;
                    break;
                case 0x8000:
                    RamBanks[2][memIndex] = value;
                    break;
                default:
                    if ((_currentSlot3Bank & 0x01) != 0)
                    {
                        // --- Bank 1, 3, 5, and 7 are contended
                        if (!supressContention)
                        {
                            ApplyDelay();
                        }
                    }
                    RamBanks[_currentSlot3Bank][memIndex] = value;
                    break;
            }
        }

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
            if (slot != 3) return;
            _currentSlot3Bank = bank & 0x07;
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
            var bankAddr = (ushort)(addr & 0x3FFF);
            switch (addr & 0xC000)
            {
                case 0x0000:
                    return (true, SelectedRomIndex, addr);
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
        public override bool IsRamBankPagedIn(int index, out ushort baseAddress)
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

        /// <summary>
        /// State of the Spectrum 128 memory device
        /// </summary>
        public class Spectrum128MemoryDeviceState : BankedMemoryDeviceState
        {
            public int CurrentSlot3Bank { get; set; }
            public bool PagingEnabled { get; set; }

            public Spectrum128MemoryDeviceState()
            {
            }

            public Spectrum128MemoryDeviceState(Spectrum128MemoryDevice device) : base(device)
            {
                CurrentSlot3Bank = device._currentSlot3Bank;
                PagingEnabled = device.PagingEnabled;
            }

            /// <summary>
            /// Restores the dvice state from this state object
            /// </summary>
            /// <param name="device">Device instance</param>
            public override void RestoreDeviceState(IDevice device)
            {
                base.RestoreDeviceState(device);
                if (!(device is Spectrum128MemoryDevice sp128)) return;

                sp128._currentSlot3Bank = CurrentSlot3Bank;
                sp128.PagingEnabled = PagingEnabled;
            }
        }
    }
}