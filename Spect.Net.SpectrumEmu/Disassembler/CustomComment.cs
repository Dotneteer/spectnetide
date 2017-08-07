namespace Spect.Net.SpectrumEmu.Disassembler
{
    /// <summary>
    /// Data structure that represents a disassembly comment
    /// </summary>
    public class CustomComment
    {
        /// <summary>
        /// Comment address
        /// </summary>
        public ushort Address { get; }

        /// <summary>
        /// Comment text
        /// </summary>
        public string Comment { get; }

        /// <summary>
        /// Optional prefix comment text
        /// </summary>
        public string PrefixComment { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        public CustomComment(ushort address, string comment, string prefixComment = null)
        {
            Address = address;
            Comment = comment;
            PrefixComment = prefixComment;
        }
    }
}