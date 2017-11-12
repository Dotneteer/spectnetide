namespace Spect.Net.SpectrumEmu.Abstraction.Models
{
    /// <summary>
    /// This interface defines the data that can be used to render 
    /// a Spectrum model's screen
    /// </summary>
    public interface IScreenConfiguration
    {
        /// <summary>
        /// Screen refresh rate per seconds
        /// </summary>
        int RefreshRate { get; }

        /// <summary>
        /// The number of frames after the flash is toggled
        /// </summary>
        int FlashToggleFrames { get; }

        /// <summary>
        /// Number of lines used for vertical synch
        /// </summary>
        int VerticalSyncLines { get; }

        /// <summary>
        /// The number of top border lines that are not visible
        /// when rendering the screen
        /// </summary>
        int NonVisibleBorderTopLines { get; }

        /// <summary>
        /// The number of border lines before the display
        /// </summary>
        int BorderTopLines { get; }

        /// <summary>
        /// Number of display lines
        /// </summary>
        int DisplayLines { get; }

        /// <summary>
        /// The number of border lines after the display
        /// </summary>
        int BorderBottomLines { get; }

        /// <summary>
        /// The number of bottom border lines that are not visible
        /// when rendering the screen
        /// </summary>
        int NonVisibleBorderBottomLines { get; }

        /// <summary>
        /// Horizontal blanking time (HSync+blanking).
        /// Given in Z80 clock cycles.
        /// </summary>
        int HorizontalBlankingTime { get; }

        /// <summary>
        /// The time of displaying left part of the border.
        /// Given in Z80 clock cycles.
        /// </summary>
        int BorderLeftTime { get; }

        /// <summary>
        /// The time of displaying a pixel row.
        /// Given in Z80 clock cycles.
        /// </summary>
        int DisplayLineTime { get; }

        /// <summary>
        /// The time of displaying right part of the border.
        /// Given in Z80 clock cycles.
        /// </summary>
        int BorderRightTime { get; }

        /// <summary>
        /// The time used to render the nonvisible right part of the border.
        /// Given in Z80 clock cycles.
        /// </summary>
        int NonVisibleBorderRightTime { get; }

        /// <summary>
        /// The time the data of a particular pixel should be prefetched
        /// before displaying it.
        /// Given in Z80 clock cycles.
        /// </summary>
        int PixelDataPrefetchTime { get; }

        /// <summary>
        /// The time the data of a particular pixel attribute should be prefetched
        /// before displaying it.
        /// Given in Z80 clock cycles.
        /// </summary>
        int AttributeDataPrefetchTime { get; }
    }
}