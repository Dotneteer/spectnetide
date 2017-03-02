namespace Spect.Net.SpectrumEmu.Machine
{
    /// <summary>
    /// This interface defines the responsibilities of an object that provides
    /// information about the current debugging mode.
    /// </summary>
    public interface IDebugInfoProvider
    {
        /// <summary>
        /// The currently defined breakpoints
        /// </summary>
        BreakpointCollection Breakpoints { get; }

        /// <summary>
        /// Gets or sets an imminent breakpoint
        /// </summary>
        ushort? ImminentBreakpoint { get; set; }
    }
}