using System.Globalization;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight.Messaging;
using Spect.Net.VsPackage.Utility;
using Spect.Net.Wpf.SpectrumControl;

namespace Spect.Net.VsPackage.Tools.Memory
{
    /// <summary>
    /// Interaction logic for MemoryToolWindowControl.xaml
    /// </summary>
    public partial class MemoryToolWindowControl
    {
        public SpectrumMemoryViewModel Vm { get; }

        public MemoryToolWindowControl()
        {
            InitializeComponent();
            DataContext = Vm = new SpectrumMemoryViewModel();
            Loaded += (s, e) =>
            {
                Messenger.Default.Register<SpectrumScreenRefreshedMessage>(this, OnScreenRefreshed);
            };
            Unloaded += (s, e) =>
            {
                Messenger.Default.Unregister<SpectrumScreenRefreshedMessage>(this);
            };
            PreviewKeyDown += (s, e) => MemoryDumpListBox.HandleListViewKeyEvents(e);
            Prompt.CommandLineEntered += OnCommandLineEntered;
            Prompt.PreviewCommandLineInput += OnPreviewCommandLineInput;
        }

        /// <summary>
        /// Foe every 10th rendered frame, we refresh the memory map.
        /// </summary>
        /// <param name="msg"></param>
        private void OnScreenRefreshed(SpectrumScreenRefreshedMessage msg)
        {
            if (Vm.ScreenRefreshCount % 10 == 0)
            {
                Dispatcher.InvokeAsync(RefreshVisibleItems);
            }
        }

        /// <summary>
        /// We accept only hexadecimal address written into the command line prompt
        /// </summary>
        private static void OnPreviewCommandLineInput(object sender, TextCompositionEventArgs e)
        {
            if (!int.TryParse(e.TextComposition.Text, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out int _))
            {
                e.Handled = true;
            }
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
        /// This method refreshes only those memory items that are visible in the 
        /// current viewport
        /// </summary>
        private void RefreshVisibleItems()
        {
            var stack = MemoryDumpListBox.GetInnerStackPanel();
            for (var i = 0; i < stack.Children.Count; i++)
            {
                if ((stack.Children[i] as FrameworkElement)?.DataContext is MemoryLineViewModel memLine)
                {
                    Vm.RefreshMemoryLine(memLine.BaseAddress);
                }
            }
        }

        /// <summary>
        /// Scrolls the disassembly item with the specified address into view
        /// </summary>
        /// <param name="address"></param>
        public void ScrollToTop(ushort address)
        {
            address &= 0xFFF7;
            var sw = MemoryDumpListBox.GetScrollViewer();
            sw?.ScrollToVerticalOffset(address/16.0);
        }
    }
}
