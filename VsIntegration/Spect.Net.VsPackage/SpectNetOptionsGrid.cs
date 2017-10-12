using System.ComponentModel;
using Microsoft.VisualStudio.Shell;

namespace Spect.Net.VsPackage
{
    /// <summary>
    /// This class stores the Spect.Net IDE options
    /// </summary>
    public class SpectNetOptionsGrid : DialogPage
    {
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
        public ushort StackManipulationEvents { get; set; } = 16;

        [Category("Disassembly")]
        [DisplayName("Allow Saving ROM Annotations")]
        [Description("Specifies ROM annotations are saved to the ROM's .disann file by default")]
        public bool SaveRomChangesToRom { get; set; } = false;


    }
}