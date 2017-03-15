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