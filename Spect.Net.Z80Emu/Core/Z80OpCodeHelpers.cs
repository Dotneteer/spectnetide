using System.Runtime.CompilerServices;

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
    }
}