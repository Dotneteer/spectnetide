using Spect.Net.SpectrumEmu.Abstraction.Devices;

namespace Spect.Net.SpectrumEmu.Abstraction.Configuration
{
    /// <summary>
    /// This class describes configuration information for the DivIde device.
    /// </summary>
    public class DivIdeDeviceInfo :
        DeviceInfoBase<IDivIdeDevice, INoConfiguration, INoProvider>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        /// <param name="device">DivIde device instance</param>
        public DivIdeDeviceInfo(IDivIdeDevice device) 
            : base(null, null, device)
        {
        }
    }
}