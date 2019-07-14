using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Spect.Net.VsPackage.SolutionItems;
using System.Collections.Generic;

namespace Spect.Net.VsPackage.VsxLibrary.Command
{
    /// <summary>
    /// This class represents a command that can be associated with a Z80 code file.
    /// </summary>
    public abstract class SpectrumProgramCommandBase : SingleProjectItemCommandBase
    {
        /// <summary>
        /// This command accepts only Z80 code files
        /// </summary>
        public override IEnumerable<string> ItemExtensionsAccepted =>
            new[]
            {
                VsHierarchyTypes.Z80_ITEM,
                VsHierarchyTypes.ZX_BASIC_ITEM,
                VsHierarchyTypes.PROJECT_EXT
            };

        /// <summary>
        /// Gets the hierarchy information for the selected item.
        /// </summary>
        /// <param name="hierarchy">Hierarchy object</param>
        /// <param name="itemId">Hierarchy item id</param>
        /// <remarks>
        /// If the selected item is the project, it retrieves the hierarchy information for the
        /// default code file
        /// </remarks>
        public void GetCodeItem(out IVsHierarchy hierarchy, out uint itemId)
        {
            IsSingleItemSelection(out hierarchy, out itemId);
            if (itemId != VSConstants.VSITEMID_ROOT) return;

            // --- We have a project item, let's query the default code file
            var currentProject = SpectNetPackage.Default.ActiveProject;
            currentProject.GetHierarchyByIdentity(currentProject.DefaultProgramItem.Identity,
                out hierarchy, out itemId);
        }
    }
}