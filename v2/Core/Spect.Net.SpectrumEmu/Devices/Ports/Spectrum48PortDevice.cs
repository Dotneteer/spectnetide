namespace Spect.Net.SpectrumEmu.Devices.Ports
{
    /// <summary>
    /// This class represents the port device used by the Spectrum 48 virtual machine
    /// </summary>
    public class Spectrum48PortDevice: UlaGenericPortDeviceBase
    {
        public Spectrum48PortDevice()
        {
            Handlers.Add(new Spectrum48PortHandler(this));
            Handlers.Add(new KempstonJoystickPortHandler(this));
        }
    }
}