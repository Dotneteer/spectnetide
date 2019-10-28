using System;

namespace Spect.Net.SpectrumEmu.Abstraction.Cpu
{
    /// <summary>
    /// This enum represents the Z80 signals the CPU can receive.
    /// </summary>
    [Flags]
    public enum Z80StateFlags
    {
        /// <summary>
        /// No signal is set.
        /// </summary>
        None = 0,

        /// <summary>
        /// Indicates if an interrupt signal arrived.
        /// </summary>
        Int = 0x01,

        /// <summary>
        /// Indicates if a Non-Maskable Interrupt signal arrived.
        /// </summary>
        Nmi = 0x02,

        /// <summary>
        /// Indicates if a RESET signal arrived.
        /// </summary>
        Reset = 0x04,

        /// <summary>
        /// Is the CPU in HALTED state?
        /// </summary>
        /// <remarks>
        /// When a software HALT instruction is executed, the CPU executes NOPs
        ///  until an interrupt is received(either a non-maskable or a maskable 
        /// interrupt while the interrupt flip-flop is enabled).
        /// </remarks>
        Halted = 0x08,

        /// <summary>
        /// Reset mask of INT.
        /// </summary>
        InvInt = 0xFF - Int,

        /// <summary>
        /// Reset mask for NMI.
        /// </summary>
        InvNmi = 0xFF - Nmi,

        /// <summary>
        /// Reset mask for RESET.
        /// </summary>
        InvReset = 0xFF - Reset,

        /// <summary>
        /// Reset mask for HALT.
        /// </summary>
        InvHalted = 0xFF - Halted
    }
}