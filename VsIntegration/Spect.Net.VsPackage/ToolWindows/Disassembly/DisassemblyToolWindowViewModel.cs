using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Antlr4.Runtime;
using Spect.Net.CommandParser.SyntaxTree;
using Spect.Net.EvalParser;
using Spect.Net.EvalParser.Generated;
using Spect.Net.EvalParser.SyntaxTree;
using Spect.Net.SpectrumEmu.Disassembler;
using Spect.Net.SpectrumEmu.Machine;
using Spect.Net.VsPackage.Z80Programs.Debugging;

// ReSharper disable IdentifierTypo

// ReSharper disable AssignNullToNotNullAttribute

// ReSharper disable ExplicitCallerInfoArgument

namespace Spect.Net.VsPackage.ToolWindows.Disassembly
{
    public class DisassemblyToolWindowViewModel: BankAwareToolWindowViewModelBase, IDisassemblyItemParent
    {
        private bool _tapeDeviceAttached;
        private ObservableCollection<DisassemblyItemViewModel> _disassemblyItems;

        /// <summary>
        /// The disassembly items belonging to this project
        /// </summary>
        public ObservableCollection<DisassemblyItemViewModel> DisassemblyItems
        {
            get => _disassemblyItems;
            private set => Set(ref _disassemblyItems, value);
        }

        /// <summary>
        /// Gets the line index
        /// </summary>
        public Dictionary<ushort, int> LineIndexes { get; private set; }

        /// <summary>
        /// We can keep track of the top address
        /// </summary>
        public ushort? TopAddress { get; set; }

        /// <summary>
        /// This event is raised when the disassembly view is refreshed.
        /// </summary>
        public event EventHandler<DisassemblyViewRefreshedEventArgs> DisassemblyViewRefreshed;

        #region Lifecycle methods

