using Spect.Net.SpectrumEmu.Machine;

namespace Spect.Net.SpectrumEmu.Test.Helpers
{
    public class TestDebugInfoProvider: IDebugInfoProvider
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
        public TestDebugInfoProvider()
        {
            Breakpoints = new BreakpointCollection();
            ImminentBreakpoint = null;
        }
    }
}