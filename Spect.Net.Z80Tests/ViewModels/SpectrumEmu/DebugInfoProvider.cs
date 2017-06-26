using Spect.Net.SpectrumEmu.Machine;
using Spect.Net.SpectrumEmu.Providers;

namespace Spect.Net.Z80Tests.ViewModels.SpectrumEmu
{
    /// <summary>
    /// This class provides debug information for the Spectrum VM
    /// </summary>
    public class DebugInfoProvider: IDebugInfoProvider
    {
        /// <summary>
        /// The currently defined breakpoints
        /// </summary>
        public BreakpointCollection Breakpoints { get; private set; }

        /// <summary>
        /// Gets or sets an imminent breakpoint
        /// </summary>
        public ushort? ImminentBreakpoint { get; set; }

        /// <summary>
        /// Entire time spent within a single ULA frame
        /// </summary>
        public ulong FrameTime { get; set; }

        /// <summary>
        /// Time spent with executing CPU instructions
        /// </summary>
        public ulong CpuTime { get; set; }

        /// <summary>
        /// Time spent with screen rendering
        /// </summary>
        public ulong ScreenRenderingTime { get; set; }

        /// <summary>
        /// Time spent with other utility activities
        /// </summary>
        public ulong UtilityTime { get; set; }

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

        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public DebugInfoProvider()
        {
            Reset();
        }

        /// <summary>
        /// The component provider should be able to reset itself
        /// </summary>
        public void Reset()
        {
            Breakpoints = new BreakpointCollection();
            ImminentBreakpoint = null;
        }
    }
}