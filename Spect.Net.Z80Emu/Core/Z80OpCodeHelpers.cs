using System;
using System.Runtime.CompilerServices;
// ReSharper disable InconsistentNaming

namespace Spect.Net.Z80Emu.Core
{
    /// <summary>
    /// This partion contains inlined helper functions to
    /// parse a 8-bit operation code
    /// </summary>
    public partial class Z80
    {
        /// <summary>
        /// Returns an 8-bit register index from the opcode
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <returns>8-bit register index</returns>
        /// <remarks>
        /// =================================
        /// | - | - | R | R | R | - | - | - | 
        /// =================================
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Reg8Index Get8BitRegisterIndex(byte opCode)
        {
            return (Reg8Index)((opCode & 0x38) >> 3);
        }
    }
}