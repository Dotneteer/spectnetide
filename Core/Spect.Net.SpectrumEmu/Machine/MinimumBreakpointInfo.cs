namespace Spect.Net.SpectrumEmu.Machine
{
    /// <summary>
    /// This class stores minimum breakpoint information
    /// </summary>
    public class MinimumBreakpointInfo : IBreakpointInfo
    {
        /// <summary>
        /// Empty breakpoint information
        /// </summary>
        public static IBreakpointInfo EmptyBreakpointInfo = new MinimumBreakpointInfo();

        /// <summary>
        /// This flag shows that the breakpoint is assigned to the
        /// CPU (and not to some source code).
        /// </summary>
        public bool IsCpuBreakpoint => true;
    }
}