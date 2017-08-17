using System.Reflection;
using Spect.Net.SpectrumEmu.Devices.Beeper;
using Spect.Net.SpectrumEmu.Machine;
using Spect.Net.Wpf.Mvvm;
using Spect.Net.Wpf.Providers;
using Spect.Net.Wpf.SpectrumControl;

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
            var vm = Default.SpectrumVmViewModel;
            vm.RomProvider = new ResourceRomProvider(typeof(Spectrum48).Assembly);
            vm.ClockProvider = new ClockProvider();
            vm.KeyboardProvider = new KeyboardProvider();
            vm.AllowKeyboardScan = true;
            vm.ScreenFrameProvider = new DelegatingScreenFrameProvider();
            vm.EarBitFrameProvider = new WaveEarbitFrameProvider(new BeeperConfiguration());
            vm.LoadContentProvider = new TzxEmbeddedResourceLoadContentProvider(Assembly.GetEntryAssembly());
            vm.SaveContentProvider = new TzxTempFileSaveContentProvider();
            vm.DisplayMode = SpectrumDisplayMode.Fit;
            vm.TapeSetName = "Pac-Man.tzx";
        }

        /// <summary>
        /// Hides the constructor from external actors
        /// </summary>
        private AppViewModel()
        {
            SpectrumVmViewModel = new SpectrumVmViewModel();
        }

        /// <summary>
        /// Contains the view model used by Spectrum control
        /// </summary>
        public SpectrumVmViewModel SpectrumVmViewModel { get; }
    }
}