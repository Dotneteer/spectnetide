using Spect.Net.SpectrumEmu.Abstraction.Cpu;

namespace Spect.Net.SpectrumEmu.Machine
{
    /// <summary>
    /// Represents tracking information for the memory state
    /// </summary>
    /// <remarks>
    /// (Such as execution, read, write)
    /// </remarks>
    public sealed class AddressTrackingState
    {
        private readonly IMemoryStatusArray _memoryStatus;

        public AddressTrackingState(IMemoryStatusArray memoryStatus)
        {
            _memoryStatus = memoryStatus;
        }

        /// <summary>
        /// Gets the status of the specified address
        /// </summary>
        /// <param name="address">Memory address</param>
        /// <returns>Status of the memory address</returns>
        public bool this[ushort address] => _memoryStatus[address];

        /// <summary>
        /// Checks if all addresses are touched between the start
        /// and end address
        /// </summary>
        /// <param name="startAddr">Start address</param>
        /// <param name="endAddr">End address (inclusive)</param>
        /// <returns></returns>
        public bool TouchedAll(ushort startAddr, ushort endAddr)
        {
            for (var i = (int)startAddr; i <= endAddr; i++)
            {
                if (!_memoryStatus[(ushort)i]) return false;
            }
            return true;
        }

        /// <summary>
        /// Checks if all addresses are touched between the start
        /// and end address
        /// </summary>
        /// <param name="startAddr">Start address</param>
        /// <param name="endAddr">End address (inclusive)</param>
        /// <returns></returns>
        public bool TouchedAny(ushort startAddr, ushort endAddr)
        {
            for (var i = (int)startAddr; i <= endAddr; i++)
            {
                if (_memoryStatus[(ushort)i])
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Clear all status flags
        /// </summary>
        internal void Clear()
        {
            _memoryStatus.ClearAll();
        }
    }

}