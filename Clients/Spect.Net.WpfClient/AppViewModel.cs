using Spect.Net.RomResources;
using Spect.Net.SpectrumEmu;
using Spect.Net.SpectrumEmu.Abstraction.Configuration;
using Spect.Net.SpectrumEmu.Devices.Keyboard;
using Spect.Net.SpectrumEmu.Devices.Memory;
using Spect.Net.SpectrumEmu.Devices.Rom;
using Spect.Net.SpectrumEmu.Providers;
using Spect.Net.Wpf.Mvvm;
using Spect.Net.Wpf.Mvvm.Messages;
using Spect.Net.Wpf.Providers;
using Spect.Net.WpfClient.Machine;

namespace Spect.Net.WpfClient
{
    /// <summary>
    /// This class represents the view model of the app
    /// </summary>
    public class AppViewModel: EnhancedViewModelBase
    {
        /// <summary>
        /// The singleton instance of the app's view model
        /// </summary>
        public static AppViewModel Default { get; private set; }

        /// <summary>
        /// Resets the app's singleton view model at startup time
        /// </summary>
        static AppViewModel()
        {
            Reset();
        }

        /// <summary>
        /// Resets the app's singleton view model at any time
        /// </summary>
        public static void Reset()
        {
            Default = new AppViewModel();
            var vm = Default.MachineViewModel;
            var spectrumConfig = SpectrumModels.ZxSpectrum48Ntsc;
            vm.MachineController = new MachineController();
            vm.ScreenConfiguration = spectrumConfig.Screen;
            vm.TapeProvider = new DefaultTapeProvider(typeof(AppViewModel).Assembly);
            vm.DeviceData = new DeviceInfoCollection
            {
                new CpuDeviceInfo(spectrumConfig.Cpu),
                new RomDeviceInfo(new ResourceRomProvider(), spectrumConfig.Rom, new SpectrumRomDevice()),
                new MemoryDeviceInfo(spectrumConfig.Memory, new Spectrum48MemoryDevice()),
                new PortDeviceInfo(null, new Spectrum48PortDevice()),
                new ClockDeviceInfo(new ClockProvider()),
                new KeyboardDeviceInfo(new KeyboardProvider(), new KeyboardDevice()),
                new ScreenDeviceInfo(spectrumConfig.Screen,
                    new DelegatingScreenFrameProvider()),
                new BeeperDeviceInfo(spectrumConfig.Beeper, new BeeperWaveProvider()),
                new TapeDeviceInfo(vm.TapeProvider)
            };
            vm.AllowKeyboardScan = true;
            vm.DisplayMode = SpectrumDisplayMode.Fit;
            vm.TapeSetName = "Pac-Man.tzx";
        }

        /// <summary>
        /// Hides the constructor from external actors
        /// </summary>
        private AppViewModel()
        {
            MachineViewModel = new MachineViewModel();
            MessengerInstance.Register<MachineDisplayModeChangedMessage>(this, OnDisplayModeChanged);
        }

        /// <summary>
        /// Simply relays the messages to controls
        /// </summary>
        private void OnDisplayModeChanged(MachineDisplayModeChangedMessage msg)
        {
            MessengerInstance.Send(new VmDisplayModeChangedMessage(msg.DisplayMode));
        }

        /// <summary>
        /// Contains the view model used by Spectrum control
        /// </summary>
        public MachineViewModel MachineViewModel { get; }
    }
}