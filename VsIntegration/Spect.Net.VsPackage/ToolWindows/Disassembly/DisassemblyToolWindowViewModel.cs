using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Spect.Net.SpectrumEmu.Disassembler;
using Spect.Net.SpectrumEmu.Machine;

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
            const string INV_S48_COMMAND = "This command cannot be used for a Spectrum 48K model.";
            const string INV_RUN_COMMAND = "This command can only be used when the virtual machine is running.";

            // --- Prepare command handling
            validationMessage = null;
            newPrompt = null;
            ushort? address = null;
            var breakPoints = MachineViewModel.SpectrumVm.DebugInfoProvider.Breakpoints;
            var isSpectrum48 = SpectNetPackage.IsSpectrum48Model();
            var banks = MachineViewModel.SpectrumVm.MemoryConfiguration.RamBanks;
            var roms = MachineViewModel.SpectrumVm.RomConfiguration.NumberOfRoms;

            var parser = new DisassemblyCommandParser(commandText);
            switch (parser.Command)
            {
                case DisassemblyCommandType.Invalid:
                    validationMessage = "Invalid command syntax";
                    return false;

                case DisassemblyCommandType.Goto:
                    if (!CheckCommandAddress(parser.Address, out validationMessage))
                    {
                        return false;
                    }
                    address = parser.Address;
                    break;

                case DisassemblyCommandType.Label:
                    if (!CheckCommandAddress(parser.Address, out validationMessage))
                    {
                        return false;
                    }
                    AnnotationHandler.SetLabel(parser.Address, parser.Arg1, out validationMessage);
                    if (validationMessage != null)
                    {
                        newPrompt = commandText;
                        return false;
                    }
                    break;

                case DisassemblyCommandType.Comment:
                    if (!CheckCommandAddress(parser.Address, out validationMessage))
                    {
                        return false;
                    }
                    AnnotationHandler.SetComment(parser.Address, parser.Arg1);
                    break;

                case DisassemblyCommandType.PrefixComment:
                    if (!CheckCommandAddress(parser.Address, out validationMessage))
                    {
                        return false;
                    }
                    AnnotationHandler.SetPrefixComment(parser.Address, parser.Arg1);
                    break;

                case DisassemblyCommandType.Retrieve:
                    if (!CheckCommandAddress(parser.Address, out validationMessage))
                    {
                        return false;
                    }
                    newPrompt = RetrieveAnnotation(parser.Address, parser.Arg1);
                    return false;

                case DisassemblyCommandType.Literal:
                    if (!CheckCommandAddress(parser.Address, out validationMessage))
                    {
                        return false;
                    }
                    var handled = ApplyLiteral(parser.Address, parser.Arg1,
                        out validationMessage, out newPrompt);
                    if (!handled) return false;
                    break;

                case DisassemblyCommandType.AddSection:
                    if (!CheckCommandAddress(parser.Address, out validationMessage))
                    {
                        return false;
                    }
                    if (!CheckCommandAddress(parser.Address2, out validationMessage))
                    {
                        return false;
                    }
                    AddSection(parser.Address2, parser.Address, parser.Arg1);
                    Disassemble();
                    break;

                case DisassemblyCommandType.SetBreakPoint:
                    if (!CheckRamBankCommand(parser.Address, out validationMessage))
                    {
                        return false;
                    }
                    if (!breakPoints.ContainsKey(parser.Address))
                    {
                        breakPoints.Add(parser.Address, MinimumBreakpointInfo.EmptyBreakpointInfo);
                    }
                    break;

                case DisassemblyCommandType.ToggleBreakPoint:
                    if (!CheckRamBankCommand(parser.Address, out validationMessage))
                    {
                        return false;
                    }
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
                    if (!CheckRamBankCommand(parser.Address, out validationMessage))
                    {
                        return false;
                    }
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
                    if (isSpectrum48)
                    {
                        validationMessage = INV_S48_COMMAND;
                        return false;
                    }
                    if (parser.Address > roms - 1)
                    {
                        validationMessage = $"This machine does not have a ROM bank #{parser.Address}";
                        return false;
                    }
                    SetRomViewMode(parser.Address);
                    address = 0;
                    break;

                case DisassemblyCommandType.SetRamBank:
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
                    if (parser.Address > banks - 1)
                    {
                        validationMessage = $"This machine does not have a RAM bank #{parser.Address}";
                        return false;

                    }
                    SetRamBankViewMode(parser.Address);
                    address = 0;
                    break;

                case DisassemblyCommandType.MemoryMode:
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
                    address = 0;
                    break;

                case DisassemblyCommandType.DisassemblyType:
                    if (FullViewMode)
                    {
                        validationMessage = "This command can be used only in ROM or RAM bank view mode";
                        return false;
                    }
                    SetDisassemblyType(parser.Arg1);
                    break;

                case DisassemblyCommandType.ReDisassembly:
                    Disassemble();
                    if (TopAddress.HasValue)
                    {
                        address = TopAddress.Value;
                    }
                    break;

                default:
                    return false;
            }
            DisassemblyViewRefreshed?.Invoke(this, new DisassemblyViewRefreshedEventArgs(address));
            return true;
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
        /// Gets the annotation for the specified address
        /// </summary>
        /// <param name="address">Flat memory address</param>
        /// <param name="annAddr">Annotation address</param>
        /// <returns>Annotations, if found; otherwise, null</returns>
        public DisassemblyAnnotation GetAnnotationFor(ushort address, out ushort annAddr)
        {
            annAddr = address;
            if (FullViewMode)
            {
                var memDevice = MachineViewModel.SpectrumVm.MemoryDevice;
                var locationInfo = memDevice.GetAddressLocation(address);
                annAddr = locationInfo.Address;
                if (locationInfo.IsInRom)
                {
                    AnnotationHandler.RomPageAnnotations.TryGetValue(locationInfo.Index, out var fullRomAnn);
                    return fullRomAnn;
                }
                return GetRamBankAnnotation(locationInfo.Index);
            }
            if (RomViewMode)
            {
                AnnotationHandler.RomPageAnnotations.TryGetValue(RomIndex, out var romAnn);
                return romAnn;
            }
            return GetRamBankAnnotation(RamBankIndex);
        }

        /// <summary>
        /// Gets the annotations for the specified RAM bank
        /// </summary>
        /// <param name="index">Ram bank index</param>
        /// <returns></returns>
        private DisassemblyAnnotation GetRamBankAnnotation(int index)
        {
            if (AnnotationHandler.RamBankAnnotations.TryGetValue(index, out var ramAnn))
            {
                return ramAnn;
            }
            var newBank = new DisassemblyAnnotation
            {
                MemoryMap =
                {
                    new MemorySection
                    {
                        StartAddress = 0,
                        EndAddress = 0x3FFF,
                        SectionType = MemorySectionType.ByteArray
                    }
                }
            };
            AnnotationHandler.RamBankAnnotations.Add(index, newBank);
            return newBank;
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
                new DisassemblyViewRefreshedEventArgs(0));
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
        /// Creates a disassembler for the curent machine
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
                                new MemorySection(0, (ushort)(memoryDevice.PageSize - 1), MemorySectionType.ByteArray)
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

            var disassembler = new Z80Disassembler(map, memory, disassemblyFlags);
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

        /// <summary>
        /// Checks if the specified address is valid for the current view mode.
        /// </summary>
        /// <param name="addr">Address to check</param>
        /// <param name="validationMessage">Validation message</param>
        /// <returns></returns>
        private bool CheckCommandAddress(ushort addr, out string validationMessage)
        {
            validationMessage = null;
            if ((RomViewMode || RamBankViewMode) && addr >= 0x4000)
            {
                validationMessage = $"Address #{addr:X4} out of range";
                return false;
            }
            return true;
        }

        /// <summary>
        /// Checks if the specified RAM bank command is valid
        /// </summary>
        /// <param name="addr">Address to check</param>
        /// <param name="validationMessage">Validation message</param>
        /// <returns></returns>
        private bool CheckRamBankCommand(ushort addr, out string validationMessage)
        {
            validationMessage = null;
            if (RamBankViewMode)
            {
                validationMessage = "This command is not available in RAM Bank view mode.";
                return false;
            }
            return CheckCommandAddress(addr, out validationMessage);
        }

        /// <summary>
        /// Whenever the tape device leaves the load mode, re-disassembly the output
        /// </summary>
        private void TapeDeviceOnLeftLoadMode(object sender, EventArgs eventArgs)
        {
            Task.Run(() =>
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
        /// Disassebles the current memory and fills up disassebly items and line indexes
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
                && MachineViewModel.VmState == VmState.Paused
                && MachineViewModel.SpectrumVm?.Cpu.Registers.PC == address;
        }

        #endregion
    }
}