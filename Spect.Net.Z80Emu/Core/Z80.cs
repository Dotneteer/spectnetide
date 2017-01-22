// ReSharper disable InconsistentNaming

using System;
using System.Runtime.CompilerServices;

namespace Spect.Net.Z80Emu.Core
{
    /// <summary>
    /// This class represents the Z80 CPU
    /// </summary>
    public partial class Z80
    {
        #region CPU and Execution Status

        /// <summary>CPU registers (General/Special)</summary>
        public Registers Registers;

        /// <summary>
        /// Is the CPU in HALTED state?
        /// </summary>
        /// <remarks>
        /// When a software HALT instruction is executed, the CPU executes NOPs
        ///  until an interrupt is received(either a nonmaskable or a maskable 
        /// interrupt while the interrupt flip-flop is enabled).
        /// </remarks>
        public bool HALTED;

        /// <summary>
        /// Interrupt Enable Flip-Flop #1
        /// </summary>
        /// <remarks>
        /// Disables interrupts from being accepted 
        /// </remarks>
        public bool IFF1;

        /// <summary>
        /// Interrupt Enable Flip-Flop #2
        /// </summary>
        /// <remarks>
        /// Temporary storage location for IFF1
        /// </remarks>
        public bool IFF2;

        /// <summary>
        /// The current Interrupt mode
        /// </summary>
        /// <remarks>
        /// IM 0: In this mode, the interrupting device can insert any 
        /// instruction on the data bus for execution by the CPU.The first 
        /// byte of a multi-byte instruction is read during the interrupt 
        /// acknowledge cycle. Subsequent bytes are read in by a normal 
        /// memory read sequence.
        /// IM 1: In this mode, the processor responds to an interrupt by 
        /// executing a restart at address 0038h.
        /// IM 2: This mode allows an indirect call to any memory location 
        /// by an 8-bit vector supplied from the peripheral device. This vector
        /// then becomes the least-significant eight bits of the indirect 
        /// pointer, while the I Register in the CPU provides the most-
        /// significant eight bits.This address points to an address in a 
        /// vector table that is the starting address for the interrupt
        /// service routine.
        /// </remarks>
        public byte IM;

        /// <summary>
        /// The interrupt is blocked
        /// </summary>
        public bool INT_BLOCKED;

        /// <summary>
        /// Indicates if an interrupt signal arrived
        /// </summary>
        public bool INT;

        /// <summary>
        /// Indicates if a Non-Maskable Interrupt signal arrived
        /// </summary>
        public bool NMI;

        /// <summary>
        /// Indicates if a RESET signal arrived
        /// </summary>
        public bool RST;

        /// <summary>
        /// The current Operation Prefix Mode
        /// </summary>
        public OpPrefixMode PrefixMode;

        /// <summary>
        /// The current Operation Index Mode
        /// </summary>
        public OpIndexMode IndexMode;

        /// <summary>
        /// The number of clock cycles 
        /// </summary>
        public ulong Ticks;

        #endregion

        #region Memory and I/O operation hooks

        /// <summary>
        /// The operation that reads the memory
        /// </summary>
        /// <remarks>
        /// The operation accepts an address (ushort), and a flag that indicates
        /// if the memory read happens during the M1 cycle. It returns the byte read 
        /// from the memory.
        /// </remarks>
        public Func<ushort, bool, byte> ReadMemory;

        /// <summary>
        /// The operation that writes the memory
        /// </summary>
        /// <remarks>
        /// The operation accepts an address (ushort), and a value (byte) to put
        /// into the specified memory address
        /// </remarks>
        public Action<ushort, byte> WriteMemory;

        /// <summary>
        /// The operation that reads an I/O port
        /// </summary>
        /// <remarks>
        /// The operation accepts an address (ushort), and returns the value (byte)
        /// read from the particular port.
        /// </remarks>
        public Func<ushort, byte> ReadPort;

        /// <summary>
        /// The operation that writes an I/O port
        /// </summary>
        /// <remarks>
        /// The operation accepts an address (ushort), and a value (byte) to
        /// write to the particular port.
        /// </remarks>
        public Action<ushort, byte> WritePort;

        #endregion

        #region Execution cycle

