using Spect.Net.SpectrumEmu.Abstraction.Devices;

namespace Spect.Net.SpectrumEmu.Devices.Memory
{
    /// <summary>
    /// This class represents the Spectrum +3 memory device
    /// </summary>
    public class SpectrumP3MemoryDevice : BankedMemoryDeviceBase
    {
        private IZ80Cpu _cpu;
        private IScreenDevice _screenDevice;
        private int[] _slots;
        private bool _isInAllRamMode;

        public override bool IsInAllRamMode => _isInAllRamMode;

        /// <summary>
        /// Initializes the device
        /// </summary>
        public SpectrumP3MemoryDevice() : base(4, 8)
        {
            _isInAllRamMode = false;
        }

        /// <summary>
        /// Resets this device by filling the memory with 0xFF
        /// </summary>
        public override void Reset()
        {
            base.Reset();
            _slots = new[]
            {
                0, 5, 2, 0
            };
            _isInAllRamMode = false;
        }

        /// <summary>
        /// Gets the state of the device so that the state can be saved
        /// </summary>
        /// <returns>The object that describes the state of the device</returns>
        public override IDeviceState GetState() => new SpectrumP3MemoryDeviceState(this);

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
            _cpu = hostVm?.Cpu;
            _screenDevice = hostVm?.ScreenDevice;
            _slots = new[]
            {
                0, 5, 2, 0
            };
            _isInAllRamMode = false;
        }

        /// <summary>
        /// Selects the ROM with the specified index
        /// </summary>
        /// <param name="romIndex">Index of the ROM</param>
        /// <remarks>
        /// When the ROM index is set, we turn back to normal mode
        /// </remarks>
        public override void SelectRom(int romIndex)
        {
            base.SelectRom(romIndex);
            _isInAllRamMode = false;
        }

        /// <summary>
        /// Reads the memory at the specified address
        /// </summary>
        /// <param name="addr">Memory address</param>
        /// <param name="noContention">Indicates non-contended read operation</param>
        /// <returns>Byte read from the memory</returns>
        public override byte Read(ushort addr, bool noContention = false)
        {
            var memIndex = addr & 0x3FFF;
            var memValue = RamBanks[_slots[(byte)(addr >> 14)]][memIndex];
            switch (addr & 0xC000)
            {
                case 0x0000:
                    return IsInAllRamMode
                        ? memValue
                        : Roms[SelectedRomIndex][memIndex];
                case 0x4000:
                    if (!noContention && _screenDevice != null)
                    {
                        _cpu?.Delay(_screenDevice.GetContentionValue(HostVm.CurrentFrameTact));
                    }
                    return memValue;
                case 0x8000:
                    return memValue;
                default:
                    // --- Bank 4, 5, 6, and 7 are contended
                    if (_slots[3] >= 4 && _screenDevice != null)
                    {
                        _cpu?.Delay(_screenDevice.GetContentionValue(HostVm.CurrentFrameTact));
                    }
                    return memValue;
            }
        }

        /// <summary>
        /// Sets the memory value at the specified address
        /// </summary>
        /// <param name="addr">Memory address</param>
        /// <param name="value">Memory value to write</param>
        /// <param name="noContention">
        /// Indicates non-contended write operation
        /// </param>
        public override void Write(ushort addr, byte value, bool noContention = false)
        {
            // ReSharper disable once SwitchStatementMissingSomeCases
            var memIndex = addr & 0x3FFF;
            switch (addr & 0xC000)
            {
                case 0x0000:
                    if (IsInAllRamMode)
                    {
                        RamBanks[_slots[0]][memIndex] = value;
                    }
                    return;
                case 0x4000:
                    if (!noContention)
                    {
                        _cpu?.Delay(_screenDevice.GetContentionValue(HostVm.CurrentFrameTact));
                    }
                    RamBanks[_slots[1]][memIndex] = value;
                    break;
                case 0x8000:
                    RamBanks[_slots[2]][memIndex] = value;
                    break;
                default:
                    var bankIndex = _slots[3];
                    if (bankIndex >= 4)
                    {
                        // --- Bank 4, 5, 6, and 7 are contended
                        if (!noContention)
                        {
                            _cpu?.Delay(_screenDevice.GetContentionValue(HostVm.CurrentFrameTact));
                        }
                    }
                    RamBanks[bankIndex][memIndex] = value;
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
            _slots[slot & 0x03] = bank;
            if (slot != 3)
            {
                _isInAllRamMode = true;
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
            => _slots[slot & 0x03];

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
                    return IsInAllRamMode
                        ? (false, _slots[0], addr)
                        : (true, SelectedRomIndex, addr);
                case 0x4000:
                    return (false, _slots[1], bankAddr);
                case 0x8000:
                    return (false, _slots[2], bankAddr);
                default:
                    return (false, _slots[3], bankAddr);
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
            if (IsInAllRamMode && _slots[0] == index)
            {
                baseAddress = 0x0000;
                return true;
            }
            if (_slots[1] == index)
            {
                baseAddress = 0x4000;
                return true;
            }
            if (_slots[2] == index)
            {
                baseAddress = 0x8000;
                return true;
            }
            if (_slots[3] == index)
            {
                baseAddress = 0xC000;
                return true;
            }
            baseAddress = 0;
            return false;
        }

        public class SpectrumP3MemoryDeviceState : BankedMemoryDeviceState
        {
            public int[] Slots { get; set; }
            public bool IsInAllRamMode { get; set; }

            public SpectrumP3MemoryDeviceState()
            {
            }

            public SpectrumP3MemoryDeviceState(SpectrumP3MemoryDevice device) : base(device)
            {
                Slots = device._slots;
                IsInAllRamMode = device.IsInAllRamMode;
            }

            /// <summary>
            /// Restores the dvice state from this state object
            /// </summary>
            /// <param name="device">Device instance</param>
            public override void RestoreDeviceState(IDevice device)
            {
                base.RestoreDeviceState(device);
                if (!(device is SpectrumP3MemoryDevice spP3)) return;

                spP3._slots = Slots;
                spP3._isInAllRamMode = IsInAllRamMode;
            }
        }
    }
}