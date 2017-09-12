using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Spect.Net.SpectrumEmu.Disassembler;
using Spect.Net.SpectrumEmu.Mvvm;
using Spect.Net.SpectrumEmu.Mvvm.Messages;
using Spect.Net.VsPackage.Utility;

// ReSharper disable ExplicitCallerInfoArgument

namespace Spect.Net.VsPackage.Tools.Disassembly
{
    /// <summary>
    /// Interaction logic for DisassemblyToolWindowControl.xaml
    /// </summary>
    public partial class DisassemblyToolWindowControl
    {
        private bool _firstTimePaused;

        public DisassemblyToolWindowViewModel Vm { get; }

        public DisassemblyToolWindowControl()
        {
            InitializeComponent();
            DataContext = Vm = new DisassemblyToolWindowViewModel();
            Loaded += (s, e) =>
            {
                Vm.EvaluateState();
                if (Vm.VmNotStopped)
                {
                    Vm.Disassemble();
                }
            };
            PreviewKeyDown += (s, e) => DisassemblyControl.DisassemblyList.HandleListViewKeyEvents(e);
            PreviewKeyDown += (s, arg) => Vm.HandleDebugKeys(arg);

            Prompt.CommandLineEntered += OnCommandLineEntered;
            Vm.SaveRomChangesToRom = true;
        }
        /// <summary>
        /// Whenever the state of the Spectrum virtual machine changes,
        /// we refrehs the memory dump
        /// </summary>
        protected override void OnVmStateChanged(VmStateChangedMessage msg)
        {
            DispatchOnUiThread(() =>
            {
                // --- We've stopped the virtual machine
                if (Vm.VmStopped)
                {
                    Vm.Clear();
                    return;
                }

                // --- The machnine runs (again)
                if (Vm.VmRuns)
                {
                    // --- We have just started the virtual machine
                    if (msg.OldState == VmState.None || msg.OldState == VmState.Stopped)
                    {
                        _firstTimePaused = true;
                        Vm.Disassemble();
                    }
                    RefreshVisibleItems();
                }

                // --- We have just started the virtual machine
                if ((msg.OldState == VmState.None || msg.OldState == VmState.Stopped)
                    && msg.NewState == VmState.Running)
                {
                    _firstTimePaused = true;
                    Vm.Disassemble();
                    return;
                }

                if (!Vm.VmPaused) return;

                // --- We have just paused the virtual machine
                if (_firstTimePaused)
                {
                    Vm.Disassemble();
                    _firstTimePaused = false;
                }

                // --- Let's refresh the current instruction
                RefreshVisibleItems();
                ScrollToTop(Vm.MachineViewModel.SpectrumVm.Cpu.Registers.PC);
            });
        }

        /// <summary>
        /// When a valid address is provided, we scroll the memory window to that address
        /// </summary>
        private void OnCommandLineEntered(object sender, CommandLineEventArgs e)
        {
            var parser = new DisassemblyCommandParser(e.CommandLine);
            switch (parser.Command)
            {
                case DisassemblyCommandType.Invalid:
                    Prompt.IsValid = false;
                    Prompt.ValidationMessage = "Invalid command syntax";
                    e.Handled = false;
                    return;

                case DisassemblyCommandType.Goto:
                    ScrollToTop(parser.Address);
                    break;

                case DisassemblyCommandType.Label:
                    Vm.AnnotationHandler.SetLabel(parser.Address, parser.Arg1);
                    break;

                case DisassemblyCommandType.Comment:
                    Vm.AnnotationHandler.SetComment(parser.Address, parser.Arg1);
                    break;

                case DisassemblyCommandType.PrefixComment:
                    Vm.AnnotationHandler.SetPrefixComment(parser.Address, parser.Arg1);
                    break;

                case DisassemblyCommandType.Retrieve:
                    RetrieveAnnotation(parser.Address, parser.Arg1);
                    e.Handled = false;
                    return;

                case DisassemblyCommandType.Literal:
                    var handled = e.Handled = ApplyLiteral(parser.Address, parser.Arg1);
                    if (!handled) return;
                    break;

                case DisassemblyCommandType.AddSection:
                    AddSection(parser.Address2, parser.Address, parser.Arg1);
                    Dispatcher.InvokeAsync(() =>
                    {
                        Vm.Disassemble();
                        RefreshVisibleItems();
                    });
                    e.Handled = true;
                    break;

                case DisassemblyCommandType.SetBreakPoint:
                    Vm.MachineViewModel.SpectrumVm.DebugInfoProvider.Breakpoints.Add(parser.Address);
                    break;

                case DisassemblyCommandType.ToggleBreakPoint:
                    break;
                case DisassemblyCommandType.RemoveBreakPoint:
                    break;
                case DisassemblyCommandType.EraseAllBreakPoint:
                    break;
                default:
                    e.Handled = false;
                    return;
            }
            e.Handled = true;
            RefreshVisibleItems();
        }

