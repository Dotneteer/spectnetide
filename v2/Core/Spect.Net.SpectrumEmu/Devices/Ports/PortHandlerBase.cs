using Spect.Net.SpectrumEmu.Abstraction.Devices;
using Spect.Net.SpectrumEmu.Abstraction.Machine;

namespace Spect.Net.SpectrumEmu.Devices.Ports
{
    /// <summary>
    /// This class is intended to be the base class of all port handlers
    /// </summary>
    public abstract class PortHandlerBase: IPortHandler
    {
        /// <summary>
        /// The host virtual machine
        /// </summary>
        public ISpectrumVm HostVm { get; private set; }

        /// <summary>
        /// Resets the port handlerto it initial state
        /// </summary>
        public virtual void Reset()
        {
        }

        /// <summary>
        /// Gets the parent device of this port handler
        /// </summary>
        public IPortDevice ParentDevice { get; }

        /// <summary>
        /// Initializes a new port handler with the specified attributes.
        /// </summary>
        /// <param name="parent">Parent device</param>
        /// <param name="mask">Port mask</param>
        /// <param name="port">Port number after masking</param>
        /// <param name="canRead">Read supported?</param>
        /// <param name="canWrite">Write supported?</param>
        protected PortHandlerBase(IPortDevice parent, ushort mask, ushort port, 
            bool canRead = true, bool canWrite = true)
        {
            ParentDevice = parent;
            PortMask = mask;
            Port = port;
            CanRead = canRead;
            CanWrite = canWrite;
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
        public ushort PortMask { get; }

        /// <summary>
        /// Port address after masking
        /// </summary>
        public ushort Port { get; }

        /// <summary>
        /// Can handle input operations?
        /// </summary>
        public bool CanRead { get; }

        /// <summary>
        /// Can handle output operations?
        /// </summary>
        public bool CanWrite { get; }

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