using System;
using System.Collections.Generic;
using Spect.Net.RomResources;
using Spect.Net.SpectrumEmu.Abstraction.Configuration;
using Spect.Net.SpectrumEmu.Abstraction.Models;
using Spect.Net.SpectrumEmu.Abstraction.Providers;
using Spect.Net.SpectrumEmu.Devices.Keyboard;
using Spect.Net.SpectrumEmu.Devices.Memory;
using Spect.Net.SpectrumEmu.Devices.Ports;
using Spect.Net.SpectrumEmu.Devices.Rom;
using Spect.Net.SpectrumEmu.Providers;

namespace Spect.Net.SpectrumEmu.Scripting
{
    /// <summary>
    /// This class provides methods that create Spectrum virtual machine instances.
    /// </summary>
    public static class SpectrumVmFactory
    {
        #region Static data members and their initialization

        // --- Provider factories to create providers when instantiating the machine
        private static readonly Dictionary<Type, Func<object>> s_ProviderFactories =
            new Dictionary<Type, Func<object>>();

        /// <summary>
        /// Resets the class members when first accessed
        /// </summary>
        static SpectrumVmFactory()
        {
            Reset();
            RegisterDefaultProviders();
        }

        /// <summary>
        /// Resets the static members of this class
        /// </summary>
        public static void Reset()
        {
            s_ProviderFactories.Clear();
        }

        /// <summary>
        /// Registers a provider
        /// </summary>
        /// <typeparam name="TProvider">Provider type</typeparam>
        /// <param name="factory">Factory method for the specified provider</param>
        public static void RegisterProvider<TProvider>(Func<TProvider> factory)
            where TProvider : class, IVmComponentProvider
        {
            s_ProviderFactories[typeof(TProvider)] = factory;
        }

        /// <summary>
        /// Registers the default providers
        /// </summary>
        public static void RegisterDefaultProviders()
        {
            RegisterProvider<IRomProvider>(() => new ResourceRomProvider(typeof(RomResourcesPlaceHolder).Assembly));
            RegisterProvider<IKeyboardProvider>(() => new ScriptingKeyboardProvider());
            RegisterProvider<IBeeperProvider>(() => new NoAudioProvider());
            RegisterProvider<ITapeProvider>(() => new ScriptingTapeProvider());
            RegisterProvider<ISoundProvider>(() => new NoAudioProvider());
        }

        #endregion

        #region Factory methods

        /// <summary>
        /// Creates a Spectrum instance with the specified model and edition name
        /// </summary>
        /// <param name="modelKey">Spectrum model name</param>
        /// <param name="editionKey">Edition name</param>
        /// <returns>The newly create Spectrum machine</returns>
        public static SpectrumVm Create(string modelKey, string editionKey)
        {
            // --- Check input
            if (modelKey == null) throw new ArgumentNullException(nameof(modelKey));
            if (editionKey == null) throw new ArgumentNullException(nameof(editionKey));

            if (!SpectrumModels.StockModels.TryGetValue(modelKey, out var model))
            {
                throw new KeyNotFoundException($"Cannot find a Spectrum model with key '{modelKey}'");
            }

            if (!model.Editions.TryGetValue(editionKey, out var edition))
            {
                throw new KeyNotFoundException($"Cannot find an edition of {modelKey} with key '{editionKey}'");
            }

            // --- Create the selected Spectrum model/edition
            DeviceInfoCollection devices;
            switch (modelKey)
            {
                case SpectrumModels.ZX_SPECTRUM_128:
                    devices = CreateSpectrum128Devices(edition);
                    break;
                case SpectrumModels.ZX_SPECTRUM_P3_E:
                    devices = CreateSpectrumP3Devices(edition);
                    break;
                default:
                    devices = CreateSpectrum48Devices(edition);
                    break;
            }

            // --- Setup the machine
            var machine = new SpectrumVm(modelKey, editionKey, devices);
            return machine;
        }

        /// <summary>
        /// Creates a Spectrum 48K instance PAL edition
        /// </summary>
        /// <returns>The newly create Spectrum machine</returns>
        public static SpectrumVm CreateSpectrum48Pal()
        {
            return Create(SpectrumModels.ZX_SPECTRUM_48, SpectrumModels.PAL);
        }

        /// <summary>
        /// Creates a Spectrum 48K instance PAL edition with turbo mode (2xCPU)
        /// </summary>
        /// <returns>The newly create Spectrum machine</returns>
        public static SpectrumVm CreateSpectrum48PalTurbo()
        {
            return Create(SpectrumModels.ZX_SPECTRUM_48, SpectrumModels.PAL_2_X);
        }

        /// <summary>
        /// Creates a Spectrum 48K instance NTSC edition
        /// </summary>
        /// <returns>The newly create Spectrum machine</returns>
        public static SpectrumVm CreateSpectrum48Ntsc()
        {
            return Create(SpectrumModels.ZX_SPECTRUM_48, SpectrumModels.NTSC);
        }

        /// <summary>
        /// Creates a Spectrum 48K instance NTSC edition with turbo mode
        /// </summary>
        /// <returns>The newly create Spectrum machine</returns>
        public static SpectrumVm CreateSpectrum48NtscTurbo()
        {
            return Create(SpectrumModels.ZX_SPECTRUM_48, SpectrumModels.NTSC_2_X);
        }

