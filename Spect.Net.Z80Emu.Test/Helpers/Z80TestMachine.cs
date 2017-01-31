using System;
using System.Collections.Generic;
using Spect.Net.Z80Emu.Core;

namespace Spect.Net.Z80Emu.Test.Helpers
{
    /// <summary>
    /// This class implements a Z80 machine that can be used for unit testing.
    /// </summary>
    public class Z80TestMachine: IDisposable
    {
        private bool _breakReceived;

        public Z80 Cpu { get; }

        public RunMode RunMode { get; }

        public byte[] Memory { get; }

        public ushort CodeEndsAt { get; private set; }

        public List<MemoryOp> MemoryAccessLog { get; }

        public List<IoOp> IoAccessLog { get; }

        public List<byte> IoInputSequence { get; }

        public int IoReadCount { get; private set; }

        public event EventHandler CpuCycleCompleted;

        public Registers RegistersBeforeRun { get; private set; }

        public byte[] MemoryBeforeRun { get; private set; }

        public Z80TestMachine(RunMode runMode = RunMode.Normal)
        {
            Cpu = new Z80();
            RunMode = runMode;
            _breakReceived = false;
            Memory = new byte[ushort.MaxValue + 1];
            MemoryAccessLog = new List<MemoryOp>();
            IoAccessLog = new List<IoOp>();
            IoInputSequence = new List<byte>();
            IoReadCount = 0;
            Cpu.ReadMemory += ReadMemory;
            Cpu.WriteMemory += WriteMemory;
            Cpu.ReadPort += ReadPort;
            Cpu.WritePort += WritePort;
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
            Cpu.Reset();
            Cpu.Registers.PC = startAddress;
        }

        /// <summary>
        /// Runs the code
        /// </summary>
        /// <returns>True, if break signal received; otherwise, false</returns>
        public bool Run()
        {
            RegistersBeforeRun = Cpu.Registers.Clone();
            MemoryBeforeRun = new byte[ushort.MaxValue + 1];
            Memory.CopyTo(MemoryBeforeRun, 0);
            var stopped = false;

            while (!stopped)
            {
                Cpu.ExecuteCpuCycle();
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
                    case RunMode.OneInstruction:
                        stopped = !Cpu.IsInOpExecution;
                        break;
                    case RunMode.UntilHalt:
                        stopped = Cpu.HALTED;
                        break;
                    case RunMode.UntilEnd:
                        stopped = Cpu.Registers.PC >= CodeEndsAt;
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

        private void WritePort(ushort addr, byte value)
        {
            IoAccessLog.Add(new IoOp(addr, value, true));
        }

        private byte ReadPort(ushort addr)
        {
            var value = IoReadCount >= IoInputSequence.Count
                ? (byte)0x00
                : IoInputSequence[IoReadCount++];
            IoAccessLog.Add(new IoOp(addr, value, false));
            return value;
        }

        private void WriteMemory(ushort addr, byte value)
        {
            Memory[addr] = value;
            MemoryAccessLog.Add(new MemoryOp(addr, value, true));
        }

        private byte ReadMemory(ushort addr, bool isInM1)
        {
            var value = Memory[addr];
            MemoryAccessLog.Add(new MemoryOp(addr, value, false));
            return value;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
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
    }
}