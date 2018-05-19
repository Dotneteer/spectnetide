using Spect.Net.SpectrumEmu.Abstraction.Devices;

namespace Spect.Net.SpectrumEmu.Devices.Ports
{
    /// <summary>
    /// Emulates the floating point bus of Spectrum +3E
    /// </summary>
    public class SpectrumP3FloppyStatusPortHandler : PortHandlerBase
    {
        private const ushort PORTMASK = 0b1111_1111_1111_1111;
        private const ushort PORT = 0b0010_1111_1111_1101;

        private IFloppyDevice _floppyDevice;

        /// <summary>
        /// Initializes a new port handler with the specified attributes.
        /// </summary>
        /// <param name="parent">Parent device</param>
        public SpectrumP3FloppyStatusPortHandler(IPortDevice parent) : base(parent, PORTMASK, PORT, true, false)
        {
        }

        /// <summary>
        /// Signs that the device has been attached to the Spectrum virtual machine
        /// </summary>
        public override void OnAttachedToVm(ISpectrumVm hostVm)
        {
            base.OnAttachedToVm(hostVm);
            _floppyDevice = hostVm.FloppyDevice;
        }

        /// <summary>
        /// Handles the read from the port
        /// </summary>
        /// <param name="addr">Full port address</param>
        /// <param name="readValue">The value read from the port</param>
        /// <returns>True, if read handled; otherwise, false</returns>
        public override bool HandleRead(ushort addr, out byte readValue)
        {
            readValue = _floppyDevice?.MainStatusRegister ?? 0xFF;
            return true;
        }
    }
}