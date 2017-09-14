// ReSharper disable InconsistentNaming

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
        IStackDebugSupport StackDebugSupport { get; }

        /// <summary>
        /// Gets the object that supports debugging jump instructions
        /// </summary>
        IBranchDebugSupport BranchDebugSupport { get; }

        /// <summary>
        /// Sets the CPU's RESET signal
        /// </summary>
        void SetResetSignal();

        /// <summary>
        /// Releases the CPU's RESET signal
        /// </summary>
        void ReleaseResetSignal();
    }
}