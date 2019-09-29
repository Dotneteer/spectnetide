using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Antlr4.Runtime;
using EnvDTE;
using Spect.Net.Assembler.Assembler;
using Spect.Net.EvalParser;
using Spect.Net.EvalParser.Generated;
using Spect.Net.EvalParser.SyntaxTree;
using Spect.Net.SpectrumEmu.Abstraction.Machine;
using Spect.Net.SpectrumEmu.Abstraction.Providers;
using Spect.Net.SpectrumEmu.Machine;
using Spect.Net.VsPackage.LanguageServices.Z80Asm;
using Spect.Net.VsPackage.ToolWindows.SpectrumEmulator;

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
        public BreakpointCollection Breakpoints { get; set; }

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
        /// Prepares this object for debugging
        /// </summary>
        public void Prepare()
        {
            SpectNetPackage.Default.EmulatorViewModel.VmStateChanged += OnVmStateChanged;
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
            // --- No compiled code, no VS breakpoints to merge
            if (CompiledOutput == null)
            {
                return;
            }

            var currentVsBreakpoints = new HashSet<Breakpoint>();
            var newVsBreakpoints = new HashSet<Breakpoint>();

            // --- Identify new breakpoints
            foreach (Breakpoint bp in SpectNetPackage.Default.ApplicationObject.Debugger.Breakpoints)
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

                // --- Set hit condition
                if (newBp.HitCountType != dbgHitCountType.dbgHitCountTypeNone)
                {
                    if (newBp.HitCountType == dbgHitCountType.dbgHitCountTypeEqual)
                    {
                        newVsBreakpoint.HitType = BreakpointHitType.Equal;
                    }
                    else if (newBp.HitCountType == dbgHitCountType.dbgHitCountTypeGreaterOrEqual)
                    {
                        newVsBreakpoint.HitType = BreakpointHitType.GreaterOrEqual;
                    }
                    else if (newBp.HitCountType == dbgHitCountType.dbgHitCountTypeMultiple)
                    {
                        newVsBreakpoint.HitType = BreakpointHitType.Multiple;
                    }
                    newVsBreakpoint.HitConditionValue = (ushort)(newBp.HitCountTarget >= 0 ? newBp.HitCountTarget : 0);
                }

                // --- Set filter condition
                if (!string.IsNullOrWhiteSpace(newBp.Condition))
                {
                    var inputStream = new AntlrInputStream(newBp.Condition);
                    var lexer = new Z80EvalLexer(inputStream);
                    var tokenStream = new CommonTokenStream(lexer);
                    var evalParser = new Z80EvalParser(tokenStream);
                    var context = evalParser.compileUnit();
                    var visitor = new Z80EvalVisitor();
                    var z80Expr = (Z80ExpressionNode)visitor.Visit(context);
                    if (evalParser.SyntaxErrors.Count == 0)
                    {
                        newVsBreakpoint.FilterExpression = z80Expr.Expression;
                    }
                }

                // --- Check the breakpoint address
                if (CompiledOutput.AddressMap.TryGetValue((fileIndex, newBp.FileLine), out var addresses))
                {
                    foreach (var addr in addresses)
                    {
                        // --- Set up breakpoints

                        newBreakpointCollection.Add(addr, newVsBreakpoint);
                    }
                    _previousVsBreakpoints.Add(newBp, newBreakpointCollection);
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
            => Breakpoints.ContainsKey(address);

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or
        /// resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            SpectNetPackage.Default.EmulatorViewModel.VmStateChanged -= OnVmStateChanged;
        }

        private async void OnVmStateChanged(object sender, VmStateChangedEventArgs args)
        {
            if (args.NewState == VmState.Running || args.NewState == VmState.Stopped)
            {
                // --- Remove current breakpoint information
                CurrentBreakpointFile = null;
                CurrentBreakpointLine = -1;
            }

            var package = SpectNetPackage.Default;
            if (args.NewState == VmState.Paused
                && package.EmulatorViewModel.Machine.RunsInDebugMode
                && !package.Options.DisableSourceNavigation)
            {
                // --- Set up breakpoint information
                var address = package.EmulatorViewModel.Machine.SpectrumVm.Cpu.Registers.PC;
                if (CompiledOutput?.SourceMap != null
                    && CompiledOutput.SourceMap.TryGetValue(address, out var fileInfo))
                {
                    // --- Add highlight to the current source code line
                    CurrentBreakpointFile = CompiledOutput
                        .SourceFileList[fileInfo.FileIndex].Filename;
                    CurrentBreakpointLine = fileInfo.Line;
                    package.ApplicationObject.Documents.Open(CurrentBreakpointFile);
                    package.ShowToolWindow<SpectrumEmulatorToolWindow>();
                    await Task.Delay(10);
                }
            }
        }
    }
}