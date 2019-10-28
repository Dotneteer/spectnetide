using System.Windows.Input;
using Spect.Net.VsPackage.VsxLibrary.ToolWindow;

namespace Spect.Net.VsPackage.ToolWindows.SpectrumEmulator
{
    /// <summary>
    /// Interaction logic for SpectrumEmulatorToolWindowControl.
    /// </summary>
    public partial class SpectrumEmulatorToolWindowControl : ISupportsMvvm<SpectrumEmulatorToolWindowViewModel>
    {
        /// <summary>
        /// The view model behind this control
        /// </summary>
        public SpectrumEmulatorToolWindowViewModel Vm { get; private set; }

        /// <summary>
        /// Sets the view model instance
        /// </summary>
        /// <param name="vm">View model instance to set</param>
        void ISupportsMvvm<SpectrumEmulatorToolWindowViewModel>.SetVm(SpectrumEmulatorToolWindowViewModel vm)
        {
            DataContext = Vm = vm;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SpectrumEmulatorToolWindowViewModel"/> class.
        /// </summary>
        public SpectrumEmulatorToolWindowControl()
        {
            InitializeComponent();
            PreviewKeyDown += OnPreviewKeyDown;
        }

        private void OnPreviewKeyDown(object s, KeyEventArgs arg)
        {
            // --- We prevent the up and down arrow keys to move the focus to the 
            // --- toolbar buttons. If we did not do that, pressing space or Enter
            // --- might activate the Stop or Pause buttons while those have the focus.
            if (arg.Key == Key.Down || arg.Key == Key.Up)
            {
                arg.Handled = true;
            }
            //Vm.HandleDebugKeys(arg);
        }
    }
}