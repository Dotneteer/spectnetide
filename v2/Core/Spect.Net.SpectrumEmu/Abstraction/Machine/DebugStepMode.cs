namespace Spect.Net.SpectrumEmu.Abstraction.Machine
{
    /// <summary>
    /// The mode the execution cycle should run in debug mode.
    /// </summary>
    public enum DebugStepMode
    {
        /// <summary>
        /// Execution stops at the next breakpoint.
        /// </summary>
        StopAtBreakpoint,

        /// <summary>
        /// Execution stops after the next instruction.
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
        /// returns from the latest subroutine call.
        /// </summary>
        StepOut
    }
}