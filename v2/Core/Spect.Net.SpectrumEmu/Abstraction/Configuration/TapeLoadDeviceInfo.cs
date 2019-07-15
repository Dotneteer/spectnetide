using Spect.Net.SpectrumEmu.Abstraction.Devices;
using Spect.Net.SpectrumEmu.Abstraction.Providers;

namespace Spect.Net.SpectrumEmu.Abstraction.Configuration
{
    /// <summary>
    /// This class describes configuration information for the tape load device.
    /// </summary>
    public sealed class TapeLoadDeviceInfo :
        DeviceInfoBase<ITapeLoadDevice, INoConfiguration, ITapeLoadProvider>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        /// <param name="loadProvider">Optional provider instance.</param>
        public TapeLoadDeviceInfo(ITapeLoadProvider loadProvider = default) : base(loadProvider)
        {
        }
    }
}