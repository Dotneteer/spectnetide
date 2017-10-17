using Microsoft.VisualStudio.Shell;
using Spect.Net.VsPackage.ToolWindows.KeyboardTool;
using Spect.Net.VsPackage.Vsx;

namespace Spect.Net.VsPackage.Commands
{
    /// <summary>
    /// Displays the BASIC List tool window
    /// </summary>
    [CommandId(0x1700)]
    [ToolWindow(typeof(KeyboardToolWindow))]
    public class ShowKeyboardCommand :
        VsxShowToolWindowCommand<SpectNetPackage, SpectNetCommandSet>
    {
        protected override void OnQueryStatus(OleMenuCommand mc)
            => mc.Enabled = Package.CurrentWorkspace?.CurrentProject != null;
    }
}