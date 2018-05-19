using Spect.Net.SpectrumEmu.Abstraction.Devices;

namespace Spect.Net.SpectrumEmu.Abstraction.Configuration
{
    /// <summary>
    /// This class describes configuration information for the DivIde device.
    /// </summary>
    public class FloppyDeviceInfo :
        DeviceInfoBase<IFloppyDevice, INoConfiguration, INoProvider>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        /// <param name="device">DivIde device instance</param>
        public FloppyDeviceInfo(IFloppyDevice device) 
            : base(null, null, device)
        {
        }
    }
}