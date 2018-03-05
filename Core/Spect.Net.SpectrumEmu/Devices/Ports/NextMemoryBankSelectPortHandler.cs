using Spect.Net.SpectrumEmu.Abstraction.Devices;

namespace Spect.Net.SpectrumEmu.Devices.Ports
{
    /// <summary>
    /// This class handles additional bank select bits 
    /// for extended memory on Spectrum Next
    /// </summary>
    public class NextMemoryBankSelectPortHandler : PortHandlerBase
    {
        private const ushort PORTMASK = 0b1101_1111_1111_1101;
        private const ushort PORT = 0b1101_1111_1111_1101;

        /// <summary>
        /// Initializes a new port handler with the specified attributes.
        /// </summary>
        public NextMemoryBankSelectPortHandler() : base(PORTMASK, PORT, false)
        {
        }

        /// <summary>
        /// Signs that the device has been attached to the Spectrum virtual machine
        /// </summary>
        public override void OnAttachedToVm(ISpectrumVm hostVm)
        {
            base.OnAttachedToVm(hostVm);
        }

        /// <summary>
        /// Resets this device
        /// </summary>
        public override void Reset()
        {
            base.Reset();
        }

        /// <summary>
        /// Writes the specified value to the port
        /// </summary>
        /// <param name="addr">Full port address</param>
        /// <param name="writeValue">Value to write to the port</param>
        public override void HandleWrite(ushort addr, byte writeValue)
        {
        }
    }
}