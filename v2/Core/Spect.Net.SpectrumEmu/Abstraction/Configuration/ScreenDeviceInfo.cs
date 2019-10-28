using Spect.Net.SpectrumEmu.Abstraction.Devices;
using Spect.Net.SpectrumEmu.Abstraction.Providers;

namespace Spect.Net.SpectrumEmu.Abstraction.Configuration
{
    /// <summary>
    /// This class describes configuration information for the screen device.
    /// </summary>
    public sealed class ScreenDeviceInfo : 
        DeviceInfoBase<IScreenDevice, IScreenConfiguration, IScreenFrameProvider>
    {
        public ScreenDeviceInfo(IScreenConfiguration configurationData = default, 
            IScreenFrameProvider provider = default) : 
            base(provider, configurationData)
        {
        }
    }
}