﻿using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Newtonsoft.Json;

namespace Spect.Net.SpectrumEmu.Abstraction.Cpu
{
    /// <summary>
    /// The Z80 CPU contains 208 bits of read/write memory that 
    /// are available to the programmer. This memory is configured to 
    /// eighteen 8-bit registers and four 16-bit registers. All Z80 CPU’s 
    /// registers are implemented using static RAM. The registers 
    /// include two sets of six general-purpose registers that can be used 
    /// individually as 8-bit registers or in pairs as 16-bit registers.
    /// There are also two sets of Accumulator and Flag registers and 
    /// six special-purpose registers.
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    public class Registers
    {
        #region Register layouts

        // ====================================================================
        // 16-bit register access
        // ====================================================================

        // --------------------------------------------------------------------
        // --- Main register set
        // --------------------------------------------------------------------

        [FieldOffset(0)]
        public ushort AF;
        [FieldOffset(2)]
        public ushort BC;
        [FieldOffset(4)]
        public ushort DE;
        [FieldOffset(6)]
        public ushort HL;

        // --- Alternate register set
        [FieldOffset(8)]
        public ushort _AF_;
        [FieldOffset(10)]
        public ushort _BC_;
        [FieldOffset(12)]
        public ushort _DE_;
        [FieldOffset(14)]
        public ushort _HL_;

        // --------------------------------------------------------------------
        // --- Special purpose registers
        // --------------------------------------------------------------------

        /// <summary>Index Register (IX)</summary>
        /// <remarks>
        /// The two independent index registers hold a 16-bit base address that
        /// is used in indexed addressing modes. In this mode, an index register
        /// is used as a base to point to a region in memory from which data is
        /// to be stored or retrieved. An additional byte is included in indexed
        /// instructions to specify a displacement from this base. This
        /// displacement is specified as a two's complement signed integer.
        /// This mode of addressing greatly simplifies many types of programs,
        /// especially when tables of data are used.
        /// </remarks>
        [FieldOffset(16)]
        public ushort IX;

        /// <summary>Index Register (IY)</summary>
        [FieldOffset(18)]
        public ushort IY;

        /// <summary>
        /// Interrupt Page Address (I) Register/Memory Refresh (R) Register
        /// </summary>
        [FieldOffset(20)]
        public ushort IR;

        /// <summary>Program Counter (PC)</summary>
        /// <remarks>
        /// The program counter holds the 16-bit address of the current 
        /// instruction being fetched from memory.The Program Counter is 
        /// automatically incremented after its contents are transferred to the 
        /// address lines.When a program jump occurs, the new value is 
        /// automatically placed in the Program Counter, overriding the 
        /// incrementer.
        /// </remarks>
        [FieldOffset(22)]
        public ushort PC;

        /// <summary>Stack Pointer (SP)</summary>
        /// <remarks>
        /// The stack pointer holds the 16-bit address of the current 
        /// top of a stack located anywhere in external system RAM memory.
        /// The external stack memory is organized as a last-in first-out 
        /// (LIFO) file. Data can be pushed onto the stack from specific CPU 
        /// registers or popped off of the stack to specific CPU registers 
        /// through the execution of PUSH and POP instructions. The data popped 
        /// from the stack is always the most recent data pushed onto it. The 
        /// stack allows simple implementation of multiple level interrupts, 
        /// unlimited subroutine nesting and simplification of many types of 
        /// data manipulation.
        /// </remarks>
        [FieldOffset(24)]
        public ushort SP;

        /// <summary>
        /// Internal register WZ to support 16-bit addressing operations
        /// </summary>
        [FieldOffset(26)]
        public ushort WZ;

        // ====================================================================
        // 8-bit register access
        // ====================================================================

        /// <summary>Accumulator</summary>
        [FieldOffset(1)]
        [JsonIgnore]
        public byte A;

        /// <summary>Flags</summary>
        [FieldOffset(0)]
        [JsonIgnore]
        public byte F;

        /// <summary>General purpose register B</summary>
        [FieldOffset(3)]
        [JsonIgnore]
        public byte B;

        /// <summary>General purpose register C</summary>
        [FieldOffset(2)]
        [JsonIgnore]
        public byte C;

        /// <summary>General purpose register D</summary>
        [FieldOffset(5)]
        [JsonIgnore]
        public byte D;

        /// <summary>General purpose register E</summary>
        [FieldOffset(4)]
        [JsonIgnore]
        public byte E;

        /// <summary>General purpose register H</summary>
        [FieldOffset(7)]
        [JsonIgnore]
        public byte H;

        /// <summary>General purpose register L</summary>
        [FieldOffset(6)]
        [JsonIgnore]
        public byte L;

        /// <summary>High 8-bit of IX</summary>
        [FieldOffset(17)]
        [JsonIgnore]
        public byte XH;

        /// <summary>Low 8-bit of IX</summary>
        [FieldOffset(16)]
        [JsonIgnore]
        public byte XL;

        /// <summary>High 8-bit of IY</summary>
        [FieldOffset(19)]
        [JsonIgnore]
        public byte YH;

        /// <summary>High 8-bit of IY</summary>
        [FieldOffset(18)]
        [JsonIgnore]
        public byte YL;

        /// <summary>Interrupt Page Address (I) Register</summary>
        /// <remarks>
        /// The Z80 CPU can be operated in a mode in which an 
        /// indirect call to any memory location can be achieved in 
        /// response to an interrupt.The I register is used for this 
        /// purpose and stores the high-order eight bits of the indirect
        /// address while the interrupting device provides the lower eight bits
        /// of the address.This feature allows interrupt routines to be
        /// dynamically located anywhere in memory with minimal access
        /// time to the routine.
        /// </remarks>
        [FieldOffset(21)]
        [JsonIgnore]
        public byte I;

        /// <summary>
        /// Memory Refresh (R) Register
        /// </summary>
        /// <remarks>
        /// The Z80 CPU contains a memory refresh counter, enabling dynamic
        /// memories to be used with the same ease as static memories. Seven bits
        /// of this 8-bit register are automatically incremented after each instruction
        /// fetch. The eighth bit remains as programmed, resulting from an LD R, A
        /// instruction. The data in the refresh counter is sent out on the lower
        /// portion of the address bus along with a refresh control signal while the CPU
        /// is decoding and executing the fetched instruction. This mode of refresh
        /// is transparent to the programmer and does not slow the CPU operation. The
        /// programmer can load the R register for testing purposes, but this register is
        /// normally not used by the programmer. During refresh, the contents of the I
        /// Register are placed on the upper eight bits of the address bus.
        /// </remarks>
        [FieldOffset(20)]
        [JsonIgnore]
        public byte R;

        /// <summary>High 8-bit of WZ</summary>
        [FieldOffset(27)]
        [JsonIgnore]
        public byte WZh;

        /// <summary>Low 8-bit of WZ</summary>
        [FieldOffset(26)]
        [JsonIgnore]
        public byte WZl;

        #endregion

        #region Individual Flag accessors

        /// <summary>
        /// Sign Flag
        /// </summary>
        [JsonIgnore]
        public bool SFlag => (F & FlagsSetMask.S) != 0;

        /// <summary>
        /// Zero Flag
        /// </summary>
        [JsonIgnore]
        public bool ZFlag => (F & FlagsSetMask.Z) != 0;

        /// <summary>
        /// R5 Flag (Bit 5 of last ALU operation result)
        /// </summary>
        [JsonIgnore]
        public bool R5Flag => (F & FlagsSetMask.R5) != 0;

        /// <summary>
        /// Half Carry Flag
        /// </summary>
        [JsonIgnore]
        public bool HFlag => (F & FlagsSetMask.H) != 0;

        /// <summary>
        /// R3 Flag (Bit 3 of last ALU operation result)
        /// </summary>
        [JsonIgnore]
        public bool R3Flag => (F & FlagsSetMask.R3) != 0;

        /// <summary>
        /// Parity/Overflow Flag
        /// </summary>
        [JsonIgnore]
        public bool PFlag => (F & FlagsSetMask.PV) != 0;

        /// <summary>
        /// Add/Subtract Flag
        /// </summary>
        [JsonIgnore]
        public bool NFlag => (F & FlagsSetMask.N) != 0;

        /// <summary>
        /// Carry Flag
        /// </summary>
        [JsonIgnore]
        public bool CFlag => (F & FlagsSetMask.C) != 0;

        #endregion

        #region Register set exchange operations

        /// <summary>
        /// Exchange the AF -- AF' register sets
        /// </summary>
        public void ExchangeAfSet()
        {
            Swap(ref AF, ref _AF_);
        }

        /// <summary>
        /// Exchange the BC -- BC', DE -- DE', HL --HL' register sets
        /// </summary>
        public void ExchangeRegisterSet()
        {
            Swap(ref BC, ref _BC_);
            Swap(ref DE, ref _DE_);
            Swap(ref HL, ref _HL_);
        }

        /// <summary>
        /// Swaps the content of specified registers
        /// </summary>
        /// <param name="orig"></param>
        /// <param name="alt"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Swap(ref ushort orig, ref ushort alt)
        {
            var temp = orig;
            orig = alt;
            alt = temp;
        }

        #endregion

        #region Register fetch/set operations

        /// <summary>
        /// Gets or sets 8-bit register values through an index.
        /// </summary>
        /// <param name="index">8-bit register index (0..7)</param>
        /// <returns></returns>
        public byte this[Reg8Index index]
        {
            get
            {
                switch (index)
                {
                    case Reg8Index.B: return B;
                    case Reg8Index.C: return C;
                    case Reg8Index.D: return D;
                    case Reg8Index.E: return E;
                    case Reg8Index.H: return H;
                    case Reg8Index.L: return L;
                    case Reg8Index.A: return A;
                    case Reg8Index.F: return F;
                    default:
                        throw new RegisterAddressException(
                            $"Index '{index}' is out of the range when reading an 8-bit register's value");
                }
            }
            set
            {
                switch (index)
                {
                    case Reg8Index.B: B = value; break;
                    case Reg8Index.C: C = value; break;
                    case Reg8Index.D: D = value; break;
                    case Reg8Index.E: E = value; break;
                    case Reg8Index.H: H = value; break;
                    case Reg8Index.L: L = value; break;
                    case Reg8Index.A: A = value; break;
                    case Reg8Index.F: F = value; break;
                    default:
                        throw new RegisterAddressException(
                            $"Index '{index}' is out of the range when writing an 8-bit register's value");
                }
            }
        }

        /// <summary>
        /// Gets or sets 8-bit register values through an index.
        /// </summary>
        /// <param name="index">8-bit register index (0..7)</param>
        /// <returns></returns>
        public ushort this[Reg16Index index]
        {
            get
            {
                switch (index)
                {
                    case Reg16Index.BC: return BC;
                    case Reg16Index.DE: return DE;
                    case Reg16Index.HL: return HL;
                    case Reg16Index.SP: return SP;
                    default:
                        throw new RegisterAddressException(
                            $"Index '{index}' is out of the range when reading a 16-bit register's value");
                }
            }
            set
            {
                switch (index)
                {
                    case Reg16Index.BC: BC = value; break;
                    case Reg16Index.DE: DE = value; break;
                    case Reg16Index.HL: HL = value; break;
                    case Reg16Index.SP: SP = value; break;
                    default:
                        throw new RegisterAddressException(
                            $"Index '{index}' is out of the range when writing a 16-bit register's value");
                }
            }
        }

        #endregion
    }

}