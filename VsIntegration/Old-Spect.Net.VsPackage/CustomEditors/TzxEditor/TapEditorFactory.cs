using System.Runtime.InteropServices;
using Spect.Net.VsPackage.Vsx;

namespace Spect.Net.VsPackage.CustomEditors.TzxEditor
{
    /// <summary>
    /// Editor factory class for the TZX viewer
    /// </summary>
    [Guid(FACTORY_ID)]
    public class TapEditorFactory: EditorFactoryBase<TzxEditorPane>
    {
        public const string FACTORY_ID = "7D837C1D-4C4D-476C-8D36-2D0C2C2C8FFA";
        public const string EXTENSION = ".tap";
    }
}