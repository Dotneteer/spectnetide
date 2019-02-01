using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EnvDTE;
using Spect.Net.Assembler.Assembler;
using Spect.Net.SpectrumEmu.Abstraction.Providers;
using Spect.Net.SpectrumEmu.Machine;
using Spect.Net.VsPackage.CustomEditors.AsmEditor;
using Spect.Net.VsPackage.ProjectStructure;
using Spect.Net.VsPackage.ToolWindows.SpectrumEmulator;

#pragma warning disable VSTHRD010 // Invoke single-threaded types on Main thread

namespace Spect.Net.VsPackage.Z80Programs.Debugging
{
    /// <summary>
    /// This class provides VS-integrated debug information 
    /// </summary>
    public class VsIntegratedSpectrumDebugInfoProvider: VmComponentProviderBase, 
        ISpectrumDebugInfoProvider,
        IDisposable
    {
        // --- Store previous breakpoints for comparison
        private readonly Dictionary<Breakpoint, Dictionary<ushort, IBreakpointInfo>> _previousVsBreakpoints = 
            new Dictionary<Breakpoint, Dictionary<ushort, IBreakpointInfo>>();

        /// <summary>
        /// The owner package
        /// </summary>
        public SpectNetPackage Package => SpectNetPackage.Default;

        /// <summary>
        /// Contains the compiled output, provided the compilation was successful
        /// </summary>
        public AssemblerOutput CompiledOutput { get; set; }

        /// <summary>
        /// Stores the taggers so that their view could be notified about
        /// breakpoint changed
        /// </summary>
        internal Dictionary<string, Z80DebugTokenTagger> Z80AsmTaggers;

        /// <summary>
        /// The name of the file with the current breakpoint
        /// </summary>
        public string CurrentBreakpointFile { get; private set; }

        /// <summary>
        /// The line number within the current breakpoint file
        /// </summary>
        public int CurrentBreakpointLine { get; private set; }

        /// <summary>
        /// Registers a new tagger
        /// </summary>
        /// <param name="document">Owner document</param>
        /// <param name="tagger">Tagger instance</param>
        public void RegisterTagger(string document, Z80DebugTokenTagger tagger)
        {
            Z80AsmTaggers[document] = tagger;
        }

        /// <summary>
        /// Removes a registered tagger
        /// </summary>
        /// <param name="document">Owner document</param>
        public void UnregisterTagger(string document)
        {
            Z80AsmTaggers.Remove(document);
        }

        /// <summary>
        /// Clears the provider
        /// </summary>
        public void Clear()
        {
            Breakpoints.Clear();
            Z80AsmTaggers.Clear();
        }

        /// <summary>
        /// The currently defined breakpoints
        /// </summary>
        public BreakpointCollection Breakpoints { get; set; }

        /// <summary>
        /// Gets or sets an imminent breakpoint
        /// </summary>
        public ushort? ImminentBreakpoint { get; set; }

        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public VsIntegratedSpectrumDebugInfoProvider()
        {
            Z80AsmTaggers = new Dictionary<string, Z80DebugTokenTagger>(StringComparer.InvariantCultureIgnoreCase);
            Breakpoints = new BreakpointCollection();
        }

        /// <summary>
        /// Prepares the machine the first time the Spectrum virtual machine is set up
        /// </summary>
        public void Prepare()
        {
            SpectNetPackage.Default.MachineViewModel.VmStateChanged += MachineViewModelOnVmStateChanged;
            Package.CodeDiscoverySolution.CurrentProject.ProjectItemRenamed += OnProjectItemRenamed;
        }

        /// <summary>
        /// Us this method to prepare the breakpoints when running the
        /// virtual machine in debug mode
        /// </summary>
        public void PrepareBreakpoints()
        {
            // --- No compiled code, no VS breakpoints to merge
            if (CompiledOutput == null)
            {
                return;
            }

            var currentVsBreakpoints = new HashSet<Breakpoint>();
            var newVsBreakpoints = new HashSet<Breakpoint>();

            // --- Identify new breakpoints
            foreach (Breakpoint bp in Package.ApplicationObject.Debugger.Breakpoints)
            {
                if (!_previousVsBreakpoints.ContainsKey(bp))
                {
                    newVsBreakpoints.Add(bp);
                }
                currentVsBreakpoints.Add(bp);
            }

            var oldBreakpoints = new HashSet<Breakpoint>();

            // --- Identify breakpoints to remove
            foreach (var bp in _previousVsBreakpoints.Keys)
            {
                if (!currentVsBreakpoints.Contains(bp))
                {
                    oldBreakpoints.Add(bp);
                }
            }

            // --- In there any change?
            if (newVsBreakpoints.Count == 0 && oldBreakpoints.Count == 0)
            {
                // --- No change, use existing breakpoints
                return;
            }

            // --- Remove old breakpoints
            foreach (var oldBp in oldBreakpoints)
            {
                _previousVsBreakpoints.Remove(oldBp);
            }

            // --- Start assembling the new breakpoint collection
            var newBreakpointCollection = new BreakpointCollection();

            // --- Keep CPU breakpoints set through the Disassembler tool
            foreach (var pair in Breakpoints.Where(bp => bp.Value.IsCpuBreakpoint))
            {
                newBreakpointCollection.Add(pair.Key, pair.Value);
            }

            // --- Add existing VS breakpoints
            foreach (var existingBp in _previousVsBreakpoints.Values)
            {
                foreach (var bp in existingBp)
                {
                    newBreakpointCollection.Add(bp.Key, bp.Value);
                }
            }
            
            // --- Create new breakpoints
            foreach (var newBp in newVsBreakpoints)
            {
                // --- Check for the file
                var fileIndex = -1;
                for (var i = 0; i < CompiledOutput.SourceFileList.Count; i++)
                {
                    if (string.Compare(newBp.File, CompiledOutput.SourceFileList[i].Filename,
                            StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        fileIndex = i;
                        break;
                    }
                }
                if (fileIndex < 0) continue;

                var newVsBreakpoint = new BreakpointInfo
                {
                    File = CompiledOutput.SourceFileList[fileIndex].Filename,
                    FileLine = newBp.FileLine,
                    HitType = BreakpointHitType.None
                };

                // --- Check the breakpoint address
                if (CompiledOutput.AddressMap.TryGetValue((fileIndex, newBp.FileLine), out var addresses))
                {
                    foreach (var addr in addresses)
                    {
                        // --- Set up breakpoints

                        newBreakpointCollection.Add(addr, newVsBreakpoint);
                    }
                }

            }

            // --- Set the new collection of breakpoints
            Breakpoints = newBreakpointCollection;
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
        {
            return Breakpoints.ContainsKey(address);
        }

        /// <summary>
        /// Responds to virtual machine state changes
        /// </summary>
        private async void MachineViewModelOnVmStateChanged(object sender, VmStateChangedEventArgs args)
        {
            if (args.NewState == VmState.Running || args.NewState == VmState.Stopped)
            {
                // --- Remove the highlight from the last breakpoint
                var prevFile = CurrentBreakpointFile;
                var prevLine = CurrentBreakpointLine;

                // --- Remove current breakpoint information
                CurrentBreakpointFile = null;
                CurrentBreakpointLine = -1;
                UpdateBreakpointVisuals(prevFile, prevLine, false);
            }
            if (args.NewState == VmState.Paused
                && Package.MachineViewModel.RunsInDebugMode
                && !Package.Options.DisableSourceNavigation)
            {
                // --- Set up breakpoint information
                var address = Package.MachineViewModel.SpectrumVm.Cpu.Registers.PC;
                if (CompiledOutput?.SourceMap != null
                    && CompiledOutput.SourceMap.TryGetValue(address, out var fileInfo))
                {
                    // --- Add highlight to the current source code line
                    CurrentBreakpointFile = CompiledOutput
                        .SourceFileList[fileInfo.FileIndex].Filename;
                    CurrentBreakpointLine = fileInfo.Line - 1;
                    Package.ApplicationObject.Documents.Open(CurrentBreakpointFile);
                    Package.ShowToolWindow<SpectrumEmulatorToolWindow>();
                    await Task.Delay(10);
                    UpdateBreakpointVisuals(CurrentBreakpointFile, CurrentBreakpointLine, true);
                }
            }
        }

        /// <summary>
        /// Updates the visual properties
        /// </summary>
        /// <param name="breakpointFile">File with breakpoint</param>
        /// <param name="breakpointLine">Breakpoint line</param>
        /// <param name="isCurrent">Indicates if the line to mark is the current breakpoint line</param>
        private void UpdateBreakpointVisuals(string breakpointFile, int breakpointLine, bool isCurrent)
        {
            if (breakpointFile == null) return;
            if (Z80AsmTaggers.TryGetValue(breakpointFile, out var tagger))
            {
                tagger.UpdateLine(breakpointLine, isCurrent);
            }
        }

        /// <summary>
        /// Responds to the event when an item has been renamed
        /// </summary>
        private void OnProjectItemRenamed(object sender, ProjectItemRenamedEventArgs args)
        {
            // --- Let's change tagger names
            if (Z80AsmTaggers.TryGetValue(args.OldName, out var tagger))
            {
                Z80AsmTaggers.Remove(args.OldName);
                Z80AsmTaggers[args.NewName] = tagger;
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or 
        /// resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            SpectNetPackage.Default.MachineViewModel.VmStateChanged -= MachineViewModelOnVmStateChanged;
            Package.CodeDiscoverySolution.CurrentProject.ProjectItemRenamed -= OnProjectItemRenamed;
        }
    }
}

#pragma warning restore VSTHRD010 // Invoke single-threaded types on Main thread
