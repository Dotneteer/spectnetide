using System;
using Spect.Net.SpectrumEmu.Abstraction.Devices;
using Spect.Net.SpectrumEmu.Abstraction.Providers;

namespace Spect.Net.SpectrumEmu.Abstraction.Configuration
{
    /// <summary>
    /// This class is intended to be a base class for all device information
    /// classes.
    /// </summary>
    /// <typeparam name="TDevice">Device type</typeparam>
    /// <typeparam name="TConfig">Configuration type</typeparam>
    /// <typeparam name="TProvider">Provider type</typeparam>
    public abstract class DeviceInfoBase<TDevice, TConfig, TProvider> : 
        IDeviceInfo<TDevice, TConfig, TProvider> 
        where TDevice : class, IDevice 
        where TConfig : class, IDeviceConfiguration 
        where TProvider : class, IVmComponentProvider
    {
        /// <summary>
        /// The type of the device
        /// </summary>
        public Type DeviceType { get; }

        /// <summary>
        /// The optional device instance.
        /// </summary>
        /// <remarks>
        /// If not provided, the virtual machine may ignore this device,
        /// or use its default one.
        /// </remarks>
        public TDevice Device { get; set; }

        /// <summary>
        /// Optional configuration object for the device
        /// </summary>
        public TConfig ConfigurationData { get; }

        /// <summary>
        /// Optional provider for the device
        /// </summary>
        public TProvider Provider { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        /// <param name="provider">Optional provider instance</param>
        /// <param name="configurationData">Optional configuration information</param>
        /// <param name="device">Optional device instance</param>
        protected DeviceInfoBase(TProvider provider = null,
            TConfig configurationData = null, 
            TDevice device = null)
        {
            DeviceType = typeof(TDevice);
            Device = device;
            ConfigurationData = configurationData;
            Provider = provider;
        }
    }
}