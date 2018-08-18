using System;
using System.Collections.Generic;
using System.Linq;
using EnvDTE;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;

#pragma warning disable VSTHRD010 // Invoke single-threaded types on Main thread

namespace Spect.Net.VsPackage.ProjectStructure
{
    /// <summary>
    /// This class represents the discovery project's items
    /// </summary>
    public abstract class Z80HierarchyBase<TProj, TItem>: IDisposable, IVsHierarchyEvents
    {
        private readonly TimeSpan _renameSpan = TimeSpan.FromMilliseconds(400);
        private DateTime? _lastDelete;
        private string _lastDeletedItem;
        private string _lastAddedItem;
        private List<string> _oldNames;

        protected readonly IVsHierarchy Hierarchy;
        protected readonly uint EventCookie;
        protected readonly List<TItem> HierarchyItems = new List<TItem>();

        /// <summary>
        /// The root item of the hierarchy
        /// </summary>
        public TProj Root { get; }

        /// <summary>
        /// Signs that a project item has been renamed
        /// </summary>
        public event EventHandler<ProjectItemRenamedEventArgs> ProjectItemRenamed;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        /// <param name="root">Root item</param>
        /// <param name="hierarchy">The object that represents the current hierarchy</param>
        protected Z80HierarchyBase(TProj root, IVsHierarchy hierarchy)
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
        /// <returns></returns>
        public abstract List<string> GetCurrentItemNames();

        #region IVsHierarchyEvent implementation

        // --- Anytime the structure of the project changes, we refresh the structure
        // --- of the entire project.

        int IVsHierarchyEvents.OnItemAdded(uint itemidParent, uint itemidSiblingPrev, uint itemidAdded)
        {
            CollectItems();
            CheckRename(itemidAdded);
            return VSConstants.S_OK;
        }

        int IVsHierarchyEvents.OnItemsAppended(uint itemidParent)
        {
            CollectItems();
            return VSConstants.S_OK;
        }

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

        int IVsHierarchyEvents.OnInvalidateItems(uint itemidParent)
        {
            return VSConstants.S_OK;
        }

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
            if (extObject is ProjectItem projectItem)
            {
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

        #endregion
    }
}

#pragma warning restore VSTHRD010 // Invoke single-threaded types on Main thread
