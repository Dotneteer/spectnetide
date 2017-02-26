namespace Spect.Net.SpectrumEmu.Ula
{
    /// <summary>
    /// This enumeration defines the particular phases of ULA rendering
    /// </summary>
    public enum UlaRenderingPhase: byte
    {
        /// <summary>
        /// The ULA does not do any rendering
        /// </summary>
        None,

        /// <summary>
        /// The ULA simple sets the border color to display the current pixel.
        /// </summary>
        Border,

        /// <summary>
        /// The ULA sets the border color to display the current pixel. It
        /// prepares to display the fist pixel in the row with prefetching the
        /// corresponding byte from the display memory.
        /// </summary>
        BorderAndFetchPixelByte,

        /// <summary>
        /// The ULA sets the border color to display the current pixel. It has
        /// already fetched the 8 pixel bits to display. It carries on
        /// preparing to display the fist pixel in the row with prefetching the
        /// corresponding attribute byte from the display memory.
        /// </summary>
        BorderAndFetchPixelAttribute,

        /// <summary>
        /// The ULA displays the next two pixels of Byte1 sequentially during a
        /// single Z80 clock cycle.
        /// </summary>
        DisplayByte1,

        /// <summary>
        /// The ULA displays the next two pixels of Byte1 sequentially during a
        /// single Z80 clock cycle. It prepares to display the pixels of the next
        /// byte in the row with prefetching the corresponding byte from the
        /// display memory.
        /// </summary>
        DisplayByte1AndFetchByte2,

        /// <summary>
        /// The ULA displays the next two pixels of Byte1 sequentially during a
        /// single Z80 clock cycle. It prepares to display the pixels of the next
        /// byte in the row with prefetching the corresponding attribute from the
        /// display memory.
        /// </summary>
        DisplayByte1AndFetchAttribute2,

        /// <summary>
        /// The ULA displays the next two pixels of Byte2 sequentially during a
        /// single Z80 clock cycle.
        /// </summary>
        DisplayByte2,

        /// <summary>
        /// The ULA displays the next two pixels of Byte2 sequentially during a
        /// single Z80 clock cycle. It prepares to display the pixels of the next
        /// byte in the row with prefetching the corresponding byte from the
        /// display memory.
        /// </summary>
        DisplayByte2AndFetchByte1,

        /// <summary>
        /// The ULA displays the next two pixels of Byte2 sequentially during a
        /// single Z80 clock cycle. It prepares to display the pixels of the next
        /// byte in the row with prefetching the corresponding attribute from the
        /// display memory.
        /// </summary>
        DisplayByte2AndFetchAttribute1
    }
}