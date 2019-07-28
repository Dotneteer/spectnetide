using System;
using System.Linq;
using Antlr4.Runtime;
using Spect.Net.CommandParser;
using Spect.Net.CommandParser.Generated;
using Spect.Net.CommandParser.SyntaxTree;
using Spect.Net.SpectrumEmu.Disassembler;

namespace Spect.Net.VsPackage.ToolWindows.BankAware
{
    /// <summary>
    /// This view model is the base of the view models that manage memory
    /// </summary>
    public abstract class BankAwareToolWindowViewModelBase : SpectrumGenericToolWindowViewModel
    {
        private bool _fullViewMode;
        private bool _romViewMode;
        private bool _ramBankViewMode;
        private int _romIndex;
        private int _ramBankIndex;

        /// <summary>
        /// Stores the object that handles annotations and their persistence
        /// </summary>
        public DisassemblyAnnotationHandler AnnotationHandler { get; protected set; }

        protected BankAwareToolWindowViewModelBase()
        {
            AnnotationHandler = new DisassemblyAnnotationHandler(this);
        }

        /// <summary>
        /// The full 64K memory is displayed
        /// </summary>
        public bool FullViewMode
        {
            get => _fullViewMode;
            set
            {
                if (!Set(ref _fullViewMode, value)) return;

                RaisePropertyChanged(nameof(ShowRomTag));
                RaisePropertyChanged(nameof(ShowBankTag));
            }
        }

        /// <summary>
        /// A ROM page is displayed
        /// </summary>
        public bool RomViewMode
        {
            get => _romViewMode;
            set
            {
                if (!Set(ref _romViewMode, value)) return;

                RaisePropertyChanged(nameof(ShowRomTag));
                RaisePropertyChanged(nameof(ShowBankTag));
            }
        }

        /// <summary>
        /// A RAM bank page is displayed
        /// </summary>
        public bool RamBankViewMode
        {
            get => _ramBankViewMode;
            set
            {
                if (!Set(ref _ramBankViewMode, value)) return;

                RaisePropertyChanged(nameof(ShowRomTag));
                RaisePropertyChanged(nameof(ShowBankTag));
            }
        }

        /// <summary>
        /// The index of the ROM page displayed in ROM view mode
        /// </summary>
        public int RomIndex
        {
            get => _romIndex;
            set => Set(ref _romIndex, value);
        }

        /// <summary>
        /// The index of the RAM page displayed in RAM bank view mode
        /// </summary>
        public int RamBankIndex
        {
            get => _ramBankIndex;
            set => Set(ref _ramBankIndex, value);
        }

        /// <summary>
        /// Should the ROM tag be displayed?
        /// </summary>
        public bool ShowRomTag => FullViewMode || RomViewMode;

        /// <summary>
        /// Should the BANK tag be displayed?
        /// </summary>
        public bool ShowBankTag => FullViewMode || RamBankViewMode;

        /// <summary>
        /// This flag indicates if the Bank view is allowed or not
        /// </summary>
        public bool BankViewAllowed => (SpectrumVm.RomConfiguration?.NumberOfRoms ?? 0) > 1;

        /// <summary>
        /// Retrieves the memory buffer to show
        /// </summary>
        protected byte[] GetMemoryBuffer()
        {
            return FullViewMode
                ? SpectrumVm.MemoryDevice?.CloneMemory()
                : (RomViewMode
                    ? SpectrumVm.MemoryDevice?.GetRomBuffer(RomIndex)
                    : SpectrumVm.MemoryDevice?.GetRamBank(RamBankIndex));
        }

        /// <summary>
        /// Retrieves the length of the memory buffer to show
        /// </summary>
        protected int? GetMemoryLength()
        {
            return FullViewMode
                ? 0x10000
                : 0x4000;
        }

        /// <summary>
        /// Sets the current view to full view mode
        /// </summary>
        public void SetFullViewMode()
        {
            FullViewMode = true;
            RomViewMode = false;
            RamBankViewMode = false;
            UpdatePageInformation();
            InitViewMode();
        }

        /// <summary>
        /// Sets the current view to ROM view mode
        /// </summary>
        /// <param name="romIndex">The ROM page to show</param>
        public void SetRomViewMode(int romIndex)
        {
            FullViewMode = false;
            RomViewMode = false;
            RamBankViewMode = false;
            RomIndex = romIndex;
            RomViewMode = true;
            InitViewMode();
        }

        /// <summary>
        /// Sets the current view to RAM bank view mode
        /// </summary>
        /// <param name="ramBankIndex">The RAM bank to show</param>
        public void SetRamBankViewMode(int ramBankIndex)
        {
            FullViewMode = false;
            RomViewMode = false;
            RamBankViewMode = false;
            RamBankIndex = ramBankIndex;
            RamBankViewMode = true;
            InitViewMode();
        }

        /// <summary>
        /// Updates the page information
        /// </summary>
        public void UpdatePageInformation()
        {
            RomIndex = SpectrumVm?.MemoryDevice?.GetSelectedRomIndex() ?? 0;
            RamBankIndex = SpectrumVm?.MemoryDevice?.GetSelectedBankIndex(3) ?? 0;
        }

        /// <summary>
        /// When the view model is first time created, use the ROM view
        /// </summary>
        protected override void Initialize()
        {
            RaisePropertyChanged(nameof(BankViewAllowed));
            SetRomViewMode(0);
        }

        /// <summary>
        /// Set the tool window into Full view mode on first start
        /// </summary>
        protected override void OnFirstStart()
        {
            SetFullViewMode();
        }