        /// <summary>
        /// Initializes a new instance of the ViewModelBase class.
        /// </summary>
        public DisassemblyToolWindowViewModel()
        {
            if (IsInDesignMode)
            {
                FullViewMode = true;
                DisassemblyItems = new ObservableCollection<DisassemblyItemViewModel>
                {
                    new DisassemblyItemViewModel(),
                    new DisassemblyItemViewModel(),
                    new DisassemblyItemViewModel(),
                    new DisassemblyItemViewModel()
                };
                return;
            }

            _tapeDeviceAttached = false;
            Package.CodeManager.CodeInjected += OnVmCodeInjected;
            Package.CodeManager.AnnotationFileChanged += OnAnnotationFileChanged;
            if (VmRuns)
            {
                AttachToTapeDeviceEvents();
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, 
        /// or resetting unmanaged resources.
        /// </summary>
        public override void Dispose()
        {
            Package.CodeManager.CodeInjected -= OnVmCodeInjected;
            Package.CodeManager.AnnotationFileChanged -= OnAnnotationFileChanged;
            base.Dispose();
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Disassembles the current virtual machine's memory
        /// </summary>
        public void Disassemble()
        {
            if (!Disassemble(out var disItems, out var lineIndexes)) return;

            DisassemblyItems = disItems;
            LineIndexes = lineIndexes;
        }

        /// <summary>
        /// Clears the disassembled items
        /// </summary>
        public void Clear()
        {
            DisassemblyItems.Clear();
        }

        /// <summary>
        /// Processes the command text
        /// </summary>
        /// <param name="commandText">The command text</param>
        /// <param name="validationMessage">
        /// Null, if the command is valid; otherwise the validation message to show
        /// </param>
        /// <param name="newPrompt">
        /// New prompt command prompt text, or null, if no text should be set
        /// </param>
        /// <returns>
        /// True, if the command has been handled; otherwise, false
        /// </returns>
        public bool ProcessCommandline(string commandText, out string validationMessage,
            out string newPrompt)
        {
            // --- Prepare command handling
            validationMessage = null;
            newPrompt = null;
            ushort? address = null;
            var breakPoints = MachineViewModel.SpectrumVm.DebugInfoProvider.Breakpoints;
            var isSpectrum48 = SpectNetPackage.IsSpectrum48Model();
            var banks = MachineViewModel.SpectrumVm.MemoryConfiguration.RamBanks;
            var roms = MachineViewModel.SpectrumVm.RomConfiguration.NumberOfRoms;

            var command = ParseCommand(commandText);
            if (command is CompactToolCommand compactCommand)
            {
                command = ParseCommand(compactCommand.CommandText);
            }
            if (command == null || command.HasSemanticError)
            {
                validationMessage = INV_SYNTAX;
                return false;
            }

            switch (command)
            {
                case GotoToolCommand gotoCommand:
                {
                    if (!ObtainAddress(gotoCommand.Address, gotoCommand.Symbol, out var resolvedAddress, out validationMessage))
                    {
                        return false;
                    }
                    address = resolvedAddress;
                    break;
                }

                case LabelToolCommand labelCommand:
                {
                    if (!ObtainAddress(labelCommand.Address, null, out var resolvedAddress,
                        out validationMessage))
                    {
                        return false;
                    }
                    AnnotationHandler.SetLabel(resolvedAddress, labelCommand.Symbol, out validationMessage);
                    if (validationMessage != null)
                    {
                        newPrompt = commandText;
                        return false;
                    }
                    break;
                }

                case CommentToolCommand commentCommand:
                {
                    if (!ObtainAddress(commentCommand.Address, commentCommand.Symbol, out var resolvedAddress,
                        out validationMessage))
                    {
                        return false;
                    }
                    AnnotationHandler.SetComment(resolvedAddress, commentCommand.Text);
                    break;
                }

                case PrefixCommentToolCommand prefixCommentCommand:
                {
                    if (!ObtainAddress(prefixCommentCommand.Address, prefixCommentCommand.Symbol, out var resolvedAddress,
                        out validationMessage))
                    {
                        return false;
                    }
                    AnnotationHandler.SetPrefixComment(resolvedAddress, prefixCommentCommand.Text);
                    break;
                }

                case RetrieveToolCommand retrieveCommand:
                {
                    if (!ObtainAddress(retrieveCommand.Address, retrieveCommand.Symbol, out var resolvedAddress,
                        out validationMessage))
                    {
                        return false;
                    }
                    newPrompt = RetrieveAnnotation(resolvedAddress, retrieveCommand.Type);
                    return false;
                }

                case LiteralToolCommand literalCommand:
                {
                    if (!ObtainAddress(literalCommand.Address, literalCommand.Symbol, out var resolvedAddress,
                        out validationMessage))
                    {
                        return false;
                    }
                    var handled = ApplyLiteral(resolvedAddress, 
                        literalCommand.IsAuto ? "#" : literalCommand.LiteralName,
                        out validationMessage, out newPrompt);
                    if (!handled) return false;
                    break;
                }

                case SectionToolCommand sectionCommand:
                {
                    if (!ObtainAddress(sectionCommand.StartAddress, sectionCommand.StartSymbol, out var startAddress,
                        out validationMessage))
                    {
                        return false;
                    }
                    if (!ObtainAddress(sectionCommand.EndAddress, sectionCommand.EndSymbol, out var endAddress,
                        out validationMessage))
                    {
                        return false;
                    }
                    AddSection(startAddress, endAddress, sectionCommand.Type);
                    Disassemble();
                    break;
                }

                case SetBreakpointToolCommand setBreakpointCommand:
                {
                    if (!ObtainAddress(setBreakpointCommand.Address, setBreakpointCommand.Symbol, out var resolvedAddress,
                        out validationMessage))
                    {
                        return false;
                    }

                    if (!PrepareBreakpointCondition(setBreakpointCommand.Condition,
                        setBreakpointCommand.HitConditionType,
                        setBreakpointCommand.HitConditionValue,
                        ref validationMessage, out var breakPoint))
                    {
                        return false;
                    }
                    breakPoints[resolvedAddress] = breakPoint;
                    break;
                }

                case ToggleBreakpointToolCommand toggleBreakpointCommand:
                {
                    if (!ObtainAddress(toggleBreakpointCommand.Address, toggleBreakpointCommand.Symbol,
                        out var resolvedAddress,
                        out validationMessage))
                    {
                        return false;
                    }

                    if (!breakPoints.ContainsKey(resolvedAddress))
                    {
                        if (!PrepareBreakpointCondition(null,
                            null,
                            0,
                            ref validationMessage, out var breakPoint))
                        {
                            return false;
                        }

                        breakPoints[resolvedAddress] = breakPoint;
                    }
                    else
                    {
                        breakPoints.Remove(resolvedAddress);
                    }
                    break;
                }

                case RemoveBreakpointToolCommand removeBreakpointCommand:
                {
                    if (!ObtainAddress(removeBreakpointCommand.Address, removeBreakpointCommand.Symbol,
                        out var resolvedAddress,
                        out validationMessage))
                    {
                        return false;
                    }
                    breakPoints.Remove(resolvedAddress);
                    break;
                }

                case UpdateBreakpointToolCommand updateBreakpointCommand:
                {
                    if (!ObtainAddress(updateBreakpointCommand.Address, updateBreakpointCommand.Symbol,
                        out var resolvedAddress,
                        out validationMessage))
                    {
                        return false;
                    }
                    newPrompt = RetrieveBreakpoint(resolvedAddress);
                    return false;
                }

                case EraseAllBreakpointsToolCommand _:
                    var keysToRemove = breakPoints.Keys.Where(k => breakPoints[k].IsCpuBreakpoint).ToArray();
                    foreach (var key in keysToRemove)
                    {
                        breakPoints.Remove(key);
                    }
                    break;

                case RomPageToolCommand romPageCommand:
                    if (isSpectrum48)
                    {
                        validationMessage = INV_S48_COMMAND;
                        return false;
                    }
                    if (romPageCommand.Page > roms - 1)
                    {
                        validationMessage = $"This machine does not have a ROM bank #{romPageCommand.Page}";
                        return false;
                    }
                    SetRomViewMode(romPageCommand.Page);
                    address = 0;
                    break;

                case BankPageToolCommand bankPageCommand:
                    if (isSpectrum48)
                    {
                        validationMessage = INV_S48_COMMAND;
                        return false;
                    }
                    if (VmStopped)
                    {
                        validationMessage = INV_RUN_COMMAND;
                        return false;
                    }
                    if (bankPageCommand.Page > banks - 1)
                    {
                        validationMessage = $"This machine does not have a RAM bank #{bankPageCommand.Page}";
                        return false;

                    }
                    SetRamBankViewMode(bankPageCommand.Page);
                    address = 0;
                    break;

                case MemoryModeToolCommand _:
                    if (isSpectrum48)
                    {
                        validationMessage = INV_S48_COMMAND;
                        return false;
                    }
                    if (VmStopped)
                    {
                        validationMessage = INV_RUN_COMMAND;
                        return false;
                    }
                    SetFullViewMode();
                    break;

                case DisassemblyTypeToolCommand disassemblyTypeCommand:
                    if (isSpectrum48)
                    {
                        validationMessage = INV_S48_COMMAND;
                        return false;
                    }
                    if (FullViewMode)
                    {
                        validationMessage = "This command can be used only in ROM or RAM bank view mode";
                        return false;
                    }

                    var type = disassemblyTypeCommand.Type.ToUpper();
                    if (type != "48" && type != "128" && type != "P3" && type != "NEXT")
                    {
                        validationMessage = "Disassembly type should be one of these: '48', '128', 'P3', or 'NEXT'";
                        return false;
                    }
                    SetDisassemblyType(disassemblyTypeCommand.Type);
                    break;

                case ReDisassemblyToolCommand _:
                    Disassemble();
                    if (TopAddress.HasValue)
                    {
                        address = TopAddress.Value;
                    }

                    break;

                case JumpToolCommand jumpCommand:
                {
                    if (MachineViewModel.MachineState != VmState.Paused)
                    {
                        validationMessage = "The 'J' command can be used only when the virtual machine is paused.";
                        return false;
                    }
                    if (!ObtainAddress(jumpCommand.Address, jumpCommand.Symbol,
                        out var resolvedAddress,
                        out validationMessage))
                    {
                        return false;
                    }

                    address = MachineViewModel.SpectrumVm.Cpu.Registers.PC = resolvedAddress;
                    break;
                }

                default:
                    validationMessage = string.Format(INV_CONTEXT, "Z80 Disassembly window");
                    return false;
            }

            DisassemblyViewRefreshed?.Invoke(this, new DisassemblyViewRefreshedEventArgs(address));
            return true;
        }

        /// <summary>
        /// Toggles the breakpoint represented by the specified item
        /// </summary>
        /// <param name="item">Item to toggle the breakpoint for</param>
        public void ToggleBreakpoint(DisassemblyItemViewModel item)
        {
            if (item == null) return;

            var breakPoints = MachineViewModel.SpectrumVm.DebugInfoProvider.Breakpoints;
            var address = item.Item.Address;
            if (breakPoints.ContainsKey(address))
            {
                breakPoints.Remove(address);
            }
            else
            {
                breakPoints[address] = new BreakpointInfo
                {
                    IsCpuBreakpoint = true,
                };
            }
        }

        #endregion

        #region Overridden methods

        /// <summary>
        /// When the view model is first time created, use the ROM view
        /// </summary>
        protected override void Initialize()
        {
            InitDisassembly();
            base.Initialize();
            if (VmNotStopped)
            {
                InitViewMode();
            }
        }

        /// <summary>
        /// Obtain the machine view model from the solution
        /// </summary>
        protected override void OnSolutionOpened()
        {
            base.OnSolutionOpened();
            InitDisassembly();
        }

        /// <summary>
        /// Override this method to init the current view mode
        /// </summary>
        public override void InitViewMode()
        {
            Disassemble();
        }

        /// <summary>
        /// Override this method to define how to refresh the view 
        /// when the virtual machine is paused
        /// </summary>
        public override void RefreshOnPause()
        {
            DisassemblyViewRefreshed?.Invoke(this, 
                new DisassemblyViewRefreshedEventArgs(MachineViewModel.SpectrumVm.Cpu.Registers.PC));
        }

        /// <summary>
        /// Override this method to refresh the current view mode
        /// </summary>
        public override void RefreshViewMode()
        {
            DisassemblyViewRefreshed?.Invoke(this,
                new DisassemblyViewRefreshedEventArgs());
        }

        /// <summary>
        /// Set the tool window into Full view mode on first start
        /// </summary>
        protected override void OnFirstStart()
        {
            base.OnFirstStart();
            DisassemblyViewRefreshed?.Invoke(this,
                new DisassemblyViewRefreshedEventArgs(MachineViewModel.DisAssViewPoint));
        }

        /// <summary>
        /// Set the tool window into Full view mode on first start
        /// </summary>
        protected override void OnStart()
        {
            base.OnStart();
            AttachToTapeDeviceEvents();
        }

        /// <summary>
        /// Set the ROM view mode when the machine is stopped
        /// </summary>
        protected override void OnStopped()
        {
            base.OnStopped();
            DetachFromTapeDeviceEvents();
            DisassemblyViewRefreshed?.Invoke(this,
                new DisassemblyViewRefreshedEventArgs(0));
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Force a new disassembly
        /// </summary>
        private void OnVmCodeInjected(object sender, EventArgs eventArgs)
        {
            Disassemble();
            DisassemblyViewRefreshed?.Invoke(this, new DisassemblyViewRefreshedEventArgs(TopAddress));
        }

        /// <summary>
        /// Handle the change of the annotation file
        /// </summary>
        private void OnAnnotationFileChanged(object sender, EventArgs eventArgs)
        {
            InitDisassembly();
            Disassemble();
            DisassemblyViewRefreshed?.Invoke(this, new DisassemblyViewRefreshedEventArgs(TopAddress));
        }

        /// <summary>
        /// Creates a disassembler for the current machine
        /// </summary>
        /// <returns></returns>
        private Z80Disassembler CreateDisassembler()
        {
            var memoryDevice = MachineViewModel.SpectrumVm.MemoryDevice;
            var memoryConfig = MachineViewModel.SpectrumVm.MemoryConfiguration;
            var disassemblyFlags = new Dictionary<int, SpectrumSpecificDisassemblyFlags>();

            // --- Create map for rom annotations
            var map = new MemoryMap();

            byte[] memory;
            if (RomViewMode)
            {
                if (AnnotationHandler.RomPageAnnotations.TryGetValue(RomIndex, out var romAnn))
                {
                    map = romAnn.MemoryMap;
                    disassemblyFlags[0] = romAnn.DisassemblyFlags;
                }
                memory = memoryDevice.GetRomBuffer(RomIndex);
            }
            else if (RamBankViewMode)
            {
                // --- Create map for the visible bank
                var ramAnn = GetRamBankAnnotation(RamBankIndex);
                map = ramAnn.MemoryMap;
                memory = memoryDevice.GetRamBank(RamBankIndex);
                disassemblyFlags[0] = ramAnn.DisassemblyFlags;
            }
            else
            {
                // --- We are in FullViewMode
                var currentRom = memoryDevice.GetSelectedRomIndex();
                if (AnnotationHandler.RomPageAnnotations.TryGetValue(currentRom, out var fullAnn))
                {
                    map = fullAnn.MemoryMap;
                    disassemblyFlags[0] = fullAnn.DisassemblyFlags;
                }
                else
                {
                    disassemblyFlags[0] = SpectrumSpecificDisassemblyFlags.None;
                }

                // --- Merge the maps of the paged banks with the ROM maps  
                if (memoryConfig.SupportsBanking)
                {
                    for (var i = 1; i <= 3; i++)
                    {
                        if (AnnotationHandler.RamBankAnnotations.TryGetValue(
                            memoryDevice.GetSelectedBankIndex(i), out var ramAnn))
                        {
                            map.Merge(ramAnn.MemoryMap, (ushort)(i * 0x4000));
                            disassemblyFlags[i] = ramAnn.DisassemblyFlags;
                        }
                        else
                        {
                            map.Merge(new MemoryMap
                            {
                                new MemorySection(0, 
                                    0x3FFF, 
                                    MemorySectionType.ByteArray)
                            }, (ushort)(i * 0x4000));
                        }
                    }
                }
                else
                {
                    // --- No banking supported
                    if (AnnotationHandler.RamBankAnnotations.TryGetValue(0, out var ramAnn))
                    {
                        map.Merge(ramAnn.MemoryMap, 0x4000);
                        disassemblyFlags[0] = ramAnn.DisassemblyFlags;
                    }
                    else
                    {
                        map.Merge(new MemoryMap
                        {
                            new MemorySection(0x4000, 0xFFFF, MemorySectionType.ByteArray)
                        });
                    }
                }
                memory = memoryDevice.CloneMemory();
            }

            var disassembler = new Z80Disassembler(map, 
                memory, 
                disassemblyFlags,
                MachineViewModel.SpectrumVm.Cpu.AllowExtendedInstructionSet);
            return disassembler;
        }

        /// <summary>
        /// Initializes disassembly items and annotations
        /// </summary>
        private void InitDisassembly()
        {
            DisassemblyItems = new ObservableCollection<DisassemblyItemViewModel>();
            LineIndexes = new Dictionary<ushort, int>();
            AnnotationHandler = new DisassemblyAnnotationHandler(this);
        }

        /// <summary>
        /// Retrieves labels, comments, or prefix comments for modification.
        /// </summary>
        /// <param name="address">Address to retrieve data from</param>
        /// <param name="dataType">Type of data to retrieve</param>
        private string RetrieveAnnotation(ushort address, string dataType)
        {
            var annotation = GetAnnotationFor(address, out var annAddr);
            dataType = dataType.ToUpper();
            var found = false;
            var commandtext = $"{dataType} {address:X4} ";
            var text = string.Empty;
            switch (dataType)
            {
                case "L":
                    found = annotation.Labels.TryGetValue(annAddr, out text);
                    break;
                case "C":
                    found = annotation.Comments.TryGetValue(annAddr, out text);
                    break;
                case "P":
                    found = annotation.PrefixComments.TryGetValue(annAddr, out text);
                    break;
            }
            return found ? commandtext + text : string.Empty;
        }

        /// <summary>
        /// Obtains the command line to set the breakpoint at the specified address
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        private string RetrieveBreakpoint(ushort address)
        {
            var breakPoints = MachineViewModel.SpectrumVm.DebugInfoProvider.Breakpoints;
            if (!breakPoints.TryGetValue(address, out var bp))
            {
                return string.Empty;
            }

            // --- Get hit condition
            string hitCondition = "";
            switch (bp.HitType)
            {
                case BreakpointHitType.Less:
                    hitCondition = $" H<{bp.HitConditionValue}";
                    break;
                case BreakpointHitType.LessOrEqual:
                    hitCondition = $" H<={bp.HitConditionValue}";
                    break;
                case BreakpointHitType.Equal:
                    hitCondition = $" H={bp.HitConditionValue}";
                    break;
                case BreakpointHitType.Greater:
                    hitCondition = $" H>{bp.HitConditionValue}";
                    break;
                case BreakpointHitType.GreaterOrEqual:
                    hitCondition = $" H>={bp.HitConditionValue}";
                    break;
                case BreakpointHitType.Multiple:
                    hitCondition = $" H*{bp.HitConditionValue}";
                    break;
            }

            // --- Get filter condition
            var filterCondition = bp.FilterExpression == null
                ? ""
                : $" C {bp.FilterExpression}";
            return $"SB {address:X4}{hitCondition}{filterCondition}";
        }

        /// <summary>
        /// Sets the disassembly type
        /// </summary>
        /// <param name="modelType"></param>
        private void SetDisassemblyType(string modelType)
        {
            DisassemblyAnnotation annotation;
            if (RomViewMode)
            {
                annotation = AnnotationHandler.RomPageAnnotations.TryGetValue(RomIndex, out var romAnn) 
                    ? romAnn : new DisassemblyAnnotation();
            }
            else
            {
                annotation = AnnotationHandler.RamBankAnnotations.TryGetValue(RamBankIndex, out var romAnn)
                    ? romAnn : new DisassemblyAnnotation();
            }
            switch (modelType)
            {
                case "48":
                    annotation.SetDisassemblyFlag(SpectrumSpecificDisassemblyFlags.Spectrum48);
                    break;
                case "128":
                    annotation.SetDisassemblyFlag(SpectrumSpecificDisassemblyFlags.Spectrum128);
                    break;
                default:
                    annotation.SetDisassemblyFlag(SpectrumSpecificDisassemblyFlags.None);
                    break;
            }
            AnnotationHandler.SaveAnnotations(annotation, 0x0000);
        }

        /// <summary>
        /// Retrieves labels, comments, or prefix comments for modification.
        /// </summary>
        /// <param name="address">Address to retrieve data from</param>
        /// <param name="literalName">Type of data to retrieve</param>
        /// <param name="validationMessage">
        /// Null, if the command is valid; otherwise the validation message to show
        /// </param>
        /// <param name="newPrompt">
        /// New prompt command prompt text, or null, if no text should be set
        /// </param>
        /// <returns>
        /// True, if the command has been handled; otherwise, false
        /// </returns>
        private bool ApplyLiteral(ushort address, string literalName, out string validationMessage,
            out string newPrompt)
        {
            validationMessage = null;
            newPrompt = null;
            var message = AnnotationHandler.ApplyLiteral(address, literalName, LineIndexes, DisassemblyItems);
            if (message == null) return true;

            if (message.StartsWith("%"))
            {
                var parts = message.Split(new[] { '%' }, StringSplitOptions.RemoveEmptyEntries);
                newPrompt = parts[0];
                message = parts[1];
            }
            validationMessage = message;
            return false;
        }

        /// <summary>
        /// Adds a new memory section to the annotations
        /// </summary>
        /// <param name="startAddress">Start address</param>
        /// <param name="endAddress">End address</param>
        /// <param name="sectionType">Memory section type</param>
        private void AddSection(ushort startAddress, ushort endAddress, string sectionType)
        {
            MemorySectionType type;
            switch (sectionType.ToUpper())
            {
                case "B":
                    type = MemorySectionType.ByteArray;
                    break;
                case "W":
                    type = MemorySectionType.WordArray;
                    break;
                case "S":
                    type = MemorySectionType.Skip;
                    break;
                case "C":
                    type = MemorySectionType.Rst28Calculator;
                    break;
                case "G":
                case "G1":
                    type = MemorySectionType.GraphArray;
                    break;
                case "G2":
                    type = MemorySectionType.GraphArray2;
                    break;
                case "G3":
                    type = MemorySectionType.GraphArray3;
                    break;
                case "G4":
                    type = MemorySectionType.GraphArray4;
                    break;
                default:
                    type = MemorySectionType.Disassemble;
                    break;
            }
            AnnotationHandler.AddSection(startAddress, endAddress, type);
        }

        /// <summary>
        /// Gets and checks the specified address.
        /// </summary>
        /// <param name="addr">Address, if specified with a number</param>
        /// <param name="symbol">Symbol, if specified</param>
        /// <param name="resolvedAddress">Resolved address</param>
        /// <param name="validationMessage">Validation message, in case of error</param>
        /// <returns>True, if the address is successfully resolved; otherwise, false.</returns>
        private bool ObtainAddress(ushort addr, string symbol, out ushort resolvedAddress, out string validationMessage)
        {
            resolvedAddress = addr;
            if (symbol != null)
            {
                if (!ResolveSymbol(symbol, out resolvedAddress))
                {
                    validationMessage = string.Format(UNDEF_SYMBOL, symbol);
                    return false;
                }
            }

            validationMessage = null;
            if ((RomViewMode || RamBankViewMode) && addr > 0x4000)
            {
                validationMessage = $"Address #{addr:X4} out of range";
                return false;
            }
            return true;
        }

        /// <summary>
        /// Whenever the tape device leaves the load mode, re-disassembly the output
        /// </summary>
        private void TapeDeviceOnLeftLoadMode(object sender, EventArgs eventArgs)
        {
#pragma warning disable VSTHRD110 // Observe result of async calls
            Task.Run(() =>
#pragma warning restore VSTHRD110 // Observe result of async calls
            {
                // --- Disassemble in the background
                if (!Disassemble(out var disItems, out var lineIndexes)) return;

                DisassemblyViewRefreshed?.Invoke(this,
                    new DisassemblyViewRefreshedEventArgs(TopAddress,
                        () =>
                        {
                            LineIndexes = lineIndexes;
                            DisassemblyItems = disItems;

                        }));
            });
        }

        /// <summary>
        /// Disassembles the current memory and fills up disassembly items and line indexes
        /// </summary>
        /// <param name="disItems">Disassembly items</param>
        /// <param name="lineIndexes">Line indexes</param>
        /// <returns></returns>
        private bool Disassemble(out ObservableCollection<DisassemblyItemViewModel> disItems, 
            out Dictionary<ushort, int> lineIndexes)
        {
            disItems = new ObservableCollection<DisassemblyItemViewModel>();
            lineIndexes = new Dictionary<ushort, int>();

            if (MachineViewModel.SpectrumVm == null) return false;

            var disassembler = CreateDisassembler();
            var output = disassembler.Disassemble();

            var idx = 0;
            foreach (var item in output.OutputItems)
            {
                disItems.Add(new DisassemblyItemViewModel(this, item));
                lineIndexes.Add(item.Address, idx++);
            }
            return true;
        }

        /// <summary>
        /// Attaches this view model to tape device events
        /// </summary>
        private void AttachToTapeDeviceEvents()
        {
            if (_tapeDeviceAttached) return;

            MachineViewModel.SpectrumVm.TapeDevice.LeftLoadMode += TapeDeviceOnLeftLoadMode;
            _tapeDeviceAttached = true;
        }

        /// <summary>
        /// Detaches this view model from tape device events
        /// </summary>
        private void DetachFromTapeDeviceEvents()
        {
            if (!_tapeDeviceAttached) return;

            MachineViewModel.SpectrumVm.TapeDevice.LeftLoadMode -= TapeDeviceOnLeftLoadMode;
            _tapeDeviceAttached = false;
        }

        /// <summary>
        /// Prepares the WatchItemViewModel instance from the command line
        /// </summary>
        /// <param name="expression">Expression to parse</param>
        /// <param name="hitConditionType">Type of hit condition</param>
        /// <param name="hitConditionValue">Hit condition value</param>
        /// <param name="validationMessage">Validation message to pass</param>
        /// <param name="newItem">The new item</param>
        /// <returns></returns>
        private static bool PrepareBreakpointCondition(string expression, string hitConditionType, 
            ushort hitConditionValue, 
            ref string validationMessage,
            out BreakpointInfo newItem)
        {
            newItem = new BreakpointInfo()
            {
                IsCpuBreakpoint = true,
                FilterCondition = expression,
                HitType = BreakpointHitType.None,
            };

            if (hitConditionType != null)
            {
                newItem.HitConditionValue = hitConditionValue;
                switch (hitConditionType)
                {
                    case "<":
                        newItem.HitType = BreakpointHitType.Less;
                        break;
                    case "<=":
                        newItem.HitType = BreakpointHitType.LessOrEqual;
                        break;
                    case "=":
                        newItem.HitType = BreakpointHitType.Equal;
                        break;
                    case ">":
                        newItem.HitType = BreakpointHitType.Greater;
                        break;
                    case ">=":
                        newItem.HitType = BreakpointHitType.GreaterOrEqual;
                        break;
                    case "*":
                        newItem.HitType = BreakpointHitType.Multiple;
                        break;
                }
            }

            if (string.IsNullOrWhiteSpace(expression))
            {
                return true;
            }

            var inputStream = new AntlrInputStream(expression);
            var lexer = new Z80EvalLexer(inputStream);
            var tokenStream = new CommonTokenStream(lexer);
            var evalParser = new Z80EvalParser(tokenStream);
            var context = evalParser.compileUnit();
            var visitor = new Z80EvalVisitor();
            var z80Expr = (Z80ExpressionNode)visitor.Visit(context);
            if (evalParser.SyntaxErrors.Count > 0)
            {
                validationMessage = "Syntax error in the specified watch expression";
                return false;
            }

            newItem.FilterExpression = z80Expr.Expression;
            return true;
        }


        #endregion

        #region IDisassemblyParent implementation

        /// <summary>
        /// Gets the label for the specified address
        /// </summary>
        /// <param name="address">Address to get the annotation for</param>
        /// <param name="label">Label, if found; otherwise, null</param>
        /// <returns>True, if found; otherwise, false</returns>
        public bool GetLabel(ushort address, out string label)
        {
            label = null;
            var ann = GetAnnotationFor(address, out var annAddr);
            return ann != null && ann.Labels.TryGetValue(annAddr, out label);
        }

        /// <summary>
        /// Gets the comment for the specified address
        /// </summary>
        /// <param name="address">Address to get the annotation for</param>
        /// <param name="comment">Comment, if found; otherwise, null</param>
        /// <returns>True, if found; otherwise, false</returns>
        public bool GetComment(ushort address, out string comment)
        {
            comment = null;
            var ann = GetAnnotationFor(address, out var annAddr);
            return ann != null && ann.Comments.TryGetValue(annAddr, out comment);
        }

        /// <summary>
        /// Gets the prefix comment for the specified address
        /// </summary>
        /// <param name="address">Address to get the annotation for</param>
        /// <param name="comment">Prefix comment, if found; otherwise, null</param>
        /// <returns>True, if found; otherwise, false</returns>
        public bool GetPrefixComment(ushort address, out string comment)
        {
            comment = null;
            var ann = GetAnnotationFor(address, out var annAddr);
            return ann != null && ann.PrefixComments.TryGetValue(annAddr, out comment);
        }

        /// <summary>
        /// Gets the literal replacement for the specified address
        /// </summary>
        /// <param name="address">Address to get the annotation for</param>
        /// <param name="symbol">Symbol, if found; otherwise, null</param>
        /// <returns>True, if found; otherwise, false</returns>
        public bool GetLiteralReplacement(ushort address, out string symbol)
        {
            symbol = null;
            var ann = GetAnnotationFor(address, out var annAddr);
            return ann != null && ann.LiteralReplacements.TryGetValue(annAddr, out symbol);
        }

        /// <summary>
        /// Checks if the specified address has a breakpoint
        /// </summary>
        /// <param name="address">Address to check</param>
        /// <returns>
        /// True, if the address has a breakpoint; otherwise, false
        /// </returns>
        public bool HasBreakpoint(ushort address)
        {
            if (RamBankViewMode)
            {
                // --- We need to convert the breakpoint address to RAM bank address
                var memoryDevice = MachineViewModel.SpectrumVm.MemoryDevice;
                if (!memoryDevice.IsRamBankPagedIn(RamBankIndex, out var baseAddr))
                {
                    return false;
                }
                address += baseAddr;
            }
            var breakpoints = MachineViewModel?.DebugInfoProvider?.Breakpoints;
            if (breakpoints == null)
            {
                return false;
            }
            return breakpoints.TryGetValue(address, out var bpInfo) && bpInfo.IsCpuBreakpoint;
        }

        /// <summary>
        /// Checks if the specified address is the current instruction
        /// </summary>
        /// <param name="address">Address to check</param>
        /// <returns>
        /// True, if the address is the current instruction; otherwise, false
        /// </returns>
        public bool IsCurrentInstruction(ushort address)
        {
            return MachineViewModel != null 
                && MachineViewModel.MachineState == VmState.Paused
                && MachineViewModel.SpectrumVm?.Cpu.Registers.PC == address;
        }

        /// <summary>
        /// Tests if the specified address has a breakpoint condition
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public bool HasCondition(ushort address)
        {
            if (RamBankViewMode)
            {
                // --- We need to convert the breakpoint address to RAM bank address
                var memoryDevice = MachineViewModel.SpectrumVm.MemoryDevice;
                if (!memoryDevice.IsRamBankPagedIn(RamBankIndex, out var baseAddr))
                {
                    return false;
                }
                address += baseAddr;
            }
            var breakpoints = MachineViewModel?.DebugInfoProvider?.Breakpoints;
            if (breakpoints == null)
            {
                return false;
            }
            return breakpoints.TryGetValue(address, out var bpInfo) 
                   && bpInfo.IsCpuBreakpoint 
                   && (bpInfo.FilterCondition != null || bpInfo.HitType != BreakpointHitType.None);
        }

        #endregion
    }
}