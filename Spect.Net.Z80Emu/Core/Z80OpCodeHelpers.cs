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

        /// <summary>
        /// Tests a condition and executes the specified action when 
        /// condition is true.
        /// </summary>
        /// <param name="condition">Condition index</param>
        /// <param name="onTrue">Action to execute when condition is true</param>
        /// <returns></returns>
        private void TestCondition(int condition, Action onTrue)
        {
            var test = s_Conditions[condition >> 1];
            var testResult = Registers.F & test;
            if ((condition & 1) == 0) testResult ^= test;
            if (testResult != 0)
            {
                onTrue();
            }
        }
    }
}