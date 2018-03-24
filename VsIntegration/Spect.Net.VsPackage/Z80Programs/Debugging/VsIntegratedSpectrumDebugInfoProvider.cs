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

namespace Spect.Net.VsPackage.Z80Programs.Debugging
{
    /// <summary>
    /// This class provides VS-integrated debug information 
    /// </summary>
    public class VsIntegratedSpectrumDebugInfoProvider: VmComponentProviderBase, 
        ISpectrumDebugInfoProvider,
        IDisposable
    {
        private bool _boundToMachine;

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
        public BreakpointCollection Breakpoints { get; }

        /// <summary>
        /// Gets or sets an imminent breakpoint
        /// </summary>
        public ushort? ImminentBreakpoint { get; set; }

        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public VsIntegratedSpectrumDebugInfoProvider()
        {
            Z80AsmTaggers = new Dictionary<string, Z80DebugTokenTagger>(StringComparer.InvariantCultureIgnoreCase);
            Breakpoints = new BreakpointCollection();
            _boundToMachine = false;
        }

        /// <summary>
        /// Prepeares the machine the first time the Spectrum virtual machine is set up
        /// </summary>
        public void Prepare()
        {
            if (!_boundToMachine)
            {
                _boundToMachine = true;
                SpectNetPackage.Default.MachineViewModel.VmStateChanged += MachineViewModelOnVmStateChanged;
                Package.CodeDiscoverySolution.CurrentProject.ProjectItemRenamed += OnProjectItemRenamed;
            }
        }

        /// <summary>
        /// Us this method to prepare the breakpoints when running the
        /// virtual machine in debug mode
        /// </summary>
        public void PrepareBreakpoints()
        {
            // --- Keep CPU breakpoints set through the Disassembler tool
            var cpuBreakPoints = Breakpoints.Where(bp => bp.Value.IsCpuBreakpoint).ToList();
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

        /// <summary>
        /// Updates the layout of the specified document file
        /// </summary>
        /// <param name="filename">Document file to update</param>
        public void UpdateLayoutWithDebugInfo(string filename)
        {
            if (Z80AsmTaggers.TryGetValue(filename, out var tagger))
            {
                tagger.UpdateLayout();
            }
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
            if (_boundToMachine)
            {
                SpectNetPackage.Default.MachineViewModel.VmStateChanged -= MachineViewModelOnVmStateChanged;
                Package.CodeDiscoverySolution.CurrentProject.ProjectItemRenamed -= OnProjectItemRenamed;
            }
        }
    }
}