using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Spect.Net.VsPackage.Vsx;
using Spect.Net.VsPackage.Utility;

// ReSharper disable ExplicitCallerInfoArgument

namespace Spect.Net.VsPackage.ToolWindows.Disassembly
{
    /// <summary>
    /// Interaction logic for DisassemblyToolWindowControl.xaml
    /// </summary>
    public partial class DisassemblyToolWindowControl : ISupportsMvvm<DisassemblyToolWindowViewModel>
    {
        public DisassemblyToolWindowViewModel Vm { get; private set; }

        /// <summary>
        /// Sets the view model instance
        /// </summary>
        /// <param name="vm">View model instance to set</param>
        void ISupportsMvvm<DisassemblyToolWindowViewModel>.SetVm(DisassemblyToolWindowViewModel vm)
        {
            if (Vm != null)
            {
                Vm.DisassemblyViewRefreshed -= OnDisassemblyViewRefreshed;
            }
            DataContext = Vm = vm;
            Vm.DisassemblyViewRefreshed += OnDisassemblyViewRefreshed;
        }

        public DisassemblyToolWindowControl()
        {
            InitializeComponent();
            Loaded += OnLoaded;
            PreviewKeyDown += (s, e) => DisassemblyControl.DisassemblyList.HandleListViewKeyEvents(e);
            PreviewKeyDown += (s, arg) => Vm.HandleDebugKeys(arg);
            Prompt.CommandLineEntered += OnCommandLineEntered;
            DisassemblyControl.TopAddressChanged += (s, e) => { Vm.TopAddress = e.NewAddress; };
            DisassemblyControl.ItemClicked += OnItemClicked;
            DisassemblyControl.ItemDoubleClicked += OnItemDoubleClicked;
            DisassemblyControl.ItemTripleClicked += OnItemTripleClicked;
        }

        /// <summary>
        /// Initializes the disassembly when the control is loaded into the memory
        /// </summary>
        private void OnLoaded(object s, RoutedEventArgs e)
        {
            if (!Vm.ViewInitializedWithSolution)
            {
                // --- Set the proper view mode when first initialized 
                Vm.ViewInitializedWithSolution = true;
                if (Vm.VmStopped)
                {
                    Vm.SetRomViewMode(0);
                }
                else
                {
                    Vm.SetFullViewMode();
                }
            }
            ScrollToTop(Vm.VmPaused ? Vm.MachineViewModel.SpectrumVm.Cpu.Registers.PC : (ushort)0);
            Vm.RefreshViewMode();
        }

        /// <summary>
        /// Refreshes the disassembly view whenever the view model asks to do so.
        /// </summary>
        private void OnDisassemblyViewRefreshed(object sender, DisassemblyViewRefreshedEventArgs args)
        {
            DispatchOnUiThread(() =>
            {
                args.RefreshAction?.Invoke();
                RefreshVisibleItems();
                if (Vm.FullViewMode)
                {
                    Vm.UpdatePageInformation();
                }
                if (args.Address != null)
                    ScrollToTop(args.Address.Value);
            });
        }

        /// <summary>
        /// When a valid address is provided, we scroll the memory window to that address
        /// </summary>
        private void OnCommandLineEntered(object sender, CommandLineEventArgs e)
        {
            e.Handled = Vm.ProcessCommandline(e.CommandLine, 
                out var validationMessage, out var newPrompt);
            if (validationMessage != null)
            {
                Prompt.IsValid = false;
                Prompt.ValidationMessage = validationMessage;
            }
            if (newPrompt != null)
            {
                Prompt.CommandText = newPrompt;
                Prompt.CommandLine.CaretIndex = Prompt.CommandText.Length;
            }
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
        /// Create an "L" command to utilize the label address
        /// </summary>
        private async void OnItemClicked(object sender, DisassemblyItemSelectedEventArgs e)
        {
            if (!SpectNetPackage.Default.Options.CommentingMode || e.Selected == null)
            {
                return;
            }
            Prompt.CommandText = $"L {e.Selected.AddressFormatted} ";
            Prompt.CommandLine.CaretIndex = Prompt.CommandText.Length;
            await Task.Delay(10);
            Prompt.SetFocus();
        }

        /// <summary>
        /// Create a "C" command to utilize the clipboard text
        /// </summary>
        private async void OnItemDoubleClicked(object sender, DisassemblyItemSelectedEventArgs e)
        {
            if (!SpectNetPackage.Default.Options.CommentingMode || e.Selected == null)
            {
                return;
            }
            Prompt.CommandText = $"C {e.Selected.AddressFormatted} {Clipboard.GetText()}";
            Prompt.CommandLine.CaretIndex = Prompt.CommandText.Length;
            await Task.Delay(20);
            Prompt.SetFocus();
        }

        /// <summary>
        /// Create a "P" command to utilize the clipboard text
        /// </summary>
        private async void OnItemTripleClicked(object sender, DisassemblyItemSelectedEventArgs e)
        {
            if (!SpectNetPackage.Default.Options.CommentingMode || e.Selected == null)
            {
                return;
            }
            Prompt.CommandText = $"P {e.Selected.AddressFormatted} {Clipboard.GetText()}";
            Prompt.CommandLine.CaretIndex = Prompt.CommandText.Length;
            await Task.Delay(20);
            Prompt.SetFocus();
        }

        /// <summary>
        /// Scrolls the disassembly item with the specified address into view
        /// </summary>
        /// <param name="address">Address to show</param>
        /// <param name="offset">Offset to wind back the top</param>
        public void ScrollToTop(ushort address, int offset = 0)
        {
            var topItem = Vm.DisassemblyItems.FirstOrDefault(i => i.Item.Address >= address);
            if (topItem == null && Vm.DisassemblyItems.Count > 0)
            {
                // --- Take the top line
                topItem = Vm.DisassemblyItems[Vm.DisassemblyItems.Count - 1];
            }

            if (topItem == null)
            {
                // --- The view is empty
                return;
            }

            // --- We found an available address, refresh the view below
            var foundAddress = topItem.Item.Address;
            var index = Vm.LineIndexes[foundAddress];
            if (address < foundAddress && index > 0)
            {
                index--;
            }
            index = offset > index ? 0 : index - offset;
            var sw = DisassemblyControl.DisassemblyList.GetScrollViewer();
            sw?.ScrollToVerticalOffset(index);
        }
    }
}
