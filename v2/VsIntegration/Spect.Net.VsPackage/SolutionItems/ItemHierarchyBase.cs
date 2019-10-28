using System;
using System.Collections.Generic;
using System.Linq;
using EnvDTE;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

#pragma warning disable VSTHRD010 // Invoke single-threaded types on Main thread

namespace Spect.Net.VsPackage.SolutionItems
{
    /// <summary>
    /// This class represents a hierarchy of host items (such as a project or a solution) and
    /// its nested items.
    /// </summary>
    /// <typeparam name="THost">The type of the host item</typeparam>
    /// <typeparam name="TItem">The type of the nested items</typeparam>
    public abstract class ItemHierarchyBase<THost, TItem> : IDisposable, IVsHierarchyEvents
    {
        private readonly object _locker = new object();
        private readonly TimeSpan _renameSpan = TimeSpan.FromMilliseconds(400);
        private DateTime? _lastDelete;
        private string _lastDeletedItem;
        private string _lastAddedItem;
        private List<string> _oldNames;
        private List<TItem> _hierarchyItems = new List<TItem>();

        protected readonly IVsHierarchy Hierarchy;
        protected readonly uint EventCookie;

        /// <summary>
        /// The root item of the hierarchy.
        /// </summary>
        public THost Root { get; }

        /// <summary>
        /// The items in this hierarchy
        /// </summary>
        protected List<TItem> HierarchyItems
        {
            get
            {
                lock (_locker)
                {
                    return _hierarchyItems;
                }
            }
            set
            {
                lock (_locker)
                {
                    _hierarchyItems = value;
                }
            }
        }

