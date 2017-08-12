using System.Linq;
using System.Windows;
using GalaSoft.MvvmLight.Messaging;
using Spect.Net.VsPackage.Utility;
using Spect.Net.Wpf.SpectrumControl;

namespace Spect.Net.VsPackage.Tools.Disassembly
{
    /// <summary>
    /// Interaction logic for DisassemblyToolWindowControl.xaml
    /// </summary>
    public partial class DisassemblyToolWindowControl
    {
        private bool _firstTimePaused;

        public DisassemblyViewModel Vm { get; }

        public DisassemblyToolWindowControl()
        {
            InitializeComponent();
            DataContext = Vm = new DisassemblyViewModel();
            Loaded += (s, e) =>
            {
                Messenger.Default.Register<SpectrumVmStateChangedMessage>(this, OnVmStateChanged);
            };
            Unloaded += (s, e) =>
            {
                Messenger.Default.Unregister<SpectrumVmStateChangedMessage>(this);
            };
            PreviewKeyDown += (s, e) => DisassemblyList.HandleListViewKeyEvents(e);
            Prompt.CommandLineEntered += OnCommandLineEntered;

        }

        /// <summary>
        /// Whenever the state of the Spectrum virtual machine changes,
        /// we refrehs the memory dump
        /// </summary>
        private void OnVmStateChanged(SpectrumVmStateChangedMessage msg)
        {
            Dispatcher.InvokeAsync(() =>
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
                    if (msg.OldState == SpectrumVmState.None || msg.OldState == SpectrumVmState.Stopped)
                    {
                        _firstTimePaused = true;
                        Vm.Disassemble();
                    }
                    RefreshVisibleItems();
                }

                // --- We have just started the virtual machine
                if ((msg.OldState == SpectrumVmState.None || msg.OldState == SpectrumVmState.Stopped)
                    && msg.NewState == SpectrumVmState.Running)
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
                ScrollToTop(Vm.SpectrumVmViewModel.SpectrumVm.Cpu.Registers.PC);
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
                    e.Handled = false;
                    return;
                case DisassemblyCommandType.Goto:
                    ScrollToTop(parser.Address);
                    break;
                case DisassemblyCommandType.Label:
                    Vm.HandleLabelCommand(parser.Address, parser.Arg1);
                    break;
                case DisassemblyCommandType.Comment:
                    break;
                case DisassemblyCommandType.PrefixComment:
                    break;
                case DisassemblyCommandType.SetBreakPoint:
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
        }

        /// <summary>
        /// This method refreshes only those memory items that are visible in the 
        /// current viewport
        /// </summary>
        private void RefreshVisibleItems()
        {
            var stack = DisassemblyList.GetInnerStackPanel();
            for (var i = 0; i < stack.Children.Count; i++)
            {
                if ((stack.Children[i] as FrameworkElement)?.DataContext is DisassemblyItemViewModel disassLine)
                {
                    // ReSharper disable once ExplicitCallerInfoArgument
                    disassLine.RaisePropertyChanged(nameof(DisassemblyItemViewModel.IsCurrentInstruction));
                }
            }
        }

        /// <summary>
        /// Scrolls the disassembly item with the specified address into view
        /// </summary>
        /// <param name="address">Address to show</param>
        /// <param name="offset">Offset to wind back the top</param>
        public void ScrollToTop(ushort address, int offset = 3)
        {
            var topItem = Vm.DisassemblyItems.FirstOrDefault(i => i.Item.Address >= address) 
                ?? Vm.DisassemblyItems[Vm.DisassemblyItems.Count - 1];
            var foundAddress = topItem.Item.Address;
            var index = Vm.LineIndexes[foundAddress];
            if (address < foundAddress && index > 0)
            {
                index--;
            }
            index = offset > index ? 0 : index - offset;
            var sw = DisassemblyList.GetScrollViewer();
            sw?.ScrollToVerticalOffset(index);
        }
    }
}
