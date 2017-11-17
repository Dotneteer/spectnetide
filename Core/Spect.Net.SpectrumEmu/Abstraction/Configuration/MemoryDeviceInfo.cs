using Spect.Net.SpectrumEmu.Abstraction.Devices;
using Spect.Net.SpectrumEmu.Abstraction.Providers;

namespace Spect.Net.SpectrumEmu.Abstraction.Configuration
{
    /// <summary>
    /// This class describes configuration information for the memory.
    /// </summary>
    public sealed class MemoryDeviceInfo: 
        DeviceInfoBase<IMemoryDevice, IMemoryConfiguration, IMemoryProvider>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        /// <param name="configurationData">Optional configuration information</param>
        public MemoryDeviceInfo(IMemoryConfiguration configurationData) : 
            base(null, configurationData)
        {
        }
    }
}