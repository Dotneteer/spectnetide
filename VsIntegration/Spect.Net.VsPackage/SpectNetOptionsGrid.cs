using System;
using System.ComponentModel;
using Microsoft.VisualStudio.Shell;

namespace Spect.Net.VsPackage
{
    /// <summary>
    /// This class stores the Spect.Net IDE options
    /// </summary>
    public class SpectNetOptionsGrid : DialogPage
    {
        private KeyboardLayoutTypeOptions _keyboardLayoutType = KeyboardLayoutTypeOptions.Default;
        private KeyboardFitTypeOptions _keyboardFitType = KeyboardFitTypeOptions.OriginalSize;

        // --- Virtual machine options
        [Category("Virtual machine")]
        [DisplayName("Use Fast Load")]
        [Description("Specifies if fast load is enabled for loading tape files")]
        public bool UseFastLoad { get; set; } = false;

        [Category("Virtual machine")]
        [DisplayName("SAVE folder")]
        [Description("When the SAVE command is used, the virtual machine strores the " +
                     "resulted .tzx files in this folder")]
        public string SaveFileFolder { get; set; } = null;

        [Category("Virtual machine")]
        [DisplayName("VM State save folder")]
        [Description("This is the default folder when saving virtual machine state (.vmstate) files")]
        public string VmStateSaveFileFolder { get; set; } = @"C:\Temp\VmState";

        [Category("Virtual machine")]
        [DisplayName("VM State project folder")]
        [Description("This is the default project folder when adding virtual machine state (.vmstate) files")]
        public string VmStateProjectFolder { get; set; } = @"VmStates";

        [Category("Virtual machine")]
        [DisplayName("#of SP Events to keep")]
        [Description("The number of Z80 CPU Stack Pointer register events" +
                     " to keep and show in the Z80 CPU Stack tool window")]
        public ushort StackPointerEvents { get; set; } = 16;

        [Category("Virtual machine")]
        [DisplayName("#of Stack Manipulation Events to keep")]
        [Description("The number of Z80 CPU stack manipulation events" +
                     " to keep and show in the Z80 CPU Stack tool window")]
        public ushort StackManipulationEvents { get; set; } = 128;

        [Category("Virtual machine")]
        [DisplayName("Confirm VM state file load")]
        [Description("Displays a confirmation message about VM state file load")]
        public bool ConfirmVmStateLoad { get; set; } = true;

        [Category("Virtual machine")]
        [DisplayName("Virtual floppy disk folder")]
        [Description("Virtual floppy disk files are added to this folder")]
        public string VfddFolder { get; set; } = @"FloppyDisks";

        // --- Run Z80 Code options
        [Category("Run Z80 Code")]
        [DisplayName("Confirm non-zero displacement")]
        [Description("Asks the user to confirm running code with non-zero displacement value")]
        public bool ConfirmNonZeroDisplacement { get; set; } = true;

        [Category("Run Z80 Code")]
        [DisplayName("Confirm machine restart")]
        [Description("Asks the user to confirm restarting the virtual " +
                     "machine before running injected Z80 code")]
        public bool ConfirmMachineRestart { get; set; } = false;

        [Category("Run Z80 Code")]
        [DisplayName("Confirm code start")]
        [Description("Displays a confirmation message about starting the code")]
        public bool ConfirmCodeStart { get; set; } = false;

        [Category("Run Z80 Code")]
        [DisplayName("Confirm successful code compilation")]
        [Description("Displays a confirmation message about successfully compiling code")]
        public bool ConfirmCodeCompile { get; set; } = false;

        [Category("Run Z80 Code")]
        [DisplayName("Confirm code injection")]
        [Description("Displays a confirmation message about injecting the code")]
        public bool ConfirmInjectCode { get; set; } = false;

        [Category("Run Z80 Code")]
        [DisplayName("Predefined debug symbols")]
        [Description("Predefined symbols to use when you start the program in debug mode")]
        public string DebugSymbols { get; set; } = "DEBUG";

        [Category("Run Z80 Code")]
        [DisplayName("Predefined symbols")]
        [Description("Predefined symbols to use when you start the program (both in debug mode and without debugging)")]
        public string RunSymbols { get; set; } = "";

        [Category("Compile Z80 Code")]
        [DisplayName("Generate output list file")]
        [Description("Turn on this option to generate output file when compiling Z80 code")]
        public bool GenerateCompilationList { get; set; } = false;

        [Category("Compile Z80 Code")]
        [DisplayName("Generate list file only for Compile")]
        [Description("Turn on this option to generate output file only for the Compile command but not for " 
                     + "Run, Debug, or Inject.")]
        public bool GenerateForCompileOnly { get; set; } = true;


        [Category("Compile Z80 Code")]
        [DisplayName("Add list file to project")]
        [Description("Turn on this option to add output list file to the current project")]
        public bool AddCompilationToProject { get; set; } = false;

