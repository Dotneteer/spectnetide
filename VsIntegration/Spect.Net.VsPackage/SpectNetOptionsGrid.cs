using System.ComponentModel;
using Microsoft.VisualStudio.Shell;

namespace Spect.Net.VsPackage
{
    /// <summary>
    /// This class stores the Spect.Net IDE options
    /// </summary>
    public class SpectNetOptionsGrid : DialogPage
    {
        // --- Virtual machine options
        [Category("Virtual machine")]
        [DisplayName("Use Fast Load")]
        [Description("Specifies if fast load is enabled for loading tape files")]
        public bool UseFastLoad { get; set; } = false;

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

        // --- Disassembly options
        [Category("Disassembly")]
        [DisplayName("Allow Saving ROM Annotations")]
        [Description("Specifies ROM annotations are saved to the ROM's .disann file by default")]
        public bool SaveRomChangesToRom { get; set; } = false;

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

        // --- Export Z80 Code options
        [Category("Export Z80 Code")]
        [DisplayName("Confirm code export")]
        [Description("Displays a confirmation message about exporting the code")]
        public bool ConfirmCodeExport { get; set; } = true;

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

    }
}