using Spect.Net.SpectrumEmu.Abstraction.Providers;
using Spect.Net.SpectrumEmu.Machine;

namespace Spect.Net.Z80Tests.ViewModels.SpectrumEmu
{
    /// <summary>
    /// This class provides debug information for the Spectrum VM
    /// </summary>
    public class DebugInfoProvider: ISpectrumDebugInfoProvider
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