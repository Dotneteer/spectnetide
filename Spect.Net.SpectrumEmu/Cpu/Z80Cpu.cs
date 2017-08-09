// ReSharper disable ConvertToAutoPropertyWhenPossible
// ReSharper disable ConvertToAutoPropertyWithPrivateSetter
// ReSharper disable InconsistentNaming

using System;
using System.Runtime.CompilerServices;
using Spect.Net.SpectrumEmu.Abstraction.Devices;
using Spect.Net.SpectrumEmu.Abstraction.Discovery;

// ReSharper disable ConvertToAutoProperty

namespace Spect.Net.SpectrumEmu.Cpu
{
    /// <summary>
    /// This class represents the Z80 CPU
    /// </summary>
    public  partial class Z80Cpu: IZ80Cpu, IZ80CpuTestSupport
    {
        private long _tacts;
        private readonly Registers _registers;
        private Z80StateFlags _stateFlags;
        private bool _iff1;
        private bool _iff2;
        private byte _interruptMode;
        private byte _opCode;
        private OpPrefixMode _prefixMode;
        private OpIndexMode _indexMode;
        private bool _isInterruptBlocked;
        private bool _isInOpExecution;
        private readonly IMemoryDevice _memoryDevice;
        private readonly IPortDevice _portDevice;

        /// <summary>
        /// Gets the current tact of the device -- the clock cycles since
        /// the device was reset
        /// </summary>
        public long Tacts => _tacts;

        /// <summary>
        /// Gets the current set of registers
        /// </summary>
        public Registers Registers => _registers;

        /// <summary>
        /// CPU signals
        /// </summary>
        public Z80StateFlags StateFlags
        {
            get => _stateFlags;
            set => _stateFlags = value;
        }

        /// <summary>
        /// Interrupt Enable Flip-Flop #1
        /// </summary>
        /// <remarks>
        /// Disables interrupts from being accepted 
        /// </remarks>
        public bool IFF1
        {
            get => _iff1;
            set => _iff1 = value;
        } 

        /// <summary>
        /// Interrupt Enable Flip-Flop #2
        /// </summary>
        /// <remarks>
        /// Temporary storage location for IFF1
        /// </remarks>
        public bool IFF2
        {
            get => _iff2;
            set => _iff2 = value;
        }

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
        public byte InterruptMode => _interruptMode;

        /// <summary>
        /// The interrupt is blocked
        /// </summary>
        public bool IsInterruptBlocked => _isInterruptBlocked;

        /// <summary>
        /// Is currently in opcode execution?
        /// </summary>
        public bool IsInOpExecution => _isInOpExecution;

        /// <summary>
        /// CPU registers (General/Special)
        /// </summary>
        /// <summary>
        /// Initializes the state of the Z80 CPU
        /// </summary>
        /// <param name="memoryDevice">The device that handles the memory</param>
        /// <param name="portDevice">The device that handles I/O ports</param>
        public Z80Cpu(IMemoryDevice memoryDevice, IPortDevice portDevice)
        {
            _memoryDevice = memoryDevice ?? throw new ArgumentNullException(nameof(memoryDevice));
            _portDevice = portDevice ?? throw new ArgumentException(nameof(portDevice));
            _registers = new Registers();
            InitializeNormalOpsExecutionTable();
            InitializeIndexedOpsExecutionTable();
            InitializeExtendedOpsExecutionTable();
            InitializeBitOpsExecutionTable();
            InitializeIndexedBitOpsExecutionTable();
            InitializeAluTables();
            ExecuteReset();
        }

        /// <summary>
        /// Allows setting the number of tacts
        /// </summary>
        /// <param name="tacts">New value of #of tacts</param>
        public void SetTacts(long tacts)
        {
            _tacts = tacts;
        }

        /// <summary>
        /// Sets the specified interrupt mode
        /// </summary>
        /// <param name="im">IM 0, 1, or 2</param>
        public void SetInterruptMode(byte im)
        {
            _interruptMode = im;
        }

        /// <summary>
        /// The current Operation Prefix Mode
        /// </summary>
        public OpPrefixMode PrefixMode
        {
            get => _prefixMode;
            set => _prefixMode = value;
        }

        /// <summary>
        /// The current Operation Index Mode
        /// </summary>
        public OpIndexMode IndexMode
        {
            get => _indexMode;
            set => _indexMode = value;
        }

        /// <summary>
        /// Block interrupts
        /// </summary>
        public void BlockInterrupt()
        {
            _isInterruptBlocked = true;
        }

        /// <summary>
        /// Increments the internal clock with the specified delay ticks
        /// </summary>
        /// <param name="ticks">Delay ticks</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Delay(int ticks)
        {
            _tacts += ticks;
        }

