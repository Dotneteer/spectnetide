using Spect.Net.VsPackage.VsxLibrary.ToolWindow;

namespace Spect.Net.VsPackage.ToolWindows.SpectrumEmulator
{
    /// <summary>
    /// Interaction logic for SpectrumEmulatorToolWindowControl.xaml
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

        public SpectrumEmulatorToolWindowControl()
        {
            InitializeComponent();
        }
    }
}
