using Spect.Net.SpectrumEmu.Abstraction.Devices;
using Spect.Net.SpectrumEmu.Abstraction.Providers;

namespace Spect.Net.SpectrumEmu.Abstraction.Configuration
{
    /// <summary>
    /// This class describes configuration information for the clock device.
    /// </summary>
    public sealed class ClockDeviceInfo: DeviceInfoBase<IClockDevice, INoConfiguration, IClockProvider>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        /// <param name="provider">Optional provider instance</param>
        public ClockDeviceInfo(IClockProvider provider) : base(provider)
        {
        }
    }
}