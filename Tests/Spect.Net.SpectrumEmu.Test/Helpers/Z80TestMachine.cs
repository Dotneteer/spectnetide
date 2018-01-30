using System;
using System.Collections.Generic;
using Spect.Net.SpectrumEmu.Abstraction.Devices;
using Spect.Net.SpectrumEmu.Abstraction.Discovery;
using Spect.Net.SpectrumEmu.Cpu;

namespace Spect.Net.SpectrumEmu.Test.Helpers
{
    /// <summary>
    /// This class implements a Z80 machine that can be used for unit testing.
    /// </summary>
    public class Z80TestMachine: IStackDebugSupport, IBranchDebugSupport
    {
        private bool _breakReceived;

        public Z80Cpu Cpu { get; }

        public RunMode RunMode { get; protected set; }

        public byte[] Memory { get; }

        public ushort CodeEndsAt { get; private set; }

        public List<MemoryOp> MemoryAccessLog { get; }

        public List<IoOp> IoAccessLog { get; }

        public List<byte> IoInputSequence { get; }

        public int IoReadCount { get; private set; }

        public event EventHandler CpuCycleCompleted;

        public Registers RegistersBeforeRun { get; private set; }

        public byte[] MemoryBeforeRun { get; private set; }

        public List<StackPointerManipulationEvent> StackPointerManipulations { get; }

        public List<StackContentManipulationEvent> StackContentManipulations { get; }

        public List<BranchEvent> BranchEvents { get; }

        public Z80TestMachine(RunMode runMode = RunMode.Normal)
        {
            Memory = new byte[ushort.MaxValue + 1];
            MemoryAccessLog = new List<MemoryOp>();
            IoAccessLog = new List<IoOp>();
            IoInputSequence = new List<byte>();
            StackPointerManipulations = new List<StackPointerManipulationEvent>();
            StackContentManipulations = new List<StackContentManipulationEvent>();
            BranchEvents = new List<BranchEvent>();
            var memDevice = new Z80TestMemoryDevice(ReadMemory, WriteMemory);
            var portDevice = new Z80TestPortDevice(ReadPort, WritePort);
            Cpu = new Z80Cpu(memDevice, portDevice);
            portDevice.Cpu = Cpu;
            RunMode = runMode;
            Cpu.StackDebugSupport = this;
            Cpu.BranchDebugSupport = this;
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
            Cpu.Reset();
            Cpu.Registers.PC = startAddress;
        }

