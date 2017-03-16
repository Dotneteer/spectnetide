namespace Spect.Net.SpectrumEmu.Devices.Screen
{
    /// <summary>
    /// Describes the Spectrum VM display parameters
    /// </summary>
    public interface IDisplayParameters
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
        /// The total number of lines in the screen
        /// </summary>
        int ScreenLines { get; }

        /// <summary>
        /// The first screen line that contains the top left display pixel
        /// </summary>
        int FirstDisplayLine { get; }

        /// <summary>
        /// The last screen line that contains the bottom right display pixel
        /// </summary>
        int LastDisplayLine { get; }

        /// <summary>
        /// The number of border pixels to the left of the display
        /// </summary>
        int BorderLeftPixels { get; }

        /// <summary>
        /// The number of displayed pixels in a display row
        /// </summary>
        int DisplayWidth { get; }

        /// <summary>
        /// The number of border pixels to the right of the display
        /// </summary>
        int BorderRightPixels { get; }

        /// <summary>
        /// The total width of the screen in pixels
        /// </summary>
        int ScreenWidth { get; }

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
        /// The time of displaying a full screen line.
        /// Given in Z80 clock cycles.
        /// </summary>
        int ScreenLineTime { get; }

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

        /// <summary>
        /// The tact within the line that should display the first pixel.
        /// Given in Z80 clock cycles.
        /// </summary>
        int FirstPixelTactInLine { get; }

        /// <summary>
        /// The tact in which the top left pixel should be displayed.
        /// Given in Z80 clock cycles.
        /// </summary>
        int FirstDisplayPixelTact { get; }

        /// <summary>
        /// The tact in which the top left screen pixel (border) should be displayed
        /// </summary>
        int FirstScreenPixelTact { get; }

        /// <summary>
        /// Defines the number of Z80 clock cycles used for the full rendering
        /// of the screen.
        /// </summary>
        int UlaFrameTactCount { get; }

        /// <summary>
        /// Tests whether the specified tact is in the visible area of the screen.
        /// </summary>
        /// <param name="line">Line index</param>
        /// <param name="tactInLine">Tact index within the line</param>
        /// <returns>
        /// True, if the tact is visible on the screen; otherwise, false
        /// </returns>
        bool IsTactVisible(int line, int tactInLine);

        /// <summary>
        /// Tests whether the tact is in the display area of the screen.
        /// </summary>
        /// <param name="line">Line index</param>
        /// <param name="tactInLine">Tact index within the line</param>
        /// <returns>
        /// True, if the tact is within the display area of the screen; otherwise, false.
        /// </returns>
        bool IsTactInDisplayArea(int line, int tactInLine);
    }
}