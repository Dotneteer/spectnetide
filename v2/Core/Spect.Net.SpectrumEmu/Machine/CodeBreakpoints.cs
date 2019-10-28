using Spect.Net.SpectrumEmu.Abstraction.Providers;

namespace Spect.Net.SpectrumEmu.Machine
{
    /// <summary>
    /// Represents the collection of breakpoints
    /// </summary>
    public sealed class CodeBreakpoints
    {
        private readonly ISpectrumDebugInfoProvider _debugInfoProvider;

        public CodeBreakpoints(ISpectrumDebugInfoProvider debugInfoProvider)
        {
            _debugInfoProvider = debugInfoProvider;
        }

        /// <summary>
        /// The number of breakpoints
        /// </summary>
        public int Count => _debugInfoProvider.Breakpoints.Count;

        /// <summary>
        /// Adds a new breakpoint at the specified address
        /// </summary>
        /// <param name="address">Breakpoint address</param>
        public void AddBreakpoint(ushort address)
        {
            _debugInfoProvider.Breakpoints[address] = new MinimumBreakpointInfo();
        }

        /// <summary>
        /// Removes the breakpoint from the specified address
        /// </summary>
        /// <param name="address"></param>
        public void RemoveBreakpoint(ushort address)
        {
            _debugInfoProvider.Breakpoints.Remove(address);
        }

        /// <summary>
        /// Clear all previously declared breakpoints
        /// </summary>
        public void ClearAllBreakpoints()
        {
            _debugInfoProvider.Breakpoints.Clear();
        }

        /// <summary>
        /// Checks if there is a breakpoint definied for the
        /// given address
        /// </summary>
        /// <param name="address">Breakpoint address</param>
        /// <returns></returns>
        public bool HasBreakpointAt(ushort address)
            => _debugInfoProvider.Breakpoints.ContainsKey(address);
    }
}