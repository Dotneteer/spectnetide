using Spect.Net.Wpf.Mvvm;

namespace Spect.Net.VsPackage.Z80Programs.ExportDisassembly
{
    public class ExportDisassemblyViewModel: EnhancedViewModelBase
    {
        private string _startAddress;
        private string _startAddressHex;
        private string _endAddress;
        private string _endAddressHex;
        private string _filename;
        private bool _hangingLabels;

        private bool _addToProject;
        private IndentDepthType _indentDepth;
        private CommentStyle _commentStyle;
        private LineLengthType _maxLineLengthType;

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
        /// The latest folder for export files
        /// </summary>
        public static string LatestFolder { get; set; }

        /// <summary>
        /// Indicates if labels should be put on separate line
        /// </summary>
        public bool HangingLabels
        {
            get => _hangingLabels;
            set => Set(ref _hangingLabels, value);
        }

        /// <summary>
        /// Indentation depth type
        /// </summary>
        public IndentDepthType IndentDepth
        {
            get => _indentDepth;
            set => Set(ref _indentDepth, value);
        }

        /// <summary>
        /// Indicates the comment style
        /// </summary>
        public CommentStyle CommentStyle
        {
            get => _commentStyle;
            set => Set(ref _commentStyle, value);
        }

        /// <summary>
        /// Indicates the maximum line length
        /// </summary>
        public LineLengthType MaxLineLengthType
        {
            get => _maxLineLengthType;
            set => Set(ref _maxLineLengthType, value);
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