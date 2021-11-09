using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Threading;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio;
using Spect.Net.VsPackage.Machines;
using Spect.Net.VsPackage.VsxLibrary.Output;
using Task = System.Threading.Tasks.Task;
using SpectNetOutput = Spect.Net.VsPackage.VsxLibrary.Output.OutputWindow;

#pragma warning disable VSTHRD010 // Invoke single-threaded types on Main thread

namespace Spect.Net.VsPackage.SolutionItems
{
    /// <summary>
    /// This class represents the structure of the solution.
    /// </summary>
    public class SolutionStructure : ItemHierarchyBase<Solution, SpectrumProject>
    {
        /// <summary>
        /// This folder stores private solution settings
        /// </summary>
        public const string PRIVATE_FOLDER = ".SpectNetIde";

        /// <summary>
        /// The name of the solution settings file
        /// </summary>
        public const string SETTINGS_FILE = ".spectrumsettings";

        /// <summary>
        /// The name of the folder that contains ROMs and annotations
        /// </summary>
        public const string ROM_FOLDER = @"Rom";

        /// <summary>
        /// Check period waiting time in milliseconds
        /// </summary>
        public const int WATCH_PERIOD = 1000;

        private CancellationTokenSource _cancellationSource;
        private JoinableTask _changeWatcherTask;
        private bool _isCollecting;

        private readonly IVsSolution _solutionService;
        private readonly SolutionEvents _solutionEvents;
        private string _activeProjectFile;
        private string _lastCollectedActiveProject;

        /// <summary>
        /// Gets the solution directory
        /// </summary>
        public string SolutionDir => Path.GetDirectoryName(Root.FullName);

        /// <summary>
        /// Spectrum code discovery projects within the solution
        /// </summary>
        public IReadOnlyList<SpectrumProject> Projects { get; private set; }

        /// <summary>
        /// The ZX Spectrum virtual machines belonging to the solution
        /// </summary>
        public MachineCollection Machines = new MachineCollection();

        /// <summary>
        /// This event is raised when the solution has been opened, and solution structure
        /// is collected.
        /// </summary>
        public event EventHandler SolutionOpened;

        /// <summary>
        /// This event is raised when the solution is about to close.
        /// </summary>
        public event EventHandler SolutionClosing;

        /// <summary>
        /// Indicates is solution is closing
        /// </summary>
        public bool IsSolutionClosing { get; private set; }

        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public SolutionStructure()
            : base(SpectNetPackage.Default.ApplicationObject.DTE.Solution,
                  Package.GetGlobalService(typeof(SVsSolution)) as IVsHierarchy)
        {
            _solutionService = Package.GetGlobalService(typeof(SVsSolution)) as IVsSolution;
            _solutionEvents = SpectNetPackage.Default.ApplicationObject.Events.SolutionEvents;
            _solutionEvents.Opened += OnSolutionOpened;
            _solutionEvents.AfterClosing += OnAfterClosing;
            _solutionEvents.ProjectAdded += OnProjectAdded;
            _solutionEvents.ProjectRemoved += OnProjectRemoved;
            Projects = new ReadOnlyCollection<SpectrumProject>(HierarchyItems);
            _isCollecting = false;
            Start();
        }

        /// <summary>
        /// Gets the active project
        /// </summary>
        public SpectrumProject ActiveProject
        {
            get
            {
                var active = GetProjectOfFile(_activeProjectFile);
                return active ?? (Projects.Count == 0 ? null : Projects[0]);
            }
        }

        /// <summary>
        /// Gets the project item that represents the active TZX file.
        /// </summary>
        public TzxProjectItem ActiveTzxItem =>
            ActiveProject?.TzxProjectItems.FirstOrDefault();

        /// <summary>
        /// Gets the project item that represents the active TAP file.
        /// </summary>
        public TapProjectItem ActiveTapItem =>
            ActiveProject?.TapProjectItems.FirstOrDefault();

        /// <summary>
        /// This event is raised when the active ZX Spectrum project changes.
        /// </summary>
        public event EventHandler<ActiveProjectChangedEventArgs> ActiveProjectChanged;

