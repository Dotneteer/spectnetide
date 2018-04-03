using Spect.Net.SpectrumEmu.Abstraction.Devices;

namespace Spect.Net.SpectrumEmu.Scripting
{
    /// <summary>
    /// This class allows querying paging information about the memory
    /// </summary>
    public sealed class MemoryPagingInfo
    {
        private readonly IMemoryDevice _memoryDevice;

        public MemoryPagingInfo(IMemoryDevice memoryDevice)
        {
            _memoryDevice = memoryDevice;
        }

        /// <summary>
        /// Gets the index of the currently selected ROM
        /// </summary>
        public int SelectedRomIndex => _memoryDevice.GetSelectedRomIndex();

        /// <summary>
        /// Gets the selected RAM bank index
        /// </summary>
        public int SelectedRamBankIndex => _memoryDevice.GetSelectedBankIndex(3);

        /// <summary>
        /// Gets the bank index paged in to the specified slot
        /// </summary>
        /// <param name="slot">Slot index</param>
        /// <returns>
        /// The index of the bank that is pages into the slot
        /// </returns>
        public int GetBankIndexForSlot(int slot) => _memoryDevice.GetSelectedBankIndex(slot);

        /// <summary>
        /// Indicates of shadow screen should be used
        /// </summary>
        public bool UsesShadowScreen => _memoryDevice.UsesShadowScreen;

        /// <summary>
        /// Indicates special mode: special RAM paging
        /// </summary>
        public bool IsInAllRamMode => _memoryDevice.IsInAllRamMode;
    }
}