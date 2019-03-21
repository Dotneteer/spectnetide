using Spect.Net.Wpf.Mvvm;

namespace Spect.Net.VsPackage.ToolWindows.SpectrumEmulator
{
    /// <summary>
    /// This class represents the view model of the 
    /// Add VM state to project command's UI
    /// </summary>
    public class AddVmStateViewModel: EnhancedViewModelBase
    {
        private string _filename;

        /// <summary>
        /// Gets or sets the name of the file (with full path) to save the code
        /// </summary>
        public string Filename
        {
            get => _filename;
            set
            {
                Set(ref _filename, value);
                RaisePropertyChanged(nameof(IsValid));
            }
        }

        /// <summary>
        /// Signs if the dialog content is valid
        /// </summary>
        public bool IsValid => !string.IsNullOrWhiteSpace(Filename);
    }
}