        /// <summary>
        /// Initializes the state of the Z80 CPU
        /// </summary>
        public Z80()
        {
            Registers = new Registers();
            InitializeNormalExecutionTables();
            ExecuteReset();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Clock(byte cycles)
        {
            Ticks += cycles;
        }

        /// <summary>
        /// Executes a CPU cycle
        /// </summary>
        public void ExecuteCpuCycle()
        {
            // --- If any of the RST, INT or NMI signals has been processed,
            // --- Execution flow in now on the corresponding way
            // --- Nothing more to do in this execution cycle
            if (ProcessCpuSignals()) return;

            if (HALTED)
            {
                // --- The HALT instruction suspends CPU operation until a 
                // --- subsequent interrupt or reset is received. While in the
                // --- HALT state, the processor executes NOPs to maintain
                // --- memory refresh logic.
                Clock(3);
                RefreshMemory();
                return;
            }

            var opCode = ReadMemory(Registers.PC, true);
            Clock(3);
            Registers.PC++;
            RefreshMemory();

            if (PrefixMode == OpPrefixMode.Bit)
            {
                // --- The CPU is already in BIT operations (0xCB) prefix mode
                INT_BLOCKED = false;
                ProcessCBPrefixedOperations(opCode);
                IndexMode = OpIndexMode.None;
                PrefixMode = OpPrefixMode.None;
                return;
            }

            if (PrefixMode == OpPrefixMode.Extended)
            {
                // --- The CPU is already in Extended operations (0xED) prefix mode
                INT_BLOCKED = false;
                ProcessEDOperations(opCode);
                IndexMode = OpIndexMode.None;
                PrefixMode = OpPrefixMode.None;
                return;
            }

            if (opCode == 0xDD)
            {
                // --- An IX index prefix received
                // --- Disable the interrupt unless the full operation code is received
                IndexMode = OpIndexMode.IX;
                INT_BLOCKED = true;
                return;
            }

            if (opCode == 0xFD)
            {
                // --- An IY index prefix received
                // --- Disable the interrupt unless the full operation code is received
                IndexMode = OpIndexMode.IY;
                INT_BLOCKED = true;
                return;
            }

            if (opCode == 0xCB)
            {
                // --- A bit operation prefix received
                // --- Disable the interrupt unless the full operation code is received
                PrefixMode = OpPrefixMode.Bit;
                INT_BLOCKED = true;
                return;
            }

            if (opCode == 0xED)
            {
                // --- An extended operation prefix received
                // --- Disable the interrupt unless the full operation code is received
                PrefixMode = OpPrefixMode.Extended;
                INT_BLOCKED = true;
                return;
            }

            // --- Normal (8-bit) operation code received
            INT_BLOCKED = false;
            PrefixMode = OpPrefixMode.None;
            ProcessStandardOperations(opCode);
        }

        /// <summary>
        /// Apply a Reset signal
        /// </summary>
        public void Reset()
        {
            RST = true;
            ExecuteCpuCycle();
            RST = false;
        }

        /// <summary>
        /// Processes the CPU signals coming from peripheral devices
        /// of the computer
        /// </summary>
        /// <returns>
        /// True, if a signal has been processed; otherwise, false
        /// </returns>
        private bool ProcessCpuSignals()
        {
            if (RST)
            {
                ExecuteReset();
                return true;
            }
            if (NMI)
            {
                ExecuteNmi();
                return true;
            }
            if (!INT || (INT_BLOCKED) || !IFF1)
            {
                return false;
            }

            ExecuteInterrupt();
            return true;
        }

        /// <summary>
        /// Executes a hard reset
        /// </summary>
        private void ExecuteReset()
        {
            HALTED = false;
            IFF1 = false;
            IFF2 = false;
            IM = 0;
            INT_BLOCKED = false;
            INT = false;
            NMI = false;
            PrefixMode = OpPrefixMode.None;
            IndexMode = OpIndexMode.None;
            Registers.PC = 0x0000;
            Registers.IR = 0x0000;
        }

        /// <summary>
        /// Executes an NMI
        /// </summary>
        private void ExecuteNmi()
        {
            IFF1 = false;
            HALTED = false;
            Registers.SP--;
            Clock(1);
            WriteMemory(Registers.SP, (byte)(Registers.PC >> 8));
            Clock(3);
            Registers.SP--;
            WriteMemory(Registers.SP, (byte)(Registers.PC & 0xFF));
            Clock(3);

            // --- NMI address
            Registers.PC = 0x0066;
        }

        /// <summary>
        /// Executes an INT
        /// </summary>
        private void ExecuteInterrupt()
        {
            IFF1 = false;
            IFF2 = false;
            HALTED = false;
            Registers.SP--;
            Clock(1);
            WriteMemory(Registers.SP, (byte)(Registers.PC >> 8));
            Clock(3);
            Registers.SP--;
            WriteMemory(Registers.SP, (byte)(Registers.PC & 0xFF));
            Clock(3);

            switch (IM)
            {
                // --- Interrupt Mode 0:
                // --- The interrupting device can place any instruction on
                // --- the data bus and the CPU executes it. Consequently, the
                // --- interrupting device provides the next instruction to be 
                // --- executed.
                case 0:
                
                // --- Interrupt Mode 1:
                // --- The CPU responds to an interrupt by executing a restart
                // --- at address 0038h.As a result, the response is identical to 
                // --- that of a nonmaskable interrupt except that the call 
                // --- location is 0038h instead of 0066h.
                case 1:
                    // --- In this implementation, we do cannot emulate a device
                    // --- that places instruction on the data bus, so we'll handle
                    // --- IM 0 and IM 1 the same way
                    Registers.MW = 0x0038;
                    Clock(5);
                    break;

                    // --- Interrupt Mode 2:
                    // --- The programmer maintains a table of 16-bit starting addresses 
                    // --- for every interrupt service routine. This table can be 
                    // --- located anywhere in memory. When an interrupt is accepted, 
                    // --- a 16-bit pointer must be formed to obtain the required interrupt
                    // --- service routine starting address from the table. The upper 
                    // --- eight bits of this pointer is formed from the contents of the I
                    // --- register.The I register must be loaded with the applicable value
                    // --- by the programmer. A CPU reset clears the I register so that it 
                    // --- is initialized to 0. The lower eight bits of the pointer must be
                    // --- supplied by the interrupting device. Only seven bits are required
                    // --- from the interrupting device, because the least-significant bit 
                    // --- must be a 0.
                    // --- This process is required, because the pointer must receive two
                    // --- adjacent bytes to form a complete 16-bit service routine starting 
                    // --- address; addresses must always start in even locations.
                default:
                    // --- Getting the lower byte from device (assume 0)
                    var adr = (ushort)(Registers.IR & 0xFF00);
                    Clock(7);
                    var l = ReadMemory(adr, true);
                    Clock(3);
                    var h = ReadMemory(++adr, true);
                    Clock(3);
                    Registers.MW += (ushort)(h * 0x100 + l);
                    Clock(6);
                    break;
            }
            Registers.PC = Registers.MW;
        }

        /// <summary>
        /// Takes care of refreching the dynamic memory
        /// </summary>
        /// <remarks>
        /// The Z80 CPU contains a memory refresh counter, enabling dynamic 
        /// memories to be used with the same ease as static memories. Seven 
        /// bits of this 8-bit register are automatically incremented after 
        /// each instruction fetch. The eighth bit remains as programmed, 
        /// resulting from an "LD R, A" instruction. The data in the refresh
        /// counter is sent out on the lower portion of the address bus along 
        /// with a refresh control signal while the CPU is decoding and 
        /// executing the fetched instruction. This mode of refresh is 
        /// transparent to the programmer and does not slow the CPU operation.
        /// </remarks>
        private void RefreshMemory()
        {
            Registers.R = (byte)(((Registers.R + 1) & 0x7F) | (Registers.R & 0x80));
            Clock(1);
        }

        #endregion

        #region Nested types

        /// <summary>
        /// Signs if the current instruction uses any of the indexed address modes
        /// </summary>
        public enum OpIndexMode
        {
            /// <summary>Indexed address mode is not used</summary>
            None,

            /// <summary>Indexed address with IX register</summary>
            IX,

            /// <summary>Indexed address with IY register</summary>
            IY
        }

        /// <summary>
        /// Operation Prefix Mode
        /// </summary>
        public enum OpPrefixMode : byte
        {
            /// <summary>No operation prefix</summary>
            None,

            /// <summary>Extended mode (0xED prefix)</summary>
            Extended,

            /// <summary>Bit operations mode (0xCB prefix)</summary>
            Bit
        }

        #endregion
    }
}