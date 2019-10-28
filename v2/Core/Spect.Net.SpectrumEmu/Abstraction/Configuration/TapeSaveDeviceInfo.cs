using Spect.Net.SpectrumEmu.Abstraction.Devices;
using Spect.Net.SpectrumEmu.Abstraction.Providers;

namespace Spect.Net.SpectrumEmu.Abstraction.Configuration
{
    /// <summary>
    /// This class describes configuration information for the tape save device.
    /// </summary>
    public sealed class TapeSaveDeviceInfo :
        DeviceInfoBase<ITapeSaveDevice, INoConfiguration, ITapeSaveProvider>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        /// <param name="saveProvider">Optional provider instance.</param>
        public TapeSaveDeviceInfo(ITapeSaveProvider saveProvider = default) : base(saveProvider)
        {
        }
    }
}