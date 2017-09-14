using Spect.Net.VsPackage.Vsx;

namespace Spect.Net.VsPackage.Tools.KeyboardTool
{
    /// <summary>
    /// Interaction logic for KeyboardToolWindowControl.xaml
    /// </summary>
    public partial class KeyboardToolWindowControl: ISupportsMvvm<KeyboardToolViewModel>
    {
        /// <summary>
        /// Gets the view model instance
        /// </summary>
        public KeyboardToolViewModel Vm { get; private set; }

        /// <summary>
        /// Sets the view model instance
        /// </summary>
        /// <param name="vm">View model instance to set</param>
        void ISupportsMvvm<KeyboardToolViewModel>.SetVm(KeyboardToolViewModel vm)
        {
            DataContext = Vm = vm;
        }

        public KeyboardToolWindowControl()
        {
            InitializeComponent();
        }
    }
}
