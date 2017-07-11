#include "stdafx.h"
#include <climits>
#include "z80_test_helpers.h"

namespace Z80
{
	Z80TestMachine::Z80TestMachine(RunMode runMode) :
		Cpu(*new TZ80Cpu()),
		Memory(new byte[USHRT_MAX + 1])
	{
		SetRunMode(runMode);
		_breakReceived = false;
		//MemoryAccessLog = new List<MemoryOp>();
		//IoAccessLog = new List<IoOp>();
		//IoInputSequence = new List<byte>();
		_ioReadCount = 0;
		Cpu.ReadMemory = [](uBit16 addr) -> byte
		{
			byte value = Memory[addr];
			//MemoryAccessLog.Add(new MemoryOp(addr, value, false));
			return value;
		}
		Cpu.WriteMemory = WriteMemory;
		Cpu.ReadPort = ReadPort;
		Cpu.WritePort = WritePort;

	}

	RunMode Z80::Z80TestMachine::GetRunMode()
	{
		return RunMode();
	}
	uBit16 Z80TestMachine::GetCodeEndsAt()
	{
		return uBit16();
	}
	int Z80TestMachine::GetIoReadCount()
	{
		return 0;
	}
	
	const TZ80Regs & Z80TestMachine::GetRegistersBeforeRun()
	{
		return _regsBeforeRun;
	}

	const byte * Z80TestMachine::GetMemoryBeforeRun()
	{
		Memory[3] = 12;
		return _memoryBeforeRun;
	}

	void Z80TestMachine::SetRunMode(RunMode mode)
	{
	}

	byte __fastcall Z80TestMachine::ReadMemory(uBit16 addr)
	{
		
	}
	void Z80TestMachine::WriteMemory(uBit16 addr, byte value)
	{
		Memory[addr] = value;
		// MemoryAccessLog.Add(new MemoryOp(addr, value, true));
	}
};
