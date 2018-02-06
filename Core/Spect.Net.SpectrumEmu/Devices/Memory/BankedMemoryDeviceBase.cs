using Spect.Net.SpectrumEmu.Abstraction.Devices;

namespace Spect.Net.SpectrumEmu.Devices.Memory
{
    /// <summary>
    /// This device represents the Spectrum 128 memory device
    /// </summary>
    public abstract class BankedMemoryDeviceBase: ContendedMemoryDeviceBase
    {
        private readonly int _defaultRomCount;
        private readonly int _defaultRamBankCount;

        protected int RomCount;
        protected int RamBankCount;
        protected byte[][] Roms;
        protected int CurrentRomIndex;
        protected int SelectedRomIndex;

        /// <summary>
        /// Default initialization
        /// </summary>
        protected BankedMemoryDeviceBase()
        {
        }

        /// <summary>
        /// Initializes the device with the specified number of ROM and ROM banks
        /// </summary>
        /// <param name="defaultRomCount">ROM count</param>
        /// <param name="defaultRamBankCount">RAM bank count</param>
        protected BankedMemoryDeviceBase(int defaultRomCount, int defaultRamBankCount)
        {
            _defaultRomCount = defaultRomCount;
            _defaultRamBankCount = defaultRamBankCount;
        }

        /// <summary>
        /// Provides access to the RAM banks
        /// </summary>
        public byte[][] RamBanks { get; protected set; }

        /// <summary>
        /// Provides access to the current ROM page
        /// </summary>
        public byte[] CurrentRom => Roms[CurrentRomIndex];

        /// <summary>
        /// Resets this device by filling the memory with 0xFF
        /// </summary>
        public override void Reset()
        {
            for (var i = 0; i < PageSize; i++)
            {
                for (var j = 0; j < RamBankCount; j++)
                {
                    RamBanks[j][i] = 0xFF;
                }
            }
            SelectedRomIndex = 0;
            CurrentRomIndex = 0;
        }

        /// <summary>
        /// Signs that the device has been attached to the Spectrum virtual machine
        /// </summary>
        public override void OnAttachedToVm(ISpectrumVm hostVm)
        {
            base.OnAttachedToVm(hostVm);
            RomCount = hostVm?.RomConfiguration?.NumberOfRoms ?? _defaultRomCount;
            RamBankCount = hostVm?.MemoryConfiguration?.RamBanks ?? _defaultRamBankCount;

            // --- Create the ROM pages
            Roms = new byte[RomCount][];
            for (var i = 0; i < RomCount; i++)
            {
                Roms[i] = new byte[PageSize];
            }

            RamBanks = new byte[RamBankCount][];
            // --- Create RAM pages
            for (var i = 0; i < RamBankCount; i++)
            {
                RamBanks[i] = new byte[PageSize];
            }

            SelectedRomIndex = 0;
            CurrentRomIndex = 0;
        }

        /// <summary>
        /// Gets the buffer that holds memory data
        /// </summary>
        /// <returns></returns>
        public override byte[] CloneMemory()
        {
            var clone = new byte[AddressableSize];
            for (var i = 0; i <= 3; i++)
            {
                var cloneAddr = (ushort) (i * PageSize);
                var addrInfo = GetAddressLocation(cloneAddr);
                if (addrInfo.IsInRom)
                {
                    Roms[addrInfo.Index].CopyTo(clone, cloneAddr);
                }
                else
                {
                    RamBanks[addrInfo.Index].CopyTo(clone, cloneAddr);
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
            buffer?.CopyTo(Roms[CurrentRomIndex], 0);
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
            if (romIndex >= RomCount)
            {
                romIndex = RomCount - 1;
            }
            SelectedRomIndex = romIndex;
            CurrentRomIndex = romIndex;
        }

        /// <summary>
        /// Retrieves the index of the selected ROM
        /// </summary>
        /// <returns>The index of the selected ROM</returns>
        public override int GetSelectedRomIndex() => SelectedRomIndex;

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
            if (romIndex >= RomCount)
            {
                romIndex = RomCount - 1;
            }
            return Roms[romIndex];
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
            if (bankIndex < 0)
            {
                bankIndex = 0;
            }
            if (bankIndex >= RamBankCount)
            {
                bankIndex = RamBankCount - 1;
            }
            return RamBanks[bankIndex];
        }

        /// <summary>
        /// Gets the state of the device so that the state can be saved
        /// </summary>
        /// <returns>The object that describes the state of the device</returns>
        public override IDeviceState GetState() => new BankedMemoryDeviceState(this);

        /// <summary>
        /// Sets the state of the device from the specified object
        /// </summary>
        /// <param name="state">Device state</param>
        public override void RestoreState(IDeviceState state) => state.RestoreDeviceState(this);

        /// <summary>
        /// State of the banked memory device
        /// </summary>
        public class BankedMemoryDeviceState : IDeviceState
        {
            public int RomCount { get; set; }
            public int RamBankCount { get; set; }
            public byte[][] Roms { get; set; }
            public byte[][] RamBanks { get; set; }
            public int CurrentRomIndex { get; set; }
            public int SelectedRomIndex { get; set; }

            public BankedMemoryDeviceState()
            {
            }

            public BankedMemoryDeviceState(BankedMemoryDeviceBase device)
            {
                RomCount = device.RomCount;
                RamBankCount = device.RamBankCount;
                Roms = device.Roms;
                RamBanks = device.RamBanks;
                CurrentRomIndex = device.CurrentRomIndex;
            }

            /// <summary>
            /// Restores the dvice state from this state object
            /// </summary>
            /// <param name="device">Device instance</param>
            public virtual void RestoreDeviceState(IDevice device)
            {
                if (!(device is BankedMemoryDeviceBase banked)) return;

                banked.RomCount = RomCount;
                banked.RamBankCount = RamBankCount;
                banked.Roms = Roms;
                banked.RamBanks = RamBanks;
                banked.CurrentRomIndex = CurrentRomIndex;
            }
        }
    }
}