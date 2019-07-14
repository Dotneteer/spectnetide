using System.Collections.Generic;
using Microsoft.VisualStudio.Shell;
using Spect.Net.VsPackage.SolutionItems;

namespace Spect.Net.VsPackage.VsxLibrary.Command
{
    /// <summary>
    /// This class represents a command that handles virtual machine state files.
    /// </summary>
    public abstract class VmStateCommandBase : SingleProjectItemCommandBase
    {
        /// <summary>
        /// This command accepts only Z80 code files
        /// </summary>
        public override IEnumerable<string> ItemExtensionsAccepted =>
            new[]
            {
                VsHierarchyTypes.VM_STATE_ITEM,
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
    }
}