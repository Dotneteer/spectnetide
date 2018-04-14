using Spect.Net.SpectrumEmu.Abstraction.Devices;

namespace Spect.Net.SpectrumEmu.Devices.Ports
{
    /// <summary>
    /// This class represents the port device used by the Spectrum 48 virtual machine
    /// </summary>
    public class Spectrum48PortDevice: UlaGenericPortDeviceBase
    {
        private readonly IPortHandler _handler = new Spectrum48PortHandler();

        public Spectrum48PortDevice()
        {
            Handlers.Add(_handler);
        }
    }
}