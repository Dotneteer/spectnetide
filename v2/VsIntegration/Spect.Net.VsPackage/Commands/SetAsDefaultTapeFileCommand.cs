using System.Collections.Generic;
using Microsoft.VisualStudio.Shell;
using Spect.Net.VsPackage.SolutionItems;
using Spect.Net.VsPackage.VsxLibrary.Command;
using Task = System.Threading.Tasks.Task;

namespace Spect.Net.VsPackage.Commands
{
    /// <summary>
    /// Set the specified tape file as the default one to load
    /// </summary>
    [CommandId(0x0803)]
    public class SetAsDefaultTapeFileCommand : SingleProjectItemCommandBase
    {
        /// <summary>
        /// This command accepts only tape files
        /// </summary>
        public override IEnumerable<string> ItemExtensionsAccepted =>
            new[]
            {
                VsHierarchyTypes.TZX_ITEM,
                VsHierarchyTypes.TAP_ITEM
            };

        /// <summary>
        /// Allows this command only within the active project
        /// </summary>
        protected override void OnQueryStatus(OleMenuCommand mc)
        {
            base.OnQueryStatus(mc);
            if (!mc.Visible) return;

            mc.Visible = IsInActiveProject;
        }

        /// <summary>
        /// Override this method to define the async command body te execute on the
        /// background thread
        /// </summary>
        protected override Task ExecuteAsync()
        {
            HostPackage.ActiveProject.SetDefaultTapeItem(this);
            return Task.FromResult(0);
        }
    }
}