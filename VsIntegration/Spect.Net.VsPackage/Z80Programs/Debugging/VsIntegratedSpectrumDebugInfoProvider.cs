using System;
using System.Linq;
using EnvDTE;
using Spect.Net.Assembler.Assembler;
using Spect.Net.SpectrumEmu.Abstraction.Providers;
using Spect.Net.SpectrumEmu.Machine;

namespace Spect.Net.VsPackage.Z80Programs.Debugging
{
    /// <summary>
    /// This class provides VS-integrated debug information 
    /// </summary>
    public class VsIntegratedSpectrumDebugInfoProvider: ISpectrumDebugInfoProvider
    {
        /// <summary>
        /// The owner package
        /// </summary>
        public SpectNetPackage Package { get; }

        /// <summary>
        /// Contains the compiled output, provided the compilation was successful
        /// </summary>
        public AssemblerOutput CompiledOutput { get; set; }

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
        /// Us this method to prepare the breakpoints when running the
        /// virtual machine in debug mode
        /// </summary>
        public void PrepareBreakpoints()
        {
            // --- Keep CPU breakpoints set through the Disassembler tool
            var cpuBreakPoints = Breakpoints.Where(bp => bp.Value.IsCpuBreakpoint);
            Breakpoints.Clear();
            foreach (var bpItem in cpuBreakPoints)
            {
                Breakpoints.Add(bpItem.Key, bpItem.Value);
            }

            // --- Merge breakpoints set in Visual Studio
            if (CompiledOutput == null) return;
            foreach (Breakpoint breakpoint in Package.ApplicationObject.Debugger.Breakpoints)
            {
                // --- Check for the file
                int fileIndex = -1;
                for (var i = 0; i < CompiledOutput.SourceFileList.Count; i++)
                {
                    if (string.Compare(breakpoint.File, CompiledOutput.SourceFileList[i].Filename,
                            StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        fileIndex = i;
                        break;
                    }
                }
                if (fileIndex < 0) continue;

                // --- Check the breakpoint address
                if (CompiledOutput.AddressMap.TryGetValue((fileIndex, breakpoint.FileLine), out var address))
                {
                    Breakpoints.Add(address, new BreakpointInfo
                    {
                        File = CompiledOutput.SourceFileList[fileIndex].Filename,
                        FileLine = breakpoint.FileLine,
                        Type = BreakpointType.NoCondition
                    });
                }
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
        {
            return Breakpoints.ContainsKey(address);
        }

        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public VsIntegratedSpectrumDebugInfoProvider(SpectNetPackage package)
        {
            Package = package;
            Breakpoints = new BreakpointCollection();
        }
    }
}