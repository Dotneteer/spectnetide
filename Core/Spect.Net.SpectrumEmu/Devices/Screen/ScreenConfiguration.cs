using Spect.Net.SpectrumEmu.Abstraction.Models;

namespace Spect.Net.SpectrumEmu.Devices.Screen
{
    /// <summary>
    /// This class represents the parameters the ULA chip uses to render the Spectrum
    /// screen.
    /// </summary>
    public class ScreenConfiguration : IScreenConfiguration
    {
        /// <summary>
        /// Screen refresh rate per seconds
        /// </summary>
        public int RefreshRate { get; }

        /// <summary>
        /// The number of frames after the flash is toggled
        /// </summary>
        public int FlashToggleFrames { get; }

        /// <summary>
        /// Number of lines used for vertical synch
        /// </summary>
        public int VerticalSyncLines { get; }

        /// <summary>
        /// The number of top border lines that are not visible
        /// when rendering the screen
        /// </summary>
        public int NonVisibleBorderTopLines { get; }

        /// <summary>
        /// The number of border lines before the display
        /// </summary>
        public int BorderTopLines { get; }

        /// <summary>
        /// Number of display lines
        /// </summary>
        public int DisplayLines { get; }

        /// <summary>
        /// The number of border lines after the display
        /// </summary>
        public int BorderBottomLines { get; }

        /// <summary>
        /// The number of bottom border lines that are not visible
        /// when rendering the screen
        /// </summary>
        public int NonVisibleBorderBottomLines { get; }

        /// <summary>
        /// Horizontal blanking time (HSync+blanking).
        /// Given in Z80 clock cycles.
        /// </summary>
        public int HorizontalBlankingTime { get; }

        /// <summary>
        /// The time of displaying left part of the border.
        /// Given in Z80 clock cycles.
        /// </summary>
        public int BorderLeftTime { get; }

        /// <summary>
        /// The time of displaying a pixel row.
        /// Given in Z80 clock cycles.
        /// </summary>
        public int DisplayLineTime { get; }

        /// <summary>
        /// The time of displaying right part of the border.
        /// Given in Z80 clock cycles.
        /// </summary>
        public int BorderRightTime { get; }

        /// <summary>
        /// The time used to render the nonvisible right part of the border.
        /// Given in Z80 clock cycles.
        /// </summary>
        public int NonVisibleBorderRightTime { get; }

        /// <summary>
        /// The time the data of a particular pixel should be prefetched
        /// before displaying it.
        /// Given in Z80 clock cycles.
        /// </summary>
        public int PixelDataPrefetchTime { get; }

        /// <summary>
        /// The time the data of a particular pixel attribute should be prefetched
        /// before displaying it.
        /// Given in Z80 clock cycles.
        /// </summary>
        public int AttributeDataPrefetchTime { get; }

        /// <summary>
        /// The total number of lines in the screen
        /// </summary>
        public int ScreenLines { get; private set; }

        /// <summary>
        /// The first screen line that contains the top left display pixel
        /// </summary>
        public int FirstDisplayLine { get; private set; }

        /// <summary>
        /// The last screen line that contains the bottom right display pixel
        /// </summary>
        public int LastDisplayLine { get; private set; }

        /// <summary>
        /// The number of border pixels to the left of the display
        /// </summary>
        public int BorderLeftPixels { get; private set; }

        /// <summary>
        /// The number of displayed pixels in a display row
        /// </summary>
        public int DisplayWidth { get; private set; }

        /// <summary>
        /// The number of border pixels to the right of the display
        /// </summary>
        public int BorderRightPixels { get; private set; }

        /// <summary>
        /// The total width of the screen in pixels
        /// </summary>
        public int ScreenWidth { get; private set; }

        /// <summary>
        /// The time of displaying a full screen line.
        /// Given in Z80 clock cycles.
        /// </summary>
        public int ScreenLineTime { get; private set; }

        /// <summary>
        /// The tact within the line that should display the first pixel.
        /// Given in Z80 clock cycles.
        /// </summary>
        public int FirstPixelTactInLine { get; private set; }

        /// <summary>
        /// The tact in which the top left pixel should be displayed.
        /// Given in Z80 clock cycles.
        /// </summary>
        public int FirstDisplayPixelTact { get; private set; }

