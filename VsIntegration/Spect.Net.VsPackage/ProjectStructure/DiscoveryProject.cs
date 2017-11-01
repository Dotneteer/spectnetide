using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using EnvDTE;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Newtonsoft.Json;
using Spect.Net.VsPackage.Utility;
using Spect.Net.VsPackage.Z80Programs;
using Spect.Net.VsPackage.Z80Programs.Commands;

namespace Spect.Net.VsPackage.ProjectStructure
{
    /// <summary>
    /// This class represents the discovery project's items
    /// </summary>
    public class DiscoveryProject: Z80HierarchyBase<Project, DiscoveryProjectItem>
    {
        private const string SETTINGS_FILE = ".z80settings";

        /// <summary>
        /// The current project settings
        /// </summary>
        public Z80ProjectSettings CurrentSettings { get; private set; }

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
        /// Z80 code file project items
        /// </summary>
        public IReadOnlyList<Z80CodeProjectItem> Z80CodeProjectItems => new ReadOnlyCollection<Z80CodeProjectItem>(
            HierarchyItems.Where(i => i.GetType() == typeof(Z80CodeProjectItem))
                .Cast<Z80CodeProjectItem>()
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

            // --- Mark the default items
            SetVisuals(CurrentSettings.DefaultAnnotationFile, AnnotationProjectItems);
            SetVisuals(CurrentSettings.DefaultTapeFile, TapProjectItems
                .Cast<TapeProjectItemBase>()
                .Union(TzxProjectItems));
            SetVisuals(CurrentSettings.DefaultCodeFile, Z80CodeProjectItems);
        }

        /// <summary>
        /// Processes the specified item, and its sub items, provided it is a folder
        /// </summary>
        /// <param name="item">Item to process</param>
        public void ProcessProjectItem(ProjectItem item)
        {
            string extension;
            try
            {
                if (item.Properties.Item("Extension") == null) return;
                extension = (item.Properties.Item("Extension").Value?.ToString() ?? string.Empty).ToLower();
            }
            catch 
            {
                // --- The freshly deleted item cannot be found, and it results in exception.
                return;
            }

            if (extension == string.Empty && item.ProjectItems.Count > 0)
            {
                // --- This is a folder
                foreach (ProjectItem subItem in item.ProjectItems)
                {
                    ProcessProjectItem(subItem);
                }
            }
            else if (string.Compare(extension, VsHierarchyTypes.DisannItem, 
                StringComparison.InvariantCultureIgnoreCase) == 0)
            {
                HierarchyItems.Add(new AnnotationProjectItem(item));
            }
            else if (string.Compare(extension, VsHierarchyTypes.RomItem,
                         StringComparison.InvariantCultureIgnoreCase) == 0)
            {
                HierarchyItems.Add(new RomProjectItem(item));
            }
            else if (string.Compare(extension, VsHierarchyTypes.TzxItem,
                         StringComparison.InvariantCultureIgnoreCase) == 0)
            {
                HierarchyItems.Add(new TzxProjectItem(item));
            }
            else if (string.Compare(extension, VsHierarchyTypes.TapItem,
                         StringComparison.InvariantCultureIgnoreCase) == 0)
            {
                HierarchyItems.Add(new TapProjectItem(item));
            }
            else if (string.Compare(extension, VsHierarchyTypes.Z80Item,
                         StringComparison.InvariantCultureIgnoreCase) == 0)
            {
                HierarchyItems.Add(new Z80CodeProjectItem(item));
            }
            else
            {
                HierarchyItems.Add(new UnusedProjectItem(item));
            }
        }

        /// <summary>
        /// Sets the default tape item to the specified one
        /// </summary>
        /// <param name="command">The command entity</param>
        public void SetDefaultTapeItem(SingleProjectItemCommandBase command)
        {
            CurrentSettings.DefaultTapeFile = command.Identity;
            SaveProjectSettings();
            command.GetItem(out var hierarchy, out var itemId);
            SetVisuals(hierarchy, itemId, TapProjectItems.Cast<TapeProjectItemBase>()
                .Union(TzxProjectItems));
        }

        /// <summary>
        /// Sets the default annotation item to the specified one
        /// </summary>
        /// <param name="command">The command entity</param>
        public void SetDefaultAnnotationItem(SingleProjectItemCommandBase command)
        {
            CurrentSettings.DefaultAnnotationFile = command.Identity;
            SaveProjectSettings();
            command.GetItem(out var hierarchy, out var itemId);
            SetVisuals(hierarchy, itemId, AnnotationProjectItems);
        }

        /// <summary>
        /// Sets the default annotation item to the specified one
        /// </summary>
        /// <param name="command">The command entity</param>
        public void SetDefaultCodeItem(SingleProjectItemCommandBase command)
        {
            CurrentSettings.DefaultCodeFile = command.Identity;
            SaveProjectSettings();
            command.GetItem(out var hierarchy, out var itemId);
            SetVisuals(hierarchy, itemId, Z80CodeProjectItems);
        }

