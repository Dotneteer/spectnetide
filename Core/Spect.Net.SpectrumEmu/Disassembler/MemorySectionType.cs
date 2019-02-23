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

        /// <summary>
        /// Create an RST 28 byte code memory section
        /// </summary>
        Rst28Calculator,

        /// <summary>
        /// Create a .DEFG array for the memory section with 1 byte in a row
        /// </summary>
        GraphArray,

        /// <summary>
        /// Create a .DEFG array for the memory section with 2 bytes in a row
        /// </summary>
        GraphArray2,

        /// <summary>
        /// Create a .DEFG array for the memory section with 3 bytes in a row
        /// </summary>
        GraphArray3,

        /// <summary>
        /// Create a .DEFG array for the memory section with 4 bytes in a row
        /// </summary>
        GraphArray4
    }
}