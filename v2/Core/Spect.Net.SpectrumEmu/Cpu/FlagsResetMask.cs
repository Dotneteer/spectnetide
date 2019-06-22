﻿// ReSharper disable InconsistentNaming

namespace Spect.Net.SpectrumEmu.Cpu
{
    /// <summary>
    /// Z80 Status Indicator Flag Reset masks
    /// </summary>
    /// <seealso cref="FlagsSetMask"/>
    public static class FlagsResetMask
    {
        /// <summary>Sign Flag</summary>
        public const byte S = 0x7F;

        /// <summary>Zero Flag</summary>
        public const byte Z = 0xBF;

        /// <summary>This flag is not used.</summary>
        public const byte R5 = 0xDF;

        /// <summary>Half Carry Flag</summary>
        public const byte H = 0xEF;

        /// <summary>This flag is not used.</summary>
        public const byte R3 = 0xF7;

        /// <summary>Parity/Overflow Flag</summary>
        public const byte PV = 0xFB;

        /// <summary>Add/Subtract Flag</summary>
        public const byte N = 0xFD;

        /// <summary>Carry Flag</summary>
        public const byte C = 0xFE;
    }
}