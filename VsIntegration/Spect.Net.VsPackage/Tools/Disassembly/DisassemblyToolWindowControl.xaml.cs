using System.Linq;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight.Messaging;
using Spect.Net.VsPackage.Utility;
using Spect.Net.VsPackage.Vsx;

// ReSharper disable ExplicitCallerInfoArgument

namespace Spect.Net.VsPackage.Tools.Disassembly
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
            DataContext = Vm = vm;
        }

        public DisassemblyToolWindowControl()
        {
            InitializeComponent();
            Loaded += (s, e) =>
            {
                Messenger.Default.Register<RefreshDisassemblyViewMessage>(this, OnRefreshView);
                Vm.EvaluateState();
                if (Vm.VmNotStopped)
                {
                    Vm.Disassemble();
                }
                Vm.SaveRomChangesToRom = true;
                UpdateRomChangesState();
            };
            Unloaded += (s, e) =>
            {
                Messenger.Default.Unregister<RefreshDisassemblyViewMessage>(this);
            };
            PreviewKeyDown += (s, e) => DisassemblyControl.DisassemblyList.HandleListViewKeyEvents(e);
            PreviewKeyDown += (s, arg) => Vm.HandleDebugKeys(arg);
            Prompt.CommandLineEntered += OnCommandLineEntered;
        }

        /// <summary>
        /// Refreshes the disassembly view whenver the view model asks to do so.
        /// </summary>
        private void OnRefreshView(RefreshDisassemblyViewMessage msg)
        {
            DispatchOnUiThread(() =>
            {
                RefreshVisibleItems();
                if (msg.Address != null)
                ScrollToTop(msg.Address.Value);
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
                UpdateRomChangesState();
            }
        }

        private void UpdateRomChangesState()
        {
            VisualStateManager.GoToState(this, "SaveRomChangesToRom_"
            + (Vm.AnnotationHandler.SaveRomChangesToRom ? "True" : "False"), true);
        }
    }
}
