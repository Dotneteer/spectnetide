using System.Windows;
using System.Windows.Input;
using Spect.Net.VsPackage.Utility;

namespace Spect.Net.VsPackage.ToolWindows.Watch
{
    /// <summary>
    /// Interaction logic for WatchItemControl.xaml
    /// </summary>
    public partial class WatchItemControl
    {
        private WatchItemViewModel Vm => DataContext as WatchItemViewModel;
        private DependencyObject _parent;

        public WatchItemControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Starts sizing the label width
        /// </summary>
        private void OnBorderMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Vm == null || e.LeftButton != MouseButtonState.Pressed) return;
            _parent = this.GetParent<WatchToolWindowControl>();
            if (_parent == null) return;
            Mouse.Capture((IInputElement) _parent, CaptureMode.SubTree);
            Vm.StartSizing(e.GetPosition((IInputElement)_parent));
        }
    }
}
