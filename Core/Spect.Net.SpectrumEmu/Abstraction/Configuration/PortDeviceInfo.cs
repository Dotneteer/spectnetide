using Spect.Net.SpectrumEmu.Abstraction.Devices;
using Spect.Net.SpectrumEmu.Abstraction.Providers;

namespace Spect.Net.SpectrumEmu.Abstraction.Configuration
{
    /// <summary>
    /// This class describes configuration information for the I/O ports.
    /// </summary>
    public sealed class PortDeviceInfo:
        DeviceInfoBase<IPortDevice, IPortConfiguration, IPortProvider>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        /// <param name="configurationData">Optional configuration information</param>
        public PortDeviceInfo(IPortConfiguration configurationData = default(IPortConfiguration)) : 
            base(null, configurationData)
        {
        }
    }
}