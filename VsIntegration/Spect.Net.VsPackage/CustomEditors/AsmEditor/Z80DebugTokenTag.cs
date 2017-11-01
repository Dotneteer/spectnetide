using Microsoft.VisualStudio.Text.Tagging;

namespace Spect.Net.VsPackage.CustomEditors.AsmEditor
{
    /// <summary>
    /// This class defines the a debug token tag used in Z80 assembly
    /// </summary>
    public class Z80DebugTokenTag: ITextMarkerTag
    {
        /// <summary>
        /// The type of the token
        /// </summary>
        public string Type { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        public Z80DebugTokenTag(string type)
        {
            Type = type;
        }
    }
}