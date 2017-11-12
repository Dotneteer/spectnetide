using System.Runtime.InteropServices;
using Spect.Net.VsPackage.Vsx;

namespace Spect.Net.VsPackage.CustomEditors.SpInvEditor
{
    /// <summary>
    /// Editor factory class for the ROM viewer
    /// </summary>
    [Guid(FACTORY_ID)]
    public class SpInvEditorFactory : EditorFactoryBase<SpectNetPackage, SpInvEditorPane>
    {
        public const string FACTORY_ID = "0E7B45BB-80A0-445E-8A35-B0108D3A1A35";
        public const string EXTENSION = ".spinv";
    }
}