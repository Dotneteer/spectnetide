using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using GalaSoft.MvvmLight.Messaging;
using Spect.Net.Wpf.SpectrumControl;

namespace Spect.Net.VsPackage.Tools.Memory
{
    /// <summary>
    /// Interaction logic for MemoryToolWindowControl.xaml
    /// </summary>
    public partial class MemoryToolWindowControl
    {
        private int _screenRefreshCount = 0;

        public SpectrumMemoryViewModel Vm { get; }

        public MemoryToolWindowControl()
        {
            InitializeComponent();
            DataContext = Vm = new SpectrumMemoryViewModel();
            Loaded += (s, e) =>
            {
                Messenger.Default.Register<SpectrumVmStateChangedMessage>(this, OnVmStateChanged);
                Messenger.Default.Register<SpectrumScreenRefreshedMessage>(this, OnScreenRefreshed);
            };
            Unloaded += (s, e) =>
            {
                Messenger.Default.Unregister<SpectrumVmStateChangedMessage>(this);
                Messenger.Default.Unregister<SpectrumScreenRefreshedMessage>(this);
            };
        }

        private void OnScreenRefreshed(SpectrumScreenRefreshedMessage obj)
        {
            if (_screenRefreshCount++ % 10 == 0)
            {
                Dispatcher.InvokeAsync(RefreshVisibleItems);
            }
        }

        /// <summary>
        /// Whenever the state of the Spectrum virtual machine changes,
        /// we refrehs the memory dump
        /// </summary>
        /// <param name="obj"></param>
        private void OnVmStateChanged(SpectrumVmStateChangedMessage obj)
        {
            if (Vm.VmStopped || Vm.VmPaused)
            {
                Vm.RefreshMemoryLines();
            }
            else
            {
                RefreshVisibleItems();
            }
        }

        private void RefreshVisibleItems()
        {
            var stack = GetInnerStackPanel(MemoryDumpListBox);
            for (var i = 0; i < stack.Children.Count; i++)
            {
                var memLine = (stack.Children[i] as FrameworkElement)?.DataContext as MemoryLineViewModel;
                if (memLine != null)
                {
                    Vm.RefreshMemoryLine(memLine.BaseAddress);
                }
            }
        }

        private static VirtualizingStackPanel GetInnerStackPanel(DependencyObject element)
        {
            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
            {
                var child = VisualTreeHelper.GetChild(element, i) as FrameworkElement;
                if (child == null) continue;

                if (child is VirtualizingStackPanel)
                {
                    return child as VirtualizingStackPanel;
                }
                var panel = GetInnerStackPanel(child);
                if (panel != null)
                {
                    return panel;
                }
            }
            return null;
        }
    }
}
