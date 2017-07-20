namespace Spect.Net.SpectrumEmu.Disassembler
{
    /// <summary>
    /// Data structure that represents a disassembly comment
    /// </summary>
    public class DisassemblyComment
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
        /// Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        public DisassemblyComment(ushort address, string comment)
        {
            Address = address;
            Comment = comment;
        }
    }
}