using System.Linq;
using Spect.Net.SpectrumEmu.Abstraction.Devices;
using Spect.Net.SpectrumEmu.Devices.Memory;
using Spect.Net.SpectrumEmu.Devices.Screen;

namespace Spect.Net.SpectrumEmu.Devices.Ports
{
    /// <summary>
    /// Emulates the floating point bus of Spectrum +3E
    /// </summary>
    public class SpectrumP3FloatingPointBusPortHandler : PortHandlerBase
    {
        private const ushort PORTMASK = 0b1111_0000_0000_0011;
        private const ushort PORT = 0b0000_0000_0000_0001;

        private IScreenDevice _screenDevice;
        private SpectrumP3MemoryDevice _memoryDevice;

        /// <summary>
        /// Initializes a new port handler with the specified attributes.
        /// </summary>
        /// <param name="parent">Parent device</param>
        public SpectrumP3FloatingPointBusPortHandler(IPortDevice parent) : base(parent, PORTMASK, PORT, true, false)
        {
        }

        /// <summary>
        /// Signs that the device has been attached to the Spectrum virtual machine
        /// </summary>
        public override void OnAttachedToVm(ISpectrumVm hostVm)
        {
            base.OnAttachedToVm(hostVm);
            _screenDevice = hostVm.ScreenDevice;
            _memoryDevice = hostVm.MemoryDevice as SpectrumP3MemoryDevice;
        }

        /// <summary>
        /// Handles the read from the port
        /// </summary>
        /// <param name="addr">Full port address</param>
        /// <param name="readValue">The value read from the port</param>
        /// <returns>True, if read handled; otherwise, false</returns>
        public override bool HandleRead(ushort addr, out byte readValue)
        {
            if (ParentDevice is GenericPortDeviceBase portDev)
            {
                // --- Floating bus is available on when paging is enabled
                if (portDev.Handlers.FirstOrDefault(
                            ph => ph.GetType() == typeof(SpectrumP3MemoryPagePortHandler)) is
                        SpectrumP3MemoryPagePortHandler memPort
                    && !memPort.PagingEnabled)
                {
                    readValue = 0xFF;
                    return true;
                }
            }

            var tact = HostVm.CurrentFrameTact % _screenDevice.RenderingTactTable.Length;
            var rt = _screenDevice.RenderingTactTable[tact];
            switch (rt.Phase)
            {
                case ScreenRenderingPhase.BorderFetchPixel:
                case ScreenRenderingPhase.DisplayB2FetchB1:
                case ScreenRenderingPhase.DisplayB1FetchB2:
                    readValue = (byte)(_memoryDevice.Read(rt.PixelByteToFetchAddress, true) | 0x01);
                    break;

                case ScreenRenderingPhase.BorderFetchPixelAttr:
                case ScreenRenderingPhase.DisplayB2FetchA1:
                case ScreenRenderingPhase.DisplayB1FetchA2:
                    readValue = (byte)(_memoryDevice.Read(rt.AttributeToFetchAddress, true) | 0x01);
                    break;
                
                default:
                    readValue = _memoryDevice.LastContendedReadValue;
                    break;
            }
            return true;
        }
    }
}