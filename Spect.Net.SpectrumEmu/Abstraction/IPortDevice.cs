namespace Spect.Net.SpectrumEmu.Abstraction
{
    /// <summary>
    /// This interface represents the port device used by the Z80 CPU.
    /// </summary>
    public interface IPortDevice: IDevice
    {
        /// <summary>
        /// Reads the port with the specified address
        /// </summary>
        /// <param name="addr">Port address</param>
        /// <returns>Byte read from the port</returns>
        byte OnReadPort(ushort addr);

        /// <summary>
        /// Sends a byte to the port with the specified address
        /// </summary>
        /// <param name="addr">Port address</param>
        /// <param name="data">Data to write to the port</param>
        /// <returns>Byte read from the memory</returns>
        void OnWritePort(ushort addr, byte data);
    }
}