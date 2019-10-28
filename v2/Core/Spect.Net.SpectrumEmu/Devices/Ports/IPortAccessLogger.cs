namespace Spect.Net.SpectrumEmu.Devices.Ports
{
    /// <summary>
    /// This interface can be used to log port access
    /// </summary>
    public interface IPortAccessLogger
    {
        /// <summary>
        /// Catch the event when a port has been read
        /// </summary>
        /// <param name="addr">Port address</param>
        /// <param name="value">Value read from the port</param>
        /// <param name="handled">The read request was handled</param>
        void PortRead(ushort addr, byte value, bool handled);

        /// <summary>
        /// Catch the event when a port has been written
        /// </summary>
        /// <param name="addr">Port address</param>
        /// <param name="value">Value read from the port</param>
        /// <param name="handled">The write request was handled</param>
        void PortWritten(ushort addr, byte value, bool handled);
    }
}