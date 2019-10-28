using Spect.Net.SpectrumEmu.Abstraction.Devices.Floppy;
using Spect.Net.Wpf.Mvvm;
using System.IO;

namespace Spect.Net.VsPackage.Dialogs.Vfdd
{
    /// <summary>
    /// This class represents the view model for the Create virtual floppy disk 
    /// file dialog
    /// </summary>
    public class CreateVfddFileViewModel : EnhancedViewModelBase
    {
        private FloppyFormat _format;
        private string _filename;

        /// <summary>
        /// Gets or sets the tape format of the export
        /// </summary>
        public FloppyFormat Format
        {
            get => _format;
            set
            {
                if (Set(ref _format, value))
                {
                    Filename = Path.ChangeExtension(Filename, "vfdd");
                }
            }
        }

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
        public bool IsValid =>
            !string.IsNullOrWhiteSpace(Filename)
            && Format >= FloppyFormat.SpectrumP3
            && Format <= FloppyFormat.Pcw;
    }
}
