using System;
using System.Collections.Generic;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;

namespace Spect.Net.VsPackage.ProjectStructure
{
    /// <summary>
    /// This class represents the discovery project's items
    /// </summary>
    public class Z80HierarchyBase<TProj, TItem>: IDisposable, IVsHierarchyEvents
    {
        protected readonly IVsHierarchy Hierarchy;
        protected readonly uint EventCookie;
        protected readonly TProj Root;
        protected readonly List<TItem> HierarchyItems = new List<TItem>();

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        /// <param name="root">Root item</param>
        /// <param name="hierarchy">The object that represents the current hierarchy</param>
        public Z80HierarchyBase(TProj root, IVsHierarchy hierarchy)
        {
            Root = root;
            Hierarchy = hierarchy;
            Hierarchy.AdviseHierarchyEvents(this, out var cookie);
            EventCookie = cookie;
            // ReSharper disable once VirtualMemberCallInConstructor
            CollectItems();
        }

        /// <inheritdoc />
        public virtual void Dispose()
        {
            Hierarchy?.UnadviseHierarchyEvents(EventCookie);
        }

        /// <summary>
        /// Collects the items of Spectrum Code Discovery project
        /// </summary>
        public virtual void CollectItems()
        {
        }

        #region IVsHierarchyEvent implementation

        // --- Anytime the structure of the project changes, we refresh the structure
        // --- of the entire project.

        int IVsHierarchyEvents.OnItemAdded(uint itemidParent, uint itemidSiblingPrev, uint itemidAdded)
        {
            CollectItems();
            return VSConstants.S_OK;
        }

        int IVsHierarchyEvents.OnItemsAppended(uint itemidParent)
        {
            CollectItems();
            return VSConstants.S_OK;
        }

        int IVsHierarchyEvents.OnItemDeleted(uint itemid)
        {
            CollectItems();
            return VSConstants.S_OK;
        }

        int IVsHierarchyEvents.OnPropertyChanged(uint itemid, int propid, uint flags)
        {
            return VSConstants.S_OK;
        }

        int IVsHierarchyEvents.OnInvalidateItems(uint itemidParent)
        {
            return VSConstants.S_OK;
        }

        int IVsHierarchyEvents.OnInvalidateIcon(IntPtr hicon)
        {
            return VSConstants.S_OK;
        }

        #endregion
    }
}