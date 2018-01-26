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
        byte ReadPort(ushort addr);

        /// <summary>
        /// Sends a byte to the port with the specified address
        /// </summary>
        /// <param name="addr">Port address</param>
        /// <param name="data">Data to write to the port</param>
        void WritePort(ushort addr, byte data);

        /// <summary>
        /// Emulates I/O contention
        /// </summary>
        /// <param name="addr">Contention address</param>
        void ContentionWait(ushort addr);
    }
}