#include "stdafx.h"
#include "z80spec.h"

namespace Z80
{
	// Initializes the state of the Z80 CPU
	TZ80Cpu::TZ80Cpu()
	{
	}

	// Executes a CPU cycle
	void TZ80Cpu::ExecuteCpuCycle()
	{
		if (BeforeProcessCpuSignals != NULL)
		{
			BeforeProcessCpuSignals(*this);
		}

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

		auto opCode = ReadMemory(Registers.PC);
		ClockP3();
		if (OperationCodeFetched != NULL) OperationCodeFetched(*this, opCode);
		Registers.PC++;
		RefreshMemory();

		if (PrefixMode == opmBit)
		{
			// --- The CPU is already in BIT operations (0xCB) prefix mode
			IsInterruptBlocked = false;
			if (ProcessingOperation != NULL)
			{
				ProcessingOperation(*this, opCode);
			}
			ProcessCBPrefixedOperations(opCode);
			IndexMode = oimNone;
			PrefixMode = opmNone;
			IsInOpExecution = false;
			return;
		}

		if (PrefixMode == opmExtended)
		{
			// --- The CPU is already in Extended operations (0xED) prefix mode
			IsInterruptBlocked = false;
			if (ProcessingOperation != NULL)
			{
				ProcessingOperation(*this, opCode);
			}
			ProcessEDOperations(opCode);
			IndexMode = oimNone;
			PrefixMode = opmNone;
			IsInOpExecution = false;
			return;
		}

		if (opCode == 0xDD)
		{
			// --- An IX index prefix received
			// --- Disable the interrupt unless the full operation code is received
			IndexMode = oimIX;
			IsInterruptBlocked = true;
			IsInOpExecution = true;
			return;
		}

		if (opCode == 0xFD)
		{
			// --- An IY index prefix received
			// --- Disable the interrupt unless the full operation code is received
			IndexMode = oimIY;
			IsInterruptBlocked = true;
			IsInOpExecution = true;
			return;
		}

		if (opCode == 0xCB)
		{
			// --- A bit operation prefix received
			// --- Disable the interrupt unless the full operation code is received
			PrefixMode = opmBit;
			IsInterruptBlocked = true;
			IsInOpExecution = true;
			return;
		}

		if (opCode == 0xED)
		{
			// --- An extended operation prefix received
			// --- Disable the interrupt unless the full operation code is received
			PrefixMode = opmExtended;
			IsInterruptBlocked = true;
			IsInOpExecution = true;
			return;
		}

		// --- Normal (8-bit) operation code received
		IsInterruptBlocked = false;
		//ProcessingOperation ? .Invoke(this, new Z80OperationCodeEventArgs(opCode, this));
		//ProcessStandardOperations(opCode);
		PrefixMode = opmNone;
		IndexMode = oimNone;
		IsInOpExecution = false;
	}

	// Apply a Reset signal
	void TZ80Cpu::Reset()
	{
		ResetSignal = true;
		ExecuteCpuCycle();
		ResetSignal = false;
	}

	// Processes the CPU signals coming from peripheral devices
	// of the computer
	// Returns: True, if a signal has been processed; otherwise, false
	bool TZ80Cpu::ProcessCpuSignals()
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

	// Takes care of refreshing the dynamic memory.
	// The Z80 CPU contains a memory refresh counter, enabling dynamic 
	// memories to be used with the same ease as static memories. Seven 
	// bits of this 8-bit register are automatically incremented after 
	// each instruction fetch. The eighth bit remains as programmed, 
	// resulting from an "LD R, A" instruction. The data in the refresh
	// counter is sent out on the lower portion of the address bus along 
	// with a refresh control signal while the CPU is decoding and 
	// executing the fetched instruction. This mode of refresh is 
	// transparent to the programmer and does not slow the CPU operation.
	void TZ80Cpu::RefreshMemory()
	{
	}

	// Processes the operations with 0xCB prefix
	// opCode: Operation code
	void TZ80Cpu::ProcessCBPrefixedOperations(byte opCode)
	{
	}

	// Processes the operations with 0xED prefix
	// opCode: Operation code
	void TZ80Cpu::ProcessEDOperations(byte opCode)
	{
	}

	// Executes a hard reset
	void TZ80Cpu::ExecuteReset()
	{
		IsInHaltedState = false;
		IFF1 = false;
		IFF2 = false;
		InterruptMode = 0;
		IsInterruptBlocked = false;
		IntSignal = false;
		NmiSignal = false;
		PrefixMode = opmNone;
		IndexMode = oimNone;
		Registers.PC = 0x0000;
		Registers.IR = 0x0000;
		IsInOpExecution = false;
	}

	// Executes an NMI
	void TZ80Cpu::ExecuteNmi()
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

	// Executes an INT
	void TZ80Cpu::ExecuteInterrupt()
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
			auto adr = (Registers.IR & 0xFF00);
			ClockP5();
			auto l = ReadMemory(adr);
			ClockP3();
			auto h = ReadMemory(++adr);
			ClockP3();
			Registers.MW += (h << 8 | l);
			ClockP6();
			break;
		}
		Registers.PC = Registers.MW;
	};
};
