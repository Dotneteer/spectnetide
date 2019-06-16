using System.Collections.Generic;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
// ReSharper disable StringLiteralTypo

namespace Spect.Net.VsPackage.Z80Programs.Commands
{
    /// <summary>
    /// This class represents a command that can be associated with a .z80asm file.
    /// </summary>
    public abstract class Z80ProgramCommandBase : SingleProjectItemCommandBase
    {
        /// <summary>
        /// This command accepts only Z80 code files
        /// </summary>
        public override IEnumerable<string> ItemExtensionsAccepted =>
            new[] {".z80Asm", ".zxbas", ".z80cdproj"};

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
            SpectNetPackage.IsSingleItemSelection(AllowProjectItem, out hierarchy, out itemId);
            if (itemId == VSConstants.VSITEMID_ROOT)
            {
                // --- We have a project item, let's query the default code file
                var currentProject = Package.CodeDiscoverySolution.CurrentProject;
                currentProject.GetHierarchyByIdentity(currentProject.DefaultZ80CodeItem.Identity,
                    out hierarchy, out itemId);
            }
        }
    }
}