        /// <summary>
        /// Signs that a project item has been renamed.
        /// </summary>
        public event EventHandler<ProjectItemRenamedEventArgs> ProjectItemRenamed;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        /// <param name="root">Root item</param>
        /// <param name="hierarchy">The object that represents the current hierarchy</param>
        protected ItemHierarchyBase(THost root, IVsHierarchy hierarchy)
        {
            Root = root;
            Hierarchy = hierarchy;
            Hierarchy.AdviseHierarchyEvents(this, out var cookie);
            EventCookie = cookie;
            // ReSharper disable once VirtualMemberCallInConstructor
            CollectItems();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting
        /// unmanaged resources.
        /// </summary>
        public virtual void Dispose()
        {
            Hierarchy?.UnadviseHierarchyEvents(EventCookie);
        }

        /// <summary>
        /// Collects the items within the host
        /// </summary>
        public virtual void CollectItems()
        {
        }

        /// <summary>
        /// Respond to the event when an item within the hierarchy has been renamed
        /// </summary>
        /// <param name="oldItem">Old name</param>
        /// <param name="newItem">New name</param>
        public virtual void OnRenameItem(string oldItem, string newItem)
        {
        }

        /// <summary>
        /// Obtain the names of items within the hierarchy
        /// </summary>
        /// <returns>
        /// List of names
        /// </returns>
        public abstract List<string> GetCurrentItemNames();

        // --- Anytime the structure of the project changes, we refresh the structure
        // --- of the entire project.

        /// <summary>
        /// Notifies clients when an item is added to the hierarchy.
        /// </summary>
        /// <param name="itemidParent">
        /// [in] Identifier of the parent, or root node of the hierarchy in which the item is added.
        /// </param>
        /// <param name="itemidSiblingPrev">
        /// [in] Identifier that indicates where the item is added in relation to other items
        /// (siblings) within the parent hierarchy (<paramref name="itemidParent" />). If the
        /// new item is added at the beginning of the sibling items, then a value of
        /// <see cref="F:Microsoft.VisualStudio.VSConstants.VSITEMID_NIL" /> is specified. If the
        /// item is added after a particular node, the Item Id of the node in question is specified.
        /// </param>
        /// <param name="itemidAdded">[in] Identifier of the added item.</param>
        /// <returns>
        /// If the method succeeds, it returns <see cref="F:Microsoft.VisualStudio.VSConstants.S_OK" />.
        /// If it fails, it returns an error code.
        /// </returns>
        /// <remarks>
        /// This event may sign that an item has been renamed, so we check for that.
        /// </remarks>
        int IVsHierarchyEvents.OnItemAdded(uint itemidParent, uint itemidSiblingPrev, uint itemidAdded)
        {
            CollectItems();
            CheckRename(itemidAdded);
            return VSConstants.S_OK;
        }

        /// <summary>
        /// Notifies clients when items are appended to the end of the hierarchy.
        /// </summary>
        /// <param name="itemidParent">
        /// [in] Identifier of the parent or root node of the hierarchy to which the item is appended.
        /// </param>
        /// <returns>
        /// If the method succeeds, it returns <see cref="F:Microsoft.VisualStudio.VSConstants.S_OK" />.
        /// If it fails, it returns an error code.
        /// </returns>
        int IVsHierarchyEvents.OnItemsAppended(uint itemidParent)
        {
            CollectItems();
            return VSConstants.S_OK;
        }

        /// <summary>
        /// Notifies clients when an item is deleted from the hierarchy.
        /// </summary>
        /// <param name="itemid">
        /// [in] Identifier of the deleted item. This is the same identifier assigned to the new
        /// item by the hierarchy when it is added to the hierarchy.
        /// </param>
        /// <returns>
        /// If the method succeeds, it returns <see cref="F:Microsoft.VisualStudio.VSConstants.S_OK" />.
        /// If it fails, it returns an error code.
        /// </returns>
        /// <remarks>
        /// This event may sign that an item has been renamed, so we check for that.
        /// </remarks>
        int IVsHierarchyEvents.OnItemDeleted(uint itemid)
        {
            _oldNames = GetCurrentItemNames();
            CollectItems();
            _lastDelete = DateTime.Now;
            Hierarchy.GetProperty(itemid, (int)__VSHPROPID.VSHPROPID_ExtObject, out var extObject);
            if (extObject is ProjectItem projectItem)
            {
                _lastDeletedItem = projectItem.FileNames[0];
            }
            return VSConstants.S_OK;
        }

        /// <summary>
        /// Notifies clients when one or more properties of an item have changed.
        /// </summary>
        /// <param name="itemid">
        /// [in] Identifier of the item whose property has changed. For a list of
        /// <paramref name="itemid" /> values, see <see langword="VSITEMID" />.
        /// </param>
        /// <param name="propid">
        /// [in] Identifier of the property of <paramref name="itemid" />. For a list of
        /// <paramref name="propid" /> values, see
        /// <see cref="T:Microsoft.VisualStudio.Shell.Interop.__VSHPROPID" />.
        /// </param>
        /// <param name="flags">
        /// [in] Not implemented.
        /// </param>
        /// <returns>
        /// If the method succeeds, it returns <see cref="F:Microsoft.VisualStudio.VSConstants.S_OK" />.
        /// If it fails, it returns an error code.
        /// </returns>
        int IVsHierarchyEvents.OnPropertyChanged(uint itemid, int propid, uint flags)
        {
            if (propid == (int)__VSHPROPID.VSHPROPID_Caption)
            {
                // --- If the VSHPROPID_Caption property changes, it's a rename
                CollectItems();
                CheckRename(itemid);
            }
            return VSConstants.S_OK;
        }

        /// <summary>
        /// Notifies clients when changes are made to the item inventory of a hierarchy.
        /// </summary>
        /// <param name="itemidParent">
        /// [in] Parent item identifier, or root, of the hierarchy whose item inventory has changed.
        /// </param>
        /// <returns>
        /// If the method succeeds, it returns <see cref="F:Microsoft.VisualStudio.VSConstants.S_OK" />.
        /// If it fails, it returns an error code.
        /// </returns>
        int IVsHierarchyEvents.OnInvalidateItems(uint itemidParent)
        {
            return VSConstants.S_OK;
        }

        /// <summary>
        /// Notifies clients when changes are made to icons.
        /// </summary>
        /// <param name="hicon">[in] Icon handle.</param>
        /// <returns>
        /// If the method succeeds, it returns <see cref="F:Microsoft.VisualStudio.VSConstants.S_OK" />.
        /// If it fails, it returns an error code.
        /// </returns>
        int IVsHierarchyEvents.OnInvalidateIcon(IntPtr hicon)
        {
            return VSConstants.S_OK;
        }

        /// <summary>
        /// Check if a rename event occurred
        /// </summary>
        /// <param name="itemId">Item of the newly added/renamed item</param>
        private void CheckRename(uint itemId)
        {
            Hierarchy.GetProperty(itemId, (int)__VSHPROPID.VSHPROPID_ExtObject, out var extObject);
            if (!(extObject is ProjectItem projectItem)) return;

            _lastAddedItem = projectItem.FileNames[0];
            var timeEllapsed = DateTime.Now - _lastDelete;
            if (_lastDelete.HasValue && timeEllapsed < _renameSpan)
            {
                if (_lastDeletedItem == _lastAddedItem)
                {
                    // --- This is tricky, we need to obtain the old file name from the
                    // --- old hierarchy
                    var newNames = GetCurrentItemNames();
                    var missingName = _oldNames.FirstOrDefault(n => !newNames.Contains(n));
                    if (missingName == null)
                    {
                        // --- It should not be so, but let's prepare for this
                        return;
                    }
                    _lastDeletedItem = missingName;
                }
                OnRenameItem(_lastDeletedItem, _lastAddedItem);
                ProjectItemRenamed?.Invoke(this,
                    new ProjectItemRenamedEventArgs(_lastDeletedItem, _lastAddedItem));
            }
            _lastDelete = null;
        }
    }
}

#pragma warning restore VSTHRD010 // Invoke single-threaded types on Main thread
