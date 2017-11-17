using System.Reflection;
using Spect.Net.RomResources;
using Spect.Net.SpectrumEmu;
using Spect.Net.SpectrumEmu.Devices.Beeper;
using Spect.Net.SpectrumEmu.Machine;
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
            vm.MachineController = new MachineController();
            vm.RomProvider = new ResourceRomProvider();
            vm.ClockProvider = new ClockProvider();
            vm.KeyboardProvider = new KeyboardProvider();
            vm.AllowKeyboardScan = true;
            vm.ScreenConfiguration = SpectrumModels.ZxSpectrum48Ntsc.ScreenConfiguration;
            vm.ScreenFrameProvider = new DelegatingScreenFrameProvider();
            vm.EarBitFrameProvider = new WaveEarbitFrameProvider(new BeeperConfiguration());
            vm.LoadContentProvider = new TzxEmbeddedResourceLoadContentProvider(Assembly.GetEntryAssembly());
            vm.SaveToTapeProvider = new DefaultSaveToTapeProvider();
            vm.DisplayMode = SpectrumDisplayMode.Fit;
            vm.TapeSetName = "Pac-Man.tzx";
        }

        /// <summary>
        /// Hides the constructor from external actors
        /// </summary>
        private AppViewModel()
        {
            MachineViewModel = new MachineViewModel();
            MachineViewModel.VmStateChanged += OnVmStateChanged;
            MessengerInstance.Register<MachineDisplayModeChangedMessage>(this, OnDisplayModeChanged);
        }

        /// <summary>
        /// Simply relays the messages to controls
        /// </summary>
        private void OnVmStateChanged(object sender, VmStateChangedEventArgs args)
        {
            MessengerInstance.Send(new VmStateChangedMessage(args.OldState, args.NewState));
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