using System.Runtime.InteropServices;
using Spect.Net.VsPackage.Vsx;

namespace Spect.Net.VsPackage.CustomEditors.VfddEditor
{
    /// <summary>
    /// Editor factory class for the ROM viewer
    /// </summary>
    [Guid(FACTORY_ID)]
    public class VfddEditorFactory : EditorFactoryBase<VfddEditorPane>
    {
        public const string FACTORY_ID = "7D994DE6-374C-4A84-9EB0-E2CE2434C1A6";
        public const string EXTENSION = ".vfdd";
    }
}