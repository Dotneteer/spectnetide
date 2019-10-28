namespace Spect.Net.SpectrumEmu.Abstraction.TestSupport
{
    /// <summary>
    /// This interface defines the operations that support 
    /// the testing of a Spectrum virtual machine device.
    /// </summary>
    public interface ISpectrumVmTestSupport
    {
        /// <summary>
        /// This flag tells if the frame has just been completed.
        /// </summary>
        bool HasFrameCompleted { get; }

        /// <summary>
        /// Writes a byte to the memory.
        /// </summary>
        /// <param name="addr">Memory address.</param>
        /// <param name="value">Data byte.</param>
        void WriteSpectrumMemory(ushort addr, byte value);

        /// <summary>
        /// Sets the ULA frame tact for testing purposes.
        /// </summary>
        /// <param name="tacts">ULA frame tact to set.</param>
        void SetUlaFrameTact(int tacts);
    }
}