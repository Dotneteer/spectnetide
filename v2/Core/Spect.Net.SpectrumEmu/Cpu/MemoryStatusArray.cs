using System.Collections;
using Spect.Net.SpectrumEmu.Abstraction.Cpu;

namespace Spect.Net.SpectrumEmu.Cpu
{
    /// <summary>
    /// This class represents a status array where every bit 
    /// indicates the status of a particular memory address
    /// within the 64K memory space
    /// </summary>
    public class MemoryStatusArray : IMemoryStatusArray
    {
        private readonly BitArray _memoryBits = new BitArray(ushort.MaxValue + 1);

        /// <summary>
        /// Resets all address status to false
        /// </summary>
        public void ClearAll()
        {
            _memoryBits.SetAll(false);  
        }

        /// <summary>
        /// Gets the status of the specified address
        /// </summary>
        /// <param name="address">Memory address</param>
        /// <returns>Status of the memory address</returns>
        public bool this[ushort address] => _memoryBits[address];

        /// <summary>
        /// Signs that the specified memory address has been touched
        /// </summary>
        /// <param name="address">Memory address</param>
        public void Touch(ushort address)
        {
            _memoryBits[address] = true;
        }

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
                if (!_memoryBits[i]) return false;
            }
            return true;
        }
    }
}