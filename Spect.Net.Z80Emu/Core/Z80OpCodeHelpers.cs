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
        /// Returns a 16-bit register index from the opcode
        /// </summary>
        /// <param name="opCode">Operation code</param>
        /// <returns>16-bit register index</returns>
        /// <remarks>
        /// =================================
        /// | - | - | R | R | - | - | - | - | 
        /// =================================
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Reg16Index Get16BitRegisterIndex(byte opCode)
        {
            return (Reg16Index)((opCode & 0x30) >> 4);
        }

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
        /// Gets the contents of the memory address pointed by PC, and then
        /// increments PC
        /// </summary>
        /// <returns>The 8-bit value at from the code</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte Get8BitFromCode()
        {
            var memValue = ReadMemory(Registers.PC, false);
            ClockP3();
            Registers.PC++;
            return memValue;
        }
        /// <summary>
        /// Gets the contents of the memory address pointed by PC, and then
        /// increments PC
        /// </summary>
        /// <returns>The 8-bit value at from the code</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ushort Get16BitFromCode()
        {
            var l = ReadMemory(Registers.PC, false);
            ClockP3();
            Registers.PC++;
            var h = ReadMemory(Registers.PC, false);
            ClockP3();
            Registers.PC++;
            return (ushort)(h << 8 | l);
        }

        /// <summary>
        /// Gets the top value from the stack.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ushort Get16BitFromStack()
        {
            ushort val = ReadMemory(Registers.SP, false);
            ClockP3();
            Registers.SP++;
            val += (ushort)(ReadMemory(Registers.SP, false) * 0x100);
            ClockP3();
            Registers.SP++;
            return val;
        }

        /// <summary>
        /// Gets the top value from the stack and puts it into MW.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void GetMWFromStack()
        {
            Registers.MW = ReadMemory(Registers.SP, false);
            ClockP3();
            Registers.SP++;
            Registers.MW += (ushort)(ReadMemory(Registers.SP, false) * 0x100);
            ClockP3();
            Registers.SP++;
        }

        /// <summary>
        /// Gets the value of MW from the code pointed by PC.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void GetMWFromCode()
        {
            Registers.MW = ReadMemory(Registers.PC, false);
            ClockP3();
            Registers.PC++;
            Registers.MW += (ushort)(ReadMemory(Registers.PC, false) << 8);
            ClockP3();
            Registers.PC++;
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