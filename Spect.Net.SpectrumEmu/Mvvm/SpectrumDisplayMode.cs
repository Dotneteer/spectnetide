namespace Spect.Net.SpectrumEmu.Mvvm
{
    /// <summary>
    /// This enumaration defines the display mode of the SpectrumControl
    /// </summary>
    public enum SpectrumDisplayMode
    {
        /// <summary>
        /// Automatically select the Normal pr ZoomN mode according to the current screen size
        /// </summary>
        Fit = 0,

        /// <summary>
        /// Normal view
        /// </summary>
        Normal = 1,

        /// <summary>
        /// 2x zoom
        /// </summary>
        Zoom2 = 2,

        /// <summary>
        /// 3x zoom
        /// </summary>
        Zoom3 = 3,

        /// <summary>
        /// 4x zoom
        /// </summary>
        Zoom4 = 4,

        /// <summary>
        /// 5x zoom
        /// </summary>
        Zoom5 = 5
    }
}