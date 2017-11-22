using System.Globalization;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight.Messaging;
using Spect.Net.VsPackage.Utility;
using Spect.Net.VsPackage.Vsx;
using Spect.Net.Wpf.Mvvm.Messages;

namespace Spect.Net.VsPackage.ToolWindows.Memory
{
    /// <summary>
    /// Interaction logic for MemoryToolWindowControl.xaml
    /// </summary>
    public partial class MemoryToolWindowControl : ISupportsMvvm<MemoryToolWindowViewModel>
    {
        public MemoryToolWindowViewModel Vm { get; private set; }

        /// <summary>
        /// Sets the view model instance
        /// </summary>
        /// <param name="vm">View model instance to set</param>
        void ISupportsMvvm<MemoryToolWindowViewModel>.SetVm(MemoryToolWindowViewModel vm)
        {
            DataContext = Vm = vm;
        }

        public MemoryToolWindowControl()
        {
            InitializeComponent();
            PreviewKeyDown += (s, e) => MemoryDumpListBox.HandleListViewKeyEvents(e);
            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
            Prompt.CommandLineEntered += OnCommandLineEntered;
            Prompt.PreviewCommandLineInput += OnPreviewCommandLineInput;
        }

        /// <summary>
        /// The control is loaded
        /// </summary>
        private void OnLoaded(object s, RoutedEventArgs e)
        {
            Messenger.Default.Register<RefreshMemoryViewMessage>(this, OnRefreshView);
            if (!Vm.InitializedWithSolution)
            {
                Vm.InitializedWithSolution = true;
                Vm.EvaluateState();
                if (Vm.VmStopped)
                {
                    Vm.SetRomView(0);                    
                }
                else
                {
                    Vm.SetFullView();
                }
            }
            Vm.RefreshMemoryLines();
        }

        private void OnUnloaded(object s, RoutedEventArgs e)
        {
            Messenger.Default.Unregister<RefreshMemoryViewMessage>(this);
        }

        /// <summary>
        /// We refresh the memory map.
        /// </summary>
        private void OnRefreshView(RefreshMemoryViewMessage obj)
        {
            DispatchOnUiThread(RefreshVisibleItems);
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
            if (ushort.TryParse(e.CommandLine, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var addr))
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
