namespace Spect.Net.SpectrumEmu.Abstraction.Machine
{
    /// <summary>
    /// This interface describes information related to a breakpoint
    /// </summary>
    public interface IBreakpointInfo
    {
        /// <summary>
        /// This flag shows that the breakpoint is assigned to the
        /// CPU (and not to some source code).
        /// </summary>
        bool IsCpuBreakpoint { get; }

        /// <summary>
        /// Type of breakpoint hit condition
        /// </summary>
        BreakpointHitType HitType { get; }

        /// <summary>
        /// Value of the hit condition
        /// </summary>
        ushort HitConditionValue { get; }

        /// <summary>
        /// The current hit count value
        /// </summary>
        int CurrentHitCount { get; set; }
    }
}