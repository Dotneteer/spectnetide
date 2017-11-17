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
        where TDevice : IDevice 
        where TConfig : IDeviceConfiguration 
        where TProvider : IVmComponentProvider
    {
        /// <summary>
        /// The type of the device
        /// </summary>
        public Type DeviceType { get; }

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
        /// <param name="configurationData">Optional configuration information</param>
        /// <param name="provider">Optional provider instance</param>
        protected DeviceInfoBase(TConfig configurationData = default(TConfig), TProvider provider = default(TProvider))
        {
            DeviceType = typeof(TDevice);
            ConfigurationData = configurationData;
            Provider = provider;
        }
    }

    /// <summary>
    /// This class describes configuration information for the CPU device.
    /// </summary>
    public sealed class CpuDeviceInfo : DeviceInfoBase<IZ80Cpu, ICpuConfiguration, IVmComponentProvider>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        /// <param name="configurationData">Optional configuration information</param>
        public CpuDeviceInfo(ICpuConfiguration configurationData) : base(configurationData)
        {
        }
    }
}