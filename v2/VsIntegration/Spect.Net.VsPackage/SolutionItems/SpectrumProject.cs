using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using EnvDTE;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Newtonsoft.Json;
using Spect.Net.SpectrumEmu.Abstraction.Models;
using Spect.Net.VsPackage.VsxLibrary;
using Spect.Net.VsPackage.VsxLibrary.Command;

#pragma warning disable VSTHRD010 // Invoke single-threaded types on Main thread

namespace Spect.Net.VsPackage.SolutionItems
{
    public class SpectrumProject : ItemHierarchyBase<Project, SpectrumProjectItemBase>
    {
        /// <summary>
        /// The name of the settings file within the project folder
        /// </summary>
        public const string SETTINGS_FILE = ".z80settings";

        /// <summary>
        /// Message to display when a project folder is invalid
        /// </summary>
        public const string DEFAULT_INV_FOLDER_MESSAGE =
            "The project folder to save the project item contains invalid characters or an absolute path.";

        /// <summary>
        /// Message to display when a file already exists within the project
        /// </summary>
        public const string DEFAULT_FILE_EXISTS_MESSAGE =
            "The file already exists in the project. Would you like to override it?";

        /// <summary>
        /// Gets the project directory
        /// </summary>
        public string ProjectDir => Path.GetDirectoryName(Root.FullName);

        /// <summary>
        /// Items in the project
        /// </summary>
        public IReadOnlyList<SpectrumProjectItemBase> ProjectItems { get; }

        /// <summary>
        /// Gets the collection of ROM file items within this project.
        /// </summary>
        public IReadOnlyList<SpectrumRomProjectItem> RomProjectItems => new ReadOnlyCollection<SpectrumRomProjectItem>(
            HierarchyItems.Where(i => i.GetType() == typeof(SpectrumRomProjectItem))
            .Cast<SpectrumRomProjectItem>()
            .ToList());

        /// <summary>
        /// Gets the collection of annotation file items within this project.
        /// </summary>
        public IReadOnlyList<AnnotationProjectItem> AnnotationProjectItems => new ReadOnlyCollection<AnnotationProjectItem>(
            HierarchyItems.Where(i => i.GetType() == typeof(AnnotationProjectItem))
            .Cast<AnnotationProjectItem>()
            .ToList());

        /// <summary>
        /// Gets the collection of TZX file items within this project.
        /// </summary>
        public IReadOnlyList<TzxProjectItem> TzxProjectItems => new ReadOnlyCollection<TzxProjectItem>(
            HierarchyItems.Where(i => i.GetType() == typeof(TzxProjectItem))
            .Cast<TzxProjectItem>()
            .ToList());

        /// <summary>
        /// Gets the collection of TAP file items within this project.
        /// </summary>
        public IReadOnlyList<TapProjectItem> TapProjectItems => new ReadOnlyCollection<TapProjectItem>(
            HierarchyItems.Where(i => i.GetType() == typeof(TapProjectItem))
            .Cast<TapProjectItem>()
            .ToList());

        /// <summary>
        /// Gets the collection of tape file items within this project.
        /// </summary>
        public IReadOnlyList<TapeProjectItemBase> TapeFileProjectItems =>
            new ReadOnlyCollection<TapeProjectItemBase>(
                HierarchyItems.Where(i => i.GetType() == typeof(TzxProjectItem)
                    || i.GetType() == typeof(TapProjectItem))
                .Cast<TapeProjectItemBase>()
                .ToList());

        /// <summary>
        /// Gets the collection of program file items within this project.
        /// </summary>
        public IReadOnlyList<ProgramItemBase> SpectrumProgramItems => new ReadOnlyCollection<ProgramItemBase>(
            HierarchyItems.Where(i => i.GetType().IsSubclassOf(typeof(ProgramItemBase)))
                .Cast<ProgramItemBase>()
                .ToList());

        /// <summary>
        /// Gets the collection of Z80 code file items within this project.
        /// </summary>
        public IReadOnlyList<Z80CodeProjectItem> Z80CodeProjectItems => new ReadOnlyCollection<Z80CodeProjectItem>(
            HierarchyItems.Where(i => i.GetType() == typeof(Z80CodeProjectItem))
                .Cast<Z80CodeProjectItem>()
                .ToList());

        /// <summary>
        /// Gets the collection of ZX BASIC code file items within this project.
        /// </summary>
        public IReadOnlyList<ZxBasicProjectItem> ZxBasicProjectItems => new ReadOnlyCollection<ZxBasicProjectItem>(
            HierarchyItems.Where(i => i.GetType() == typeof(ZxBasicProjectItem))
                .Cast<ZxBasicProjectItem>()
                .ToList());

