namespace AntlrZ80Asm.Assembler
{
    /// <summary>
    /// Type of the fixup
    /// </summary>
    public enum FixupType
    {
        /// <summary>JR operation</summary>
        Jr,

        /// <summary>8-bit value</summary>
        Bit8,

        /// <summary>16-bit value</summary>
        Bit16
    }
}