        /// <summary>
        /// The tact in which the top left screen pixel (border) should be displayed
        /// </summary>
        public int FirstScreenPixelTact { get; private set; }
        
        /// <summary>
        /// Defines the number of Z80 clock cycles used for the full rendering
        /// of the screen.
        /// </summary>
        public int UlaFrameTactCount { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        public ScreenConfiguration(IScreenConfiguration configData)
        {
            // --- Simple configuration values
            RefreshRate = configData.RefreshRate;
            FlashToggleFrames = configData.FlashToggleFrames;
            VerticalSyncLines = configData.VerticalSyncLines;
            NonVisibleBorderTopLines = configData.NonVisibleBorderTopLines;
            BorderTopLines = configData.BorderTopLines;
            BorderBottomLines = configData.BorderBottomLines;
            NonVisibleBorderBottomLines = configData.NonVisibleBorderBottomLines;
            DisplayLines = configData.DisplayLines;
            BorderLeftTime = configData.BorderLeftTime;
            BorderRightTime = configData.BorderRightTime;
            DisplayLineTime = configData.DisplayLineTime;
            HorizontalBlankingTime = configData.HorizontalBlankingTime;
            NonVisibleBorderRightTime = configData.NonVisibleBorderRightTime;
            PixelDataPrefetchTime = configData.PixelDataPrefetchTime;
            AttributeDataPrefetchTime = configData.AttributeDataPrefetchTime;

            // --- Calculated configuration values
            CalculateValues();
        }

        private void CalculateValues()
        {
            ScreenLines = BorderTopLines + DisplayLines + BorderBottomLines;
            FirstDisplayLine = VerticalSyncLines + NonVisibleBorderTopLines + BorderTopLines;
            LastDisplayLine = FirstDisplayLine + DisplayLines - 1;
            BorderLeftPixels = 2 * BorderLeftTime;
            BorderRightPixels = 2 * BorderRightTime;
            DisplayWidth = 2 * DisplayLineTime;
            ScreenWidth = BorderLeftPixels + DisplayWidth + BorderRightPixels;
            FirstPixelTactInLine = HorizontalBlankingTime + BorderLeftTime;
            ScreenLineTime = FirstPixelTactInLine + DisplayLineTime + BorderRightTime + NonVisibleBorderRightTime;
            UlaFrameTactCount = (FirstDisplayLine + DisplayLines + BorderBottomLines + NonVisibleBorderTopLines) *
                                ScreenLineTime;
            FirstDisplayPixelTact = FirstDisplayLine * ScreenLineTime
                                    + HorizontalBlankingTime + BorderLeftTime;
            FirstScreenPixelTact = (VerticalSyncLines + NonVisibleBorderTopLines) * ScreenLineTime
                                   + HorizontalBlankingTime;
        }

        /// <summary>
        /// Tests whether the specified tact is in the visible area of the screen.
        /// </summary>
        /// <param name="line">Line index</param>
        /// <param name="tactInLine">Tacts index within the line</param>
        /// <returns>
        /// True, if the tact is visible on the screen; otherwise, false
        /// </returns>
        public bool IsTactVisible(int line, int tactInLine)
        {
            var firstVisibleLine = VerticalSyncLines + NonVisibleBorderTopLines;
            var lastVisibleLine = firstVisibleLine + BorderTopLines + DisplayLines + BorderBottomLines;
            return
                line >= firstVisibleLine
                && line < lastVisibleLine
                && tactInLine >= HorizontalBlankingTime
                && tactInLine < ScreenLineTime - NonVisibleBorderRightTime;
        }

        /// <summary>
        /// Tests whether the tact is in the display area of the screen.
        /// </summary>
        /// <param name="line">Line index</param>
        /// <param name="tactInLine">Tacts index within the line</param>
        /// <returns>
        /// True, if the tact is within the display area of the screen; otherwise, false.
        /// </returns>
        public bool IsTactInDisplayArea(int line, int tactInLine)
        {
            return line >= FirstDisplayLine 
                && line <= LastDisplayLine
                && tactInLine >= FirstPixelTactInLine
                && tactInLine < FirstPixelTactInLine + DisplayLineTime;
        }
    }
}