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
        public bool IsInHaltedState;

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
        public byte InterruptMode;

        /// <summary>
        /// The interrupt is blocked
        /// </summary>
        public bool IsInterruptBlocked;

        /// <summary>
        /// Indicates if an interrupt signal arrived
        /// </summary>
        public bool IntSignal;

        /// <summary>
        /// Indicates if a Non-Maskable Interrupt signal arrived
        /// </summary>
        public bool NmiSignal;

        /// <summary>
        /// Indicates if a RESET signal arrived
        /// </summary>
        public bool ResetSignal;

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
        public ulong Tacts;

        /// <summary>
        /// Is currently in opcode execution?
        /// </summary>
        public bool IsInOpExecution;

        #endregion

        #region Memory and I/O operation hooks

        /// <summary>
        /// The operation that reads the memory (out of the M1 machine cycle)
        /// </summary>
        /// <remarks>
        /// The operation accepts an address (ushort). It returns the byte read 
        /// from the memory.
        /// </remarks>
        public Func<ushort, byte> ReadMemory;

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
            InitializeNormalOpsExecutionTable();
            InitializeIndexedOpsExecutionTable();
            InitializeExtendedOpsExecutionTable();
            InitializeBitOpsExecutionTable();
            InitializeIndexedBitOpsExecutionTable();
            InitializeALUTables();
            ExecuteReset();
        }

        /// <summary>
        /// Increments the internal clock with the specified delay ticks
        /// </summary>
        /// <param name="ticks">Delay ticks</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Delay(int ticks)
        {
            Tacts += (ulong)ticks;
        }

        /// <summary>
        /// Increments the internal clock counter with 1
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ClockP1()
        {
            Tacts += 1;
        }

        /// <summary>
        /// Increments the internal clock counter with 2
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ClockP2()
        {
            Tacts += 2;
        }

        /// <summary>
        /// Increments the internal clock counter with 3
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ClockP3()
        {
            Tacts += 3;
        }

        /// <summary>
        /// Increments the internal clock counter with 4
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ClockP4()
        {
            Tacts += 4;
        }

        /// <summary>
        /// Increments the internal clock counter with 5
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ClockP5()
        {
            Tacts += 5;
        }

        /// <summary>
        /// Increments the internal clock counter with 6
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ClockP6()
        {
            Tacts += 6;
        }

        /// <summary>
        /// Increments the internal clock counter with 7
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ClockP7()
        {
            Tacts += 7;
        }

        /// <summary>
        /// Gets the contents of the current index register
        /// </summary>
        /// <returns>Index register contents</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ushort GetIndexReg()
        {
            return IndexMode == OpIndexMode.IY ? Registers.IY : Registers.IX;
        }

        /// <summary>
        /// Sets the contents of the current index register
        /// </summary>
        /// <param name="value">The new value of the index register</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SetIndexReg(ushort value)
        {
            if (IndexMode == OpIndexMode.IY)
            {
                Registers.IY = value;
            }
            else
            {
                Registers.IX = value;
            }
        }

        /// <summary>
        /// This event is raised right before the Z80 CPU signals are
        /// being processed.
        /// </summary>
        public EventHandler<Z80EventArgs> BeforeProcessCpuSignals;

        /// <summary>
        /// This event is raised right after a new operation code has
        /// been fetched.
        /// </summary>
        public EventHandler<Z80OperationCodeEventArgs> OperationCodeFetched;

        /// <summary>
        /// This event is raised right before an operation code is
        /// about to be processed
        /// </summary>
        public EventHandler<Z80OperationCodeEventArgs> ProcessingOperation;

        /// <summary>
        /// Executes a CPU cycle
        /// </summary>
        public void ExecuteCpuCycle()
        {
            BeforeProcessCpuSignals?.Invoke(this, new Z80EventArgs(this));

            // --- If any of the RST, INT or NMI signals has been processed,
            // --- Execution flow in now on the corresponding way
            // --- Nothing more to do in this execution cycle
            if (ProcessCpuSignals()) return;

            if (IsInHaltedState)
            {
                // --- The HALT instruction suspends CPU operation until a 
                // --- subsequent interrupt or reset is received. While in the
                // --- HALT state, the processor executes NOPs to maintain
                // --- memory refresh logic.
                ClockP3();
                RefreshMemory();
                return;
            }

            var opCode = ReadMemory(Registers.PC);
            ClockP3();
            OperationCodeFetched?.Invoke(this, new Z80OperationCodeEventArgs(opCode, this));
            Registers.PC++;
            RefreshMemory();

            if (PrefixMode == OpPrefixMode.Bit)
            {
                // --- The CPU is already in BIT operations (0xCB) prefix mode
                IsInterruptBlocked = false;
                ProcessingOperation?.Invoke(this, new Z80OperationCodeEventArgs(opCode, this));
                ProcessCBPrefixedOperations(opCode);
                IndexMode = OpIndexMode.None;
                PrefixMode = OpPrefixMode.None;
                IsInOpExecution = false;
                return;
            }

            if (PrefixMode == OpPrefixMode.Extended)
            {
                // --- The CPU is already in Extended operations (0xED) prefix mode
                IsInterruptBlocked = false;
                ProcessingOperation?.Invoke(this, new Z80OperationCodeEventArgs(opCode, this));
                ProcessEDOperations(opCode);
                IndexMode = OpIndexMode.None;
                PrefixMode = OpPrefixMode.None;
                IsInOpExecution = false;
                return;
            }

            if (opCode == 0xDD)
            {
                // --- An IX index prefix received
                // --- Disable the interrupt unless the full operation code is received
                IndexMode = OpIndexMode.IX;
                IsInterruptBlocked = true;
                IsInOpExecution = true;
                return;
            }

            if (opCode == 0xFD)
            {
                // --- An IY index prefix received
                // --- Disable the interrupt unless the full operation code is received
                IndexMode = OpIndexMode.IY;
                IsInterruptBlocked = true;
                IsInOpExecution = true;
                return;
            }

            if (opCode == 0xCB)
            {
                // --- A bit operation prefix received
                // --- Disable the interrupt unless the full operation code is received
                PrefixMode = OpPrefixMode.Bit;
                IsInterruptBlocked = true;
                IsInOpExecution = true;
                return;
            }

            if (opCode == 0xED)
            {
                // --- An extended operation prefix received
                // --- Disable the interrupt unless the full operation code is received
                PrefixMode = OpPrefixMode.Extended;
                IsInterruptBlocked = true;
                IsInOpExecution = true;
                return;
            }

            // --- Normal (8-bit) operation code received
            IsInterruptBlocked = false;
            ProcessingOperation?.Invoke(this, new Z80OperationCodeEventArgs(opCode, this));
            ProcessStandardOperations(opCode);
            PrefixMode = OpPrefixMode.None;
            IndexMode = OpIndexMode.None;
            IsInOpExecution = false;
        }

        /// <summary>
        /// Apply a Reset signal
        /// </summary>
        public void Reset()
        {
            ResetSignal = true;
            ExecuteCpuCycle();
            ResetSignal = false;
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
            if (ResetSignal)
            {
                ExecuteReset();
                return true;
            }
            if (NmiSignal)
            {
                ExecuteNmi();
                return true;
            }
            if (!IntSignal || IsInterruptBlocked || !IFF1)
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
            IsInHaltedState = false;
            IFF1 = false;
            IFF2 = false;
            InterruptMode = 0;
            IsInterruptBlocked = false;
            IntSignal = false;
            NmiSignal = false;
            PrefixMode = OpPrefixMode.None;
            IndexMode = OpIndexMode.None;
            Registers.PC = 0x0000;
            Registers.IR = 0x0000;
            IsInOpExecution = false;
        }

        /// <summary>
        /// Executes an NMI
        /// </summary>
        private void ExecuteNmi()
        {
            if (IsInHaltedState)
            {
                // --- We emulate stepping over the HALT instruction
                Registers.PC++;
            }
            IFF1 = false;
            IsInHaltedState = false;
            Registers.SP--;
            ClockP1();
            WriteMemory(Registers.SP, (byte)(Registers.PC >> 8));
            ClockP3();
            Registers.SP--;
            WriteMemory(Registers.SP, (byte)(Registers.PC & 0xFF));
            ClockP3();

            // --- NMI address
            Registers.PC = 0x0066;
        }

        /// <summary>
        /// Executes an INT
        /// </summary>
        private void ExecuteInterrupt()
        {
            if (IsInHaltedState)
            {
                // --- We emulate stepping over the HALT instruction
                Registers.PC++;
            }
            IFF1 = false;
            IFF2 = false;
            IsInHaltedState = false;
            Registers.SP--;
            ClockP1();
            WriteMemory(Registers.SP, (byte)(Registers.PC >> 8));
            ClockP3();
            Registers.SP--;
            WriteMemory(Registers.SP, (byte)(Registers.PC & 0xFF));
            ClockP3();

            switch (InterruptMode)
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
                    ClockP5();
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
                    ClockP2();
                    var adr = (ushort)(Registers.IR & 0xFF00);
                    ClockP5();
                    var l = ReadMemory(adr);
                    ClockP3();
                    var h = ReadMemory(++adr);
                    ClockP3();
                    Registers.MW += (ushort)(h * 0x100 + l);
                    ClockP6();
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
            ClockP1();
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