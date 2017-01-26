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
        S = 0x80,

        /// <summary>Zero Flag</summary>
        Z = 0x40,

        /// <summary>This flag is not used.</summary>
        R5 = 0x20,

        /// <summary>Half Carry Flag</summary>
        H = 0x10,

        /// <summary>This flag is not used.</summary>
        R3 = 0x08,

        /// <summary>Parity/Overflow Flag</summary>
        PV = 0x04,

        /// <summary>Add/Subtract Flag</summary>
        N = 0x02,

        /// <summary>Carry Flag</summary>
        C = 0x01
    }
}