using Microsoft.VisualStudio.Shell;
using Spect.Net.VsPackage.VsxLibrary.Command;
using Task = System.Threading.Tasks.Task;

namespace Spect.Net.VsPackage.Commands
{
    /// <summary>
    /// Sets the invoking project as the active one
    /// </summary>
    [CommandId(0x0822)]
    public class SetAsActiveProjectCommand : ProjectCommandBase
    {
        /// <summary>
        /// We allow this command only if the solution has more than one
        /// ZX Spectrum Code Discovery project
        /// </summary>
        protected override void OnQueryStatus(OleMenuCommand mc)
        {
            base.OnQueryStatus(mc);
            if (!mc.Visible) return;

            var solution = HostPackage.Solution;
            mc.Visible = solution.Projects.Count > 1;
        }

        /// <summary>
        /// Sets the active project to the current project file
        /// </summary>
        protected override Task ExecuteAsync()
        {
            HostPackage.Solution.SetActiveProject(ItemPath);
            return Task.FromResult(0);
        }
    }
}