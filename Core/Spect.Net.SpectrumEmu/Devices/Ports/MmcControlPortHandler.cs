using Spect.Net.SpectrumEmu.Abstraction.Devices;

namespace Spect.Net.SpectrumEmu.Devices.Ports
{
    /// <summary>
    /// This class handles the control port of MMC
    /// </summary>
    public class MmcControlPortHandler : PortHandlerBase
    {
        private IMmcDevice _mmcDevice;

        private const ushort PORTMASK = 0b0000_0000_1111_1111;
        private const ushort PORT = 0b0000_0000_1110_1011;

        /// <summary>
        /// Initializes a new port handler with the specified attributes.
        /// </summary>
        public MmcControlPortHandler() : base(PORTMASK, PORT)
        {
        }

        /// <summary>
        /// Signs that the device has been attached to the Spectrum virtual machine
        /// </summary>
        public override void OnAttachedToVm(ISpectrumVm hostVm)
        {
            base.OnAttachedToVm(hostVm);
            _mmcDevice = hostVm.MmcDevice;
        }

        /// <summary>
        /// Resets this device
        /// </summary>
        public override void Reset()
        {
            base.Reset();
            _mmcDevice?.Reset();
        }

        /// <summary>
        /// Writes the specified value to the port
        /// </summary>
        /// <param name="addr">Full port address</param>
        /// <param name="writeValue">Value to write to the port</param>
        public override void HandleWrite(ushort addr, byte writeValue)
        {
            _mmcDevice.WriteControlRegister(writeValue);
        }

        /// <summary>
        /// Handles the read from the port
        /// </summary>
        /// <param name="addr">Full port address</param>
        /// <param name="readValue">The value read from the port</param>
        /// <returns>True, if read handled; otherwise, false</returns>
        public override bool HandleRead(ushort addr, out byte readValue)
        {
            readValue = _mmcDevice.ReadControlRegister();
            return true;
        }
    }
}