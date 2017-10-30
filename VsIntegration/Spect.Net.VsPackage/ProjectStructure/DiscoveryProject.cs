using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using EnvDTE;
using Microsoft.VisualStudio.Shell.Interop;
using Newtonsoft.Json;
using Spect.Net.VsPackage.Utility;

namespace Spect.Net.VsPackage.ProjectStructure
{
    /// <summary>
    /// This class represents the discovery project's items
    /// </summary>
    public class DiscoveryProject: Z80HierarchyBase<Project, DiscoveryProjectItem>
    {
        private const string SETTINGS_FILE = ".z80settings";
        private Z80ProjectSettings _currentSettings;

        /// <summary>
        /// Items in the project
        /// </summary>
        public IReadOnlyList<DiscoveryProjectItem> ProjectItems { get; }

        /// <summary>
        /// Rom file project items
        /// </summary>
        public IReadOnlyList<RomProjectItem> RomProjectItems => new ReadOnlyCollection<RomProjectItem>(
            HierarchyItems.Where(i => i.GetType() == typeof(RomProjectItem))
            .Cast<RomProjectItem>()
            .ToList());

        /// <summary>
        /// Annotation file project items
        /// </summary>
        public IReadOnlyList<AnnotationProjectItem> AnnotationProjectItems => new ReadOnlyCollection<AnnotationProjectItem>(
            HierarchyItems.Where(i => i.GetType() == typeof(AnnotationProjectItem))
            .Cast<AnnotationProjectItem>()
            .ToList());

        /// <summary>
        /// TZX file project items
        /// </summary>
        public IReadOnlyList<TzxProjectItem> TzxProjectItems => new ReadOnlyCollection<TzxProjectItem>(
            HierarchyItems.Where(i => i.GetType() == typeof(TzxProjectItem))
            .Cast<TzxProjectItem>()
            .ToList());

        /// <summary>
        /// TAP file project items
        /// </summary>
        public IReadOnlyList<TapProjectItem> TapProjectItems => new ReadOnlyCollection<TapProjectItem>(
            HierarchyItems.Where(i => i.GetType() == typeof(TapProjectItem))
            .Cast<TapProjectItem>()
            .ToList());

        /// <summary>
        /// Virtual machine state file project items
        /// </summary>
        public IReadOnlyList<VmStateProjectItem> VmStateProjectItems => new ReadOnlyCollection<VmStateProjectItem>(
            HierarchyItems.Where(i => i.GetType() == typeof(VmStateProjectItem))
            .Cast<VmStateProjectItem>()
            .ToList());

        /// <summary>
        /// Unused project items
        /// </summary>
        public IReadOnlyList<UnusedProjectItem> UnusedProjectItems => new ReadOnlyCollection<UnusedProjectItem>(
            HierarchyItems.Where(i => i.GetType() == typeof(UnusedProjectItem))
            .Cast<UnusedProjectItem>()
            .ToList());

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        /// <param name="project">The Project automation object</param>
        /// <param name="hierarchy">The object that represents the current hierarchy</param>
        public DiscoveryProject(Project project, IVsHierarchy hierarchy)
            : base(project, hierarchy)
        {
            ProjectItems = new ReadOnlyCollection<DiscoveryProjectItem>(HierarchyItems);
        }

        /// <summary>
        /// Collects the items of Spectrum Code Discovery project
        /// </summary>
        public override void CollectItems()
        {
            HierarchyItems.Clear();
            foreach (ProjectItem item in Root.ProjectItems)
            {
                ProcessProjectItem(item);
            }
            LoadProjectSettings();
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
                HierarchyItems.Add(new AnnotationProjectItem(item));
            }
            else if (extension == VsHierarchyTypes.RomItem)
            {
                HierarchyItems.Add(new RomProjectItem(item));
            }
            else if (extension == VsHierarchyTypes.TzxItem)
            {
                HierarchyItems.Add(new TzxProjectItem(item));
            }
            else if (extension == VsHierarchyTypes.TapItem)
            {
                HierarchyItems.Add(new TapProjectItem(item));
            }
            else if (extension == VsHierarchyTypes.VmStateItem)
            {
                HierarchyItems.Add(new VmStateProjectItem(item));
            }
            else
            {
                HierarchyItems.Add(new UnusedProjectItem(item));
            }
        }

        /// <summary>
        /// Sets the default tape item to the specified one
        /// </summary>
        /// <param name="identity"></param>
        public void SetDefaultTapeItem(string identity)
        {
            _currentSettings.DefaultTapeFile = identity;
            SaveProjectSettings();
        }

        /// <summary>
        /// Sets the default annotation item to the specified one
        /// </summary>
        /// <param name="identity"></param>
        public void SetDefaultAnnotationItem(string identity)
        {
            _currentSettings.DefaultAnnotationFile = identity;
            SaveProjectSettings();
        }

        private string ProjectDir => Path.GetDirectoryName(Root.FullName);

        /// <summary>
        /// Loads the project settings from the settings file
        /// </summary>
        private void LoadProjectSettings()
        {
            try
            {
                var contents = File.ReadAllText(Path.Combine(ProjectDir, SETTINGS_FILE));
                _currentSettings = JsonConvert.DeserializeObject<Z80ProjectSettings>(contents);
            }
            catch
            {
                _currentSettings = new Z80ProjectSettings();
            }
        }

        private void SaveProjectSettings()
        {
            if (_currentSettings == null)
            {
                return;
            }
            var contents = JsonConvert.SerializeObject(_currentSettings);
            File.WriteAllText(Path.Combine(ProjectDir, SETTINGS_FILE), contents);
        }

        /// <summary>
        /// This class can be used to save project settins
        /// </summary>
        // ReSharper disable UnusedAutoPropertyAccessor.Local
        private class Z80ProjectSettings
        {
            public string DefaultTapeFile { get; set; }
            public string DefaultAnnotationFile { get; set; }
        }
        // ReSharper restore UnusedAutoPropertyAccessor.Local
    }
}