        /// <summary>
        /// Tests if the specified file is in the active project
        /// </summary>
        /// <param name="filename">Filename to test</param>
        /// <returns>True, if the file is within the active project</returns>
        public bool IsFileInActiveProject(string filename)
        {
            return GetProjectOfFile(filename) == ActiveProject;
        }

        /// <summary>
        /// Gets the project that contains the specified file
        /// </summary>
        /// <param name="filename">Filename to get the host project for</param>
        /// <returns>The host project instance, if exists; otherwise, null</returns>
        public SpectrumProject GetProjectOfFile(string filename)
            => Projects.FirstOrDefault(p => p.Root.FileName == filename || p.ContainsFile(filename));

        /// <summary>
        /// Scans the solution for Spectrum Code Discovery projects
        /// </summary>
        public void CollectProjects()
        {
            if (_isCollecting) return;
            _isCollecting = true;
            var oldActiveProject = _lastCollectedActiveProject;
            try
            {
                var collectedItems = new List<SpectrumProject>();
                foreach (Project project in Root.Projects)
                {
                    switch (project.Kind)
                    {
                        case VsHierarchyTypes.SOLUTION_FOLDER:
                            Traverse(project, collectedItems);
                            break;
                        case VsHierarchyTypes.CODE_DISCOVERY_PROJECT:
                            _solutionService.GetProjectOfUniqueName(project.UniqueName, out var projectHierarchy);
                            collectedItems.Add(new SpectrumProject(project, projectHierarchy));
                            break;
                    }
                }

                // Before we reassign HierarchyItems to new collection 
                // we have to call Dispose manually for all previous items, to unsibscribe from HierarchyEvents
                foreach(SpectrumProject proj in HierarchyItems)
                {
                    proj.Dispose();
                }

                HierarchyItems = collectedItems;
                Projects = new ReadOnlyCollection<SpectrumProject>(HierarchyItems);
                var contents = File.ReadAllText(Path.Combine(SolutionDir, PRIVATE_FOLDER, SETTINGS_FILE));
                var settings = JsonConvert.DeserializeObject<SpectrumSolutionSettings>(contents);
                _activeProjectFile = settings.DefaultProject;
                SetVisuals();
            }
            catch
            {
                // --- This exception is intentionally ignored.
            }
            finally
            {
                _isCollecting = false;
                CheckActiveProjectChange(oldActiveProject);
            }
        }

        /// <summary>
        /// Sets the active project file
        /// </summary>
        /// <param name="projectfile">Name of the active project file</param>
        public void SetActiveProject(string projectfile)
        {
            var oldActiveProject = ActiveProject?.Root?.FileName;
            _activeProjectFile = projectfile;
            var settings = new SpectrumSolutionSettings
            {
                DefaultProject = _activeProjectFile
            };
            var contents = JsonConvert.SerializeObject(settings, Formatting.Indented);
            File.WriteAllText(Path.Combine(SolutionDir, PRIVATE_FOLDER, SETTINGS_FILE), contents);

            // --- Set the visual properties
            SetVisuals();
            CheckActiveProjectChange(oldActiveProject);
        }

        /// <summary>
        /// Sets the visual style of the active project to bold, while the others
        /// in the category to normal.
        /// </summary>
        private void SetVisuals()
        {
            if (_activeProjectFile == null) return;

            // --- Get the solution service
            var solSrv = (IVsSolution)Package.GetGlobalService(typeof(SVsSolution));

            // --- Get the solution explorer window
            var shell = (IVsUIShell)Package.GetGlobalService(typeof(SVsUIShell));
            var solutionExplorer = new Guid(ToolWindowGuids80.SolutionExplorer);
            shell.FindToolWindow(0, ref solutionExplorer, out var frame);

            // --- Get solution explorer's DocView
            frame.GetProperty((int)__VSFPROPID.VSFPROPID_DocView, out var obj);
            var window = (IVsUIHierarchyWindow2)obj;

            // --- Remove the Bold flag from all files
            foreach (var project in Projects)
            {
                var fileFullName = project.Root.FileName;
                if (string.IsNullOrEmpty(fileFullName)) continue;
                solSrv.GetProjectOfUniqueName(project.Root.UniqueName, out var projectHierarchy);
                if (projectHierarchy.ParseCanonicalName(fileFullName, out var projectItemId) == VSConstants.S_OK)
                {
                    window.SetItemAttribute((IVsUIHierarchy)projectHierarchy, projectItemId,
                        (uint)__VSHIERITEMATTRIBUTE.VSHIERITEMATTRIBUTE_Bold, false);
                }
            }

            // --- Add Bold flag to the current default file
            var activeProject = GetProjectOfFile(_activeProjectFile);
            solSrv.GetProjectOfUniqueName(activeProject.Root.UniqueName, out var activeHierarchy);
            if (activeHierarchy.ParseCanonicalName(_activeProjectFile, out var activeItemId) == VSConstants.S_OK)
            {
                window.SetItemAttribute((IVsUIHierarchy) activeHierarchy, activeItemId,
                    (uint) __VSHIERITEMATTRIBUTE.VSHIERITEMATTRIBUTE_Bold, true);
            }
        }

