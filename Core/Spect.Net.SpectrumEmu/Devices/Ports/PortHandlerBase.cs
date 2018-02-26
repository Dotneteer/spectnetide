using Spect.Net.SpectrumEmu.Abstraction.Devices;

namespace Spect.Net.SpectrumEmu.Devices.Ports
{
    /// <summary>
    /// This class is intended to be the base class of all port handlers
    /// </summary>
    public abstract class PortHandlerBase: IPortHandler
    {
        public ISpectrumVm HostVm { get; private set; }

        /// <summary>
        /// Resets the port handlerto it initial state
        /// </summary>
        public virtual void Reset()
        {
        }

        /// <summary>
        /// Signs that the device has been attached to the Spectrum virtual machine
        /// </summary>
        public virtual void OnAttachedToVm(ISpectrumVm hostVm)
        {
            HostVm = hostVm;
        }

        /// <summary>
        /// Mask for partial port decoding
        /// </summary>
        public abstract ushort PortMask { get; }

        /// <summary>
        /// Port address after masking
        /// </summary>
        public abstract ushort Port { get; }

        /// <summary>
        /// Can handle input operations?
        /// </summary>
        public virtual bool CanRead => true;

        /// <summary>
        /// Can handle output operations?
        /// </summary>
        public virtual bool CanWrite => true;

        /// <summary>
        /// Handles the read from the port
        /// </summary>
        /// <param name="addr">Full port address</param>
        /// <param name="readValue">The value read from the port</param>
        /// <returns>True, if read handled; otherwise, false</returns>
        public virtual bool HandleRead(ushort addr, out byte readValue)
        {
            readValue = 0xff;
            return false;
        }

        /// <summary>
        /// Writes the specified value to the port
        /// </summary>
        /// <param name="addr">Full port address</param>
        /// <param name="writeValue">Value to write to the port</param>
        public virtual void HandleWrite(ushort addr, byte writeValue)
        {
        }
    }
}