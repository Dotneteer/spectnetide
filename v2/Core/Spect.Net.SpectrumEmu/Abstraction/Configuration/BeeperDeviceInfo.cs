using Spect.Net.SpectrumEmu.Abstraction.Devices;
using Spect.Net.SpectrumEmu.Abstraction.Providers;

namespace Spect.Net.SpectrumEmu.Abstraction.Configuration
{
    /// <summary>
    /// This class describes configuration information for the beeper device.
    /// </summary>
    public class BeeperDeviceInfo: DeviceInfoBase<IBeeperDevice, IAudioConfiguration, IBeeperProvider>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        /// <param name="configurationData">Optional configuration information</param>
        /// <param name="provider">Optional provider instance</param>
        public BeeperDeviceInfo(IAudioConfiguration configurationData, IBeeperProvider provider) : 
            base(provider, configurationData)
        {
        }
    }
}