        /// <summary>
        /// Retrieves lables, comments, or prefix comments for modification.
        /// </summary>
        /// <param name="address">Address to retrive data from</param>
        /// <param name="dataType">Type of data to retrieve</param>
        private void RetrieveAnnotation(ushort address, string dataType)
        {
            dataType = dataType.ToUpper();
            var found = false;
            var commandtext = $"{dataType} {address:X4} ";
            var text = string.Empty;
            switch (dataType)
            {
                case "L":
                    found = Vm.Annotations.Labels.TryGetValue(address, out text);
                    break;
                case "C":
                    found = Vm.Annotations.Comments.TryGetValue(address, out text);
                    break;
                case "P":
                    found = Vm.Annotations.PrefixComments.TryGetValue(address, out text);
                    break;
            }
            if (!found) return;

            Prompt.CommandText = commandtext + text;
            Prompt.CommandLine.CaretIndex = Prompt.CommandText.Length;
        }

        /// <summary>
        /// Retrieves lables, comments, or prefix comments for modification.
        /// </summary>
        /// <param name="address">Address to retrive data from</param>
        /// <param name="literalName">Type of data to retrieve</param>
        private bool ApplyLiteral(ushort address, string literalName)
        {
            var message = Vm.AnnotationHandler.ApplyLiteral(address, literalName);
            if (message == null) return true;

            Prompt.IsValid = false;
            if (message.StartsWith("%"))
            {
                var parts = message.Split(new[] {'%'}, StringSplitOptions.RemoveEmptyEntries);
                Prompt.CommandText = parts[0];
                message = parts[1];
            }
            Prompt.ValidationMessage = message;
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
            Vm.AnnotationHandler.AddSection(startAddress, endAddress, type);
        }

        /// <summary>
        /// This method refreshes only those memory items that are visible in the 
        /// current viewport
        /// </summary>
        private void RefreshVisibleItems()
        {
            var stack = DisassemblyControl.DisassemblyList.GetInnerStackPanel();
            for (var i = 0; i < stack.Children.Count; i++)
            {
                if (!((stack.Children[i] as FrameworkElement)?.DataContext is DisassemblyItemViewModel disassLine))
                {
                    continue;
                }
                disassLine.RaisePropertyChanged(nameof(DisassemblyItemViewModel.LabelFormatted));
                disassLine.RaisePropertyChanged(nameof(DisassemblyItemViewModel.InstructionFormatted));
                disassLine.RaisePropertyChanged(nameof(DisassemblyItemViewModel.IsCurrentInstruction));
                disassLine.RaisePropertyChanged(nameof(DisassemblyItemViewModel.PrefixCommentFormatted));
                disassLine.RaisePropertyChanged(nameof(DisassemblyItemViewModel.HasPrefixComment));
                disassLine.RaisePropertyChanged(nameof(DisassemblyItemViewModel.CommentFormatted));
                disassLine.RaisePropertyChanged(nameof(DisassemblyItemViewModel.HasBreakpoint));
            }
        }

        /// <summary>
        /// Scrolls the disassembly item with the specified address into view
        /// </summary>
        /// <param name="address">Address to show</param>
        /// <param name="offset">Offset to wind back the top</param>
        public void ScrollToTop(ushort address, int offset = 0)
        {
            var topItem = Vm.DisassemblyItems.FirstOrDefault(i => i.Item.Address >= address) 
                ?? Vm.DisassemblyItems[Vm.DisassemblyItems.Count - 1];
            var foundAddress = topItem.Item.Address;
            var index = Vm.LineIndexes[foundAddress];
            if (address < foundAddress && index > 0)
            {
                index--;
            }
            topItem.IsSelected = true;
            index = offset > index ? 0 : index - offset;
            var sw = DisassemblyControl.DisassemblyList.GetScrollViewer();
            sw?.ScrollToVerticalOffset(index);
        }

        /// <summary>
        /// Allow changing the ROM-related annotations
        /// </summary>
        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                Vm.AnnotationHandler.SaveRomChangesToRom = Vm.SaveRomChangesToRom = !Vm.SaveRomChangesToRom;
            }
        }
    }
}
