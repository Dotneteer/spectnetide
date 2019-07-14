
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using EnvDTE;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

#pragma warning disable VSTHRD010 // Invoke single-threaded types on Main thread

namespace Spect.Net.VsPackage.VsxLibrary.Command
{
    /// <summary>
    /// This class represents a command that can be associated with a .z80asm file.
    /// </summary>
    public abstract class SingleProjectItemCommandBase : SpectNetCommandBase
    {
        /// <summary>
        /// Gets the file item extensions accepted by this command
        /// </summary>
        public abstract IEnumerable<string> ItemExtensionsAccepted { get; }

        /// <summary>
        /// Override this method to define the status query action
        /// </summary>
        /// <param name="mc"></param>
        protected override void OnQueryStatus(OleMenuCommand mc)
        {
            mc.Visible = SpectNetPackage.Default.ActiveProject != null
                && ItemPath != null;
        }

        /// <summary>
        /// Gets the currently selected Z80 program item
        /// </summary>
        /// <param name="hierarchy"></param>
        /// <param name="itemId"></param>
        public void GetItem(out IVsHierarchy hierarchy, out uint itemId) =>
            IsSingleItemSelection(out hierarchy, out itemId);

        /// <summary>
        /// Gets the full path of the item; or null, if there is no .z80asm item selected.
        /// </summary>
        public string ItemPath
        {
            get
            {
                var singleItem = IsSingleItemSelection(out var hierarchy, out var itemId);
                if (!singleItem) return null;

                // ReSharper disable once SuspiciousTypeConversion.Global
                if (hierarchy is IVsProject project)
                {
                    project.GetMkDocument(itemId, out var itemFullPath);
                    var ext = Path.GetExtension(itemFullPath);
                    return ItemExtensionsAccepted.Any(e => string.Compare(e, ext, StringComparison.OrdinalIgnoreCase) == 0)
                        ? itemFullPath
                        : null;
                }
                return null;
            }
        }

        /// <summary>
        /// Gets the identity of the project item.
        /// </summary>
        public string Identity
        {
            get
            {
                var singleItem = IsSingleItemSelection(out var hierarchy, out var itemId);
                if (!singleItem) return null;

                hierarchy.GetProperty(itemId, (int)__VSHPROPID.VSHPROPID_ExtObject, out var objProj);
                if (objProj is ProjectItem projItem)
                {
                    var identity = projItem.Properties.Item("Identity");
                    return identity?.Value?.ToString();
                }
                return null;
            }
        }

        /// <summary>
        /// Tests if the file behind the command is in the active project
        /// </summary>
        public bool IsInActiveProject 
            => SpectNetPackage.Default.Solution.IsFileInActiveProject(ItemPath);

        /// <summary>
        /// This method checks if there is only a single item selected in Solution Explorer.
        /// </summary>
        /// <param name="hierarchy">The selected hierarchy</param>
        /// <param name="itemid">The selected item in the hierarchy</param>
        /// <returns>
        /// True, if only a single item is selected; otherwise, false
        /// </returns>
        public static bool IsSingleItemSelection(out IVsHierarchy hierarchy, out uint itemid)
        {
            hierarchy = null;
            itemid = VSConstants.VSITEMID_NIL;

            var monitorSelection = Package.GetGlobalService(typeof(SVsShellMonitorSelection)) as IVsMonitorSelection;
            var solution = Package.GetGlobalService(typeof(SVsSolution)) as IVsSolution;
            if (monitorSelection == null || solution == null)
            {
                return false;
            }

            var hierarchyPtr = IntPtr.Zero;
            var selectionContainerPtr = IntPtr.Zero;

            try
            {
                // --- Obtain the current selection
                var hr = monitorSelection.GetCurrentSelection(out hierarchyPtr,
                    out itemid,
                    out var multiItemSelect,
                    out selectionContainerPtr);

                if (ErrorHandler.Failed(hr) || hierarchyPtr == IntPtr.Zero || itemid == VSConstants.VSITEMID_NIL)
                {
                    // --- There is no selection
                    return false;
                }

                // --- Multiple items are selected
                if (multiItemSelect != null) return false;

                // --- No hierarchy, no selection
                hierarchy = Marshal.GetObjectForIUnknown(hierarchyPtr) as IVsHierarchy;
                if (hierarchy == null) return false;

                // --- Return true only when the hierarchy is a project inside the Solution
                // --- and it has a ProjectID Guid
                return !ErrorHandler.Failed(solution.GetGuidOfProject(hierarchy, out _));
            }
            finally
            {
                // --- Release unmanaged resources
                if (selectionContainerPtr != IntPtr.Zero)
                {
                    Marshal.Release(selectionContainerPtr);
                }
                if (hierarchyPtr != IntPtr.Zero)
                {
                    Marshal.Release(hierarchyPtr);
                }
            }
        }
    }

}

#pragma warning restore VSTHRD010 // Invoke single-threaded types on Main thread
