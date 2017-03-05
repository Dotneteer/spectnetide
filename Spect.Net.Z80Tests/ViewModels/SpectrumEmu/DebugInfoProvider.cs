using Spect.Net.SpectrumEmu.Machine;

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
        public BreakpointCollection Breakpoints { get; }

        /// <summary>
        /// Gets or sets an imminent breakpoint
        /// </summary>
        public ushort? ImminentBreakpoint { get; set; }

        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public DebugInfoProvider()
        {
            Breakpoints = new BreakpointCollection();
        }
    }
}