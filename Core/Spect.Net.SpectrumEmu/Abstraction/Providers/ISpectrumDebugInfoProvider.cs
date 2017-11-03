using Spect.Net.SpectrumEmu.Machine;

namespace Spect.Net.SpectrumEmu.Abstraction.Providers
{
    /// <summary>
    /// This interface defines the responsibilities of an object that provides
    /// information about the current debugging mode.
    /// </summary>
    public interface ISpectrumDebugInfoProvider: IVmComponentProvider
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
        /// Us this method to prepare the breakpoints when running the
        /// virtual machine in debug mode
        /// </summary>
        void PrepareBreakpoints();

        /// <summary>
        /// Checks if the virtual machine should stop at the specified address
        /// </summary>
        /// <param name="address">Address to check</param>
        /// <returns>
        /// True, if the address means a breakpoint to stop; otherwise, false
        /// </returns>
        bool ShouldBreakAtAddress(ushort address);
    }
}