        /// <summary>
        /// Gets the collection of Z80 unit test file items within this project.
        /// </summary>
        public IReadOnlyList<Z80TestProjectItem> Z80TestProjectItems => new ReadOnlyCollection<Z80TestProjectItem>(
            HierarchyItems.Where(i => i.GetType() == typeof(Z80TestProjectItem))
                .Cast<Z80TestProjectItem>()
                .ToList());

        /// <summary>
        /// Gets the collection of ZX Spectrum project config file items within this project.
        /// </summary>
        public IReadOnlyList<SpectrumProjectConfigItem> SpConfProjectItems => new ReadOnlyCollection<SpectrumProjectConfigItem>(
            HierarchyItems.Where(i => i.GetType() == typeof(SpectrumProjectConfigItem))
                .Cast<SpectrumProjectConfigItem>()
                .ToList());

        /// <summary>
        /// Gets the collection of unused file items within this project.
        /// </summary>
        public IReadOnlyList<UnusedProjectItem> UnusedProjectItems => new ReadOnlyCollection<UnusedProjectItem>(
            HierarchyItems.Where(i => i.GetType() == typeof(UnusedProjectItem))
            .Cast<UnusedProjectItem>()
            .ToList());

        /// <summary>
        /// The default annotation item within this project.
        /// </summary>
        public AnnotationProjectItem DefaultAnnotationItem { get; set; }

        /// <summary>
        /// The default tape item within this project.
        /// </summary>
        public TapeProjectItemBase DefaultTapeItem { get; set; }

        /// <summary>
        /// The default program item within this project.
        /// </summary>
        public ProgramItemBase DefaultProgramItem { get; set; }

        /// <summary>
        /// The name of the Spectrum model this project utilizes
        /// </summary>
        public string ModelName { get; private set; }

        /// <summary>
        /// The name of the particular edition within the Spectrum model
        /// </summary>
        public string EditionName { get; private set; }