        [Category("Compile Z80 Code")]
        [DisplayName("List file name suffix")]
        [Description("Leave this option empty to omit file name suffix. Set it to a date pattern (e.g. 'dd-mm-yyyy') to add date to the file name.")]
        public string CompilationFileSuffix { get; set; } = null;

        [Category("Compile Z80 Code")]
        [DisplayName("List file project folder")]
        [Description("Compilation list files are added to this folder within the project.")]
        public string CompilationProjectFolder { get; set; } = @"Listings";

        [Category("Compile Z80 Code")]
        [DisplayName("List file save folder")]
        [Description("Compilation list files are saved to this folder.")]
        public string CompilationFileFolder { get; set; } = @"C:\Temp\Listings";

        [Category("Compile Z80 Code")]
        [DisplayName("List file extension")]
        [Description("Use this extension when creating list files.")]
        public string CompilationFileExtension { get; set; } = ".list";

        [Category("Compile Z80 Code")]
        [DisplayName("List file line template")]
        [Description("Template to define the list output line format. Placeholders: {A}: address; {C}: operation codes; " +
                     "{CP}: operation codes padded with spaces (11 characters); {F}: file index, {L}: line number; {S}: source code. " +
                     "Other special characters: '\\t': tabulator.")]
        public string CompilationLineTemplate { get; set; } = "{A} {CX} {F} {L} {S}";

        [Category("Compile Z80 Code")]
        [DisplayName("List file output mode")]
        [Description("This option specifies how source file information should be put into the list file. " +
                     "None: no file information output, except file index; " +
                     "Header: Display a file header for each list section; " + 
                     "FileMap: Display a file map at the beginning of the list.")]
        public ListFileOutputMode ListFileOutputMode { get; set; } = ListFileOutputMode.Header;

        // --- Export Z80 Code options
        [Category("Export Z80 Code")]
        [DisplayName("Confirm code export")]
        [Description("Displays a confirmation message about exporting the code")]
        public bool ConfirmCodeExport { get; set; } = true;

        [Category("Export Z80 Code")]
        [DisplayName("Default export path")]
        [Description("The default path to show in the Export Z80 Code dialog")]
        public string CodeExportPath { get; set; } = @"C:\Temp";

        [Category("Export Z80 Code")]
        [DisplayName("Tape folder")]
        [Description("Exported Z80 code files are added to this folder")]
        public string TapeFolder { get; set; } = @"TapeFiles";

        // --- Debugger options
        [Category("Debugger")]
        [DisplayName("Disable source navigation")]
        [Description("This option allows the user to disable navigation to the source code of the " +
                     "running app, whenever a breakpoint is reached.")]
        public bool DisableSourceNavigation { get; set; } = false;

        [Category("Debugger")]
        [DisplayName("Skip over interrupt method")]
        [Description("Skips over the Z80 instructions executed while the maskable interrupt routine runs.")]
        public bool SkipInterruptRoutine { get; set; } = false;

        [Category("Debugger")]
        [DisplayName("Highlight entire breakpoint line")]
        [Description("If set to true, highlights the entire line of the current breakpoint line; " +
                     "otherwise, only the instruction part.")]
        public bool FullLineHighlight { get; set; } = true;

        [Category("Disassembly View")]
        [DisplayName("Turn on commenting mode")]
        [Description("In commenting mode, you can double-click to prepare an 'C' command for the disassembly clicked.")]
        public bool CommentingMode { get; set; } = false;

        [Category("Disassembly View")]
        [DisplayName("Disassembly export folder")]
        [Description("Disassembly export files are added to this folder")]
        public string DisassExportFolder { get; set; } = @"DisassemblyFiles";

        [Category("Memory View")]
        [DisplayName("Export folder")]
        [Description("Memory export files are added to this folder")]
        public string MemoryExportFolder { get; set; } = @"MemoryFiles";

        [Category("Keyboard Tool")]
        [DisplayName("Keyboard layout")]
        [Description("You can select the type of keyboard layout to display in the Zx Spectrum Keyboard tool window.")]
        public KeyboardLayoutTypeOptions KeyboardLayoutType
        {
            get => _keyboardLayoutType;
            set
            {
                if (_keyboardLayoutType == value) return;
                _keyboardLayoutType = value;
                KeyboardLayoutTypeChanged?.Invoke(this, 
                    new KeyboardLayoutTypeChangedEventArgs(value));
            }
        }

        [Category("Keyboard Tool")]
        [DisplayName("Keyboard display mode")]
        [Description("You can select the display type to use for the ZX Spectrum keyboard tool window.")]
        public KeyboardFitTypeOptions KeyboardFitType
        {
            get => _keyboardFitType;
            set
            {
                if (_keyboardFitType == value) return;
                _keyboardFitType = value;
                KeyboardFitTypeChanged?.Invoke(this,
                    new KeyboardFitTypeChangedEventArgs(value));
            }
        }

        [Category("Unit Tests")]
        [DisplayName("Confirm successful test compilation")]
        [Description("Displays a confirmation message about successfully compiling unit tests.")]
        public bool ConfirmTestCompile { get; set; } = true;

