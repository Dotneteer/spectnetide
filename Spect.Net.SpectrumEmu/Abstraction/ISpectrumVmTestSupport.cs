namespace Spect.Net.SpectrumEmu.Abstraction
{
    /// <summary>
    /// This interface defines the operations that support 
    /// the testing of a Spectrum virtual machine device.
    /// </summary>
    public interface ISpectrumVmTestSupport
    {
        /// <summary>
        /// Writes a byte to the memory
        /// </summary>
        /// <param name="addr">Memory address</param>
        /// <param name="value">Data byte</param>
        void WriteSpectrumMemory(ushort addr, byte value);
    }
}