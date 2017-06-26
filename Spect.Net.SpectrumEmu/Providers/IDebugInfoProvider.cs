using Spect.Net.SpectrumEmu.Machine;

namespace Spect.Net.SpectrumEmu.Providers
{
    /// <summary>
    /// This interface defines the responsibilities of an object that provides
    /// information about the current debugging mode.
    /// </summary>
    public interface IDebugInfoProvider: IVmComponentProvider
    {
        /// <summary>
        /// The currently defined breakpoints
        /// </summary>
        BreakpointCollection Breakpoints { get; }

        /// <summary>
        /// Gets or sets an imminent breakpoint
        /// </summary>
        ushort? ImminentBreakpoint { get; set; }

        /// <summary>
        /// Entire time spent within a single ULA frame
        /// </summary>
        ulong FrameTime { get; set; }

        /// <summary>
        /// Time spent with executing CPU instructions
        /// </summary>
        ulong CpuTime { get; set; }

        /// <summary>
        /// Time spent with screen rendering
        /// </summary>
        ulong ScreenRenderingTime { get; set; }

        /// <summary>
        /// Time spent with other utility activities
        /// </summary>
        ulong UtilityTime { get; set; }

        /// <summary>
        /// Entire time spent within a single ULA frame
        /// </summary>
        double FrameTimeInMs { get; set; }

        /// <summary>
        /// Time spent with executing CPU instructions
        /// </summary>
        double CpuTimeInMs { get; set; }

        /// <summary>
        /// Time spent with screen rendering
        /// </summary>
        double ScreenRenderingTimeInMs { get; set; }

        /// <summary>
        /// Time spent with other utility activities
        /// </summary>
        double UtilityTimeInMs { get; set; }
    }
}