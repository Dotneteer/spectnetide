using Spect.Net.SpectrumEmu.Abstraction.Devices;

namespace Spect.Net.SpectrumEmu.Devices.Ports
{
    /// <summary>
    /// This port handler emulates the Kempston Joystick
    /// </summary>
    public class KempstonJoystickPortHandler : PortHandlerBase
    {
        private const ushort PORTMASK = 0b0000_0000_0001_1111;
        private const ushort PORT = 0b0000_0000_0001_1111;

        public KempstonJoystickPortHandler(IPortDevice parent) : base(parent, PORTMASK, PORT, true, false)
        {
        }

        /// <summary>
        /// Handles the read from the port
        /// </summary>
        /// <param name="addr">Full port address</param>
        /// <param name="readValue">The value read from the port</param>
        /// <returns>True, if read handled; otherwise, false</returns>
        public override bool HandleRead(ushort addr, out byte readValue)
        {
            readValue = 0xff;
            var device = HostVm.KempstonDevice;
            if (device == null || !device.IsPresent) return false;
            readValue = (byte)(
                (device.RightPressed ? 0x01 : 0x00) |
                (device.LeftPressed ? 0x02 : 0x00) |
                (device.DownPressed ? 0x04 : 0x00) |
                (device.UpPressed ? 0x08 : 0x00) |
                (device.FirePressed ? 0x10 : 0x00));
            return true;
        }
    }
}