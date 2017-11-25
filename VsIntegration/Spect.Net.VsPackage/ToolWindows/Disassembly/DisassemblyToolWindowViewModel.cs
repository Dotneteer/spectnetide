using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Spect.Net.SpectrumEmu.Disassembler;
using Spect.Net.SpectrumEmu.Machine;
using Spect.Net.VsPackage.ToolWindows.Memory;

// ReSharper disable AssignNullToNotNullAttribute

// ReSharper disable ExplicitCallerInfoArgument

namespace Spect.Net.VsPackage.ToolWindows.Disassembly
{
    public class DisassemblyToolWindowViewModel: BankAwareToolWindowViewModelBase, IDisassemblyItemParent
    {
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
        /// Stores the object that handles annotations and their persistence
        /// </summary>
        public DisassemblyAnnotationHandler AnnotationHandler { get; private set; }

        /// <summary>
        /// We can keep track of the top address
        /// </summary>
        public ushort? TopAddress { get; set; }

        /// <summary>
        /// Selected disassembly items
        /// </summary>
        public IList<DisassemblyItemViewModel> SelectedItems 
            => DisassemblyItems.Where(item => item.IsSelected).ToList();

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

            InitDisassembly();
            if (VmNotStopped)
            {
                // ReSharper disable once VirtualMemberCallInConstructor
                InitViewMode();
            }

            //InitDisassembly();
            MessengerInstance.Register<AnnotationFileChangedMessage>(this, OnAnnotationItemChanged);
            MessengerInstance.Register<VmCodeInjectedMessage>(this, OnVmCodeInjected);
        }

        /// <summary>
        /// Force a new disassembly
        /// </summary>
        private void OnVmCodeInjected(VmCodeInjectedMessage msg)
        {
            Disassemble();
            MessengerInstance.Send(new RefreshMemoryViewMessage(TopAddress));
        }

        /// <summary>
        /// Handle the change of the annotation file
        /// </summary>
        private void OnAnnotationItemChanged(AnnotationFileChangedMessage msg)
        {
            InitDisassembly();
            Disassemble();
            MessengerInstance.Send(new RefreshMemoryViewMessage(TopAddress));
        }