        /// <summary>
        /// Configuration data for the Spectrum edition used within this project
        /// </summary>
        public SpectrumEdition SpectrumConfiguration { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        /// <param name="project">The Project automation object</param>
        /// <param name="hierarchy">The object that represents the current hierarchy</param>
        public SpectrumProject(Project project, IVsHierarchy hierarchy)
            : base(project, hierarchy)
        {
            ProjectItems = new ReadOnlyCollection<SpectrumProjectItemBase>(HierarchyItems);
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
            LoadSpectrumModelInfo();
            LoadProjectSettings();

            // --- Mark the default items
            SetVisuals(DefaultAnnotationItem, AnnotationProjectItems);
            SetVisuals(DefaultTapeItem, TapeFileProjectItems);
            SetVisuals(DefaultProgramItem, Z80CodeProjectItems);
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
            else if (string.Compare(extension, VsHierarchyTypes.DISANN_ITEM,
                StringComparison.InvariantCultureIgnoreCase) == 0)
            {
                HierarchyItems.Add(new AnnotationProjectItem(item));
            }
            else if (string.Compare(extension, VsHierarchyTypes.ROM_ITEM,
                         StringComparison.InvariantCultureIgnoreCase) == 0)
            {
                HierarchyItems.Add(new SpectrumRomProjectItem(item));
            }
            else if (string.Compare(extension, VsHierarchyTypes.TZX_ITEM,
                         StringComparison.InvariantCultureIgnoreCase) == 0)
            {
                HierarchyItems.Add(new TzxProjectItem(item));
            }
            else if (string.Compare(extension, VsHierarchyTypes.TAP_ITEM,
                         StringComparison.InvariantCultureIgnoreCase) == 0)
            {
                HierarchyItems.Add(new TapProjectItem(item));
            }
            else if (string.Compare(extension, VsHierarchyTypes.Z80_ITEM,
                         StringComparison.InvariantCultureIgnoreCase) == 0)
            {
                HierarchyItems.Add(new Z80CodeProjectItem(item));
            }
            else if (string.Compare(extension, VsHierarchyTypes.ZX_BASIC_ITEM,
                         StringComparison.InvariantCultureIgnoreCase) == 0)
            {
                HierarchyItems.Add(new ZxBasicProjectItem(item));
            }
            else if (string.Compare(extension, VsHierarchyTypes.TEST_ITEM,
                         StringComparison.InvariantCultureIgnoreCase) == 0)
            {
                HierarchyItems.Add(new Z80TestProjectItem(item));
            }
            else if (string.Compare(extension, VsHierarchyTypes.SP_CONF_ITEM,
                         StringComparison.InvariantCultureIgnoreCase) == 0)
            {
                HierarchyItems.Add(new SpectrumProjectConfigItem(item));
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
            DefaultProgramItem = GetProjectItemByIdentity(command.Identity, SpectrumProgramItems);
            SaveProjectSettings();
            SetVisuals(DefaultProgramItem, Z80CodeProjectItems);
        }

        /// <summary>
        /// Gets a project item by its identity
        /// </summary>
        /// <typeparam name="TItem">Project item type</typeparam>
        /// <param name="identity">Identity of the item we search for</param>
        /// <param name="projectItems">Project items with the specified type</param>
        /// <returns>Project item, if found; otherwise, null</returns>
        public TItem GetProjectItemByIdentity<TItem>(string identity, IEnumerable<TItem> projectItems)
            where TItem : SpectrumProjectItemBase
        {
            return projectItems.FirstOrDefault(item => item.Identity == identity);
        }

        /// <summary>
        /// Obtain the names of items within the hierarchy
        /// </summary>
        /// <returns></returns>
        public override List<string> GetCurrentItemNames()
        {
            return HierarchyItems.Select(i => i.Filename).ToList();
        }

        /// <summary>
        /// Respond to the event when an item within the hierarchy has been renamed
        /// </summary>
        /// <param name="oldItem">Old name</param>
        /// <param name="newItem">New name</param>
        public override void OnRenameItem(string oldItem, string newItem)
        {
            try
            {
                var contents = File.ReadAllText(Path.Combine(ProjectDir, SETTINGS_FILE));
                var settings = JsonConvert.DeserializeObject<SpectrumProjectSettings>(contents);

                // --- Adjust default code item
                if (oldItem.EndsWith(settings.DefaultCodeFile))
                {
                    DefaultProgramItem = HierarchyItems.OfType<Z80CodeProjectItem>()
                        .FirstOrDefault(i => i.Filename == newItem);
                    if (DefaultProgramItem != null)
                    {
                        SetVisuals(DefaultProgramItem, Z80CodeProjectItems);
                    }
                }

                // --- Adjust default tape item
                if (oldItem.EndsWith(settings.DefaultTapeFile))
                {
                    DefaultTapeItem = HierarchyItems.OfType<TapeProjectItemBase>()
                        .FirstOrDefault(i => i.Filename == newItem);
                    if (DefaultTapeItem != null)
                    {
                        SetVisuals(DefaultTapeItem, TapeFileProjectItems);
                    }
                }

                // --- Adjust default annotation item
                if (oldItem.EndsWith(settings.DefaultAnnotationFile))
                {
                    DefaultAnnotationItem = HierarchyItems.OfType<AnnotationProjectItem>()
                        .FirstOrDefault(i => i.Filename == newItem);
                    if (DefaultAnnotationItem != null)
                    {
                        SetVisuals(DefaultAnnotationItem, AnnotationProjectItems);
                    }
                }

                SaveProjectSettings();
            }
            catch
            {
                // --- This exception in intentionally ignored
            }
        }

        #region Static methods

        /// <summary>
        /// Adds the exported file to the project structure
        /// </summary>
        /// <param name="projectFolder">Project folder to add the file</param>
        /// <param name="filename">Filename to add to the project</param>
        /// <param name="invFolderMessage">Message to display when folder name is invalid</param>
        /// <param name="fileExistsMessage">Message to display when file already exists</param>
        /// <param name="confirmOverwrite">When a file exists, the user must confirm overwriting it</param>
        public static void AddFileToProject(string projectFolder, string filename, string invFolderMessage = null,
            string fileExistsMessage = null, bool confirmOverwrite = true)
        {
            var folderSegments = projectFolder.Split(new[] { '/', '\\' },
                StringSplitOptions.RemoveEmptyEntries);

            foreach (var segment in folderSegments)
            {
                bool valid;
                try
                {
                    valid = !Path.IsPathRooted(segment);
                }
                catch
                {
                    valid = false;
                }
                if (!valid)
                {
                    VsxDialogs.Show(invFolderMessage ?? DEFAULT_INV_FOLDER_MESSAGE,
                        "Invalid characters in path");
                    return;
                }
            }

            // --- Obtain the project and its items
            var project = SpectNetPackage.Default.CurrentRoot;
            var projectItems = project.ProjectItems;
            var currentIndex = 0;
            var find = true;
            while (currentIndex < folderSegments.Length)
            {
                // --- Find or create folder segments
                var segment = folderSegments[currentIndex];
                if (find)
                {
                    // --- We are in "find" mode
                    var found = false;
                    // --- Search for the folder segment
                    foreach (ProjectItem projItem in projectItems)
                    {
                        var folder = projItem.Properties.Item("FolderName").Value?.ToString();
                        if (string.Compare(folder, segment, StringComparison.InvariantCultureIgnoreCase) == 0)
                        {
                            // --- We found the folder, we'll go no with search within this segment
                            projectItems = projItem.ProjectItems;
                            found = true;
                            break;
                        }
                    }
                    if (!found)
                    {
                        // --- Move to "create" mode
                        find = false;
                    }
                }
                if (!find)
                {
                    // --- We're in create mode, add and locate the new folder segment
                    var found = false;
                    projectItems.AddFolder(segment);
                    var parent = projectItems.Parent;
                    if (parent is Project projectType)
                    {
                        projectItems = projectType.ProjectItems;
                    }
                    else if (parent is ProjectItem itemType)
                    {
                        projectItems = itemType.ProjectItems;
                    }
                    foreach (ProjectItem projItem in projectItems)
                    {
                        var folder = projItem.Properties.Item("FolderName").Value?.ToString();
                        if (string.Compare(folder, segment, StringComparison.InvariantCultureIgnoreCase) == 0)
                        {
                            // --- We found the folder, we'll go no with search within this segment
                            projectItems = projItem.ProjectItems;
                            found = true;
                            break;
                        }
                    }
                    if (!found)
                    {
                        // --- This should not happen...
                        VsxDialogs.Show($"The folder segment {segment} could not be created.",
                            "Adding project item failed");
                        return;
                    }
                }

                // --- Move to the next segment
                currentIndex++;
            }

            // --- Check if that filename exists within the project folder
            var tempFile = Path.GetFileName(filename);
            ProjectItem toDelete = null;
            foreach (ProjectItem projItem in projectItems)
            {
                var file = Path.GetFileName(projItem.FileNames[0]);
                if (string.Compare(file, tempFile,
                        StringComparison.InvariantCultureIgnoreCase) == 0)
                {
                    if (confirmOverwrite)
                    {
                        var answer = VsxDialogs.Show(fileExistsMessage ?? DEFAULT_FILE_EXISTS_MESSAGE,
                            "File already exists",
                            MessageBoxButton.YesNo, VsxMessageBoxIcon.Question, 1);
                        if (answer == VsxDialogResult.No)
                        {
                            return;
                        }
                    }
                    toDelete = projItem;
                    break;
                }
            }

            // --- Remove existing file
            toDelete?.Delete();

            // --- Add the item to the appropriate item
            projectItems.AddFromFileCopy(filename);

            // --- Refresh the solution's content
            SpectNetPackage.Default.CurrentProject.CollectItems();
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Loads the project settings from the settings file
        /// </summary>
        private void LoadProjectSettings()
        {
            try
            {
                var contents = File.ReadAllText(Path.Combine(ProjectDir, SETTINGS_FILE));
                var settings = JsonConvert.DeserializeObject<SpectrumProjectSettings>(contents);
                DefaultAnnotationItem = GetProjectItemByIdentity(settings.DefaultAnnotationFile,
                    AnnotationProjectItems);
                DefaultTapeItem = GetProjectItemByIdentity(settings.DefaultTapeFile,
                    TapeFileProjectItems);
                DefaultProgramItem = GetProjectItemByIdentity(settings.DefaultCodeFile,
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
            var settings = new SpectrumProjectSettings
            {
                DefaultTapeFile = GetIdentityByFullName(DefaultTapeItem?.Filename),
                DefaultAnnotationFile = GetIdentityByFullName(DefaultAnnotationItem?.Filename),
                DefaultCodeFile = GetIdentityByFullName(DefaultProgramItem?.Filename)
            };
            var contents = JsonConvert.SerializeObject(settings, Formatting.Indented);
            File.WriteAllText(Path.Combine(ProjectDir, SETTINGS_FILE), contents);
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
        /// Sets the visual style of the item passed with its identity to bold, while the others
        /// in the category to normal.
        /// </summary>
        /// <typeparam name="TItem">Project item type</typeparam>
        /// <param name="item">Item to set the visuals for</param>
        /// <param name="categoryItems">Other items in the same category</param>
        private void SetVisuals<TItem>(TItem item, IEnumerable<TItem> categoryItems)
            where TItem : SpectrumProjectItemBase
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
            where TItem : SpectrumProjectItemBase
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

        /// <summary>
        /// Loads the Spectrum model information
        /// </summary>
        private void LoadSpectrumModelInfo()
        {
            var configItem = SpConfProjectItems.FirstOrDefault();
            if (configItem != null)
            {
                try
                {
                    var data = File.ReadAllText(configItem.Filename);
                    SpectrumProjectConfigSerializer.Deserialize(data, out var confVm);
                    ModelName = confVm.ModelName;
                    EditionName = confVm.EditionName;
                    SpectrumConfiguration = confVm.ConfigurationData;
                }
                catch
                {
                    // --- This exception is intentionally ignored
                }
            }
        }

        #endregion
    }

}

#pragma warning restore VSTHRD010 // Invoke single-threaded types on Main thread
