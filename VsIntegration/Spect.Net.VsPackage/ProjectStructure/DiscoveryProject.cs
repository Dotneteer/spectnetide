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
        /// TZX and TAP file project items
        /// </summary>
        public IReadOnlyList<TapeProjectItemBase> TapeFileProjectItems => 
            new ReadOnlyCollection<TapeProjectItemBase>(
                HierarchyItems.Where(i => i.GetType() == typeof(TzxProjectItem) 
                    || i.GetType() == typeof(TapProjectItem))
                .Cast<TapeProjectItemBase>()
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
        /// The default annotation item
        /// </summary>
        public AnnotationProjectItem DefaultAnnotationItem { get; set; }

        /// <summary>
        /// The default tape item
        /// </summary>
        public TapeProjectItemBase DefaultTapeItem { get; set; }

        /// <summary>
        /// The default Z80 code item
        /// </summary>
        public Z80CodeProjectItem DefaultZ80CodeItem { get; set; }

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
            SetVisuals(DefaultAnnotationItem, AnnotationProjectItems);
            SetVisuals(DefaultTapeItem, TapeFileProjectItems);
            SetVisuals(DefaultZ80CodeItem, Z80CodeProjectItems);
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
            DefaultTapeItem = GetProjectItemByIdentity(command.Identity, TapeFileProjectItems);
            SaveProjectSettings();
            SetVisuals(DefaultTapeItem, TapeFileProjectItems);
        }

        /// <summary>
        /// Sets the default annotation item to the specified one
        /// </summary>
        /// <param name="command">The command entity</param>
        public void SetDefaultAnnotationItem(SingleProjectItemCommandBase command)
        {
            DefaultAnnotationItem = GetProjectItemByIdentity(command.Identity, AnnotationProjectItems);
            SaveProjectSettings();
            SetVisuals(DefaultAnnotationItem, AnnotationProjectItems);
        }

        /// <summary>
        /// Sets the default annotation item to the specified one
        /// </summary>
        /// <param name="command">The command entity</param>
        public void SetDefaultCodeItem(SingleProjectItemCommandBase command)
        {
            DefaultZ80CodeItem = GetProjectItemByIdentity(command.Identity, Z80CodeProjectItems);
            SaveProjectSettings();
            SetVisuals(DefaultZ80CodeItem, Z80CodeProjectItems);
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
                if (item.Identity != identity) continue;
                var solSrv = (IVsSolution)Package.GetGlobalService(typeof(SVsSolution));
                solSrv.GetProjectOfUniqueName(Root.UniqueName, out hierarchy);
                if (hierarchy.ParseCanonicalName(item.Filename, out itemId) == VSConstants.S_OK)
                {
                    return;
                }
                hierarchy = null;
                break;
            }
        }

        /// <summary>
        /// Obtains the hierarchy information for the item specified by its identity
        /// </summary>
        /// <param name="identity">Identity information</param>
        /// <returns>The full file name, if found; otherwise, null</returns>
        public string GetFullNameByIdentity(string identity)
        {
            return HierarchyItems.FirstOrDefault(item => item.Identity == identity)?.Filename;
        }

        /// <summary>
        /// Obtains the hierarchy information for the item specified by its identity
        /// </summary>
        /// <param name="fullName">Identity information</param>
        /// <returns>The full file name, if found; otherwise, null</returns>
        public string GetIdentityByFullName(string fullName)
        {
            return HierarchyItems.FirstOrDefault(item => item.Filename == fullName)?.Identity;
        }

        /// <summary>
        /// Gets a project item by its identity
        /// </summary>
        /// <typeparam name="TItem">Project item type</typeparam>
        /// <param name="identity">Identity of the item we search for</param>
        /// <param name="projectItems">Project items with the specified type</param>
        /// <returns>Project item, if found; otherwise, null</returns>
        public TItem GetProjectItemByIdentity<TItem>(string identity, IEnumerable<TItem> projectItems)
            where TItem: DiscoveryProjectItem
        {
            return projectItems.FirstOrDefault(item => item.Identity == identity);
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
                var settings = JsonConvert.DeserializeObject<Z80ProjectSettings>(contents);
                DefaultAnnotationItem = GetProjectItemByIdentity(settings.DefaultAnnotationFile,
                    AnnotationProjectItems);
                DefaultTapeItem = GetProjectItemByIdentity(settings.DefaultTapeFile,
                    TapeFileProjectItems);
                DefaultZ80CodeItem = GetProjectItemByIdentity(settings.DefaultCodeFile,
                    Z80CodeProjectItems);
            }
            catch
            {
                // --- This exception is intentionally ingnored
            }
        }

        /// <summary>
        /// Saves the project settings to the project file
        /// </summary>
        private void SaveProjectSettings()
        {
            var settings = new Z80ProjectSettings
            {
                DefaultTapeFile = GetIdentityByFullName(DefaultTapeItem?.Filename),
                DefaultAnnotationFile = GetIdentityByFullName(DefaultAnnotationItem?.Filename),
                DefaultCodeFile = GetIdentityByFullName(DefaultZ80CodeItem?.Filename)
            };
            var contents = JsonConvert.SerializeObject(settings);
            File.WriteAllText(Path.Combine(ProjectDir, SETTINGS_FILE), contents);
        }

        /// <summary>
        /// Sets the visual style of the item passed with its identity to bold, while the others
        /// in the category to normal.
        /// </summary>
        /// <typeparam name="TItem">Project item type</typeparam>
        /// <param name="item">Item to set the visuals for</param>
        /// <param name="categoryItems">Other items in the same category</param>
        private void SetVisuals<TItem>(TItem item, IEnumerable<TItem> categoryItems)
            where TItem : DiscoveryProjectItem
        {
            GetHierarchyByIdentity(item?.Identity, out var hierarchy, out var itemId);
            if (hierarchy == null)
            {
                return;
            }
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