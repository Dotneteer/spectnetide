using System;
using System.IO;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Spect.Net.VsPackage.Vsx;

namespace Spect.Net.VsPackage.Z80Programs
{
    /// <summary>
    /// This class represents a command that can be associated with a .z80asm file.
    /// </summary>
    [CommandId(0x0803)]
    public abstract class TapeCommandBase : VsxAsyncCommand<SpectNetPackage, SpectNetCommandSet>
    {
        /// <summary>
        /// Override this method to define the status query action
        /// </summary>
        /// <param name="mc"></param>
        protected override void OnQueryStatus(OleMenuCommand mc)
        {
            mc.Visible = ItemPath != null;
        }

        /// <summary>
        /// Gets the currently selected Z80 program item
        /// </summary>
        /// <param name="hierarchy"></param>
        /// <param name="itemId"></param>
        protected void GetItem(out IVsHierarchy hierarchy, out uint itemId) =>
            SpectNetPackage.IsSingleProjectItemSelection(out hierarchy, out itemId);

        /// <summary>
        /// Gets the full path of the item; or null, if there is no .z80asm item selected.
        /// </summary>
        protected virtual string ItemPath
        {
            get
            {
                var singleItem = SpectNetPackage.IsSingleProjectItemSelection(out var hierarchy, out var itemId);
                if (!singleItem) return null;

                // ReSharper disable once SuspiciousTypeConversion.Global
                if (hierarchy is IVsProject project)
                {
                    project.GetMkDocument(itemId, out var itemFullPath);
                    var ext = Path.GetExtension(itemFullPath);
                    return string.Compare(".tap", ext, StringComparison.OrdinalIgnoreCase) == 0
                        || string.Compare(".tzx", ext, StringComparison.OrdinalIgnoreCase) == 0
                        ? itemFullPath
                        : null;
                }
                return null;
            }
        }
    }
}