        /// <summary>
        /// Increments the internal clock counter with 1
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ClockP1()
        {
            _tacts += 1;
        }

        /// <summary>
        /// Increments the internal clock counter with 2
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ClockP2()
        {
            _tacts += 2;
        }

        /// <summary>
        /// Increments the internal clock counter with 3
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ClockP3()
        {
            _tacts += 3;
        }

        /// <summary>
        /// Increments the internal clock counter with 4
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ClockP4()
        {
            _tacts += 4;
        }

        /// <summary>
        /// Increments the internal clock counter with 5
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ClockP5()
        {
            _tacts += 5;
        }

        /// <summary>
        /// Increments the internal clock counter with 6
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ClockP6()
        {
            _tacts += 6;
        }

        /// <summary>
        /// Increments the internal clock counter with 7
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ClockP7()
        {
            _tacts += 7;
        }

        /// <summary>
        /// Gets the contents of the current index register
        /// </summary>
        /// <returns>Index register contents</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ushort GetIndexReg()
        {
            return _indexMode == OpIndexMode.IY ? _registers.IY : _registers.IX;
        }

        /// <summary>
        /// Sets the contents of the current index register
        /// </summary>
        /// <param name="value">The new value of the index register</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SetIndexReg(ushort value)
        {
            if (_indexMode == OpIndexMode.IY)
            {
                _registers.IY = value;
            }
            else
            {
                _registers.IX = value;
            }
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

            // --- Get operation code and refresh the memory
            var opCode = ReadMemory(_registers.PC);
            ClockP3();
            _registers.PC++;
            RefreshMemory();

            if (_prefixMode == OpPrefixMode.None)
            {
                // -- The CPU is about to execute a standard operation
                switch (opCode)
                {
                    case 0xDD:
                        // --- An IX index prefix received
                        // --- Disable the interrupt unless the full operation code is received
                        _indexMode = OpIndexMode.IX;
                        _isInOpExecution = _isInterruptBlocked = true;
                        return;

                    case 0xFD:
                        // --- An IY index prefix received
                        // --- Disable the interrupt unless the full operation code is received
                        _indexMode = OpIndexMode.IY;
                        _isInOpExecution = _isInterruptBlocked = true;
                        return;

                    case 0xCB:
                        // --- A bit operation prefix received
                        // --- Disable the interrupt unless the full operation code is received
                        _prefixMode = OpPrefixMode.Bit;
                        _isInOpExecution = _isInterruptBlocked = true;
                        return;

                    case 0xED:
                        // --- An extended operation prefix received
                        // --- Disable the interrupt unless the full operation code is received
                        _prefixMode = OpPrefixMode.Extended;
                        _isInOpExecution = _isInterruptBlocked = true;
                        return;

                    default:
                        // --- Normal (8-bit) operation code received
                        _isInterruptBlocked = false;
                        _opCode = opCode;
                        ProcessStandardOperations();
                        _prefixMode = OpPrefixMode.None;
                        _indexMode = OpIndexMode.None;
                        _isInOpExecution = false;
                        return;
                }
            }

            if (_prefixMode == OpPrefixMode.Bit)
            {
                // --- The CPU is already in BIT operations (0xCB) prefix mode
                _isInterruptBlocked = false;
                _opCode = opCode;
                ProcessCBPrefixedOperations();
                _indexMode = OpIndexMode.None;
                _prefixMode = OpPrefixMode.None;
                _isInOpExecution = false;
                return;
            }

            if (_prefixMode == OpPrefixMode.Extended)
            {
                // --- The CPU is already in Extended operations (0xED) prefix mode
                _isInterruptBlocked = false;
                _opCode = opCode;
                ProcessEDOperations();
                _indexMode = OpIndexMode.None;
                _prefixMode = OpPrefixMode.None;
                _isInOpExecution = false;
            }
        }

        /// <summary>
        /// Gets the memory device associated with the CPU
        /// </summary>
        public IMemoryDevice MemoryDevice => _memoryDevice;

        /// <summary>
        /// Gets the device that handles Z80 CPU I/O operations
        /// </summary>
        public IPortDevice PortDevice => _portDevice;

        /// <summary>
        /// Gets the object that support debugging the stack
        /// </summary>
        public IStackDebugSupport StackDebugSupport { get; set; }

        /// <summary>
        /// Read the memory at the specified address
        /// </summary>
        /// <param name="addr">Memory address</param>
        /// <returns>Byte read from the memory</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte ReadMemory(ushort addr) => 
            _memoryDevice.OnReadMemory(addr);

