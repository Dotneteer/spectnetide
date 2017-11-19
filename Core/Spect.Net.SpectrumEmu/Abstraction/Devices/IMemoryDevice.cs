namespace Spect.Net.SpectrumEmu.Abstraction.Devices
{
    /// <summary>
    /// This interface represents the Spectrum's memory device
    /// </summary>
    public interface IMemoryDevice : ISpectrumBoundDevice
    {
        /// <summary>
        /// The addressable size of the memory
        /// </summary>
        int AddressableSize { get; }
        
        /// <summary>
        /// Reads the memory at the specified address
        /// </summary>
        /// <param name="addr">Memory address</param>
        /// <returns>Byte read from the memory</returns>
        byte Read(ushort addr);

        /// <summary>
        /// Sets the memory value at the specified address
        /// </summary>
        /// <param name="addr">Memory address</param>
        /// <param name="value">Memory value to write</param>
        /// <returns>Byte read from the memory</returns>
        void Write(ushort addr, byte value);

        /// <summary>
        /// Gets the buffer that holds memory data
        /// </summary>
        /// <returns></returns>
        byte[] CloneMemory();

        /// <summary>
        /// The ULA reads the memory at the specified address
        /// </summary>
        /// <param name="addr">Memory address</param>
        /// <returns>Byte read from the memory</returns>
        /// <remarks>
        /// We need this device to emulate the contention for the screen memory
        /// between the CPU and the ULA.
        /// </remarks>
        byte UlaRead(ushort addr);

        /// <summary>
        /// Fills up the memory from the specified buffer
        /// </summary>
        /// <param name="buffer">Contains the row data to fill up the memory</param>
        void CopyRom(byte[] buffer);
    }
}