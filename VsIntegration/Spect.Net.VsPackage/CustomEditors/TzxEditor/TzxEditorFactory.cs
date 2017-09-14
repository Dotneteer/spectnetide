using System.Runtime.InteropServices;
using Spect.Net.VsPackage.Vsx;

namespace Spect.Net.VsPackage.CustomEditors.TzxEditor
{
    /// <summary>
    /// Editor factory class for the TZX viewer
    /// </summary>
    [Guid(FACTORY_ID)]
    public class TzxEditorFactory: EditorFactoryBase<SpectNetPackage, TzxEditorPane>
    {
        public const string FACTORY_ID = "D7B7B816-CF85-4EF7-B127-58EC607363DA";
        public const string EXTENSION = ".tzx";
    }
}