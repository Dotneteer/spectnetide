using Spect.Net.SpectrumEmu.Abstraction.Devices;
using Spect.Net.SpectrumEmu.Abstraction.Providers;

namespace Spect.Net.SpectrumEmu.Abstraction.Configuration
{
    /// <summary>
    /// This class describes configuration information for the ROM device.
    /// </summary>
    public sealed class RomDeviceInfo: DeviceInfoBase<IRomDevice, IRomConfiguration, IRomProvider>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        /// <param name="provider">Optional provider instance</param>
        /// <param name="configurationData">Optional configuration information</param>
        public RomDeviceInfo(IRomProvider provider, IRomConfiguration configurationData) : 
            base(provider, configurationData)
        {
        }
    }
}