using System.ComponentModel;
using Microsoft.VisualStudio.Shell;

namespace Spect.Net.VsPackage
{
    public class SpectNetExtraOptionsGrid: DialogPage
    {
        // --- Virtual machine options
        [Category("Virtual machine")]
        [DisplayName("Use Fast Load")]
        [Description("Specifies if fast load is enabled for loading tape files")]
        public bool UseFastLoad { get; set; } = false;
    }
}