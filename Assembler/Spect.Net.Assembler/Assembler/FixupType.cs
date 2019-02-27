namespace Spect.Net.Assembler.Assembler
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
        Bit16,

        /// <summary>EQU pragma value</summary>
        Equ,

        /// <summary>ENT pragma value</summary>
        Ent,

        /// <summary>XENT pragma value</summary>
        Xent,

        /// <summary>Structure byte array</summary>
        Struct,

        /// <summary>8-bit field value</summary>
        FieldBit8,

        /// <summary>16-bit field value</summary>
        FieldBit16
    }
}