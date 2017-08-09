using System.Globalization;
using System.Linq;
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
                if (Vm.VmStopped)
                {
                    Vm.Clear();
                }
                else
                {
                    Vm.Disassemble();
                }
            });
        }

        /// <summary>
        /// When a valid address is provided, we scroll the memory window to that address
        /// </summary>
        private void OnCommandLineEntered(object sender, CommandLineEventArgs e)
        {
            if (ushort.TryParse(e.CommandLine, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out ushort addr))
            {
                ScrollToTop(addr);
                e.Handled = true;
            }
        }

        /// <summary>
        /// Scrolls the disassembly item with the specified address into view
        /// </summary>
        /// <param name="address"></param>
        public void ScrollToTop(ushort address)
        {
            var topItem = Vm.DisassemblyItems.FirstOrDefault(i => i.Item.Address >= address) 
                ?? Vm.DisassemblyItems[Vm.DisassemblyItems.Count - 1];
            var foundAddress = topItem.Item.Address;
            var index = Vm.LineIndexes[foundAddress];
            if (address < foundAddress && index > 0)
            {
                index--;
            }
            var sw = DisassemblyList.GetScrollViewer();
            sw?.ScrollToVerticalOffset(index);
        }
    }
}
