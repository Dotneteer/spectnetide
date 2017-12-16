using System.Windows;
using Spect.Net.VsPackage.Vsx;

namespace Spect.Net.VsPackage.ToolWindows.KeyboardTool
{
    /// <summary>
    /// Interaction logic for CompoundKeyboardToolWindowControl.xaml
    /// </summary>
    public partial class CompoundKeyboardToolWindowControl : ISupportsMvvm<KeyboardToolViewModel>
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
            Spectrum48Keyboard.SetVm(vm);
            Spectrum128Keyboard.SetVm(vm);
            Spectrum48Keyboard2.SetVm(vm);
            Spectrum128Keyboard2.SetVm(vm);
        }

        public CompoundKeyboardToolWindowControl()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        /// <summary>
        /// Refresh the keyboard layout when displaying the control
        /// </summary>
        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            Vm.RaisePropertyChanged(nameof(KeyboardToolViewModel.IsSpectrum48Layout));
            Vm.RaisePropertyChanged(nameof(KeyboardToolViewModel.IsOriginalSize));
        }
    }
}
