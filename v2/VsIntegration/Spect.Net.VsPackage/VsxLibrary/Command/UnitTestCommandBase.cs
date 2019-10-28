using System.Collections.Generic;
using Microsoft.VisualStudio.Shell;
using Spect.Net.VsPackage.SolutionItems;

namespace Spect.Net.VsPackage.VsxLibrary.Command
{
    /// <summary>
    /// This class represents a command that can be executed on a Z80 Unit Test file
    /// </summary>
    public abstract class UnitTestCommandBase : SingleProjectItemCommandBase
    {
        /// <summary>
        /// This command accepts only Z80 code files
        /// </summary>
        public override IEnumerable<string> ItemExtensionsAccepted =>
            new[]
            {
                VsHierarchyTypes.TEST_ITEM,
                VsHierarchyTypes.PROJECT_EXT
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