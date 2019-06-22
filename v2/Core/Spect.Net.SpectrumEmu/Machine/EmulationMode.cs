﻿namespace Spect.Net.SpectrumEmu.Machine
{
    /// <summary>
    /// This enumeration defines how the spectrum emulation mode
    /// should work
    /// </summary>
    public enum EmulationMode
    {
        /// <summary>
        /// Run the virtual machine until stopped
        /// </summary>
        Continuous,

        /// <summary>
        /// Run the virtual machine in debugger mode
        /// </summary>
        Debugger,

        /// <summary>
        /// Run the VM until the CPU is halted
        /// </summary>
        UntilHalt,

        /// <summary>
        /// Run the CPU until the current ULA rendering frame ends
        /// by the ULA clock
        /// </summary>
        UntilFrameEnds,

        /// <summary>
        /// Run the CPU until a specified value of the PC register is reached.
        /// </summary>
        UntilExecutionPoint
    }

    /// <summary>
    /// The mode the execution cycle should run indebug mode
    /// </summary>
    public enum DebugStepMode
    {
        /// <summary>
        /// Execution stops at the next breakpoint
        /// </summary>
        StopAtBreakpoint,

        /// <summary>
        /// Execution stops after the next instruction
        /// </summary>
        StepInto,

        /// <summary>
        /// Execution stops after the next instruction. If that should
        /// be a subroutine call, the execution stops after returning
        /// from the subroutine.
        /// </summary>
        StepOver,

        /// <summary>
        /// Execution stops after the first RET (unconditional or conditional)
        /// returns from the lates subroutine call.
        /// </summary>
        StepOut
    }
}