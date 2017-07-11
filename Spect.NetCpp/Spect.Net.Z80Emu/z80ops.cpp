#include "stdafx.h"
#include "z80ops.h"

namespace Z80
{
	// "NOP" operation
	// 
	// =================================
	// | 0 | 0 | Q | Q | 0 | 0 | 0 | 1 | 
	// =================================
	// T-States: 4 (4)
	Z80OPERATION nop_op(TZ80Cpu & cpu)
	{
	}

	// "LD QQ,NN" operation
	// 
	// The 16-bit integer value is loaded to the QQ register pair.
	// 
	// =================================
	// | 0 | 0 | Q | Q | 0 | 0 | 0 | 1 | 
	// =================================
	// |             N Low             |
	// =================================
	// |             N High            |
	// =================================
	// QQ: 00=BC, 01=DE, 10=HL, 11=SP
	// T-States: 4, 3, 3 (10)
	Z80OPERATION ld_qq_nn_op(TZ80Cpu & cpu)
	{
		cpu.Registers.C = cpu.ReadMemory(cpu.Registers.PC++);
		cpu.Registers.B = cpu.ReadMemory(cpu.Registers.PC++);
		cpu.ClockP6();
	}

	Z80Operation const standardOpCodes[] = {
		nop_op,
	};
};