using Spect.Net.SpectrumEmu.Abstraction.Devices;
using Spect.Net.SpectrumEmu.Abstraction.Providers;

namespace Spect.Net.SpectrumEmu.Abstraction.Configuration
{
    /// <summary>
    /// This class describes configuration information for the tape device.
    /// </summary>
    public sealed class SoundDeviceInfo:
        DeviceInfoBase<ISoundDevice, IAudioConfiguration, ISoundProvider>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        /// <param name="provider">Optional provider instance</param>
        /// <param name="configuration">Configuration data</param>
        public SoundDeviceInfo(IAudioConfiguration configuration, ISoundProvider provider) : 
            base(provider, configuration)
        {
        }
    }
}