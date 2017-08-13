using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using EnvDTE;
using Spect.Net.VsPackage.Utility;

namespace Spect.Net.VsPackage.ProjectStructure
{
    /// <summary>
    /// This class represents the discovery project's items
    /// </summary>
    public class DiscoveryProject
    {
        private readonly List<DiscoveryProjectItem> _projectItems = new List<DiscoveryProjectItem>();

        /// <summary>
        /// Holds the associated DTE project
        /// </summary>
        public Project Project { get; }

        /// <summary>
        /// Items in the project
        /// </summary>
        public IReadOnlyList<DiscoveryProjectItem> ProjectItems { get; }

        /// <summary>
        /// Rom file project items
        /// </summary>
        public IReadOnlyList<RomProjectItem> RomProjectItems => new ReadOnlyCollection<RomProjectItem>(
            _projectItems.Where(i => i.GetType() == typeof(RomProjectItem)).Cast<RomProjectItem>().ToList());

        /// <summary>
        /// Annotation file project items
        /// </summary>
        public IReadOnlyList<AnnotationProjectItem> AnnotationProjectItems => new ReadOnlyCollection<AnnotationProjectItem>(
            _projectItems.Where(i => i.GetType() == typeof(AnnotationProjectItem)).Cast<AnnotationProjectItem>().ToList());

        /// <summary>
        /// TZX file project items
        /// </summary>
        public IReadOnlyList<TzxProjectItem> TzxProjectItems => new ReadOnlyCollection<TzxProjectItem>(
            _projectItems.Where(i => i.GetType() == typeof(TzxProjectItem)).Cast<TzxProjectItem>().ToList());

        /// <summary>
        /// TZX file project items
        /// </summary>
        public IReadOnlyList<VmStateProjectItem> VmStateProjectItems => new ReadOnlyCollection<VmStateProjectItem>(
            _projectItems.Where(i => i.GetType() == typeof(VmStateProjectItem)).Cast<VmStateProjectItem>().ToList());

        /// <summary>
        /// TZX file project items
        /// </summary>
        public IReadOnlyList<UnusedProjectItem> UnusedProjectItems => new ReadOnlyCollection<UnusedProjectItem>(
            _projectItems.Where(i => i.GetType() == typeof(UnusedProjectItem)).Cast<UnusedProjectItem>().ToList());

        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public DiscoveryProject(Project project)
        {
            Project = project;
            ProjectItems = new ReadOnlyCollection<DiscoveryProjectItem>(_projectItems);
            CollectProjectItems();
        }

        /// <summary>
        /// Collects the items of Spectrum Code Discovery project
        /// </summary>
        public void CollectProjectItems()
        {
            foreach (ProjectItem item in Project.ProjectItems)
            {
                ProcessProjectItem(item);
            }
        }

        /// <summary>
        /// Processes the specified item, and its sub items, provided it is a folder
        /// </summary>
        /// <param name="item">Item to process</param>
        public void ProcessProjectItem(ProjectItem item)
        {
            if (item.Properties.Item("Extension") == null) return;

            var extension = (item.Properties.Item("Extension").Value?.ToString() ?? string.Empty).ToLower();
            if (extension == string.Empty && item.ProjectItems.Count > 0)
            {
                // --- This is a folder
                foreach (ProjectItem subItem in item.ProjectItems)
                {
                    ProcessProjectItem(subItem);
                }
            }
            else if (extension == VsHierarchyTypes.DisannItem)
            {
                _projectItems.Add(new AnnotationProjectItem(item));
            }
            else if (extension == VsHierarchyTypes.RomItem)
            {
                _projectItems.Add(new RomProjectItem(item));
            }
            else if (extension == VsHierarchyTypes.TzxItem)
            {
                _projectItems.Add(new TzxProjectItem(item));
            }
            else if (extension == VsHierarchyTypes.VmStateItem)
            {
                _projectItems.Add(new VmStateProjectItem(item));
            }
            else
            {
                _projectItems.Add(new UnusedProjectItem(item));
            }
        }
    }
}