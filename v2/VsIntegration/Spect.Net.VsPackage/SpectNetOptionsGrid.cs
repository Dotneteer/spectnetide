using System;
using System.ComponentModel;
using Microsoft.VisualStudio.Shell;
using Spect.Net.VsPackage.VsxLibrary;

// ReSharper disable UnusedMember.Global

namespace Spect.Net.VsPackage
{
    /// <summary>
    /// This class stores the Spect.Net IDE options
    /// </summary>
    public class SpectNetOptionsGrid : DialogPage
    {
        private KeyboardLayoutTypeOptions _keyboardLayoutType = KeyboardLayoutTypeOptions.Default;
        private KeyboardFitTypeOptions _keyboardFitType = KeyboardFitTypeOptions.OriginalSize;
        private ZoomFactor _zoomFactor = ZoomFactor.One;
        private KeyboardShiftOptions _keyboardShift = KeyboardShiftOptions.Normal;
        private byte _optimize = 1;

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
        [DisplayName("Add saved files to project")]
        [Description("Automatically adds saved files to the active project")]
        public bool AddSavedFilesToProject { get; set; } = true;

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

        [Category("Virtual machine")]
        [DisplayName("Tool window zoom factor")]
        [Description("Sets the zoom factor of ZX Spectrum related tool windows")]
        [TypeConverter(typeof(TypedEnumConverter<ZoomFactor>))]
        public ZoomFactor ToolWindowZoomFactor
        {
            get => _zoomFactor;
            set
            {
                if (_zoomFactor == value) return;

                _zoomFactor = value;
                ZoomFactorChanged?.Invoke(this, new ZoomFactorChangedEventArgs(value));
            }
        } 

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

        [Category("Assembler Listing Output File")]
        [DisplayName("Generate listing output file")]
        [Description("Turn on this option to generate output file when compiling Z80 code")]
        public bool GenerateCompilationList { get; set; } = false;

        [Category("Assembler Listing Output File")]
        [DisplayName("Generate listing file only for Compile")]
        [Description("Turn on this option to generate output file only for the Compile command but not for "
                     + "Run, Debug, or Inject.")]
        public bool GenerateForCompileOnly { get; set; } = true;

        [Category("Assembler Listing Output File")]
        [DisplayName("Add listing file to project")]
        [Description("Turn on this option to add output list file to the current project")]
        public bool AddCompilationToProject { get; set; } = false;

        [Category("Assembler Listing Output File")]
        [DisplayName("Listing file name suffix")]
        [Description("Leave this option empty to omit file name suffix. Set it to a date pattern (e.g. 'dd-mm-yyyy') to add date to the file name.")]
        public string CompilationFileSuffix { get; set; } = null;

        [Category("Assembler Listing Output File")]
        [DisplayName("Listing file project folder")]
        [Description("Compilation listing files are added to this folder within the project.")]
        public string CompilationProjectFolder { get; set; } = @"Listings";

        [Category("Assembler Listing Output File")]
        [DisplayName("Listing file save folder")]
        [Description("Compilation list files are saved to this folder.")]
        public string CompilationFileFolder { get; set; } = @"C:\Temp\Listings";

        [Category("Assembler Listing Output File")]
        [DisplayName("Listing file extension")]
        [Description("Use this extension when creating list files.")]
        public string CompilationFileExtension { get; set; } = ".list";

        [Category("Assembler Listing Output File")]
        [DisplayName("Listing file line template")]
        [Description("Template to define the listing output line format. Placeholders: {A}: address; {C}: operation codes; " +
                     "{CP}: operation codes padded with spaces (11 characters); {F}: file index, {L}: line number; {S}: source code. " +
                     "Other special characters: '\\t': tabulator.")]
        public string CompilationLineTemplate { get; set; } = "{A} {CX} {F} {L} {S}";

        [Category("Assembler Listing Output File")]
        [DisplayName("Source file name output mode")]
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
        [DisplayName("Export after compile")]
        [Description("Specifies optional export on compile: <format:(tap|tzx|hex)>; <auto-start:(0|1)>; <clear:(0|1)>; <pause-0:(0|1)>; <start-address:(int)>; <border-color:(int)>; <screen-file-path:(path)>")]
        public string ExportOnCompile { get; set; } = null;

