namespace Spect.Net.SpectrumEmu.Abstraction.Machine
{
    /// <summary>
    /// This enumeration tells the reason why the execution cycle
    /// of the SpectrumEngine completed.
    /// </summary>
    public enum ExecutionCompletionReason
    {
        /// <summary>The machine is still executing.</summary>
        None,

        /// <summary>Execution cancelled by the user.</summary>
        Cancelled,

        /// <summary>CPU reached the specified termination point.</summary>
        TerminationPointReached,

        /// <summary>CPU reached any of the specified breakpoints.</summary>
        BreakpointReached,

        /// <summary>CPU reached a HALT instruction.</summary>
        Halted,

        /// <summary>The current CPU frame has been completed.</summary>
        CpuFrameCompleted,

        /// <summary>The current screen rendering frame has been completed.</summary>
        RenderFrameCompleted,

        /// <summary>There was an internal exception that has stopped the machine.</summary>
        Exception
    }
}