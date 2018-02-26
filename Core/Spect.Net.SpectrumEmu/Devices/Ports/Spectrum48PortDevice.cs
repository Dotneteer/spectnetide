using Spect.Net.SpectrumEmu.Abstraction.Devices;

namespace Spect.Net.SpectrumEmu.Devices.Ports
{
    /// <summary>
    /// This class represents the port device used by the Spectrum 48 virtual machine
    /// </summary>
    public class Spectrum48PortDevice: GenericPortDeviceBase
    {
        private readonly IPortHandler _handler = new Spectrum48PortHandler();

        public Spectrum48PortDevice()
        {
            Handlers.Add(_handler);
        }

        /// <summary>
        /// Handles Spectrum 48 ports
        /// </summary>
        /// <param name="addr">Port address</param>
        /// <param name="data">Data to write to the port</param>
        public void HandleSpectrum48PortWrites(ushort addr, byte data)
        {
            // --- Handle I/O contention
            ContentionWait(addr);

            // --- Carry out the I/O write operation
            if ((addr & 0x0001) == 0)
            {
                _handler.HandleWrite(addr, data);
            }
        }
    }
}