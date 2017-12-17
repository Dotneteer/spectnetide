namespace Spect.Net.VsPackage.ToolWindows.KeyboardTool
{
    /// <summary>
    /// The view model of the Keyboard tool window
    /// </summary>
    public class KeyboardToolViewModel : SpectrumGenericToolWindowViewModel
    {
        private KeyboardLayoutTypeOptions _keyboardLayoutType;
        private KeyboardFitTypeOptions _keyboardFitType;

        /// <summary>
        /// Keyboard layout type
        /// </summary>
        public KeyboardLayoutTypeOptions KeyboardLayoutType
        {
            get => _keyboardLayoutType;
            set
            {
                if (Set(ref _keyboardLayoutType, value))
                {
                    RaisePropertyChanged(nameof(IsSpectrum48Layout));
                }
            }
        }

        /// <summary>
        /// Spectrum 48 layout is selected?
        /// </summary>
        public bool IsSpectrum48Layout => 
            KeyboardLayoutType == KeyboardLayoutTypeOptions.Default && SpectNetPackage.IsSpectrum48Model() 
            || KeyboardLayoutType == KeyboardLayoutTypeOptions.Spectrum48;

        /// <summary>
        /// Keyboard fit type
        /// </summary>
        public KeyboardFitTypeOptions KeyboardFitType
        {
            get => _keyboardFitType;
            set
            {
                if (!Set(ref _keyboardFitType, value)) return;
                RaisePropertyChanged(nameof(IsOriginalSize));
            }
        }

        /// <summary>
        /// Is the keyboard displayed in original size?
        /// </summary>
        public bool IsOriginalSize => _keyboardFitType == KeyboardFitTypeOptions.OriginalSize;

        /// <summary>
        /// Instantiates this view model
        /// </summary>
        public KeyboardToolViewModel()
        {
            if (IsInDesignMode)
            {
                KeyboardLayoutType = KeyboardLayoutTypeOptions.Spectrum128;
                KeyboardFitType = KeyboardFitTypeOptions.OriginalSize;
                return;
            }

            KeyboardLayoutType = SpectNetPackage.Default.Options.KeyboardLayoutType;
            KeyboardFitType = SpectNetPackage.Default.Options.KeyboardFitType;

            SpectNetPackage.Default.Options.KeyboardLayoutTypeChanged 
                += OnKeyboardLayoutTypeChanged;
            SpectNetPackage.Default.Options.KeyboardFitTypeChanged
                += OnKeyboardFitTypeChanged;
        }

        /// <summary>
        /// Handle the event when the keyboard layout type has changed
        /// </summary>
        private void OnKeyboardLayoutTypeChanged(object sender, KeyboardLayoutTypeChangedEventArgs args)
        {
            KeyboardLayoutType = args.LayoutType;
        }

        /// <summary>
        /// Handle the event when the keyboard fit type has changed
        /// </summary>
        private void OnKeyboardFitTypeChanged(object sender, KeyboardFitTypeChangedEventArgs args)
        {
            KeyboardFitType = args.FitType;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, 
        /// or resetting unmanaged resources.
        /// </summary>
        public override void Dispose()
        {
            SpectNetPackage.Default.Options.KeyboardLayoutTypeChanged
                -= OnKeyboardLayoutTypeChanged;
            SpectNetPackage.Default.Options.KeyboardFitTypeChanged
                -= OnKeyboardFitTypeChanged;
            base.Dispose();
        }
    }
}