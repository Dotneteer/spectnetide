using Microsoft.VisualStudio.Text.Tagging;

namespace Spect.Net.VsPackage.CustomEditors.AsmEditor
{
    /// <summary>
    /// Represents a Z80 debug token tag
    /// </summary>
    public class Z80DebugTokenTag : TextMarkerTag
    {
        public Z80DebugTokenTag(string type) : base(type)
        {
        }
    }
}