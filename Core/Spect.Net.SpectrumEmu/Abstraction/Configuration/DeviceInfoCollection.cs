using System;
using System.Collections.Generic;
using Spect.Net.SpectrumEmu.Abstraction.Devices;
using Spect.Net.SpectrumEmu.Abstraction.Providers;

namespace Spect.Net.SpectrumEmu.Abstraction.Configuration
{
    /// <summary>
    /// This class provides a collection of device information for a particular
    /// ZX Spectrum virtual machine
    /// </summary>
    public class DeviceInfoCollection: 
        Dictionary<Type, IDeviceInfo<IDevice, IDeviceConfiguration, IVmComponentProvider>>
    {
        /// <summary>
        /// Adds device information using the device type as the key in the dictionary
        /// </summary>
        /// <param name="deviceInfo">Device information</param>
        public void Add(IDeviceInfo<IDevice, IDeviceConfiguration, IVmComponentProvider> deviceInfo)
        {
            Add(deviceInfo.DeviceType, deviceInfo);
        }
    }
}