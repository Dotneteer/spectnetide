using System.Runtime.InteropServices;
using Spect.Net.VsPackage.Vsx;

namespace Spect.Net.VsPackage.CustomEditors.DisannEditor
{
    /// <summary>
    /// Editor factory class for the ROM viewer
    /// </summary>
    [Guid(FACTORY_ID)]
    public class DisAnnEditorFactory : EditorFactoryBase<DisAnnEditorPane>
    {
        public const string FACTORY_ID = "C39C68C9-0ECF-4F06-BEEC-96B8AF23F88D";
        public const string EXTENSION = ".disann";
    }
}