        /// <summary>
        /// Creates a Spectrum 128K instance
        /// </summary>
        /// <returns>The newly create Spectrum machine</returns>
        public static SpectrumVm CreateSpectrum128()
        {
            return Create(SpectrumModels.ZX_SPECTRUM_128, SpectrumModels.PAL);
        }

        /// <summary>
        /// Creates a Spectrum +3E instance
        /// </summary>
        /// <returns>The newly create Spectrum machine</returns>
        public static SpectrumVm CreateSpectrumP3E()
        {
            return Create(SpectrumModels.ZX_SPECTRUM_P3_E, SpectrumModels.PAL);
        }

        #endregion

        #region Implementation methods

        /// <summary>
        /// Gets a provider instance for the specified provider types
        /// </summary>
        /// <typeparam name="TProvider">Service provider type</typeparam>
        /// <param name="optional">In the provider optional?</param>
        /// <exception cref="KeyNotFoundException">
        /// The requested mandatory provider cannot be found
        /// </exception>
        /// <returns>Provider instance, if found; otherwise, null</returns>
        private static TProvider GetProvider<TProvider>(bool optional = true)
            where TProvider : class, IVmComponentProvider
        {
            if (s_ProviderFactories.TryGetValue(typeof(TProvider), out var factory))
            {
                return (TProvider)factory();
            }

            return optional
                ? (TProvider)default
                : throw new KeyNotFoundException($"Cannot find a factory for {typeof(TProvider)}");
        }

        /// <summary>
        /// Create the collection of devices for the Spectrum 48K virtual machine
        /// </summary>
        /// <param name="spectrumConfig">Machine configuration</param>
        /// <returns></returns>
        private static DeviceInfoCollection CreateSpectrum48Devices(SpectrumEdition spectrumConfig)
        {
            return new DeviceInfoCollection
            {
                new CpuDeviceInfo(spectrumConfig.Cpu),
                new RomDeviceInfo(GetProvider<IRomProvider>(false), spectrumConfig.Rom, new SpectrumRomDevice()),
                new MemoryDeviceInfo(spectrumConfig.Memory, new Spectrum48MemoryDevice()),
                new PortDeviceInfo(null, new Spectrum48PortDevice()),
                new ClockDeviceInfo(new ClockProvider()),
                new KeyboardDeviceInfo(GetProvider<IKeyboardProvider>(), new KeyboardDevice()),
                new ScreenDeviceInfo(spectrumConfig.Screen),
                new BeeperDeviceInfo(spectrumConfig.Beeper, GetProvider<IBeeperProvider>()),
                new TapeDeviceInfo(GetProvider<ITapeProvider>())
            };
        }

        /// <summary>
        /// Create the collection of devices for the Spectrum 48K virtual machine
        /// </summary>
        /// <param name="spectrumConfig">Machine configuration</param>
        /// <returns></returns>
        private static DeviceInfoCollection CreateSpectrum128Devices(SpectrumEdition spectrumConfig)
        {
            return new DeviceInfoCollection
            {
                new CpuDeviceInfo(spectrumConfig.Cpu),
                new RomDeviceInfo(GetProvider<IRomProvider>(false), spectrumConfig.Rom, new SpectrumRomDevice()),
                new MemoryDeviceInfo(spectrumConfig.Memory, new Spectrum128MemoryDevice()),
                new PortDeviceInfo(null, new Spectrum128PortDevice()),
                new ClockDeviceInfo(new ClockProvider()),
                new KeyboardDeviceInfo(GetProvider<IKeyboardProvider>(), new KeyboardDevice()),
                new ScreenDeviceInfo(spectrumConfig.Screen),
                new BeeperDeviceInfo(spectrumConfig.Beeper, GetProvider<IBeeperProvider>()),
                new TapeDeviceInfo(GetProvider<ITapeProvider>()),
                new SoundDeviceInfo(spectrumConfig.Sound, GetProvider<ISoundProvider>())
            };
        }

        /// <summary>
        /// Create the collection of devices for the Spectrum +3E virtual machine
        /// </summary>
        /// <param name="spectrumConfig">Machine configuration</param>
        /// <returns></returns>
        private static DeviceInfoCollection CreateSpectrumP3Devices(SpectrumEdition spectrumConfig)
        {
            return new DeviceInfoCollection
            {
                new CpuDeviceInfo(spectrumConfig.Cpu),
                new RomDeviceInfo(GetProvider<IRomProvider>(false), spectrumConfig.Rom, new SpectrumRomDevice()),
                new MemoryDeviceInfo(spectrumConfig.Memory, new SpectrumP3MemoryDevice()),
                new PortDeviceInfo(null, new SpectrumP3PortDevice()),
                new ClockDeviceInfo(new ClockProvider()),
                new KeyboardDeviceInfo(GetProvider<IKeyboardProvider>(), new KeyboardDevice()),
                new ScreenDeviceInfo(spectrumConfig.Screen),
                new BeeperDeviceInfo(spectrumConfig.Beeper, GetProvider<IBeeperProvider>()),
                new TapeDeviceInfo(GetProvider<ITapeProvider>()),
                new SoundDeviceInfo(spectrumConfig.Sound, GetProvider<ISoundProvider>())
            };
        }

        #endregion
    }
}