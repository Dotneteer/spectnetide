namespace Spect.Net.SpectrumEmu.Abstraction.Devices
{
    /// <summary>
    /// This interface represents the Spectrum's memory device
    /// </summary>
    public interface ISpectrumMemoryDevice : ISpectrumBoundDevice, IMemoryDevice
    {
        /// <summary>
        /// The ULA reads the memory at the specified address
        /// </summary>
        /// <param name="addr">Memory address</param>
        /// <returns>Byte read from the memory</returns>
        /// <remarks>
        /// We need this device to emulate the contention for the screen memory
        /// between the CPU and the ULA.
        /// </remarks>
        byte OnUlaReadMemory(ushort addr);

        /// <summary>
        /// Fills up the memory from the specified buffer
        /// </summary>
        /// <param name="buffer">Contains the row data to fill up the memory</param>
        /// <param name="startAddress">Z80 memory address to start filling up</param>
        void FillMemory(byte[] buffer, ushort startAddress = 0);
    }
}