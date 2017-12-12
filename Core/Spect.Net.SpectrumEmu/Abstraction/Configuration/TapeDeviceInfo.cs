using Spect.Net.SpectrumEmu.Abstraction.Devices;
using Spect.Net.SpectrumEmu.Abstraction.Providers;

namespace Spect.Net.SpectrumEmu.Abstraction.Configuration
{
    /// <summary>
    /// This class describes configuration information for the tape device.
    /// </summary>
    public sealed class TapeDeviceInfo:
        DeviceInfoBase<ITapeDevice, INoConfiguration, ITapeProvider>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        /// <param name="provider">Optional provider instance</param>
        public TapeDeviceInfo(ITapeProvider provider = default(ITapeProvider)) : base(provider)
        {
        }
    }
}