        /// <summary>
        /// Refresh the view mode for every start/continue
        /// </summary>
        protected override void OnStart()
        {
            RefreshViewMode();
        }

        /// <summary>
        /// Refresh the memory view for each pause
        /// </summary>
        protected override void OnPaused()
        {
            if (FullViewMode)
            {
                UpdatePageInformation();
            }
            RefreshOnPause();
        }

        /// <summary>
        /// Override to handle the first paused state
        /// of the virtual machine
        /// </summary>
        protected override void OnFirstPaused()
        {
            SetFullViewMode();
        }

        /// <summary>
        /// Set the ROM view mode when the machine is stopped
        /// </summary>
        protected override void OnStopped()
        {
            SetRomViewMode(0);
        }

        /// <summary>
        /// Override this method to init the current view mode
        /// </summary>
        public virtual void InitViewMode()
        {
        }

        /// <summary>
        /// Override this method to refresh the current view mode
        /// </summary>
        public virtual void RefreshViewMode()
        {
        }

        /// <summary>
        /// Override this method to define how to refresh the view 
        /// when the virtual machine is paused
        /// </summary>
        public virtual void RefreshOnPause()
        {
        }

        /// <summary>
        /// Refreshes the item at the current address
        /// </summary>
        /// <param name="addr">Item address</param>
        public virtual void RefreshItem(int addr)
        {
        }

        /// <summary>
        /// Refreshes the item at the specified address, if it 
        /// should be refreshed in the current view
        /// </summary>
        /// <param name="addr"></param>
        public virtual void RefreshItemIfItMayChange(ushort addr)
        {
            // --- ROM items never change
            if (RomViewMode) return;

            if (RamBankViewMode)
            {
                var memDevice = SpectrumVm.MemoryDevice;
                if (!memDevice.IsRamBankPagedIn(RamBankIndex, out _))
                {
                    // --- The current RAM bank is not paged in, so it may not change
                    return;
                }
            }

            // --- The content at the address may change
            RefreshItem(addr);
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
                var memDevice = SpectrumVm.MemoryDevice;
                var (isInRom, index, locAddress) = memDevice.GetAddressLocation(address);
                annAddr = locAddress;
                if (!isInRom) return GetRamBankAnnotation(index);

                AnnotationHandler.RomPageAnnotations.TryGetValue(index, out var fullRomAnn);
                return fullRomAnn;
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
        public DisassemblyAnnotation GetRamBankAnnotation(int index)
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

        /// <summary>
        /// Parses the specified command
        /// </summary>
        /// <param name="commandText">Text to parse</param>
        /// <returns>Parsed command node, if successful; otherwise, null</returns>
        public ToolCommandNode ParseCommand(string commandText)
        {
            var inputStream = new AntlrInputStream(commandText.TrimStart());
            var lexer = new CommandToolLexer(inputStream);
            var tokenStream = new CommonTokenStream(lexer);
            var parser = new CommandToolParser(tokenStream);
            var context = parser.compileUnit();
            var visitor = new CommandToolVisitor();
            return parser.SyntaxErrors.Count > 0 || context.toolCommand() == null
                ? null
                : (ToolCommandNode)visitor.VisitToolCommand(context.toolCommand());
        }

        /// <summary>
        /// Resolves the value of the specified symbol
        /// </summary>
        /// <param name="symbol">Symbol to resolve</param>
        /// <param name="value">The resolved value</param>
        /// <returns>True, if resolution is successful; otherwise, false</returns>
        public bool ResolveSymbol(string symbol, out ushort value)
        {
            value = 0;

            if (FullViewMode)
            {
                // TODO: Implement symbol lookup in the comiled code
                // --- #1: check the compiled code
                //if (CompilerOutput != null && CompilerOutput.Symbols.TryGetValue(symbol, out var symbolValue))
                //{
                //    value = symbolValue.Value;
                //    return true;
                //}

                // #2: Check user defined RAM annotations
                var labelAddrs = AnnotationHandler.RamBankAnnotations[0].Labels
                    .Where(kp =>
                        string.Compare(kp.Value, symbol, StringComparison.InvariantCultureIgnoreCase) == 0)
                    .Select(kp => kp.Key)
                    .ToList();
                if (labelAddrs.Count > 0)
                {
                    // --- Address found in the current RAM
                    value = (ushort)(labelAddrs[0] + 0x4000);
                    return true;
                }
            }

            if (!RomViewMode && !FullViewMode) return false;
            // #3: Check ROM annotations
            var curRomIndex = SpectrumVm.MemoryDevice.GetSelectedRomIndex();
            if (RomViewMode)
            {
                curRomIndex = RomIndex;
            }

            var romLabelAddrs = AnnotationHandler.RomPageAnnotations[curRomIndex].Labels
                .Where(kp =>
                    string.Compare(kp.Value, symbol, StringComparison.InvariantCultureIgnoreCase) == 0)
                .Select(kp => kp.Key)
                .ToList();
            if (romLabelAddrs.Count == 0) return false;

            // --- Address found in the current ROM
            value = romLabelAddrs[0];
            return true;
        }

        /// <summary>
        /// Gets and checks the specified address.
        /// </summary>
        /// <param name="addr">Address, if specified with a number</param>
        /// <param name="symbol">Symbol, if specified</param>
        /// <param name="resolvedAddress">Resolved address</param>
        /// <param name="validationMessage">Validation message, in case of error</param>
        /// <returns>True, if the address is successfully resolved; otherwise, false.</returns>
        protected bool ObtainAddress(ushort addr, string symbol, out ushort resolvedAddress, out string validationMessage)
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
    }
}