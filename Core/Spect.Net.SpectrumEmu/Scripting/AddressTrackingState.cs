using Spect.Net.SpectrumEmu.Cpu;

namespace Spect.Net.SpectrumEmu.Scripting
{
    /// <summary>
    /// Represents tracking information for the memory state
    /// </summary>
    /// <remarks>
    /// (Such as execution, read, write)
    /// </remarks>
    public sealed class AddressTrackingState
    {
        private readonly MemoryStatusArray _memoryStatus;

        public AddressTrackingState(MemoryStatusArray memoryStatus)
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
        public bool Touched(ushort startAddr, ushort endAddr)
        {
            for (var i = startAddr; i <= endAddr; i++)
            {
                if (!_memoryStatus[i]) return false;
            }
            return true;
        }
    }
}