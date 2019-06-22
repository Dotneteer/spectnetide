using Spect.Net.SpectrumEmu.Abstraction.Devices;

namespace Spect.Net.SpectrumEmu.Devices.Ports
{
    /// <summary>
    /// This class represents the register value handler port 
    /// of the AY-3-8912 PSG chip
    /// </summary>
    public class SoundRegisterValuePortHandler: PortHandlerBase
    {
        private const ushort PORTMASK = 0b1100_0000_0000_0010;
        private const ushort PORT = 0b1000_0000_0000_0000;
        private ISoundDevice _soundDevice;

        /// <summary>
        /// Initializes a new port handler with the specified attributes.
        /// </summary>
        public SoundRegisterValuePortHandler(IPortDevice parent) : base(parent, PORTMASK, PORT)
        {
        }

        /// <summary>
        /// Signs that the device has been attached to the Spectrum virtual machine
        /// </summary>
        public override void OnAttachedToVm(ISpectrumVm hostVm)
        {
            base.OnAttachedToVm(hostVm);
            _soundDevice = hostVm.SoundDevice;
        }

        /// <summary>
        /// Handles the read from the port
        /// </summary>
        /// <param name="addr">Full port address</param>
        /// <param name="readValue">The value read from the port</param>
        /// <returns>True, if read handled; otherwise, false</returns>
        public override bool HandleRead(ushort addr, out byte readValue)
        {
            readValue = _soundDevice.GetRegisterValue();
            return true;
        }

        /// <summary>
        /// Writes the specified value to the port
        /// </summary>
        /// <param name="addr">Full port address</param>
        /// <param name="writeValue">Value to write to the port</param>
        public override void HandleWrite(ushort addr, byte writeValue)
        {
            _soundDevice.SetRegisterValue(writeValue);
        }
    }
}