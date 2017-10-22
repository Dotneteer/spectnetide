using System.ComponentModel;
using Spect.Net.VsPackage.Vsx.Output;

namespace Spect.Net.VsPackage.Z80Programs
{
    [DisplayName("Z80 Build Output")]
    [AutoActivate(true)]
    [ClearWithSolution(true)]
    public class Z80OutputPane : OutputPaneDefinition
    {
    }
}