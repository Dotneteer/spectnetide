namespace Spect.Net.Spectrum.Ula
{
    /// <summary>
    /// This class represents the parameters the ULA chip uses to render the Spectrum
    /// screen.
    /// </summary>
    public class UlaVideoDisplayParameters
    {
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
        public int DisplayHeight { get; }

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
        /// The total number of lines in the screen
        /// </summary>
        public int ScreenHeight { get; }

        /// <summary>
        /// The number of border pixels to the left of the display
        /// </summary>
        public int BorderLeftPixels { get; }

        /// <summary>
        /// The number of displayed pixels in a display row
        /// </summary>
        public int DisplayWidth { get; }

        /// <summary>
        /// The number of border pixels to the right of the display
        /// </summary>
        public int BorderRightPixels { get; }

        /// <summary>
        /// The total width of the screen in pixels
        /// </summary>
        public int ScreenWidth { get; }

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
        /// The time of displaying a full screen line.
        /// Given in Z80 clock cycles.
        /// </summary>
        public int ScreenLineTime { get; }

        /// <summary>
        /// The time the data of a particular pixel should be prefetched
        /// before displaying it.
        /// Given in Z80 clock cycles.
        /// </summary>
        public int PixelDataPrefetchTime { get; }

        /// <summary>
        /// Defines the number of Z80 clock cycles used for the full rendering
        /// of the screen.
        /// </summary>
        public int UlaFrameTactCount { get; }

        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public UlaVideoDisplayParameters()
        {
            VerticalSyncLines = 8;
            NonVisibleBorderTopLines = 8; // --- In a real screen this value is 0
            BorderTopLines = 48; // --- In a real screen this value is 55
            BorderBottomLines = 48; // --- In a real screen this value is 56
            NonVisibleBorderBottomLines = 8; // --- In a real screen this value is 0
            DisplayHeight = 192;
            ScreenHeight = BorderTopLines + DisplayHeight + BorderBottomLines;
            BorderLeftPixels = 48;
            BorderRightPixels = 48;
            DisplayWidth = 256;
            ScreenWidth = BorderLeftPixels + DisplayWidth + BorderRightPixels;
            HorizontalBlankingTime = 40;
            BorderLeftTime = 24;
            DisplayLineTime = 128;
            BorderRightTime = 32;
            PixelDataPrefetchTime = 2;
            ScreenLineTime = HorizontalBlankingTime + BorderLeftTime + DisplayLineTime + BorderRightTime;
            UlaFrameTactCount = (VerticalSyncLines + NonVisibleBorderTopLines + BorderTopLines
                                 + DisplayHeight + BorderBottomLines + NonVisibleBorderTopLines)*
                                (HorizontalBlankingTime + ScreenWidth/2);
        }

        /// <summary>
        /// Tests whether the specified tact is in the invisible area of the screen.
        /// </summary>
        /// <param name="line">Line index</param>
        /// <param name="tact">Tact index within the line</param>
        /// <returns>
        /// True, if the tact is invisible on the screen; otherwise, false
        /// </returns>
        public bool IsTactInvisible(int line, int tact)
        {
            var firstVisibleLine = VerticalSyncLines + NonVisibleBorderTopLines;
            var lastVisibleLine = firstVisibleLine + DisplayHeight + BorderBottomLines;
            return
                // --- Invisible at the top
                (line < firstVisibleLine)

                // --- Invisible at the bottom
                || (line > lastVisibleLine)

                // --- Invisible on the left
                || (tact < HorizontalBlankingTime + BorderLeftTime);
        }

        /// <summary>
        /// Tests whether the tact is in the border area of the screen.
        /// </summary>
        /// <param name="line">Line index</param>
        /// <param name="tact">Tact index within the line</param>
        /// <returns>
        /// True, if the tact is within the border of the screen; otherwise, false.
        /// </returns>
        /// <remarks>
        /// The prefetch area at the left edge of the display area is not taken into
        /// account as border.
        /// </remarks>
        public bool IsTactInBorder(int line, int tact)
        {
            var firstVisibleLine = VerticalSyncLines + NonVisibleBorderTopLines;
            var lastVisibleLine = firstVisibleLine + DisplayHeight + BorderBottomLines;
            var lastTact = HorizontalBlankingTime + BorderLeftTime;
            return
                // --- Border area at the top
                (line >= firstVisibleLine && line < firstVisibleLine + BorderTopLines)

                // --- Border area at the bottom
                || (line >= lastVisibleLine && line < lastVisibleLine - BorderBottomLines)

                // --- Border on the left, excluding the pixel prefetch area
                || (tact >= HorizontalBlankingTime && tact < lastTact - PixelDataPrefetchTime)

                // --- Border on the right
                || (tact >= lastTact + DisplayLineTime && tact < lastTact);
        }
    }
}