        /// <summary>
        /// Obtains the hierarchy information for the item specified by its identity
        /// </summary>
        /// <param name="identity">Identity information</param>
        /// <param name="hierarchy">Hiearchy object</param>
        /// <param name="itemId"></param>
        public void GetHierarchyByIdentity(string identity, out IVsHierarchy hierarchy, out uint itemId)
        {
            hierarchy = null;
            itemId = VSConstants.VSITEMID_NIL;

            if (string.IsNullOrWhiteSpace(identity)) return;
            foreach (var item in HierarchyItems)
            {
                var itemIdentity = item.DteProjectItem.Properties.Item("Identity").Value?.ToString();
                if (itemIdentity == identity)
                {
                    var solSrv = (IVsSolution)Package.GetGlobalService(typeof(SVsSolution));
                    solSrv.GetProjectOfUniqueName(Root.UniqueName, out hierarchy);
                    var fileFullName = item.DteProjectItem.FileNames[0];
                    if (!string.IsNullOrEmpty(fileFullName)
                        && hierarchy.ParseCanonicalName(fileFullName, out itemId) == VSConstants.S_OK)
                    {
                        return;
                    }
                    hierarchy = null;
                    break;
                }
            }
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
                CurrentSettings = JsonConvert.DeserializeObject<Z80ProjectSettings>(contents);
            }
            catch
            {
                CurrentSettings = new Z80ProjectSettings();
            }
        }

        /// <summary>
        /// Saves the project settings to the project file
        /// </summary>
        private void SaveProjectSettings()
        {
            if (CurrentSettings == null)
            {
                return;
            }
            var contents = JsonConvert.SerializeObject(CurrentSettings);
            File.WriteAllText(Path.Combine(ProjectDir, SETTINGS_FILE), contents);
        }

        /// <summary>
        /// Sets the visual style of the item passed with its identity to bold, while the others
        /// in the category to normal.
        /// </summary>
        /// <typeparam name="TItem">Project item type</typeparam>
        /// <param name="identity">Element identity</param>
        /// <param name="categoryItems">Other items in the same category</param>
        private void SetVisuals<TItem>(string identity, IEnumerable<TItem> categoryItems)
            where TItem : DiscoveryProjectItem
        {
            GetHierarchyByIdentity(identity, out var hierarchy, out var itemId);
            SetVisuals(hierarchy, itemId, categoryItems);
        }

        /// <summary>
        /// Sets the visual style of the passed hierarchy element to bold, while the others
        /// in the category to normal.
        /// </summary>
        /// <typeparam name="TItem">Project item type</typeparam>
        /// <param name="hierarchy">Hierarchy value</param>
        /// <param name="itemId">Item identifier</param>
        /// <param name="categoryItems">Other items in the same category</param>
        private void SetVisuals<TItem>(IVsHierarchy hierarchy, uint itemId, IEnumerable<TItem> categoryItems)
            where TItem: DiscoveryProjectItem
        {
            // --- Get the solution service
            var solSrv = (IVsSolution)Package.GetGlobalService(typeof(SVsSolution));

            // --- Get the solution explorer window
            var shell = (IVsUIShell)Package.GetGlobalService(typeof(SVsUIShell));
            var solutionExplorer = new Guid(ToolWindowGuids80.SolutionExplorer);
            shell.FindToolWindow(0, ref solutionExplorer, out var frame);

            // --- Get solution explorer's DocView
            frame.GetProperty((int)__VSFPROPID.VSFPROPID_DocView, out var obj);
            var window = (IVsUIHierarchyWindow2)obj;

            // --- Get the project hierarchy
            solSrv.GetProjectOfUniqueName(Root.UniqueName, out var projectHierarchy);

            // --- Remove the Bold flag from all files
            foreach (var projItem in categoryItems)
            {
                string fileFullName = null;

                try
                {
                    fileFullName = projItem.DteProjectItem.FileNames[0];
                }
                catch
                {
                    // --- This exception is intentionally ignored
                }

                if (string.IsNullOrEmpty(fileFullName)) continue;

                if (projectHierarchy.ParseCanonicalName(fileFullName, out var projectItemId) == VSConstants.S_OK)
                {
                    window.SetItemAttribute((IVsUIHierarchy)projectHierarchy, projectItemId,
                        (uint)__VSHIERITEMATTRIBUTE.VSHIERITEMATTRIBUTE_Bold, false);
                }
            }

            // --- Add Bold flag to the current default file
            window.SetItemAttribute((IVsUIHierarchy)hierarchy, itemId, 
                (uint)__VSHIERITEMATTRIBUTE.VSHIERITEMATTRIBUTE_Bold, true);
        }
    }
}