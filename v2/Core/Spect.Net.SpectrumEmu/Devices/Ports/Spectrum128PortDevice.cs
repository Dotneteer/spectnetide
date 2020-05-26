using Spect.Net.SpectrumEmu.Abstraction.Devices;
using Spect.Net.SpectrumEmu.Abstraction.Devices.Screen;

namespace Spect.Net.SpectrumEmu.Devices.Ports
{
    /// <summary>
    /// This class represents the port device used by the Spectrum 128 virtual machine
    /// </summary>
    public class Spectrum128PortDevice: UlaGenericPortDeviceBase
    {
        private readonly Spectrum128MemoryPagePortHandler _memoryHandler;
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
            Handlers.Add(new Spectrum48PortHandler(this));
            _memoryHandler = new Spectrum128MemoryPagePortHandler(this);
            Handlers.Add(_memoryHandler);
            Handlers.Add(new SoundRegisterIndexPortHandler(this));
            Handlers.Add(new SoundRegisterValuePortHandler(this));
        }

        /// <summary>
        /// Define how to handle an unattached port
        /// </summary>
        /// <param name="addr">Port address</param>
        /// <returns>Port value for the unhandled port address</returns>
        public override byte UnhandledRead(ushort addr)
        {
            var tact = HostVm.CurrentFrameTact % ScreenDevice.RenderingTactTable.Length;
            var rt = ScreenDevice.RenderingTactTable[tact];
            var memAddr = (ushort)0;
            switch (rt.Phase)
            {
                case ScreenRenderingPhase.BorderFetchPixel:
                case ScreenRenderingPhase.DisplayB1FetchB2:
                case ScreenRenderingPhase.DisplayB2FetchB1:
                    memAddr = rt.PixelByteToFetchAddress;
                    break;
                case ScreenRenderingPhase.BorderFetchPixelAttr:
                case ScreenRenderingPhase.DisplayB1FetchA2:
                case ScreenRenderingPhase.DisplayB2FetchA1:
                    memAddr = rt.AttributeToFetchAddress;
                    break;
            }

            if (memAddr == 0) return 0xFF;
            var readValue = MemoryDevice.Read(memAddr, true);
            return readValue;
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