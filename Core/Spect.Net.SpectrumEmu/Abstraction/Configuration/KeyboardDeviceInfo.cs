using Spect.Net.SpectrumEmu.Abstraction.Devices;
using Spect.Net.SpectrumEmu.Abstraction.Providers;

namespace Spect.Net.SpectrumEmu.Abstraction.Configuration
{
    /// <summary>
    /// This class describes configuration information for the keyboard device.
    /// </summary>
    public class KeyboardDeviceInfo :
        DeviceInfoBase<IKeyboardDevice, INoConfiguration, IKeyboardProvider>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        /// <param name="provider">Optional provider instance</param>
        /// <param name="device">Keyboard device instance</param>
        public KeyboardDeviceInfo(IKeyboardProvider provider, IKeyboardDevice device) 
            : base(provider, null, device)
        {
        }
    }
}