using Spect.Net.SpectrumEmu;
using Spect.Net.SpectrumEmu.Abstraction.Providers;
using Spect.Net.SpectrumEmu.Machine;
using Spect.Net.SpectrumEmu.Providers;
using Spect.Net.SpectrumEmu.Scripting;
using Spect.Net.Wpf.Mvvm;
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
            SpectrumMachine.Reset();
            SpectrumMachine.RegisterProvider<IRomProvider>(() => new ResourceRomProvider());
            SpectrumMachine.RegisterProvider<IKeyboardProvider>(() => new KeyboardProvider());
            SpectrumMachine.RegisterProvider<IBeeperProvider>(() => new AudioWaveProvider());
            SpectrumMachine.RegisterProvider<ITapeProvider>(() => new DefaultTapeProvider(typeof(AppViewModel).Assembly));
            Default = new AppViewModel();
        }

        /// <summary>
        /// Hides the constructor from external actors
        /// </summary>
        private AppViewModel()
        {
            var machine = SpectrumMachine.CreateMachine(SpectrumModels.ZX_SPECTRUM_48, SpectrumModels.PAL);
            var vm = MachineViewModel = new MachineViewModel(machine);
            vm.AllowKeyboardScan = true;
            vm.DisplayMode = SpectrumDisplayMode.Fit;
            vm.TapeSetName = "Pac-Man.tzx";
        }

        /// <summary>
        /// Contains the view model used by Spectrum control
        /// </summary>
        public MachineViewModel MachineViewModel { get; }
    }
}