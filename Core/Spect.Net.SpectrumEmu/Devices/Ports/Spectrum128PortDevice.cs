using Spect.Net.SpectrumEmu.Abstraction.Devices;

namespace Spect.Net.SpectrumEmu.Devices.Ports
{
    /// <summary>
    /// This class represents the port device used by the Spectrum 128 virtual machine
    /// </summary>
    public class Spectrum128PortDevice: GenericPortDeviceBase
    {
        protected IMemoryDevice MemoryDevice;
        protected ISoundDevice SoundDevice;

        /// <summary>
        /// Indicates if paging is enabled or not
        /// </summary>
        /// <remarks>
        /// Port 0x7FFD, Bit 5: 
        /// False - paging is enables
        /// True - paging is not enabled and further output to the port
        /// is ignored until the computer is reset
        /// </remarks>
        public bool PagingEnabled { get; protected set; }

        public Spectrum128PortDevice()
        {
            Handlers.Add(new Spectrum48PortHandler());
            Handlers.Add(new Spectrum128MemoryPagePortHandler());
            Handlers.Add(new SoundRegisterIndexPortHandler());
            Handlers.Add(new SoundRegisterValuePortHandler());
        }

        /// <summary>
        /// Gets the state of the device so that the state can be saved
        /// </summary>
        /// <returns>The object that describes the state of the device</returns>
        public override IDeviceState GetState() => null;

        /// <summary>
        /// Sets the state of the device from the specified object
        /// </summary>
        /// <param name="state">Device state</param>
        public override void RestoreState(IDeviceState state)
        {
        }
    }
}