        /// <summary>
        /// Gets the resource name for the specified ROM
        /// </summary>
        /// <param name="romName">Name of the ROM</param>
        /// <param name="page">Page of the ROM (-1 means single ROM page)</param>
        /// <returns>ROM resource name</returns>
        public string GetRomResourceName(string romName, int page = -1)
        {
            var fullRomName = page == -1 ? romName : $"{romName}-{page}";
            var romItem = ActiveProject.RomProjectItems.FirstOrDefault(ri =>
                string.Compare(Path.GetFileNameWithoutExtension(ri.Filename), fullRomName,
                    StringComparison.InvariantCultureIgnoreCase) == 0);
            return romItem?.Filename;
        }

        /// <summary>
        /// Gets the resource name for the specified ROM annotation
        /// </summary>
        /// <param name="romName">Name of the ROM</param>
        /// <param name="page">Page of the ROM (-1 means single ROM page)</param>
        /// <returns>ROM annotation resource name</returns>
        public string GetAnnotationResourceName(string romName, int page = -1)
        {
            var fullRomName = page == -1 ? romName : $"{romName}-{page}";
            return Path.Combine(ActiveProject.ProjectDir, ROM_FOLDER, fullRomName + ".disann");
        }

        /// <summary>
        /// Loads the annotations of the ROM.
        /// </summary>
        /// <param name="romName">Name of the ROM</param>
        /// <param name="page">Page of the ROM (-1 means single ROM page)</param>
        /// <returns>Annotations of the ROM in serialized format</returns>
        public string LoadRomAnnotation(string romName, int page = -1)
        {
            var fullRomName = page == -1 ? romName : $"{romName}-{page}";
            var annFilename = Path.Combine(ActiveProject.ProjectDir, ROM_FOLDER, fullRomName + ".disann");
            return File.ReadAllText(annFilename);
        }

        /// <summary>
        /// Loads the binary contents of the ROM.
        /// </summary>
        /// <param name="romName">Name of the ROM</param>
        /// <param name="page">Page of the ROM (-1 means single ROM page)</param>
        /// <returns>Binary contents of the ROM</returns>
        public byte[] LoadRomBytes(string romName, int page = -1)
        {
            var fullRomName = page == -1 ? romName : $"{romName}-{page}";
            var romFilename = Path.Combine(ActiveProject.ProjectDir, ROM_FOLDER, fullRomName + ".rom");

            // --- Get the content of the ROM file
            using (var stream = new StreamReader(romFilename).BaseStream)
            {
                stream.Seek(0, SeekOrigin.Begin);
                var bytes = new byte[stream.Length];
                stream.Read(bytes, 0, bytes.Length);
                return bytes;
            }
        }

        /// <summary>
        /// Traverses the specified project for ZX Spectrum projects
        /// </summary>
        /// <param name="projectToTraverse"></param>
        /// <param name="projects">List to add project information to</param>
        private void Traverse(Project projectToTraverse, ICollection<SpectrumProject> projects)
        {
            foreach (ProjectItem item in projectToTraverse.ProjectItems)
            {
                var project = item.SubProject;
                if (project == null) continue;

                if (project.Kind == VsHierarchyTypes.SOLUTION_FOLDER)
                {
                    Traverse(project, projects);
                }
                else if (project.Kind == VsHierarchyTypes.CODE_DISCOVERY_PROJECT)
                {
                    _solutionService.GetProjectOfUniqueName(project.UniqueName, out var projectHierarchy);
                    projects.Add(new SpectrumProject(project, projectHierarchy));
                }
            }
        }

