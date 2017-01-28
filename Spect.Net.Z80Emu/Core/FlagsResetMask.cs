using System;
// ReSharper disable InconsistentNaming

namespace Spect.Net.Z80Emu.Core
{
    /// <summary>
    /// Z80 Status Indicator Flag Reset masks
    /// </summary>
    /// <seealso cref="FlagsSetMask"/>
    [Flags]
    public enum FlagsResetMask: byte
    {
        /// <summary>Sign Flag</summary>
        S = 0x7F,

        /// <summary>Zero Flag</summary>
        Z = 0xBF,

        /// <summary>This flag is not used.</summary>
        R5 = 0xDF,

        /// <summary>Half Carry Flag</summary>
        H = 0xEF,

        /// <summary>This flag is not used.</summary>
        R3 = 0xF7,

        /// <summary>Parity/Overflow Flag</summary>
        PV = 0xFB,

        /// <summary>Add/Subtract Flag</summary>
        N = 0xFD,

        /// <summary>Carry Flag</summary>
        C = 0xFE
    }
}