using Spect.Net.SpectrumEmu.Abstraction.Devices;

namespace Spect.Net.SpectrumEmu.Abstraction.Configuration
{
    /// <summary>
    /// This class describes configuration information for the MultiMedia Card device.
    /// </summary>
    public class MmcDeviceInfo :
        DeviceInfoBase<IMmcDevice, INoConfiguration, INoProvider>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        /// <param name="device">MMC device instance</param>
        public MmcDeviceInfo(IMmcDevice device) 
            : base(null, null, device)
        {
        }
    }
}