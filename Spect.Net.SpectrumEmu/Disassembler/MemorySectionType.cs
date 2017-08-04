namespace Spect.Net.SpectrumEmu.Disassembler
{
    /// <summary>
    /// This enumeration represents the memory section types that can be used
    /// when disassemblying a project.
    /// </summary>
    public enum MemorySectionType
    {
        /// <summary>
        /// Simply skip the section without any output code generation
        /// </summary>
        Skip,

        /// <summary>
        /// Create Z80 disassembly for the memory section
        /// </summary>
        Disassemble,

        /// <summary>
        /// Create a byte array for the memory section
        /// </summary>
        ByteArray,

        /// <summary>
        /// Create a word array for the memory section
        /// </summary>
        WordArray,
    }
}