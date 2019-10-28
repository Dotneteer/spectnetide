using Microsoft.VisualStudio.Shell;
using Spect.Net.VsPackage.VsxLibrary.Command;

namespace Spect.Net.VsPackage.Commands
{
    /// <summary>
    /// This command debugs the default program within the active project.
    /// </summary>
    [CommandId(0x0807)]
    public class DebugDefaultProgramCommand : DebugProgramCommand
    {
        /// <summary>
        /// The item is allowed only when there is a default code file selected
        /// </summary>
        protected override void OnQueryStatus(OleMenuCommand mc)
        {
            base.OnQueryStatus(mc);
            if (!mc.Visible) return;

            var project = HostPackage.ActiveProject;
            var visible = project.DefaultProgramItem != null;
            if (visible)
            {
                project.GetHierarchyByIdentity(project.DefaultProgramItem.Identity, out var hierarchy, out _);
                visible &= hierarchy != null;
            }
            mc.Visible = visible;
        }
    }
}