        /// <summary>
        /// Runs the code
        /// </summary>
        /// <returns>True, if break signal received; otherwise, false</returns>
        public bool Run()
        {
            RegistersBeforeRun = Clone(Cpu.Registers);
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
                        stopped = (Cpu.StateFlags & Z80StateFlags.Halted) != 0;
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

        protected virtual byte ReadMemory(ushort addr, bool noContention = false)
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
        private static Registers Clone(Registers regs)
        {
            return new Registers
            {
                _AF_ = regs._AF_,
                _BC_ = regs._BC_,
                _DE_ = regs._DE_,
                _HL_ = regs._HL_,
                AF = regs.AF,
                BC = regs.BC,
                DE = regs.DE,
                HL = regs.HL,
                SP = regs.SP,
                PC = regs.PC,
                IX = regs.IX,
                IY = regs.IY,
                IR = regs.IR,
                WZ = regs.WZ
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
            private readonly Func<ushort, bool, byte> _readFunc;
            private readonly Action<ushort, byte> _writeFunc;

            public Z80TestMemoryDevice(Func<ushort, bool, byte> readFunc, Action<ushort, byte> writeFunc)
            {
                _readFunc = readFunc;
                _writeFunc = writeFunc;
            }

            public int AddressableSize => 0x1_0000;

            /// <summary>
            /// The size of a memory page
            /// </summary>
            public int PageSize { get; set; }

            public virtual byte Read(ushort addr, bool noContention) => _readFunc(addr, noContention);

            public virtual void Write(ushort addr, byte value) => _writeFunc(addr, value);

            /// <summary>
            /// Emulates memory contention
            /// </summary>
            /// <param name="addr">Contention address</param>
            public void ContentionWait(ushort addr)
            {
            }

            /// <summary>
            /// Gets the buffer that holds memory data
            /// </summary>
            /// <returns></returns>
            public byte[] CloneMemory()
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// Fills up the memory from the specified buffer
            /// </summary>
            /// <param name="buffer">Contains the row data to fill up the memory</param>
            public void CopyRom(byte[] buffer)
            {
                throw new NotImplementedException();
            }

            public void SelectRom(int romIndex)
            {
                throw new NotImplementedException();
            }

            public int GetSelectedRomIndex()
            {
                throw new NotImplementedException();
            }

            public void PageIn(int slot, int bank)
            {
                throw new NotImplementedException();
            }

            public int GetSelectedBankIndex(int slot)
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// Indicates of shadow screen should be used
            /// </summary>
            public bool UseShadowScreen { get; set; }

            /// <summary>
            /// Gets the data for the specfied ROM page
            /// </summary>
            /// <param name="romIndex">Index of the ROM</param>
            /// <returns>
            /// The buffer that holds the binary data for the specified ROM page
            /// </returns>
            public byte[] GetRomBuffer(int romIndex)
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// Gets the data for the specfied RAM bank
            /// </summary>
            /// <param name="bankIndex">Index of the RAM bank</param>
            /// <returns>
            /// The buffer that holds the binary data for the specified RAM bank
            /// </returns>
            public byte[] GetRamBank(int bankIndex)
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// Gets the location of the address
            /// </summary>
            /// <param name="addr">Address to check the location</param>
            /// <returns>
            /// IsInRom: true, if the address is in ROM
            /// Index: ROM/RAM bank index
            /// Address: Index within the bank
            /// </returns>
            public (bool IsInRom, int Index, ushort Address) GetAddressLocation(ushort addr)
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// Checks if the RAM bank with the specified index is paged in
            /// </summary>
            /// <param name="index">RAM bank index</param>
            /// <param name="baseAddress">Base memory address, provided the bank is paged in</param>
            /// <returns>True, if the bank is paged in; otherwise, false</returns>
            public bool IsRamBankPagedIn(int index, out ushort baseAddress)
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// Resets this device
            /// </summary>
            public void Reset()
            {
            }

            /// <summary>
            /// The virtual machine that hosts the device
            /// </summary>
            public ISpectrumVm HostVm { get; set; }

            /// <summary>
            /// Signs that the device has been attached to the Spectrum virtual machine
            /// </summary>
            public void OnAttachedToVm(ISpectrumVm hostVm)
            {
            }
        }

        /// <summary>
        /// The test machine uses this port device 
        /// </summary>
        public class Z80TestPortDevice : IPortDevice
        {
            private readonly Func<ushort, byte> _readFunc;
            private readonly Action<ushort, byte> _writeFunc;
            public IZ80Cpu Cpu { get; set; }

            public Z80TestPortDevice(Func<ushort, byte> readFunc, Action<ushort, byte> writeFunc)
            {
                _readFunc = readFunc;
                _writeFunc = writeFunc;
            }

            public virtual byte ReadPort(ushort addr)
            {
                ContentionWait(addr);
                return _readFunc(addr);
            } 

            public virtual void WritePort(ushort addr, byte data)
            {
                ContentionWait(addr);
                _writeFunc(addr, data);
            }

            /// <summary>
            /// Emulates I/O contention
            /// </summary>
            /// <param name="addr">Contention address</param>
            public void ContentionWait(ushort addr)
            {
                Cpu.Delay(4);
            }

            public virtual void Reset() { }

            /// <summary>
            /// The virtual machine that hosts the device
            /// </summary>
            public ISpectrumVm HostVm { get; set; }

            /// <summary>
            /// Signs that the device has been attached to the Spectrum virtual machine
            /// </summary>
            public void OnAttachedToVm(ISpectrumVm hostVm)
            {
            }
        }

        /// <summary>
        /// Resets the debug support
        /// </summary>
        void IStackDebugSupport.Reset()
        {
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