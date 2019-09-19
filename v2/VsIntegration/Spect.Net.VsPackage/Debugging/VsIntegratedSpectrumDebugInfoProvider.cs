using System;
using System.Collections.Generic;
using EnvDTE;
using Spect.Net.Assembler.Assembler;
using Spect.Net.SpectrumEmu.Abstraction.Machine;
using Spect.Net.SpectrumEmu.Abstraction.Providers;

namespace Spect.Net.VsPackage.Debugging
{
    public class VsIntegratedSpectrumDebugInfoProvider: VmComponentProviderBase,
        ISpectrumDebugInfoProvider,
        IDisposable
    {
        // --- Store previous breakpoints for comparison
        private readonly Dictionary<Breakpoint, Dictionary<ushort, IBreakpointInfo>> _previousVsBreakpoints =
            new Dictionary<Breakpoint, Dictionary<ushort, IBreakpointInfo>>();

        /// <summary>
        /// Contains the compiled output, provided the compilation was successful
        /// </summary>
        public AssemblerOutput CompiledOutput { get; set; }

        /// <summary>
        /// The name of the file with the current breakpoint
        /// </summary>
        public string CurrentBreakpointFile { get; private set; }

        /// <summary>
        /// The line number within the current breakpoint file
        /// </summary>
        public int CurrentBreakpointLine { get; private set; }

        /// <summary>
        /// The currently defined breakpoints
        /// </summary>
        public BreakpointCollection Breakpoints { get; }

        /// <summary>
        /// Gets or sets an imminent breakpoint
        /// </summary>
        public ushort? ImminentBreakpoint { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        public VsIntegratedSpectrumDebugInfoProvider()
        {
            Breakpoints = new BreakpointCollection();
        }

        /// <summary>
        /// Clears the provider
        /// </summary>
        public void Clear()
        {
            Breakpoints.Clear();
        }

        /// <summary>
        /// Us this method to prepare the breakpoints when running the
        /// virtual machine in debug mode
        /// </summary>
        public void PrepareBreakpoints()
        {
            // TODO: Implement this method
        }

        /// <summary>
        /// Resets the current hit count of breakpoints
        /// </summary>
        public void ResetHitCounts()
        {
            foreach (var bp in Breakpoints.Values)
            {
                bp.CurrentHitCount = 0;
            }
        }


        /// <summary>
        /// Checks if the virtual machine should stop at the specified address
        /// </summary>
        /// <param name="address">Address to check</param>
        /// <returns>
        /// True, if the address means a breakpoint to stop; otherwise, false
        /// </returns>
        public bool ShouldBreakAtAddress(ushort address)
            => Breakpoints.ContainsKey(address);

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or
        /// resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            // TODO: Implement this method
        }
    }
}