        [Category("Export Z80 Code")]
        [DisplayName("Default export path")]
        [Description("The default path to show in the Export Z80 Code dialog")]
        public string CodeExportPath { get; set; } = @"C:\Temp";

        [Category("Export Z80 Code")]
        [DisplayName("Tape folder")]
        [Description("Exported Z80 code files and saved files are added to this folder")]
        public string TapeFolder { get; set; } = @"TapeFiles";

        // --- Debugger options
        [Category("Debugger")]
        [DisplayName("Disable source navigation")]
        [Description("This option allows the user to disable navigation to the source code of the " +
                     "running app, whenever a breakpoint is reached.")]
        public bool DisableSourceNavigation { get; set; } = false;

        [Category("Debugger")]
        [DisplayName("Force showing source code")]
        [Description("When a breakpoint is reached, forces displaying the source code")]
        public bool ForceShowSourceCode { get; set; } = true;

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
        [TypeConverter(typeof(TypedEnumConverter<KeyboardLayoutTypeOptions>))]
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
        [TypeConverter(typeof(TypedEnumConverter<KeyboardFitTypeOptions>))]
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

        [Category("Keyboard Tool")]
        [DisplayName("Keyboard shift keys")]
        [Description("You can select how the physical keys are bound to the ZX Spectrum shift keys.")]
        [TypeConverter(typeof(TypedEnumConverter<KeyboardShiftOptions>))]
        public KeyboardShiftOptions KeyboardShift
        {
            get => _keyboardShift;
            set
            {
                if (_keyboardShift == value) return;
                _keyboardShift = value;
                KeyboardShiftTypeChanged?.Invoke(this,
                    new KeyboardShiftTypeChangedEventArgs(value));
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

        [Category("Diagnostics")]
        [DisplayName("ASM editor syntax highligting")]
        [Description("Turns on or off ASM syntax highlighting")]
        public bool AsmSyntaxHighlighting { get; set; } = true;

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

        [Category("ZX BASIC options")]
        [DisplayName("ZXB utility path")]
        [Description("The full path where the ZXB utility (ZXB.EXE) can be found.")]
        public string ZxbPath { get; set; }

        [Category("ZX BASIC options")]
        [DisplayName("Optimization level")]
        [Description("The optimization level to use with the --optimize option of ZXB")]
        public byte Optimize
        {
            get => _optimize;
            set
            {
                _optimize = value;
                if (_optimize > 4) _optimize = 4;
            }
        }

        [Category("ZX BASIC options")]
        [DisplayName("Machine code origin")]
        [Description("The machine code origin value to use with the --org option of ZXB")]
        public ushort OrgValue { get; set; } = 0x8000;

        [Category("ZX BASIC options")]
        [DisplayName("Use 1 as array base index")]
        [Description("The --array-base option of ZXB")]
        public bool ArrayBaseOne { get; set; }

        [Category("ZX BASIC options")]
        [DisplayName("Use 1 as string base index")]
        [Description("The --string-base option of ZXB")]
        public bool StringBaseOne { get; set; }

        [Category("ZX BASIC options")]
        [DisplayName("String manipulation heap size")]
        [Description("The --heap-size option of ZXB")]
        public ushort HeapSize { get; set; } = 4096;

        [Category("ZX BASIC options")]
        [DisplayName("Checking heap size")]
        [Description("The --debug-memory option of ZXB")]
        public bool DebugMemory { get; set; } = false;

        [Category("ZX BASIC options")]
        [DisplayName("Check array index boundaries")]
        [Description("The --debug-array option of ZXB")]
        public bool DebugArray { get; set; } = false;

        [Category("ZX BASIC options")]
        [DisplayName("Use strict Boolean values")]
        [Description("The --strict-bool option of ZXB")]
        public bool StrictBool { get; set; } = false;

        [Category("ZX BASIC options")]
        [DisplayName("Enable BREAK key")]
        [Description("The --strict-bool option of ZXB")]
        public bool EnableBreak { get; set; } = false;

        [Category("ZX BASIC options")]
        [DisplayName("Require explicit DIM")]
        [Description("The --explicit option of ZXB")]
        public bool ExplicitDim { get; set; } = false;

        [Category("ZX BASIC options")]
        [DisplayName("Require strict types")]
        [Description("The --strict option of ZXB")]
        public bool StrictTypes { get; set; } = false;

        [Category("ZX BASIC options")]
        [DisplayName("Store generated .z80asm file")]
        [Description("When compiling a ZX BASIC files, store the generated .z80asm file in the project.")]
        public bool StoreGeneratedZ80Files { get; set; } = true;

        [Category("ZX BASIC options")]
        [DisplayName("Nest generated .z80asm file")]
        [Description("When compiling a ZX BASIC files, add the generated .z80asm file to the project as a nested item of its .zxbas file.")]
        public bool NestGeneratedZ80Files { get; set; } = false;

        [Category("Peripherals")]
        [DisplayName("Kempston Joystick Emulation")]
        [Description("Keyboard configuration to emulate Kempston Joystick. You need to restart the virtual machine so that changing this option takes effect.")]
        [TypeConverter(typeof(TypedEnumConverter<KempstonEmulationOptions>))]
        public KempstonEmulationOptions KempstonEmulation { get; set; } = KempstonEmulationOptions.Left;

        /// <summary>
        /// Signs that the keyboard layout type has changed
        /// </summary>
        public event EventHandler<KeyboardLayoutTypeChangedEventArgs> KeyboardLayoutTypeChanged;

        /// <summary>
        /// Signs that the keyboard fit type has changed
        /// </summary>
        public event EventHandler<KeyboardFitTypeChangedEventArgs> KeyboardFitTypeChanged;

        /// <summary>
        /// Signs that the keyboard shift type has changed
        /// </summary>
        public event EventHandler<KeyboardShiftTypeChangedEventArgs> KeyboardShiftTypeChanged;

        /// <summary>
        /// Signs that the tool window zoom factor changes
        /// </summary>
        public event EventHandler<ZoomFactorChangedEventArgs> ZoomFactorChanged;
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
        [Description("ZX Spectrum 48")]
        Spectrum48,
        [Description("ZX Spectrum 128")]
        Spectrum128
    }

    /// <summary>
    /// Options for fitting the keyboard
    /// </summary>
    public enum KeyboardFitTypeOptions
    {
        [Description("Original size")]
        OriginalSize,
        [Description("Fit to tool window")]
        Fit
    }

    /// <summary>
    /// Preset zoom sizes
    /// </summary>
    public enum ZoomFactor
    {
        [Description("50%")]
        Zero50,
        [Description("80%")]
        Zero80,
        [Description("100%")]
        One,
        [Description("120%")]
        One20,
        [Description("133%")]
        One33,
        [Description("150%")]
        One50,
        [Description("200%")]
        Two,
        [Description("250%")]
        Two50
    }

    /// <summary>
    /// Options for Kempston Joystick keyboard emulation
    /// </summary>
    public enum KempstonEmulationOptions
    {
        Off,
        [Description("L-R-D-U-F: 5-8-6-7-0")]
        Left,
        [Description("L-R-D-U-F: 1-2-3-4-5")]
        Middle,
        [Description("L-R-D-U-F: 6-7-8-9-0")]
        Right
    }

    /// <summary>
    /// Options for Kempston Joystick keyboard emulation
    /// </summary>
    public enum KeyboardShiftOptions
    {
        [Description("Shift: Symbol, AltGr: Caps")]
        Normal,
        [Description("Shift: Caps, AltGr: Symbol")]
        Swapped
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

    /// <summary>
    /// Zoom factor changed event
    /// </summary>
    public class ZoomFactorChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Fit type set
        /// </summary>
        public ZoomFactor ZoomFactor { get; }

        /// <summary>Initializes a new instance of the <see cref="T:System.EventArgs" /> class.</summary>
        public ZoomFactorChangedEventArgs(ZoomFactor zoomFactor)
        {
            ZoomFactor = zoomFactor;
        }
    }

    /// <summary>
    /// Keyboard shift changed event
    /// </summary>
    public class KeyboardShiftTypeChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Fit type set
        /// </summary>
        public KeyboardShiftOptions KeyboardShiftOptions { get; }

        /// <summary>Initializes a new instance of the <see cref="T:System.EventArgs" /> class.</summary>
        public KeyboardShiftTypeChangedEventArgs(KeyboardShiftOptions keyboardShiftOptions)
        {
            KeyboardShiftOptions = keyboardShiftOptions;
        }
    }

}