        [Category("Unit Tests")]
        [DisplayName("Confirm machine restart")]
        [Description("Asks the user to confirm restarting the virtual " +
                     "machine before running unit tests.")]
        public bool ConfirmTestMachineRestart { get; set; } = true;

        [Category("Unit Tests")]
        [DisplayName("Verbose execution time logging")]
        [Description("Log detailed information about execution time")]
        public bool VerboseTestExecutionLogging { get; set; } = true;

        [Category("Unit Tests")]
        [DisplayName("Log execution time in T-States")]
        [Description("Log the execution time in Z80 CPU T-States")]
        public bool TestTStateExecutionLogging { get; set; } = true;

        [Category("Diagnostics")]
        [DisplayName("Log I/O port access")]
        [Description("Logs the usage of ZX Spectrum I/O ports")]
        public bool LogIoAccess { get; set; } = false;

        [Category("Diagnostics")]
        [DisplayName("Log Next register access")]
        [Description("Logs the usage of ZX Spectrum Next registers")]
        public bool LogNextRegAccess { get; set; } = false;

        [Category("Diagnostics")]
        [DisplayName("Log floppy controller commands")]
        [Description("Logs the low level commands sent to the uPD765a FDC")]
        public bool LogFloppyCommands { get; set; } = false;

        [Category("Memory View")]
        [DisplayName("BC Highlighted Background")]
        [Description("Background color used for the BC register pair (#rrggbb)")]
        public string BcColor { get; set; } = @"#ff0000";

        [Category("Memory View")]
        [DisplayName("DE Highlighted Background")]
        [Description("Background color used for the DE register pair (#rrggbb)")]
        public string DeColor { get; set; } = @"#00ff00";

        [Category("Memory View")]
        [DisplayName("HL Highlighted Background")]
        [Description("Background color used for the HL register pair (#rrggbb)")]
        public string HlColor { get; set; } = @"#0000ff";

        [Category("Memory View")]
        [DisplayName("IX Highlighted Background")]
        [Description("Background color used for the IX register pair (#rrggbb)")]
        public string IxColor { get; set; } = @"#ffff00";

        [Category("Memory View")]
        [DisplayName("IY Highlighted Background")]
        [Description("Background color used for the IY register pair (#rrggbb)")]
        public string IyColor { get; set; } = @"#ff00ff";

        [Category("Memory View")]
        [DisplayName("SP Highlighted Background")]
        [Description("Background color used for the Stack Pointer (#rrggbb)")]
        public string SpColor { get; set; } = @"#00ffff";

        [Category("Memory View")]
        [DisplayName("PC Highlighted Background")]
        [Description("Background color used for the Program Counter (#rrggbb)")]
        public string PcColor { get; set; } = @"#ffff00";

        [Category("Memory View")]
        [DisplayName("Symbol Border Color")]
        [Description("Border color for memory pointing to a symbol (#rrggbb)")]
        public string SymbolColor { get; set; } = @"OrangeRed";

        [Category("Memory View")]
        [DisplayName("Annotation Border Color")]
        [Description("Border color for memory pointing to an annotation label (#rrggbb)")]
        public string AnnotationColor { get; set; } = @"Green";

        /// <summary>
        /// Signs that the keyboard layout type has changed
        /// </summary>
        public event EventHandler<KeyboardLayoutTypeChangedEventArgs> KeyboardLayoutTypeChanged;

        /// <summary>
        /// Signs that the keyboard fit type has changed
        /// </summary>
        public event EventHandler<KeyboardFitTypeChangedEventArgs> KeyboardFitTypeChanged;
    }

    /// <summary>
    /// Options for list file output
    /// </summary>
    public enum ListFileOutputMode
    {
        None,
        Header,
        FileMap
    }

    /// <summary>
    /// Options for keyboard layout
    /// </summary>
    public enum KeyboardLayoutTypeOptions
    {
        Default = 0,
        Spectrum48,
        Spectrum128
    }

    /// <summary>
    /// Options for fitting the keyboard
    /// </summary>
    public enum KeyboardFitTypeOptions
    {
        OriginalSize,
        Fit
    }

    /// <summary>
    /// Arguments of keyboard layout type changed event
    /// </summary>
    public class KeyboardLayoutTypeChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Layout type set
        /// </summary>
        public KeyboardLayoutTypeOptions LayoutType { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.EventArgs" /> class.
        /// </summary>
        public KeyboardLayoutTypeChangedEventArgs(KeyboardLayoutTypeOptions layoutType)
        {
            LayoutType = layoutType;
        }
    }

    /// <summary>
    /// Arguments of keyboard fit type changed event
    /// </summary>
    public class KeyboardFitTypeChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Fit type set
        /// </summary>
        public KeyboardFitTypeOptions FitType { get; }

        /// <summary>Initializes a new instance of the <see cref="T:System.EventArgs" /> class.</summary>
        public KeyboardFitTypeChangedEventArgs(KeyboardFitTypeOptions fitType)
        {
            FitType = fitType;
        }
    }
}