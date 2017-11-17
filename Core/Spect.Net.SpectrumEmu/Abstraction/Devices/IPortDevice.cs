namespace Spect.Net.SpectrumEmu.Abstraction.Devices
{
    /// <summary>
    /// This interface represents a port device that can be attached to a 
    /// Spectrum virtual machine
    /// </summary>
    public interface IPortDevice : ISpectrumBoundDevice
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