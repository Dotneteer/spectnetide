using Spect.Net.VsPackage.VsxLibrary.ToolWindow;

namespace Spect.Net.VsPackage.ToolWindows.Keyboard
{
    /// <summary>
    /// Interaction logic for KeyboardToolWindowControl.xaml
    /// </summary>
    public partial class KeyboardToolWindowControl : ISupportsMvvm<KeyboardToolWindowViewModel>
    {
        /// <summary>
        /// Gets the view model instance
        /// </summary>
        public KeyboardToolWindowViewModel Vm { get; private set; }

        /// <summary>
        /// Sets the view model instance
        /// </summary>
        /// <param name="vm">View model instance to set</param>
        public void SetVm(KeyboardToolWindowViewModel vm)
        {
            DataContext = Vm = vm;
        }

        public KeyboardToolWindowControl()
        {
            InitializeComponent();
        }
    }
}
