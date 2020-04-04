using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Spect.Net.VsPackage.VsxLibrary.Command;

namespace Spect.Net.VsPackage.Commands
{
    /// <summary>
    /// This command runs the default program within the active project.
    /// </summary>
    [CommandId(0x0806)]
    public class RunDefaultProgramCommand : RunProgramCommand
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

        /// <summary>
        /// Gets the document that this command should use
        /// </summary>
        /// <param name="hierarchy"></param>
        /// <param name="itemId"></param>
        protected override void GetAffectedItem(out IVsHierarchy hierarchy, out uint itemId)
        {
            // --- We have a project item, let's query the default code file
            var currentProject = SpectNetPackage.Default.Solution.ActiveProject;
            currentProject.GetHierarchyByIdentity(currentProject.DefaultProgramItem.Identity,
                out hierarchy, out itemId);
        }
    }
}