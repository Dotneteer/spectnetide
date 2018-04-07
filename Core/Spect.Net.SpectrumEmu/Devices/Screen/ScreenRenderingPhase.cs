namespace Spect.Net.SpectrumEmu.Devices.Screen
{
    /// <summary>
    /// This enumeration defines the particular phases of ULA rendering
    /// </summary>
    public enum ScreenRenderingPhase: byte
    {
        /// <summary>
        /// The ULA does not do any rendering
        /// </summary>
        None,

        /// <summary>
        /// The ULA sets the border color to display the current pixel.
        /// </summary>
        Border,

        /// <summary>
        /// The ULA sets the border color to display the current pixel. It
        /// prepares to display the fist pixel in the row with prefetching the
        /// corresponding byte from the display memory.
        /// </summary>
        BorderFetchPixel,

        /// <summary>
        /// The ULA sets the border color to display the current pixel. It has
        /// already fetched the 8 pixel bits to display. It carries on
        /// preparing to display the fist pixel in the row with prefetching the
        /// corresponding attribute byte from the display memory.
        /// </summary>
        BorderFetchPixelAttr,

        /// <summary>
        /// The ULA displays the next two pixels of Byte1 sequentially during a
        /// single Z80 clock cycle.
        /// </summary>
        DisplayB1,

        /// <summary>
        /// The ULA displays the next two pixels of Byte1 sequentially during a
        /// single Z80 clock cycle. It prepares to display the pixels of the next
        /// byte in the row with prefetching the corresponding byte from the
        /// display memory.
        /// </summary>
        DisplayB1FetchB2,

        /// <summary>
        /// The ULA displays the next two pixels of Byte1 sequentially during a
        /// single Z80 clock cycle. It prepares to display the pixels of the next
        /// byte in the row with prefetching the corresponding attribute from the
        /// display memory.
        /// </summary>
        DisplayB1FetchA2,

        /// <summary>
        /// The ULA displays the next two pixels of Byte2 sequentially during a
        /// single Z80 clock cycle.
        /// </summary>
        DisplayB2,

        /// <summary>
        /// The ULA displays the next two pixels of Byte2 sequentially during a
        /// single Z80 clock cycle. It prepares to display the pixels of the next
        /// byte in the row with prefetching the corresponding byte from the
        /// display memory.
        /// </summary>
        DisplayB2FetchB1,

        /// <summary>
        /// The ULA displays the next two pixels of Byte2 sequentially during a
        /// single Z80 clock cycle. It prepares to display the pixels of the next
        /// byte in the row with prefetching the corresponding attribute from the
        /// display memory.
        /// </summary>
        DisplayB2FetchA1
    }
}