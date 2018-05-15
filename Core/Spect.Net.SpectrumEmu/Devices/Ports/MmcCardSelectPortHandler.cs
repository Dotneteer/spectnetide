using Spect.Net.SpectrumEmu.Abstraction.Devices;

namespace Spect.Net.SpectrumEmu.Devices.Ports
{
    /// <summary>
    /// This class handles the card select port of MMC
    /// </summary>
    public class MmcCardSelectPortHandler : PortHandlerBase
    {
        private IMmcDevice _mmcDevice;

        private const ushort PORTMASK = 0b0000_0000_1111_1111;
        private const ushort PORT = 0b0000_0000_1110_0111;

        /// <summary>
        /// Initializes a new port handler with the specified attributes.
        /// </summary>
        public MmcCardSelectPortHandler(IPortDevice parent) : base(parent, PORTMASK, PORT, false)
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
            if (writeValue == 0xF6)
            {
                writeValue = 0xFE;
            }
            _mmcDevice.SelectCard(writeValue);
        }
    }
}