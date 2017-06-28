#pragma once

#include "commondefs.h"

namespace Z80
{
	class T80ZCpu;

	// Memory reader and writer hooks
	typedef byte (__fastcall * TReadOperation)(uBit16 addr);
	typedef void (__fastcall * TWriteOperation)(uBit16 addr, byte val);

	// Z80 event callback functions
	typedef void (__fastcall * TZ80CpuEventCallback)(T80ZCpu& cpu);
	typedef void (__fastcall * TZ80CpuOpCodeEventCallback)(T80ZCpu& cpu, byte opCode);

	// Operation Prefix Mode
	enum OpPrefixMode : byte
	{
		NoPrefix,       // No operation prefix
		ExtendedPrefix, // Extended mode (0xED prefix)
		BitPrefix       // Bit operations mode (0xCB prefix)
	};

	// Signs if the current instruction uses any of the indexed address modes
	enum OpIndexMode
	{
		NoIndex,  // Indexed address mode is not used
		IXMode,   // Indexed address with IX register
		IYMode    // Indexed address with IY register
	};

	// Z80 Registers
	struct TZ80Regs {
		// Common registers/register pairs
		union
		{
			uBit16 BC;
			struct
			{
				byte C;
				byte B;
			};
		};
		union
		{
			uBit16 DE;
			struct
			{
				byte E;
				byte D;
			};
		};
		union
		{
			uBit16 HL;
			struct
			{
				byte L;
				byte H;
			};
		};
		union
		{
			uBit16 AF;
			struct
			{
				byte F;
				byte A;
			};
		};

		// Alternate register set
		struct
		{
			union
			{
				uBit16 BC;
				struct
				{
					byte C;
					byte B;
				};
			};
			union
			{
				uBit16 DE;
				struct
				{
					byte E;
					byte D;
				};
			};
			union
			{
				uBit16 HL;
				struct
				{
					byte L;
					byte H;
				};
			};
			union
			{
				uBit16 AF;
				struct
				{
					byte F;
					byte A;
				};
			};
		} Alt;

		// Program Counter
		union
		{
			uBit16 PC;
			struct {
				byte lPC;
				byte hPC;
			};
		};

		// Stack Pointer
		union
		{
			uBit16 SP;
			struct {
				byte lSP;
				byte hSP;
			};
		};

		// Interrupt vector/Refresh register
		union
		{
			uBit16 IR;
			struct {
				byte R;
				byte I;
			};
		};

		// Index registers
		union
		{
			uBit16 IX;
			struct
			{
				byte lX;
				byte hX;
			};
		};
		union
		{
			uBit16 IY;
			struct
			{
				byte lY;
				byte hY;
			};
		};
	};

	// Z80 CPU State information
	struct TZ80CpuState
	{
		// The 13 pair of Z80 registers
		TZ80Regs Registers;

		// Is the CPU in HALTED state?
		// When a software HALT instruction is executed, the CPU executes NOPs
		// until an interrupt is received (either a nonmaskable or a maskable 
		// interrupt while the interrupt flip-flop is enabled).
		bool IsInHaltedState;

		// Interrupt Enable Flip-Flop #1
		// Disables interrupts from being accepted 
		bool IFF1;

		// Interrupt Enable Flip-Flop #2
		// Temporary storage location for IFF1
		bool IFF2;

		// The current Interrupt mode
		// IM 0: In this mode, the interrupting device can insert any 
		// instruction on the data bus for execution by the CPU.The first 
		// byte of a multi-byte instruction is read during the interrupt 
		// acknowledge cycle. Subsequent bytes are read in by a normal 
		// memory read sequence.
		// IM 1: In this mode, the processor responds to an interrupt by 
		// executing a restart at address 0038h.
		// IM 2: This mode allows an indirect call to any memory location 
		// by an 8-bit vector supplied from the peripheral device. This vector
		// then becomes the least-significant eight bits of the indirect 
		// pointer, while the I Register in the CPU provides the most-
		// significant eight bits.This address points to an address in a 
		// vector table that is the starting address for the interrupt
		// service routine.
		byte InterruptMode;

		// The interrupt is blocked
		bool IsInterruptBlocked;

		// Indicates if an interrupt signal arrived
		bool IntSignal;

		// Indicates if a Non-Maskable Interrupt signal arrived
		bool NmiSignal;

		// Indicates if a RESET signal arrived
		bool ResetSignal;

		// The current Operation Prefix Mode
		OpPrefixMode PrefixMode;

		// The current Operation Index Mode
		OpIndexMode IndexMode;

		// Is currently in opcode execution?
		// Z80 has multi-byte opcodes. This flag signs that the CPU
		// is processing such an instruction.
		bool IsInOpExecution;

		// The number of clock cycles since the start of the system
		// This counter overflows in about 20 minutes, but it is long enough
		// time to not cause any issue.
		unsigned int Tacts;
	};

	// This structure represents the Z80 CPU
	class TZ80Cpu : TZ80CpuState
	{
	public:
		// The operation that reads the memory (out of the M1 machine cycle)
		// The operation accepts an address (uBit16). It returns the byte read 
		// from the memory.
		TReadOperation ReadMemory;

		// The operation that writes the memory
		// The operation accepts an address (uBit16), and a value (byte) to put
		// into the specified memory address
		TWriteOperation WriteMemory;

		// The operation that reads an I/O port
		// The operation accepts an address (uBit16), and returns the value (byte)
		// read from the particular port.
		TReadOperation ReadPort;

		// The operation that writes an I/O port
		// The operation accepts an address (uBit16), and a value (byte) to
		// write to the particular port.
		TWriteOperation WritePort;

		// This event is raised right before the Z80 CPU signals are
		// being processed.
		TZ80CpuEventCallback BeforeProcessCpuSignals;

		// This event is raised right after a new operation code has
		// been fetched.
		TZ80CpuOpCodeEventCallback OperationCodeFetched;

		// This event is raised right before an operation code is
		// about to be processed
		TZ80CpuOpCodeEventCallback ProcessingOperation;

		// Increments the internal clock with the specified delay ticks
		void __forceinline Delay(byte ticks) { Tacts += ticks; }

	private:
		// Methods that increment the internal clock counter
		void __forceinline ClockP1() { Tacts += 1; }
		void __forceinline ClockP2() { Tacts += 2; }
		void __forceinline ClockP3() { Tacts += 3; }
		void __forceinline ClockP4() { Tacts += 4; }
		void __forceinline ClockP5() { Tacts += 5; }
		void __forceinline ClockP6() { Tacts += 6; }
		void __forceinline ClockP7() { Tacts += 7; }


	};
};