using Spect.Net.SpectrumEmu.Abstraction.Devices;

namespace Spect.Net.SpectrumEmu.Devices.Ports
{
    /// <summary>
    /// This class handles the extended memory paging port of Spectrum +3E
    /// </summary>
    public class NextRegisterSelectPortHandler : PortHandlerBase
    {
        private INextFeatureSetDevice _nextDevice;

        private const ushort PORTMASK = 0b0010_0100_0011_1011;
        private const ushort PORT = 0b1111_1111_1111_1111;

        /// <summary>
        /// Initializes a new port handler with the specified attributes.
        /// </summary>
        public NextRegisterSelectPortHandler() : base(PORTMASK, PORT, false)
        {
        }

        /// <summary>
        /// Signs that the device has been attached to the Spectrum virtual machine
        /// </summary>
        public override void OnAttachedToVm(ISpectrumVm hostVm)
        {
            base.OnAttachedToVm(hostVm);
            _nextDevice = hostVm.NextDevice;
        }

        /// <summary>
        /// Resets this device
        /// </summary>
        public override void Reset()
        {
            base.Reset();
            _nextDevice.Reset();
        }

        /// <summary>
        /// Writes the specified value to the port
        /// </summary>
        /// <param name="addr">Full port address</param>
        /// <param name="writeValue">Value to write to the port</param>
        public override void HandleWrite(ushort addr, byte writeValue)
        {
            _nextDevice.SetRegisterIndex(writeValue);
        }
    }
}