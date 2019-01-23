using Spect.Net.VsPackage.Vsx;

namespace Spect.Net.VsPackage.ToolWindows.Watch
{
    /// <summary>
    /// Interaction logic for WatchToolWindowControl.xaml
    /// </summary>
    public partial class WatchToolWindowControl: ISupportsMvvm<WatchToolWindowViewModel>
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
        }

        public WatchToolWindowControl()
        {
            InitializeComponent();
            PreviewKeyDown += (s, arg) => Vm?.HandleDebugKeys(arg);
            Prompt.CommandLineEntered += OnCommandLineEntered;
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
