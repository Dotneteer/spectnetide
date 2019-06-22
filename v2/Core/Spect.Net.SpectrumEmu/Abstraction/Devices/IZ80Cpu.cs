// ReSharper disable InconsistentNaming

using System;
using Spect.Net.SpectrumEmu.Abstraction.Discovery;
using Spect.Net.SpectrumEmu.Cpu;

namespace Spect.Net.SpectrumEmu.Abstraction.Devices
{
    /// <summary>
    /// This interface represents a Z80Cpu
    /// </summary>
    public interface IZ80Cpu : IClockBoundDevice
    {

        /// <summary>
        /// Gets the current set of registers
        /// </summary>
        Registers Registers { get; }

        /// <summary>
        /// CPU signals
        /// </summary>
        Z80StateFlags StateFlags { get; set; }

        /// <summary>
        /// Specifies the contention mode that affects the CPU.
        /// False: ULA contention mode;
        /// True: Gate array contention mode;
        /// </summary>
        bool UseGateArrayContention { get; set; }

        /// <summary>
        /// Interrupt Enable Flip-Flop #1
        /// </summary>
        /// <remarks>
        /// Disables interrupts from being accepted 
        /// </remarks>
        bool IFF1 { get; }

        /// <summary>
        /// Interrupt Enable Flip-Flop #2
        /// </summary>
        /// <remarks>
        /// Temporary storage location for IFF1
        /// </remarks>
        bool IFF2 { get; }

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
        byte InterruptMode { get; }

        /// <summary>
        /// The interrupt is blocked
        /// </summary>
        bool IsInterruptBlocked { get; }

        /// <summary>
        /// Is currently in opcode execution?
        /// </summary>
        bool IsInOpExecution { get; }

        /// <summary>
        /// Increments the internal clock with the specified delay ticks
        /// </summary>
        /// <param name="ticks">Delay ticks</param>
        void Delay(int ticks);

        /// <summary>
        /// Executes a CPU cycle
        /// </summary>
        void ExecuteCpuCycle();

        /// <summary>
        /// Turns off the CPU and fills register values accordingly.
        /// </summary>
        void TurnOffCpu();

        /// <summary>
        /// Checks if the next instruction to be executed is a call instruction or not
        /// </summary>
        /// <returns>
        /// 0, if the next instruction is not a call; otherwise the length of the call instruction
        /// </returns>
        int GetCallInstructionLength();

        /// <summary>
        /// Gets the memory device associated with the CPU
        /// </summary>
        IMemoryDevice MemoryDevice { get; }

        /// <summary>
        /// Gets the device that handles Z80 CPU I/O operations
        /// </summary>
        IPortDevice PortDevice { get; }

        /// <summary>
        /// Gets the object that supports debugging the stack
        /// </summary>
        IStackDebugSupport StackDebugSupport { get; set; }

        /// <summary>
        /// Gets the object that supports debugging jump instructions
        /// </summary>
        IBranchDebugSupport BranchDebugSupport { get; }

        /// <summary>
        /// This flag indicates if the CPU entered into a maskable
        /// interrupt method as a result of an INT signal
        /// </summary>
        bool MaskableInterruptModeEntered { get; }

        /// <summary>
        /// This flag signs if the Z80 extended instruction set (Spectrum Next)
        /// is allowed, or NOP instructions should be executed instead of
        /// these extended operations.
        /// </summary>
        bool AllowExtendedInstructionSet { get; }

        /// <summary>
        /// Sets the CPU's RESET signal
        /// </summary>
        void SetResetSignal();

        /// <summary>
        /// Releases the CPU's RESET signal
        /// </summary>
        void ReleaseResetSignal();

        /// <summary>
        /// This event is raised just before a maskable interrupt is about to execute
        /// </summary>
        event EventHandler InterruptExecuting;

        /// <summary>
        /// This event is raised just before a non-maskable interrupt is about to execute
        /// </summary>
        event EventHandler NmiExecuting;

        /// <summary>
        /// This event is raised just before the memory is being read
        /// </summary>
        event EventHandler<AddressEventArgs> MemoryReading;

        /// <summary>
        /// This event is raised right after the memory has been read
        /// </summary>
        event EventHandler<AddressAndDataEventArgs> MemoryRead;

        /// <summary>
        /// This event is raised just before the memory is being written
        /// </summary>
        event EventHandler<AddressAndDataEventArgs> MemoryWriting;

        /// <summary>
        /// This event is raised just after the memory has been written
        /// </summary>
        event EventHandler<AddressAndDataEventArgs> MemoryWritten;

        /// <summary>
        /// This event is raised just before a port is being read
        /// </summary>
        event EventHandler<AddressEventArgs> PortReading;

        /// <summary>
        /// This event is raised right after a port has been read
        /// </summary>
        event EventHandler<AddressAndDataEventArgs> PortRead;

        /// <summary>
        /// This event is raised just before a port is being written
        /// </summary>
        event EventHandler<AddressAndDataEventArgs> PortWriting;

        /// <summary>
        /// This event is raised just after a port has been written
        /// </summary>
        event EventHandler<AddressAndDataEventArgs> PortWritten;

        /// <summary>
        /// This event is raised just before a Z80 operation is being executed
        /// </summary>
        event EventHandler<Z80InstructionExecutionEventArgs> OperationExecuting;

        /// <summary>
        /// This event is raised just after a Z80 operation has been executed
        /// </summary>
        event EventHandler<Z80InstructionExecutionEventArgs> OperationExecuted;
    }
}