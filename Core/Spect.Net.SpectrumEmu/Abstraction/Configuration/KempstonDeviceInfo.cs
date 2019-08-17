using Spect.Net.SpectrumEmu.Abstraction.Devices;
using Spect.Net.SpectrumEmu.Abstraction.Providers;

namespace Spect.Net.SpectrumEmu.Abstraction.Configuration
{
    /// <summary>
    /// This class describes configuration information for the Kempston device.
    /// </summary>
    public class KempstonDeviceInfo :
        DeviceInfoBase<IKempstonDevice, INoConfiguration, IKempstonProvider>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        /// <param name="provider">Kempston provider</param>
        public KempstonDeviceInfo(IKempstonProvider provider) : base(provider)
        {
        }
    }
}