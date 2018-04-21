using Spect.Net.SpectrumEmu.Abstraction.Devices;
// ReSharper disable ArgumentsStyleLiteral

namespace Spect.Net.SpectrumEmu.Devices.Ports
{
    /// <summary>
    /// This class handles the control port of DivIDE
    /// </summary>
    public class DivIdeControlPortHandler : PortHandlerBase
    {
        private IDivIdeDevice _divIdeDevice;

        private const ushort PORTMASK = 0b0000_0000_1111_1111;
        private const ushort PORT = 0b0000_0000_1110_0011;

        /// <summary>
        /// Initializes a new port handler with the specified attributes.
        /// </summary>
        /// <param name="parent">Parent device</param>
        public DivIdeControlPortHandler(IPortDevice parent) : base(parent, PORTMASK, PORT, canRead: false)
        {
        }

        /// <summary>
        /// Signs that the device has been attached to the Spectrum virtual machine
        /// </summary>
        public override void OnAttachedToVm(ISpectrumVm hostVm)
        {
            base.OnAttachedToVm(hostVm);
            _divIdeDevice = hostVm.DivIdeDevice;
        }

        /// <summary>
        /// Resets this device
        /// </summary>
        public override void Reset()
        {
            base.Reset();
            _divIdeDevice?.Reset();
        }

        /// <summary>
        /// Writes the specified value to the port
        /// </summary>
        /// <param name="addr">Full port address</param>
        /// <param name="writeValue">Value to write to the port</param>
        public override void HandleWrite(ushort addr, byte writeValue)
        {
            _divIdeDevice?.SetControlRegister(writeValue);
        }
    }
}