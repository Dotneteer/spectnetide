using Spect.Net.SpectrumEmu.Abstraction.Providers;

namespace Spect.Net.SpectrumEmu.Machine
{
    /// <summary>
    /// Default implementation of the Spectrum debug info provider
    /// </summary>
    public class SpectrumDebugInfoProvider: ISpectrumDebugInfoProvider
    {
        /// <summary>
        /// The component provider should be able to reset itself
        /// </summary>
        public void Reset()
        {
        }

        /// <summary>
        /// The currently defined breakpoints
        /// </summary>
        public BreakpointCollection Breakpoints { get; }

        /// <summary>
        /// Gets or sets an imminent breakpoint
        /// </summary>
        public ushort? ImminentBreakpoint { get; set; }

        /// <summary>
        /// Entire time spent within a single ULA frame
        /// </summary>
        public long FrameTime { get; set; }

        /// <summary>
        /// Time spent with executing CPU instructions
        /// </summary>
        public long CpuTime { get; set; }

        /// <summary>
        /// Time spent with screen rendering
        /// </summary>
        public long ScreenRenderingTime { get; set; }

        /// <summary>
        /// Time spent with other utility activities
        /// </summary>
        public long UtilityTime { get; set; }

        /// <summary>
        /// Entire time spent within a single ULA frame
        /// </summary>
        public double FrameTimeInMs { get; set; }

        /// <summary>
        /// Time spent with executing CPU instructions
        /// </summary>
        public double CpuTimeInMs { get; set; }

        /// <summary>
        /// Time spent with screen rendering
        /// </summary>
        public double ScreenRenderingTimeInMs { get; set; }

        /// <summary>
        /// Time spent with other utility activities
        /// </summary>
        public double UtilityTimeInMs { get; set; }

        public SpectrumDebugInfoProvider()
        {
            Breakpoints = new BreakpointCollection();
        }
    }
}