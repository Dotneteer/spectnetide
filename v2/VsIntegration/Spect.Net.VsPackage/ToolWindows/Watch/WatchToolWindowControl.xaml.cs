using Spect.Net.VsPackage.VsxLibrary.ToolWindow;
using System.Windows;
using System.Windows.Input;

namespace Spect.Net.VsPackage.ToolWindows.Watch
{
    /// <summary>
    /// Interaction logic for WatchToolWindowControl.xaml
    /// </summary>
    public partial class WatchToolWindowControl : ISupportsMvvm<WatchToolWindowViewModel>
    {
        /// <summary>
        /// Gets the view model instance
        /// </summary>
        public WatchToolWindowViewModel Vm { get; private set; }

        /// <summary>
        /// Sets the view model instance
        /// </summary>
        /// <param name="vm">View model instance to set</param>
        public void SetVm(WatchToolWindowViewModel vm)
        {
            DataContext = Vm = vm;
            Vm.CommandLineModified += OnCommandLineModified;
        }

        private void OnCommandLineModified(object sender, string commandText)
        {
            Prompt.CommandText = commandText;
            Prompt.CommandLine.CaretIndex = Prompt.CommandText.Length;

        }

        public WatchToolWindowControl()
        {
            InitializeComponent();
            Loaded += WatchToolWindowControl_Loaded;
            Unloaded += WatchToolWindowControl_Unloaded;
        }

        private void WatchToolWindowControl_Loaded(object sender, RoutedEventArgs e)
        {
            PreviewKeyDown += WatchToolWindowControl_PreviewKeyDown;
            Prompt.CommandLineEntered += OnCommandLineEntered;
        }

        private void WatchToolWindowControl_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            Vm.HandleDebugKeys(e);
        }

        private void WatchToolWindowControl_Unloaded(object sender, System.Windows.RoutedEventArgs e)
        {
            PreviewKeyDown -= WatchToolWindowControl_PreviewKeyDown;
            // ReSharper disable once DelegateSubtraction
            Prompt.CommandLineEntered -= OnCommandLineEntered;
            if (Vm != null)
            {
                Vm.CommandLineModified -= OnCommandLineModified;
            }
        }


        /// <summary>
        /// When a valid address is provided, we scroll the memory window to that address
        /// </summary>
        private void OnCommandLineEntered(object sender, CommandLineEventArgs e)
        {
            e.Handled = Vm.ProcessCommandline(e.CommandLine,
                out var validationMessage);
            if (validationMessage != null)
            {
                Prompt.IsValid = false;
                Prompt.ValidationMessage = validationMessage;
            }
        }

        private void SpectrumToolWindowControlBase_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Vm.HandleSizing(e.GetPosition(this));
        }

        private void SpectrumToolWindowControlBase_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Vm.EndSizing();
        }
    }
}