        /// <summary>
        /// Set the memory value at the specified address
        /// </summary>
        /// <param name="addr">Memory address</param>
        /// <param name="value">Memory value to write</param>
        /// <returns>Byte read from the memory</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteMemory(ushort addr, byte value) => 
            _memoryDevice.OnWriteMemory(addr, value);

        /// <summary>
        /// Read the port with the specified address
        /// </summary>
        /// <param name="addr">Port address</param>
        /// <returns>Byte read from the port</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte ReadPort(ushort addr) =>
            _portDevice.OnReadPort(addr);

        /// <summary>
        /// Write data to the port with the specified address
        /// </summary>
        /// <param name="addr">Memory address</param>
        /// <param name="data">Memory value to write</param>
        /// <returns>Byte read from the memory</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WritePort(ushort addr, byte data) =>
            _portDevice.OnWritePort(addr, data);

        /// <summary>
        /// Apply a Reset signal
        /// </summary>
        public void Reset()
        {
            _stateFlags |= Z80StateFlags.Reset;
            ExecuteReset();
            _stateFlags &= Z80StateFlags.InvReset;
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
            if (_stateFlags == Z80StateFlags.None) return false;

            if ((_stateFlags & Z80StateFlags.Int) != 0 && !_isInterruptBlocked && _iff1)
            {
                ExecuteInterrupt();
                return true;
            }

            if ((_stateFlags & Z80StateFlags.Halted) != 0)
            {
                // --- The HALT instruction suspends CPU operation until a 
                // --- subsequent interrupt or reset is received. While in the
                // --- HALT state, the processor executes NOPs to maintain
                // --- memory refresh logic.
                ClockP3();
                RefreshMemory();
                return true;
            }

            if ((_stateFlags & Z80StateFlags.Reset) != 0)
            {
                ExecuteReset();
                return true;
            }

            if ((_stateFlags & Z80StateFlags.Nmi) != 0)
            {
                ExecuteNmi();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Executes a hard reset
        /// </summary>
        private void ExecuteReset()
        {
            _iff1 = false;
            _iff2 = false;
            _interruptMode = 0;
            _isInterruptBlocked = false;
            _stateFlags = Z80StateFlags.None;
            _prefixMode = OpPrefixMode.None;
            _indexMode = OpIndexMode.None;
            _registers.PC = 0x0000;
            _registers.IR = 0x0000;
            _isInOpExecution = false;
        }

        /// <summary>
        /// Executes an NMI
        /// </summary>
        private void ExecuteNmi()
        {
            if ((_stateFlags & Z80StateFlags.Halted) != 0)
            {
                // --- We emulate stepping over the HALT instruction
                _registers.PC++;
            }
            _iff1 = false;
            _stateFlags &= Z80StateFlags.InvHalted;
            _registers.SP--;
            ClockP1();
            WriteMemory(_registers.SP, (byte)(_registers.PC >> 8));
            ClockP3();
            _registers.SP--;
            WriteMemory(_registers.SP, (byte)(_registers.PC & 0xFF));
            ClockP3();

            // --- NMI address
            _registers.PC = 0x0066;
        }

        /// <summary>
        /// Executes an INT
        /// </summary>
        private void ExecuteInterrupt()
        {
            if ((_stateFlags & Z80StateFlags.Halted) != 0)
            {
                // --- We emulate stepping over the HALT instruction
                _registers.PC++;
            }
            _iff1 = false;
            _iff2 = false;
            _stateFlags &= Z80StateFlags.InvHalted;
            _registers.SP--;
            ClockP1();
            WriteMemory(_registers.SP, (byte)(_registers.PC >> 8));
            ClockP3();
            _registers.SP--;
            WriteMemory(_registers.SP, (byte)(_registers.PC & 0xFF));
            ClockP3();

            switch (_interruptMode)
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
                    _registers.MW = 0x0038;
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
                    var adr = (ushort)(_registers.IR & 0xFF00);
                    ClockP5();
                    var l = ReadMemory(adr);
                    ClockP3();
                    var h = ReadMemory(++adr);
                    ClockP3();
                    _registers.MW += (ushort)(h * 0x100 + l);
                    ClockP6();
                    break;
            }
            _registers.PC = _registers.MW;
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
            _registers.R = (byte)(((_registers.R + 1) & 0x7F) | (_registers.R & 0x80));
            ClockP1();
        }

        /// <summary>
        /// Signs if the current instruction uses any of the indexed address modes
        /// </summary>
        public enum OpIndexMode
        {
            /// <summary>Indexed address mode is not used</summary>
            None = 0,

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
            None = 0,

            /// <summary>Extended mode (0xED prefix)</summary>
            Extended,

            /// <summary>Bit operations mode (0xCB prefix)</summary>
            Bit
        }
    }
}