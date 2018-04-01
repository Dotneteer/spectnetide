using Spect.Net.SpectrumEmu.Abstraction.Devices;

namespace Spect.Net.SpectrumEmu.Scripting
{
    /// <summary>
    /// Gets the current screen rendering status of the machine.
    /// </summary>
    public sealed class ScreenRenderingStatus
    {
        private readonly ISpectrumVm _spectrumVm;
        private readonly int _ulaTacts;
        private readonly int _screenLineTime;

        public ScreenRenderingStatus(ISpectrumVm spectrumVm)
        {
            _spectrumVm = spectrumVm;
            _ulaTacts = spectrumVm.ScreenConfiguration.ScreenRenderingFrameTactCount;
            _screenLineTime = spectrumVm.ScreenConfiguration.ScreenLineTime;

        }

        /// <summary>
        /// Gets the current tact within the frame
        /// </summary>
        public int CurrentFrameTact => _spectrumVm.CurrentFrameTact % _ulaTacts;

        /// <summary>
        /// Gets the current raster line being rendered
        /// </summary>
        public int RasterLine => CurrentFrameTact / _screenLineTime;

        /// <summary>
        /// Gets the detailed information about the current rendering tact
        /// </summary>
        public ScreenRenderingTact CurrentRenderingTact =>
            new ScreenRenderingTact(_spectrumVm.ScreenDevice.RenderingTactTable[CurrentFrameTact]);
    }
}