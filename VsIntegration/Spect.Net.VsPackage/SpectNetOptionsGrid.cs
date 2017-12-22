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
        [DisplayName("#of SP Events to keep")]
        [Description("The number of Z80 CPU Stack Pointer register events" +
                     " to keep and show in the Z80 CPU Stack tool window")]
        public ushort StackPointerEvents { get; set; } = 16;

        [Category("Virtual machine")]
        [DisplayName("#of Stack Manipulation Events to keep")]
        [Description("The number of Z80 CPU stack manipulation events" +
                     " to keep and show in the Z80 CPU Stack tool window")]
        public ushort StackManipulationEvents { get; set; } = 128;

        // --- Run Z80 Code options
        [Category("Run Z80 Code")]
        [DisplayName("Confirm non-zero displacement")]
        [Description("Asks the user to confirm running code with non-zero displacement value")]
        public bool ConfirmNonZeroDisplacement { get; set; } = true;

        [Category("Run Z80 Code")]
        [DisplayName("Confirm machine restart")]
        [Description("Asks the user to confirm restrating the virtual " +
                     "machine before running injected Z80 code")]
        public bool ConfirmMachineRestart { get; set; } = true;

        [Category("Run Z80 Code")]
        [DisplayName("Confirm code start")]
        [Description("Displays a confirmation message about starting the code")]
        public bool ConfirmCodeStart { get; set; } = true;

        [Category("Run Z80 Code")]
        [DisplayName("Confirm successful code compilation")]
        [Description("Displays a confirmation message about successfully compiling code")]
        public bool ConfirmCodeCompile { get; set; } = true;

        [Category("Run Z80 Code")]
        [DisplayName("Predefined debug symbols")]
        [Description("Predefined symbols to use when you start the program in debug mode")]
        public string DebugSymbols { get; set; } = "DEBUG";

        [Category("Run Z80 Code")]
        [DisplayName("Predefined symbols")]
        [Description("Predefined symbols to use when you start the program (both in debug mode and without debugging)")]
        public string RunSymbols { get; set; } = "";

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
        public bool FullLineHighlight { get; set; } = false;

        [Category("Disassembly View")]
        [DisplayName("Turn on commenting mode")]
        [Description("In commenting mode, you can double-click to prepare an 'C' command for the disassembly clicked.")]
        public bool CommentingMode { get; set; } = false;

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