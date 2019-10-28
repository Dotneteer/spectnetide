using System;
using Spect.Net.SpectrumEmu.Abstraction.Devices;
using Spect.Net.SpectrumEmu.Abstraction.Providers;

namespace Spect.Net.SpectrumEmu.Abstraction.Configuration
{
    /// <summary>
    /// This interface couples a particular device with its configuration
    /// information and provider.
    /// </summary>
    /// <typeparam name="TDevice">Device type</typeparam>
    /// <typeparam name="TConfig">Configuration type</typeparam>
    /// <typeparam name="TProvider">Provider type</typeparam>
    public interface IDeviceInfo<out TDevice, out TConfig, out TProvider>
        where TDevice: IDevice
        where TConfig: IDeviceConfiguration
        where TProvider: IVmComponentProvider
    {
        /// <summary>
        /// The type of the device
        /// </summary>
        Type DeviceType { get; }

        /// <summary>
        /// The optional device instance.
        /// </summary>
        /// <remarks>
        /// If not provided, the virtual machine may ignore this device,
        /// or use its default one.
        /// </remarks>
        TDevice Device { get; }

        /// <summary>
        /// Optional configuration object for the device
        /// </summary>
        TConfig ConfigurationData { get; }

        /// <summary>
        /// Optional provider for the device
        /// </summary>
        TProvider Provider { get; }
    }
}