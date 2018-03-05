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

        public Z80TestMachine(RunMode runMode = RunMode.Normal, bool allowExtendedInstructions = false, 
            ITbBlueControlDevice tbBlue = null)
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
            Cpu = new Z80Cpu(memDevice, portDevice, allowExtendedInstructions, tbBlue);
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

            public virtual byte Read(ushort addr, bool noContention) => _readFunc(addr, noContention);

            public virtual void Write(ushort addr, byte value) => _writeFunc(addr, value);

            public void ContentionWait(ushort addr)
            {
            }

            public byte[] CloneMemory()
            {
                throw new NotImplementedException();
            }

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

            public void PageIn(int slot, int bank, bool bank16Mode = true)
            {
                throw new NotImplementedException();
            }

            public int GetSelectedBankIndex(int slot)
            {
                throw new NotImplementedException();
            }

            public bool UseShadowScreen { get; set; }

            public byte[] GetRomBuffer(int romIndex)
            {
                throw new NotImplementedException();
            }

            public byte[] GetRamBank(int bankIndex, bool bank16Mode = true)
            {
                throw new NotImplementedException();
            }

            public (bool IsInRom, int Index, ushort Address) GetAddressLocation(ushort addr)
            {
                throw new NotImplementedException();
            }

            public bool IsRamBankPagedIn(int index, out ushort baseAddress)
            {
                throw new NotImplementedException();
            }

            public void PageInNext(int slot, int bank)
            {
                throw new NotImplementedException();
            }

            public void Reset()
            {
            }

            IDeviceState IDevice.GetState()
            {
                throw new NotImplementedException();
            }

            public void RestoreState(IDeviceState state)
            {
                throw new NotImplementedException();
            }

            public ISpectrumVm HostVm { get; set; }

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
            /// Gets the state of the device so that the state can be saved
            /// </summary>
            /// <returns>The object that describes the state of the device</returns>
            IDeviceState IDevice.GetState()
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// Sets the state of the device from the specified object
            /// </summary>
            /// <param name="state">Device state</param>
            public void RestoreState(IDeviceState state)
            {
                throw new NotImplementedException();
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