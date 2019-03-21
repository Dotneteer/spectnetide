using System.Runtime.InteropServices;
using Spect.Net.VsPackage.Vsx;

namespace Spect.Net.VsPackage.CustomEditors.RomEditor
{
    /// <summary>
    /// Editor factory class for the ROM viewer
    /// </summary>
    [Guid(FACTORY_ID)]
    public class RomEditorFactory : EditorFactoryBase<RomEditorPane>
    {
        public const string FACTORY_ID = "FDC52CBB-1609-4B94-A72D-3D1E26B96B8F";
        public const string EXTENSION = ".rom";
    }
}