using Spect.Net.SpectrumEmu.Abstraction.Devices;

namespace Spect.Net.SpectrumEmu.Devices.Ports
{
    /// <summary>
    /// This class represents the port device used by the Spectrum 128 virtual machine
    /// </summary>
    public class Spectrum128PortDevice: GenericPortDeviceBase
    {
        private readonly Spectrum128MemoryPagePortHandler _memoryHandler;
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
            _memoryHandler = new Spectrum128MemoryPagePortHandler();
            Handlers.Add(_memoryHandler);
            Handlers.Add(new SoundRegisterIndexPortHandler());
            Handlers.Add(new SoundRegisterValuePortHandler());
        }

        /// <summary>
        /// Gets the state of the device so that the state can be saved
        /// </summary>
        /// <returns>The object that describes the state of the device</returns>
        public override IDeviceState GetState() => new Spectrum128PortDeviceState(this);

        /// <summary>
        /// Sets the state of the device from the specified object
        /// </summary>
        /// <param name="state">Device state</param>
        public override void RestoreState(IDeviceState state) => state.RestoreDeviceState(this);

        /// <summary>
        /// State of the Spectrum 128 port device
        /// </summary>
        public class Spectrum128PortDeviceState : IDeviceState
        {
            public bool PagingEnabled { get; set; }

            public Spectrum128PortDeviceState()
            {
            }

            public Spectrum128PortDeviceState(Spectrum128PortDevice device)
            {
                PagingEnabled = device._memoryHandler.PagingEnabled;
            }

            /// <summary>
            /// Restores the dvice state from this state object
            /// </summary>
            /// <param name="device">Device instance</param>
            public void RestoreDeviceState(IDevice device)
            {
                if (!(device is Spectrum128PortDevice sp128)) return;

                sp128._memoryHandler.PagingEnabled = PagingEnabled;
            }
        }
    }
}