        /// <summary>
        /// The annotations that should be handled by the disassembler
        /// </summary>
        public DisassemblyAnnotation Annotations => AnnotationHandler.MergedAnnotations;

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
            MessengerInstance.Send(new RefreshMemoryViewMessage(
                MachineViewModel.SpectrumVm.Cpu.Registers.PC));
        }

        ///// <summary>
        ///// Whenever the state of the Spectrum virtual machine changes,
        ///// we refrehs the memory dump
        ///// </summary>
        //protected override void OnVmStateChanged(object sender, VmStateChangedEventArgs args)
        //{
        //    // --- We've stopped the virtual machine
        //    if (VmStopped)
        //    {
        //        Clear();
        //        return;
        //    }

        //    // --- The machnine runs (again)
        //    if (VmRuns)
        //    {
        //        if (MachineViewModel.IsFirstStart)
        //        {
        //            // --- We have just started the virtual machine
        //            Disassemble();
        //        }
        //        MessengerInstance.Send(new RefreshMemoryViewMessage());
        //        return;
        //    }

        //    if (VmPaused)
        //    {
        //        if (MachineViewModel.IsFirstPause)
        //        {
        //            // --- We have paused the virtual machine first time
        //            Disassemble();
        //        }

        //        // --- Let's refresh the current instruction
        //        MessengerInstance.Send(new RefreshMemoryViewMessage(
        //            MachineViewModel.SpectrumVm.Cpu.Registers.PC));
        //    }
        //}

        /// <summary>
        /// Disassembles the current virtual machine's memory
        /// </summary>
        public void Disassemble()
        {
            if (MachineViewModel.SpectrumVm == null) return;

            var disassembler = CreateDisassembler();
            var output = disassembler.Disassemble();
            DisassemblyItems = new ObservableCollection<DisassemblyItemViewModel>();
            LineIndexes = new Dictionary<ushort, int>();

            var idx = 0;
            foreach (var item in output.OutputItems)
            {
                DisassemblyItems.Add(new DisassemblyItemViewModel(this, item));
                LineIndexes.Add(item.Address, idx++);
            }
        }

        /// <summary>
        /// Clears the disassembled items
        /// </summary>
        public void Clear()
        {
            DisassemblyItems.Clear();
        }

        /// <summary>
        /// Refrehses the disassembly line with the given address
        /// </summary>
        /// <param name="addr"></param>
        public void RefreshDisassembly(ushort addr)
        {
            // --- Get the item, provided it exists
            if (!LineIndexes.TryGetValue(addr, out var index))
            {
                return;
            }

            var startAddr = DisassemblyItems[index].Item.Address;
            var disassembler = CreateDisassembler();
            var output = disassembler.Disassemble(startAddr, (ushort)(startAddr + 1));
            if (output.OutputItems.Count > 0)
            {
                DisassemblyItems[index].Item = output.OutputItems[0];
            }
        }

        /// <summary>
        /// Processes the command text
        /// </summary>
        /// <param name="commandText">The command text</param>
        /// <param name="validationMessage">
        /// Null, if the command is valid; otherwise the validation message to show
        /// </param>
        /// <param name="newPrompt">
        /// New promt command prompt text, or null, if no text should be set
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

            var parser = new DisassemblyCommandParser(commandText);
            switch (parser.Command)
            {
                case DisassemblyCommandType.Invalid:
                    validationMessage = "Invalid command syntax";
                    return false;

                case DisassemblyCommandType.Goto:
                    address = parser.Address;
                    break;

                case DisassemblyCommandType.Label:
                    AnnotationHandler.SetLabel(parser.Address, parser.Arg1, out validationMessage);
                    if (validationMessage != null)
                    {
                        newPrompt = commandText;
                        return false;
                    }
                    break;

                case DisassemblyCommandType.Comment:
                    AnnotationHandler.SetComment(parser.Address, parser.Arg1);
                    break;

                case DisassemblyCommandType.PrefixComment:
                    AnnotationHandler.SetPrefixComment(parser.Address, parser.Arg1);
                    break;

                case DisassemblyCommandType.Retrieve:
                    newPrompt = RetrieveAnnotation(parser.Address, parser.Arg1);
                    return false;

                case DisassemblyCommandType.Literal:
                    var handled = ApplyLiteral(parser.Address, parser.Arg1,
                        out validationMessage, out newPrompt);
                    if (!handled) return false;
                    break;

                case DisassemblyCommandType.AddSection:
                    AddSection(parser.Address2, parser.Address, parser.Arg1);
                    Disassemble();
                    break;

                case DisassemblyCommandType.SetBreakPoint:
                    if (!breakPoints.ContainsKey(parser.Address))
                    {
                        breakPoints.Add(parser.Address, MinimumBreakpointInfo.EmptyBreakpointInfo);
                    }
                    break;

                case DisassemblyCommandType.ToggleBreakPoint:
                    if (!breakPoints.ContainsKey(parser.Address))
                    {
                        breakPoints.Add(parser.Address, MinimumBreakpointInfo.EmptyBreakpointInfo);
                    }
                    else
                    {
                        breakPoints.Remove(parser.Address);
                    }
                    break;

                case DisassemblyCommandType.RemoveBreakPoint:
                    breakPoints.Remove(parser.Address);
                    break;

                case DisassemblyCommandType.EraseAllBreakPoint:
                    var keysToRemove = breakPoints.Keys.Where(k => breakPoints[k].IsCpuBreakpoint);
                    foreach (var key in keysToRemove)
                    {
                        breakPoints.Remove(key);
                    }
                    break;

                case DisassemblyCommandType.SetRomPage:
                    SetRomViewMode(parser.Address);
                    address = 0;
                    break;

                case DisassemblyCommandType.SetRamBank:
                    SetRamBankViewMode(parser.Address);
                    address = 0;
                    break;

                case DisassemblyCommandType.MemoryMode:
                    SetFullViewMode();
                    address = 0;
                    break;

                default:
                    return false;
            }
            MessengerInstance.Send(new RefreshMemoryViewMessage(address));
            return true;
        }

        #region Helpers

        /// <summary>
        /// Creates a disassembler for the curent machine
        /// </summary>
        /// <returns></returns>
        private Z80Disassembler CreateDisassembler()
        {
            var memoryDevice = MachineViewModel.SpectrumVm.MemoryDevice;
            var memoryConfig = MachineViewModel.SpectrumVm.MemoryConfiguration;

            // --- Create map for rom annotations
            var currentRom = memoryDevice.GetSelectedRomIndex();
            var map = new MemoryMap();
            if (AnnotationHandler.RomPageAnnotations.TryGetValue(currentRom, out var romAnn))
            {
                map = romAnn.MemoryMap;
            }

            byte[] memory;
            if (RomViewMode)
            {
                memory = memoryDevice.GetRomBuffer(memoryDevice.GetSelectedRomIndex());
            }
            else if (RamBankViewMode)
            {
                // --- Create map for the visible bank
                if (AnnotationHandler.RamBankAnnotations.TryGetValue(RamBankIndex, out var ann))
                {
                    map = ann.MemoryMap;
                }
                memory = memoryDevice.GetRamBank(RamBankIndex);
            }
            else
            {
                // --- We are in FullViewMode
                // --- Merge the maps of the paged banks with the ROM maps  
                if (memoryConfig.SupportsBanking)
                {
                    for (var i = 1; i <= 3; i++)
                    {
                        if (AnnotationHandler.RamBankAnnotations.TryGetValue(0, out var ramAnn))
                        {
                            map.Merge(ramAnn.MemoryMap, (ushort)(i * 0x4000));
                        }
                    }
                }
                else
                {
                    // --- No banking supported
                    if (AnnotationHandler.RamBankAnnotations.TryGetValue(0, out var ramAnn))
                    {
                        map.Merge(ramAnn.MemoryMap, 0x4000);
                    }
                }
                memory = memoryDevice.CloneMemory();
            }

            var disassembler = new Z80Disassembler(map, memory, 
                SpectrumSpecificDisassemblyFlags.Spectrum48All);
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
        /// Retrieves lables, comments, or prefix comments for modification.
        /// </summary>
        /// <param name="address">Address to retrive data from</param>
        /// <param name="dataType">Type of data to retrieve</param>
        private string RetrieveAnnotation(ushort address, string dataType)
        {
            dataType = dataType.ToUpper();
            var found = false;
            var commandtext = $"{dataType} {address:X4} ";
            var text = string.Empty;
            switch (dataType)
            {
                case "L":
                    found = Annotations.Labels.TryGetValue(address, out text);
                    break;
                case "C":
                    found = Annotations.Comments.TryGetValue(address, out text);
                    break;
                case "P":
                    found = Annotations.PrefixComments.TryGetValue(address, out text);
                    break;
            }
            return found ? commandtext + text : null;
        }

        /// <summary>
        /// Retrieves lables, comments, or prefix comments for modification.
        /// </summary>
        /// <param name="address">Address to retrive data from</param>
        /// <param name="literalName">Type of data to retrieve</param>
        /// <param name="validationMessage">
        /// Null, if the command is valid; otherwise the validation message to show
        /// </param>
        /// <param name="newPrompt">
        /// New promt command prompt text, or null, if no text should be set
        /// </param>
        /// <returns>
        /// True, if the command has been handled; otherwise, false
        /// </returns>
        private bool ApplyLiteral(ushort address, string literalName, out string validationMessage,
            out string newPrompt)
        {
            validationMessage = null;
            newPrompt = null;
            var message = AnnotationHandler.ApplyLiteral(address, literalName);
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
                default:
                    type = MemorySectionType.Disassemble;
                    break;
            }
            AnnotationHandler.AddSection(startAddress, endAddress, type);
        }

        #endregion

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
                && MachineViewModel.VmState == VmState.Paused
                && MachineViewModel.SpectrumVm?.Cpu.Registers.PC == address;
        }

        /// <summary>
        /// Gets the annotation for the specified address
        /// </summary>
        /// <param name="address">Flat memory address</param>
        /// <param name="annAddr">Annotation address</param>
        /// <returns>Annotations, if found; otherwise, null</returns>
        private DisassemblyAnnotation GetAnnotationFor(ushort address, out ushort annAddr)
        {
            annAddr = 0;
            var memDevice = MachineViewModel.SpectrumVm.MemoryDevice;
            var locationInfo = memDevice.GetAddressLocation(address);
            annAddr = locationInfo.Address;
            if (locationInfo.IsInRom)
            {
                return AnnotationHandler.RomPageAnnotations.TryGetValue(locationInfo.Index, out var romAnn) 
                    ? romAnn : null;
            }
            return AnnotationHandler.RamBankAnnotations.TryGetValue(locationInfo.Index, out var ramAnn)
                ? ramAnn : null;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, 
        /// or resetting unmanaged resources.
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();
            MessengerInstance.Unregister<AnnotationFileChangedMessage>(this);
            MessengerInstance.Unregister<VmCodeInjectedMessage>(this);
        }
    }
}