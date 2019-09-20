using System.Windows.Media.Animation;

namespace Spect.Net.VsPackage.ToolWindows.SpectrumEmulator
{
    /// <summary>
    /// This class represents the view model of the ZX Spectrum emulator
    /// </summary>
    public class SpectrumEmulatorToolWindowViewModel: SpectrumToolWindowViewModelBase
    {
        private int _lineLeft;
        private int _lineRight;
        private int _lineTop;

        public int LineLeft
        {
            get => _lineLeft;
            set => Set(ref _lineLeft, value);
        }

        public int LineRight
        {
            get => _lineRight;
            set => Set(ref _lineRight, value);
        }

        public int LineTop
        {
            get => _lineTop;
            set => Set(ref _lineTop, value);
        }

        public SpectrumEmulatorToolWindowViewModel()
        {
            LineLeft = 0;
            LineRight = 300;
            LineTop = 100;
        }

        protected override void OnSolutionClosing()
        {
            SpectrumVm.BeeperProvider.KillSound();
            SpectrumVm.SoundProvider?.KillSound();
        }
    }
}