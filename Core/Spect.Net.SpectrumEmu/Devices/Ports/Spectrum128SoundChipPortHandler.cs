using Spect.Net.SpectrumEmu.Abstraction.Devices;

namespace Spect.Net.SpectrumEmu.Devices.Ports
{
    /// <summary>
    /// This class represents the handler for the Spectrum 128 PSG chip
    /// </summary>
    public class Spectrum128SoundChipPortHandler: PortHandlerBase
    {
        private ISoundDevice _soundDevice;

        /// <summary>
        /// Mask for partial port decoding
        /// </summary>
        public override ushort PortMask => 0b1000_0000_0000_0010;

        /// <summary>
        /// Port address after masking
        /// </summary>
        public override ushort Port => 0b1000_0000_0000_0000;

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
            if (addr == 0xFFFD)
            {
                _soundDevice.SetRegisterIndex(writeValue);
            }
            else if (addr == 0xBFFD || addr == 0xBEFD)
            {
                _soundDevice.SetRegisterValue(writeValue);
            }
        }
    }
}