        /// <summary>
        /// Obtain the names of items within the hierarchy
        /// </summary>
        /// <returns></returns>
        public override List<string> GetCurrentItemNames()
        {
            return HierarchyItems.Select(i => i.Root.FullName).ToList();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting
        /// unmanaged resources.
        /// </summary>
        public override void Dispose()
        {
            _solutionEvents.Opened -= OnSolutionOpened;
            _ = StopAsync();
            base.Dispose();
        }

        /// <summary>
        /// Starts solution structure collection
        /// </summary>
        private void Start()
        {
            if (_changeWatcherTask != null) return;
            _cancellationSource = new CancellationTokenSource();
            _changeWatcherTask = SpectNetPackage.Default.JoinableTaskFactory.StartOnIdle(
                () => CollectInBackgroundAsync(_cancellationSource.Token),
                VsTaskRunContext.UIThreadIdlePriority);
        }

        /// <summary>
        /// Stops solution structure collection
        /// </summary>
        /// <returns></returns>
        private async Task StopAsync()
        {
            _cancellationSource.Cancel();
            await _changeWatcherTask;
            _cancellationSource = null;
            _changeWatcherTask = null;
        }

        /// <summary>
        /// Collects solution structure in the background
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private async Task CollectInBackgroundAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                CollectProjects();
                await Task.Delay(WATCH_PERIOD, token);
            }
        }

        /// <summary>
        /// Collects project information every time a solution has been opened.
        /// </summary>
        private void OnSolutionOpened()
        {
            CreateMachines();
            SolutionOpened?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Handle the event when the solution has been closed
        /// </summary>
        private void OnAfterClosing()
        {
            IsSolutionClosing = true;
            try
            {
                _lastCollectedActiveProject = null;
                _ = Machines.DisposeMachinesAsync();
                SolutionClosing?.Invoke(this, EventArgs.Empty);
            }
            finally
            {
                IsSolutionClosing = false;
            }
        }

        /// <summary>
        /// Handle the event when a project is removed
        /// </summary>
        /// <param name="project"></param>
        private void OnProjectRemoved(Project project)
        {
            _ = Machines.DisposeMachineAsync(project);
        }

        /// <summary>
        /// Handle the event when a new project was added to the solution
        /// </summary>
        /// <param name="project"></param>
        private void OnProjectAdded(Project project)
        {
            CreateMachines();
        }

        /// <summary>
        /// Creates machines for all ZX Spectrum project
        /// </summary>
        private void CreateMachines()
        {
            // --- Get all projects within the solution
            CollectProjects();

            // --- Create a virtual machine for each project
            foreach (var spectrumProject in Projects)
            {
                Machines.GetOrCreateMachine(spectrumProject.Root,
                    spectrumProject.ModelName,
                    spectrumProject.EditionName);
            }
        }

        /// <summary>
        /// Checks if the active project has changed
        /// </summary>
        /// <param name="oldActiveProject">The file name of the old active project</param>
        private void CheckActiveProjectChange(string oldActiveProject)
        {
            _lastCollectedActiveProject = ActiveProject?.Root?.FileName;
            if (oldActiveProject == _lastCollectedActiveProject) return;

            var oldProject = Projects.FirstOrDefault(p => p.Root.FileName == oldActiveProject);
            var newProject = Projects.FirstOrDefault(p => p.Root.FileName == _lastCollectedActiveProject);
            ActiveProjectChanged?.Invoke(this, 
                new ActiveProjectChangedEventArgs(oldProject, newProject));
            if (newProject == null) return;

            SpectNetPackage.Log($"New active project: {newProject.Root.FileName}");
        }
    }
}

#pragma warning restore VSTHRD010 // Invoke single-threaded types on Main thread
