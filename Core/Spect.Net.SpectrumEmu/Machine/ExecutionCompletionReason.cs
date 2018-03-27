namespace Spect.Net.SpectrumEmu.Machine
{
    /// <summary>
    /// This enumeration tells the reason why the execution cycle
    /// of the SpectrumEngine completed.
    /// </summary>
    public enum ExecutionCompletionReason
    {
        /// <summary>The machine is still executing</summary>
        None,

        /// <summary>Execution cancelled by the user</summary>
        Cancelled,

        /// <summary>Execution timed out</summary>
        Timeout,

        /// <summary>CPU reached the specified termination point</summary>
        TerminationPointReached,

        /// <summary>CPU reached any of the specified breakpoints</summary>
        BreakpointReached,

        /// <summary>CPU reached a HALT instrution</summary>
        Halted,

        /// <summary>The current screen rendering frame has been completed</summary>
        FrameCompleted
    }
}