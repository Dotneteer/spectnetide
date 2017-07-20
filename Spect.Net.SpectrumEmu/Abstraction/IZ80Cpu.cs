// ReSharper disable InconsistentNaming

using Spect.Net.SpectrumEmu.Cpu;

namespace Spect.Net.SpectrumEmu.Abstraction
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
        Z80StateFlags StateFlags { get; }

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
        /// Allows setting the number of tacts
        /// </summary>
        /// <param name="tacts">New value of #of tacts</param>
        void SetTacts(long tacts);

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
    }
}