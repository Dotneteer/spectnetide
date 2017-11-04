using Spect.Net.SpectrumEmu.Abstraction.Providers;
using Spect.Net.SpectrumEmu.Machine;

namespace Spect.Net.SpectrumEmu.Test.Helpers
{
    public class TestDebugInfoProvider: VmComponentProviderBase, ISpectrumDebugInfoProvider
    {
        /// <summary>
        /// Initialize the provider
        /// </summary>
        public TestDebugInfoProvider()
        {
            // ReSharper disable once VirtualMemberCallInConstructor
            Reset();
        }

        /// <summary>
        /// The currently defined breakpoints
        /// </summary>
        public BreakpointCollection Breakpoints { get; private set; }

        /// <summary>
        /// Gets or sets an imminent breakpoint
        /// </summary>
        public ushort? ImminentBreakpoint { get; set; }

        /// <summary>
        /// Us this method to prepare the breakpoints when running the
        /// virtual machine in debug mode
        /// </summary>
        public void PrepareBreakpoints()
        {
        }

        /// <summary>
        /// Checks if the virtual machine should stop at the specified address
        /// </summary>
        /// <param name="address">Address to check</param>
        /// <returns>
        /// True, if the address means a breakpoint to stop; otherwise, false
        /// </returns>
        public bool ShouldBreakAtAddress(ushort address)
        {
            return Breakpoints.ContainsKey(address);
        }

        /// <summary>
        /// The component provider should be able to reset itself
        /// </summary>
        public override void Reset()
        {
            Breakpoints = new BreakpointCollection();
            ImminentBreakpoint = null;
        }
    }
}