using System;
using System.Collections.Generic;
using Spect.Net.SpectrumEmu.Abstraction.Devices;
using Spect.Net.SpectrumEmu.Abstraction.Discovery;
using ZXMAK2.Engine.Cpu;
using ZmakCPU = ZXMAK2.Engine.Cpu.Processor.Z80Cpu;

namespace Spect.Net.SpectrumEmu.Test.Helpers
{
    /// <summary>
    /// This class implements a Z80 machine that can be used for unit testing.
    /// </summary>
    public class ZmakTestMachine
    {
        private bool _breakReceived;

        public ZmakCPU Cpu { get; }

        public RunMode RunMode { get; protected set; }

        public byte[] Memory { get; }

        public ushort CodeEndsAt { get; private set; }

        public List<MemoryOp> MemoryAccessLog { get; }

        public List<IoOp> IoAccessLog { get; }

        public List<byte> IoInputSequence { get; }

        public int IoReadCount { get; private set; }

        public event EventHandler CpuCycleCompleted;

        public CpuRegs RegistersBeforeRun { get; private set; }

        public byte[] MemoryBeforeRun { get; private set; }

        public List<StackPointerManipulationEvent> StackPointerManipulations { get; }

        public List<StackContentManipulationEvent> StackContentManipulations { get; }

        public List<BranchEvent> BranchEvents { get; }

        public ZmakTestMachine(RunMode runMode = RunMode.Normal)
        {
            Memory = new byte[ushort.MaxValue + 1];
            MemoryAccessLog = new List<MemoryOp>();
            IoAccessLog = new List<IoOp>();
            IoInputSequence = new List<byte>();
            StackPointerManipulations = new List<StackPointerManipulationEvent>();
            StackContentManipulations = new List<StackContentManipulationEvent>();
            BranchEvents = new List<BranchEvent>();
            Cpu = new ZmakCPU();
            Cpu.RDMEM_M1 = ReadMemory;
            Cpu.RDMEM = ReadMemory;
            Cpu.RDNOMREQ = obj => { };
            Cpu.NMIACK_M1 = () => { };
            Cpu.RDPORT = ReadPort;
            Cpu.WRMEM = WriteMemory;
            Cpu.WRNOMREQ = obj => { };
            Cpu.WRPORT = WritePort;
            Cpu.INTACK_M1 = () => { };
            Cpu.RESET = () => { };

            RunMode = runMode;
            _breakReceived = false;
        }

        /// <summary>
        /// Initializes the code passed in <paramref name="programCode"/>. This code
        /// is put into the memory from <paramref name="codeAddress"/> and
        /// code execution starts at <paramref name="startAddress"/>
        /// </summary>
        /// <param name="programCode"></param>
        /// <param name="codeAddress"></param>
        /// <param name="startAddress"></param>
        /// <returns>True, if break has been signalled.</returns>
        public void InitCode(IEnumerable<byte> programCode = null, ushort codeAddress = 0, 
            ushort startAddress = 0)
        {
            // --- Initialize the code
            if (programCode != null)
            {
                foreach (var op in programCode)
                {
                    Memory[codeAddress++] = op;
                }
                CodeEndsAt = codeAddress;
                while (codeAddress != 0)
                {
                    Memory[codeAddress++] = 0;
                }
            }

            // --- Init code execution
            Cpu.regs.PC = startAddress;
        }

