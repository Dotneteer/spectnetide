#pragma once
#include "../Spect.Net.Z80Emu/z80spec.h"

namespace Z80
{
	enum RunMode
	{
		// Run while the machine is disposed or a break signal arrives.
		rmNormal,

		// Run a single CPU Execution cycle, even if an operation
		// contains multiple bytes
		rmOneCycle,

		// Pause when the next single instruction is executed.
		rmOneInstruction,

		// Run until a HALT instruction is reached.
		rmUntilHalt,

		// Run until a break signal arrives.
		rmUntilBreak,

		// Run until the whole injected code is executed
		rmUntilEnd
	};

	// This class implements a Z80 machine that can be used for unit testing.
	class Z80TestMachine
	{
	public:
		// Initializes the test machine
		Z80TestMachine(RunMode runMode = rmNormal);

		// The CPU of this test machine
		TZ80Cpu& Cpu;

		// Gets the current run mode of the CPU
		RunMode GetRunMode();

		byte* Memory;

		uBit16 GetCodeEndsAt();

		int GetIoReadCount();

		const TZ80Regs& GetRegistersBeforeRun();

		const byte* GetMemoryBeforeRun();

	protected:
		void SetRunMode(RunMode mode);

	private:
		uBit16 _codeEndCount;
		int _ioReadCount;
		byte* _memoryBeforeRun;
		TZ80Regs _regsBeforeRun;
		bool _breakReceived;
		byte __fastcall ReadMemory(uBit16 addr);
		void __fastcall WriteMemory(uBit16 addr, byte value);

		//	public  Z80TestMachine(RunMode runMode = RunMode.Normal)
		//{
		//	Cpu = new Z80();
		//	RunMode = runMode;
		//	_breakReceived = false;
		//	Memory = new byte[ushort.MaxValue + 1];
		//	MemoryAccessLog = new List<MemoryOp>();
		//	IoAccessLog = new List<IoOp>();
		//	IoInputSequence = new List<byte>();
		//	IoReadCount = 0;
		//	Cpu.ReadMemory += ReadMemory;
		//	Cpu.WriteMemory += WriteMemory;
		//	Cpu.ReadPort += ReadPort;
		//	Cpu.WritePort += WritePort;
		//}

		///// <summary>
		///// Initializes the code passed in <paramref name="programCode"/>. This code
		///// is put into the memory from <paramref name="codeAddress"/> and
		///// code execution starts at <paramref name="startAddress"/>
		///// </summary>
		///// <param name="programCode"></param>
		///// <param name="codeAddress"></param>
		///// <param name="startAddress"></param>
		///// <returns>True, if break has been signalled.</returns>
		//public void InitCode(IEnumerable<byte> programCode = null, ushort codeAddress = 0,
		//	ushort startAddress = 0)
		//{
		//	// --- Initialize the code
		//	if (programCode != null)
		//	{
		//		foreach(var op in programCode)
		//		{
		//			Memory[codeAddress++] = op;
		//		}
		//		CodeEndsAt = codeAddress;
		//		while (codeAddress != 0)
		//		{
		//			Memory[codeAddress++] = 0;
		//		}
		//	}

		//	// --- Init code execution
		//	Cpu.Reset();
		//	Cpu.Registers.PC = startAddress;
		//}

		///// <summary>
		///// Runs the code
		///// </summary>
		///// <returns>True, if break signal received; otherwise, false</returns>
		//public bool Run()
		//{
		//	RegistersBeforeRun = Clone(Cpu.Registers);
		//	MemoryBeforeRun = new byte[ushort.MaxValue + 1];
		//	Memory.CopyTo(MemoryBeforeRun, 0);
		//	var stopped = false;

		//	while (!stopped)
		//	{
		//		Cpu.ExecuteCpuCycle();
		//		CpuCycleCompleted ? .Invoke(this, EventArgs.Empty);
		//		switch (RunMode)
		//		{
		//		case RunMode.Normal:
		//		case RunMode.UntilBreak:
		//			stopped = _breakReceived;
		//			break;
		//		case RunMode.OneCycle:
		//			stopped = true;
		//			break;
		//		case RunMode.OneInstruction:
		//			stopped = !Cpu.IsInOpExecution;
		//			break;
		//		case RunMode.UntilHalt:
		//			stopped = Cpu.IsInHaltedState;
		//			break;
		//		case RunMode.UntilEnd:
		//			stopped = Cpu.Registers.PC >= CodeEndsAt;
		//			break;
		//		default:
		//			throw new ArgumentOutOfRangeException();
		//		}
		//	}
		//	return _breakReceived;
		//}

		//public void Break()
		//{
		//	_breakReceived = true;
		//}

		//protected virtual void WritePort(ushort addr, byte value)
		//{
		//	IoAccessLog.Add(new IoOp(addr, value, true));
		//}

		//protected virtual byte ReadPort(ushort addr)
		//{
		//	var value = IoReadCount >= IoInputSequence.Count
		//		? (byte)0x00
		//		: IoInputSequence[IoReadCount++];
		//	IoAccessLog.Add(new IoOp(addr, value, false));
		//	return value;
		//}

		//protected virtual void WriteMemory(ushort addr, byte value)
		//{
		//	Memory[addr] = value;
		//	MemoryAccessLog.Add(new MemoryOp(addr, value, true));
		//}

		//protected virtual byte ReadMemory(ushort addr)
		//{
		//	var value = Memory[addr];
		//	MemoryAccessLog.Add(new MemoryOp(addr, value, false));
		//	return value;
		//}

		///// <summary>
		///// Holds information about a memory operation
		///// </summary>
		//public class MemoryOp
		//{
		//	public ushort Address{ get; }
		//	public byte Values{ get; }
		//	public bool IsWrite{ get; }

		//		/// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
		//		public MemoryOp(ushort address, byte values, bool isWrite)
		//	{
		//		Address = address;
		//		Values = values;
		//		IsWrite = isWrite;
		//	}
		//}

		///// <summary>
		///// Holds information about an I/O operation
		///// </summary>
		//public class IoOp
		//{
		//	public ushort Address{ get; }
		//	public byte Value{ get; }
		//	public bool IsOutput{ get; }

		//		/// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
		//		public IoOp(ushort address, byte value, bool isOutput)
		//	{
		//		Address = address;
		//		Value = value;
		//		IsOutput = isOutput;
		//	}
		//}

		///// <summary>
		///// Clones the current set of registers
		///// </summary>
		///// <param name="regs"></param>
		///// <returns></returns>
		//private static Registers Clone(Registers regs)
		//{
		//	return new Registers
		//	{
		//		_AF_ = regs._AF_,
		//		_BC_ = regs._BC_,
		//		_DE_ = regs._DE_,
		//		_HL_ = regs._HL_,
		//		AF = regs.AF,
		//		BC = regs.BC,
		//		DE = regs.DE,
		//		HL = regs.HL,
		//		SP = regs.SP,
		//		PC = regs.PC,
		//		IX = regs.IX,
		//		IY = regs.IY,
		//		IR = regs.IR,
		//		MW = regs.MW
		//	};
		//}
	};
}

