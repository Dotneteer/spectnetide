using Spect.Net.Wpf.Mvvm;

namespace Spect.Net.VsPackage.Z80Programs.ExportMemory
{

    public class ExportMemoryViewModel: EnhancedViewModelBase
    {
        private string _startAddress;
        private string _startAddressHex;
        private string _endAddress;
        private string _endAddressHex;
        private string _filename;
        private bool _addToProject;

        /// <summary>
        /// The latest export folder
        /// </summary>
        public static string LatestFolder { get; set; }

        /// <summary>
        /// Start address of the code
        /// </summary>
        public string StartAddress
        {
            get => _startAddress;
            set
            {
                if (Set(ref _startAddress, value))
                {
                    RaisePropertyChanged(nameof(IsValid));
                    StartAddressHex = int.TryParse(value, out var intVal)
                        ? $"#{intVal:X4}"
                        : "#????";
                }
            }
        }

        /// <summary>
        /// Start address of the code in hex format
        /// </summary>
        public string StartAddressHex
        {
            get => _startAddressHex;
            set => Set(ref _startAddressHex, value);
        }

        /// <summary>
        /// End address of the code
        /// </summary>
        public string EndAddress
        {
            get => _endAddress;
            set
            {
                if (Set(ref _endAddress, value))
                {
                    RaisePropertyChanged(nameof(IsValid));
                    EndAddressHex = int.TryParse(value, out var intVal)
                        ? $"#{intVal:X4}"
                        : "#????";
                }
            }
        }

        /// <summary>
        /// Start address of the code in hex format
        /// </summary>
        public string EndAddressHex
        {
            get => _endAddressHex;
            set => Set(ref _endAddressHex, value);
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
        /// Indicates if the saved project file should be added to the project
        /// </summary>
        public bool AddToProject
        {
            get => _addToProject;
            set => Set(ref _addToProject, value);
        }

        /// <summary>
        /// Signs if the dialog content is valid
        /// </summary>
        public bool IsValid =>
            !string.IsNullOrWhiteSpace(Filename)
            && !string.IsNullOrWhiteSpace(StartAddress)
            && !string.IsNullOrWhiteSpace(EndAddress);
    }
}