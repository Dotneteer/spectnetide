namespace Spect.Net.SpectrumEmu.Abstraction.Devices
{
    /// <summary>
    /// This interface represents an entity that can handle a fully or
    /// partially decoded Spectrum port
    /// </summary>
    public interface IPortHandler
    {
        /// <summary>
        /// Resets the port handlerto it initial state
        /// </summary>
        void Reset();

        /// <summary>
        /// Signs that the device has been attached to the Spectrum virtual machine
        /// </summary>
        void OnAttachedToVm(ISpectrumVm hostVm);

        /// <summary>
        /// Mask for partial port decoding
        /// </summary>
        ushort PortMask { get; }

        /// <summary>
        /// Port address after masking
        /// </summary>
        ushort Port { get; }

        /// <summary>
        /// Can handle input operations?
        /// </summary>
        bool CanRead { get; }

        /// <summary>
        /// Can handle output operations?
        /// </summary>
        bool CanWrite { get; }

        /// <summary>
        /// Handles the read from the port
        /// </summary>
        /// <param name="addr">Full port address</param>
        /// <param name="readValue">The value read from the port</param>
        /// <returns>True, if read handled; otherwise, false</returns>
        bool HandleRead(ushort addr, out byte readValue);

        /// <summary>
        /// Writes the specified value to the port
        /// </summary>
        /// <param name="addr">Full port address</param>
        /// <param name="writeValue">Value to write to the port</param>
        void HandleWrite(ushort addr, byte writeValue);
    }
}