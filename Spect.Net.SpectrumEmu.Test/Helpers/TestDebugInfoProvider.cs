using Spect.Net.SpectrumEmu.Machine;
using Spect.Net.SpectrumEmu.Providers;

namespace Spect.Net.SpectrumEmu.Test.Helpers
{
    public class TestDebugInfoProvider: IDebugInfoProvider
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
        public TestDebugInfoProvider()
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