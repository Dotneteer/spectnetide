namespace Spect.Net.Assembler.Assembler
{
    /// <summary>
    /// Represents an item in the output list
    /// </summary>
    public class ListFileItem 
    {
        /// <summary>
        /// The index of the file that results this output
        /// </summary>
        public int FileIndex { get; set; }

        /// <summary>
        /// Output address
        /// </summary>
        public int Address { get; set; }

        /// <summary>
        /// Index of the output segment
        /// </summary>
        public int SegmentIndex { get; set; }

        /// <summary>
        /// The index within the segment where the code emission starts
        /// for this item
        /// </summary>
        public int CodeStartIndex { get; set; }

        /// <summary>
        /// The length of the emitted code
        /// </summary>
        public int CodeLength { get; set; }

        /// <summary>
        /// Source code line number
        /// </summary>
        public int LineNumber { get; set; }

        /// <summary>
        /// Source code text
        /// </summary>
        public string SourceText { get; set; }
    }
}