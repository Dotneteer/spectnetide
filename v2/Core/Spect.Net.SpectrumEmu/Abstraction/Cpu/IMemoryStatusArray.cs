namespace Spect.Net.SpectrumEmu.Abstraction.Cpu
{
    /// <summary>
    /// This interface represents a status array where every bit 
    /// indicates the status of a particular memory address
    /// within the 64K memory space.
    /// </summary>
    public interface IMemoryStatusArray
    {
        /// <summary>
        /// Resets all address status to false.
        /// </summary>
        void ClearAll();

        /// <summary>
        /// Gets the status of the specified address.
        /// </summary>
        /// <param name="address">Memory address.</param>
        /// <returns>Status of the memory address.</returns>
        bool this[ushort address] { get; }

        /// <summary>
        /// Signs that the specified memory address has been touched.
        /// </summary>
        /// <param name="address">Memory address.</param>
        void Touch(ushort address);

        /// <summary>
        /// Checks if all addresses are touched between the start
        /// and end address.
        /// </summary>
        /// <param name="startAddr">Start address.</param>
        /// <param name="endAddr">End address (inclusive).</param>
        /// <returns></returns>
        bool Touched(ushort startAddr, ushort endAddr);
    }
}