using Spect.Net.VsPackage.ToolWindows;
using Spect.Net.VsPackage.ToolWindows.BankAware;
using System.Windows;

namespace Spect.Net.VsPackage.CustomEditors.RomEditor
{
    /// <summary>
    /// This class defines the user control that displays the ROM
    /// </summary>
    public partial class RomEditorControl
    {
        private MemoryViewModel _vm;

        /// <summary>
        /// The view model that represents the ROM
        /// </summary>
        public MemoryViewModel Vm
        {
            get => _vm;
            set => DataContext = _vm = value;
        }

        public RomEditorControl()
        {
            InitializeComponent();
            PreviewKeyDown += (s, e) => MemoryDumpListBox.HandleListViewKeyEvents(e);
            Prompt.CommandLineEntered += OnCommandLineEntered;
            DataContextChanged += (s, e) => Vm = DataContext as MemoryViewModel;
        }

        /// <summary>
        /// When a valid address is provided, we scroll the memory window to that address
        /// </summary>
        private void OnCommandLineEntered(object sender, CommandLineEventArgs e)
        {
            void SignInvalidCommand()
            {
                Prompt.IsValid = false;
                Prompt.ValidationMessage = "Invalid command syntax";
                e.Handled = false;
            }

            var parser = new RomEditorCommandParser(e.CommandLine);
            switch (parser.Command)
            {
                case RomEditorCommandType.Invalid:
                    SignInvalidCommand();
                    return;

                case RomEditorCommandType.Goto:
                    ScrollToTop(parser.Address);
                    break;

                case RomEditorCommandType.Disassemble:
                    if (!Vm.AllowDisassembly)
                    {
                        SignInvalidCommand();
                        return;
                    }
                    Vm.ShowDisassembly = true;
                    MemoryRow.Height = new GridLength(1, GridUnitType.Star);
                    DisassemblyRow.Height = new GridLength(1, GridUnitType.Star);
                    Vm.Disassembly(parser.Address);
                    break;

                case RomEditorCommandType.ExitDisass:
                    if (!Vm.AllowDisassembly)
                    {
                        SignInvalidCommand();
                        return;
                    }
                    Vm.ShowDisassembly = false;
                    MemoryRow.Height = new GridLength(1, GridUnitType.Star);
                    DisassemblyRow.Height = new GridLength(0);
                    break;
                default:
                    e.Handled = false;
                    return;
            }
            e.Handled = true;
        }

        /// <summary>
        /// Scrolls the disassembly item with the specified address into view
        /// </summary>
        /// <param name="address"></param>
        public void ScrollToTop(ushort address)
        {
            address &= 0xFFF7;
            var sw = MemoryDumpListBox.GetScrollViewer();
            sw?.ScrollToVerticalOffset(address / 16.0);
        }
    }
}