        /// <summary>
        /// Runs the code
        /// </summary>
        /// <returns>True, if break signal received; otherwise, false</returns>
        public bool Run()
        {
            RegistersBeforeRun = Clone(Cpu.regs);
            MemoryBeforeRun = new byte[ushort.MaxValue + 1];
            Memory.CopyTo(MemoryBeforeRun, 0);
            var stopped = false;

            while (!stopped)
            {
                Cpu.ExecCycle();
                CpuCycleCompleted?.Invoke(this, EventArgs.Empty);
                switch (RunMode)
                {
                    case RunMode.Normal:
                    case RunMode.UntilBreak:
                        stopped = _breakReceived;
                        break;
                    case RunMode.OneCycle:
                        stopped = true;
                        break;
                    case RunMode.UntilHalt:
                        stopped = Cpu.HALTED;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            return _breakReceived;
        }

        public void Break()
        {
            _breakReceived = true;
        }

        protected virtual byte ReadMemory(ushort addr)
        {
            var value = Memory[addr];
            MemoryAccessLog.Add(new MemoryOp(addr, value, false));
            return value;
        }

        protected virtual void WriteMemory(ushort addr, byte value)
        {
            Memory[addr] = value;
            MemoryAccessLog.Add(new MemoryOp(addr, value, true));
        }

        protected virtual byte ReadPort(ushort addr)
        {
            var value = IoReadCount >= IoInputSequence.Count
                ? (byte)0x00
                : IoInputSequence[IoReadCount++];
            IoAccessLog.Add(new IoOp(addr, value, false));
            return value;
        }

        protected virtual void WritePort(ushort addr, byte value)
        {
            IoAccessLog.Add(new IoOp(addr, value, true));
        }

        /// <summary>
        /// Clones the current set of registers
        /// </summary>
        /// <param name="regs"></param>
        /// <returns></returns>
        private static CpuRegs Clone(CpuRegs regs)
        {
            return new CpuRegs
            {
                _AF = regs._AF,
                _BC = regs._BC,
                _DE = regs._DE,
                _HL = regs._HL,
                AF = regs.AF,
                BC = regs.BC,
                DE = regs.DE,
                HL = regs.HL,
                SP = regs.SP,
                PC = regs.PC,
                IX = regs.IX,
                IY = regs.IY,
                IR = regs.IR,
                MW = regs.MW
            };
        }

        /// <summary>
        /// Holds information about a memory operation
        /// </summary>
        public class MemoryOp
        {
            public ushort Address { get; }
            public byte Values { get; }
            public bool IsWrite { get; }

            /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
            public MemoryOp(ushort address, byte values, bool isWrite)
            {
                Address = address;
                Values = values;
                IsWrite = isWrite;
            }
        }

        /// <summary>
        /// Holds information about an I/O operation
        /// </summary>
        public class IoOp
        {
            public ushort Address { get; }
            public byte Value { get; }
            public bool IsOutput { get; }

            /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
            public IoOp(ushort address, byte value, bool isOutput)
            {
                Address = address;
                Value = value;
                IsOutput = isOutput;
            }
        }

        /// <summary>
        /// The test machine uses this memory device
        /// </summary>
        public class Z80TestMemoryDevice : IMemoryDevice
        {
            private readonly Func<ushort, byte> _readFunc;
            private readonly Action<ushort, byte> _writeFunc;

            public Z80TestMemoryDevice(Func<ushort, byte> readFunc, Action<ushort, byte> writeFunc)
            {
                _readFunc = readFunc;
                _writeFunc = writeFunc;
            }

            public virtual byte OnReadMemory(ushort addr) => _readFunc(addr);

            public virtual void OnWriteMemory(ushort addr, byte value) => _writeFunc(addr, value);

            /// <summary>
            /// Gets the buffer that holds memory data
            /// </summary>
            /// <returns></returns>
            public byte[] GetMemoryBuffer()
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// The test machine uses this port device 
        /// </summary>
        public class Z80TestPortDevice : IPortDevice
        {
            private readonly Func<ushort, byte> _readFunc;
            private readonly Action<ushort, byte> _writeFunc;

            public Z80TestPortDevice(Func<ushort, byte> readFunc, Action<ushort, byte> writeFunc)
            {
                _readFunc = readFunc;
                _writeFunc = writeFunc;
            }

            public virtual byte OnReadPort(ushort addr) => _readFunc(addr);

            public virtual void OnWritePort(ushort addr, byte data) => _writeFunc(addr, data);

            public virtual void Reset() { }
        }

        /// <summary>
        /// Records a stack pointer manipulation event
        /// </summary>
        /// <param name="ev">Event information</param>
        public void RecordStackPointerManipulationEvent(StackPointerManipulationEvent ev)
        {
            StackPointerManipulations.Add(ev);
        }

        /// <summary>
        /// Records a stack content manipulation event
        /// </summary>
        /// <param name="ev">Event information</param>
        public void RecordStackContentManipulationEvent(StackContentManipulationEvent ev)
        {
            StackContentManipulations.Add(ev);
        }

        /// <summary>
        /// Records a branching event
        /// </summary>
        /// <param name="ev">Event information</param>
        public void RecordBranchEvent(BranchEvent ev)
        {
            BranchEvents.Add(ev);           
        }
    }
}