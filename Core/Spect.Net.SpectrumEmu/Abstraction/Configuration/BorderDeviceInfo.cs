using Spect.Net.SpectrumEmu.Abstraction.Devices;

namespace Spect.Net.SpectrumEmu.Abstraction.Configuration
{
    /// <summary>
    /// This class describes configuration information for the border device.
    /// </summary>
    public class BorderDeviceInfo: DeviceInfoBase<IBorderDevice, INoConfiguration, INoProvider>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        /// <param name="device">Device instance</param>
        public BorderDeviceInfo(IBorderDevice device = null) : base(null, null, device)
        {
        }
    }
}