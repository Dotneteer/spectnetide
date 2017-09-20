using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Spect.Net.SpectrumEmu.Disassembler;
using Spect.Net.SpectrumEmu.Mvvm;
using Spect.Net.SpectrumEmu.Mvvm.Messages;
using Spect.Net.VsPackage.Vsx;
// ReSharper disable AssignNullToNotNullAttribute

// ReSharper disable ExplicitCallerInfoArgument

namespace Spect.Net.VsPackage.Tools.Disassembly
{
    public class DisassemblyToolWindowViewModel: SpectrumGenericToolWindowViewModel, IDisassemblyItemParent
    {
        private ObservableCollection<DisassemblyItemViewModel> _disassemblyItems;
        private bool _saveRomChangesToRom;
        private bool _firstTimePaused;

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
        /// Signs that ROM changes should be saved to the ROM annotations file
        /// </summary>
        public bool SaveRomChangesToRom
        {
            get => _saveRomChangesToRom;
            set => Set(ref _saveRomChangesToRom, value);
        }

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
        }

        /// <summary>
        /// The annotations that should be handled by the disassembler
        /// </summary>
        public DisassemblyAnnotation Annotations => AnnotationHandler.MergedAnnotations;

        /// <summary>
        /// Obtain the machine view model from the solution
        /// </summary>
        protected override void OnSolutionOpened(SolutionOpenedMessage msg)
        {
            base.OnSolutionOpened(msg);
            InitDisassembly();
        }

        /// <summary>
        /// Whenever the state of the Spectrum virtual machine changes,
        /// we refrehs the memory dump
        /// </summary>
        protected override void OnVmStateChanged(MachineStateChangedMessage msg)
        {
            base.OnVmStateChanged(msg);

            // --- We've stopped the virtual machine
            if (VmStopped)
            {
                Clear();
                return;
            }

            // --- The machnine runs (again)
            if (VmRuns)
            {
                // --- We have just started the virtual machine
                if (msg.OldState == VmState.None || msg.OldState == VmState.Stopped)
                {
                    _firstTimePaused = true;
                    Disassemble();
                }
                MessengerInstance.Send(new RefreshDisassemblyViewMessage());
                //RefreshVisibleItems();
            }

            // --- We have just started the virtual machine
            if ((msg.OldState == VmState.None || msg.OldState == VmState.Stopped)
                && msg.NewState == VmState.Running)
            {
                _firstTimePaused = true;
                Disassemble();
                return;
            }

            if (!VmPaused) return;

            // --- We have just paused the virtual machine
            if (_firstTimePaused)
            {
                Disassemble();
                _firstTimePaused = false;
            }

            // --- Let's refresh the current instruction
            MessengerInstance.Send(new RefreshDisassemblyViewMessage(
                MachineViewModel.SpectrumVm.Cpu.Registers.PC));
            //RefreshVisibleItems();
            //ScrollToTop(Vm.MachineViewModel.SpectrumVm.Cpu.Registers.PC);
        }

        /// <summary>
        /// Disassembles the current virtual machine's memory
        /// </summary>
        public void Disassemble()
        {
            if (MachineViewModel.SpectrumVm == null) return;
            if (VmStopped) return;

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
                    if (!breakPoints.Contains(parser.Address))
                    {
                        breakPoints.Add(parser.Address);
                    }
                    break;

                case DisassemblyCommandType.ToggleBreakPoint:
                    if (!breakPoints.Contains(parser.Address))
                    {
                        breakPoints.Add(parser.Address);
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
                    breakPoints.Clear();
                    break;

                default:
                    return false;
            }
            MessengerInstance.Send(new RefreshDisassemblyViewMessage(address));
            return true;
        }


        #region Helpers

        /// <summary>
        /// Creates a disassembler for the curent machine
        /// </summary>
        /// <returns></returns>
        private Z80Disassembler CreateDisassembler()
        {
            var disassembler = new Z80Disassembler(AnnotationHandler.MergedAnnotations.MemoryMap, 
                MachineViewModel.SpectrumVm.MemoryDevice.GetMemoryBuffer(), 
                // TODO: Change flags according to ROM model
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
            var workspace = VsxPackage.GetPackage<SpectNetPackage>().CurrentWorkspace;
            string romAnnotationFile = null;
            var romItem = workspace.RomItem;
            if (romItem != null)
            {
                romAnnotationFile =
                    Path.Combine(
                        Path.GetDirectoryName(romItem.Filename),
                        Path.GetFileNameWithoutExtension(romItem.Filename)) + ".disann";
            }

            var customAnnotationFile = workspace.AnnotationItem?.Filename;
            AnnotationHandler = new DisassemblyAnnotationHandler(this, romAnnotationFile, customAnnotationFile);
            AnnotationHandler.RestoreAnnotations();
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
                default:
                    type = MemorySectionType.Disassemble;
                    break;
            }
            AnnotationHandler.AddSection(startAddress, endAddress, type);
        }

        #endregion
    }
}