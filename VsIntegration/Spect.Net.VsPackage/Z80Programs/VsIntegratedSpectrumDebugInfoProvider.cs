using System.Collections.Generic;
using Spect.Net.SpectrumEmu.Abstraction.Providers;
using Spect.Net.SpectrumEmu.Machine;

namespace Spect.Net.VsPackage.Z80Programs
{
    /// <summary>
    /// This interface extends the functionality of the standard ISpectrumDebugInfoProvider
    /// </summary>
    public interface IVsIntegratedSpectrumDebugInfoProvider : ISpectrumDebugInfoProvider
    {
        /// <summary>
        /// Sets the source map that resolves file/line information into a 
        /// breakpoint address
        /// </summary>
        /// <param name="sourceMap">Source map to use for address resolution</param>
        void SetSourceMap(Dictionary<ushort, (int FileIndex, int Line)> sourceMap);

        /// <summary>
        /// Merges breakpoints defined in the VS IDE with the CPU breakpoints
        /// </summary>
        void MergeBreakPoints();
    }

    /// <summary>
    /// This class provides VS-integrated debug information 
    /// </summary>
    public class VsIntegratedSpectrumDebugInfoProvider: ISpectrumDebugInfoProvider
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

        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public VsIntegratedSpectrumDebugInfoProvider()
        {
            Breakpoints = new BreakpointCollection();
        }
    }
}