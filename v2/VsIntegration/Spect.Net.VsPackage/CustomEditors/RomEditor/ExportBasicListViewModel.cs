using Spect.Net.Wpf.Mvvm;

namespace Spect.Net.VsPackage.CustomEditors.RomEditor
{
    public class ExportBasicListViewModel : EnhancedViewModelBase
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
        /// The latest folder for export files
        /// </summary>
        public static string LatestFolder { get; set; }

        /// <summary>
        /// Signs if the dialog content is valid
        /// </summary>
        public bool IsValid => !string.IsNullOrWhiteSpace(Filename);
    }
}
