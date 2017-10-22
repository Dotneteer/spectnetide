namespace Spect.Net.SpectrumEmu.Machine
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
    }
}