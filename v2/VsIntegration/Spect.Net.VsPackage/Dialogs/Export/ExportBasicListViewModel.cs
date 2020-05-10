using Spect.Net.Wpf.Mvvm;
using System.IO;

namespace Spect.Net.VsPackage.Dialogs.Export
{
    public class ExportBasicListViewModel : EnhancedViewModelBase
    {
        private string _filename;
        private bool _mimicZxBasic;

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
        /// Tries to convert a part of syntax to ZX BASIC
        /// </summary>
        public bool MimicZxBasic
        {
            get => _mimicZxBasic;
            set
            {
                if (!Set(ref _mimicZxBasic, value)) return;
                Filename = Path.ChangeExtension(Filename, _mimicZxBasic ? ".zxbas" : ".bas" );
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
