using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Threading;
using Task = System.Threading.Tasks.Task;

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
        /// The name of the settings file within the project folder
        /// </summary>
        public const string SETTINGS_FILE = ".projsettings";

        /// <summary>
        /// Check period waiting time in milleseconds
        /// </summary>
        public const int WATCH_PERIOD = 1000;

        private CancellationTokenSource _cancellationSource;
        private JoinableTask _changeWatcherTask;
        private bool _isCollecting;

        private readonly IVsSolution _solutionService;
        private readonly SolutionEvents _solutionEvents;

        /// <summary>
        /// Gets the solution directory
        /// </summary>
        public string SolutionDir => Path.GetDirectoryName(Root.FullName);

        /// <summary>
        /// Spectrum code discovery projects within the solution
        /// </summary>
        public IReadOnlyList<SpectrumProject> Projects { get; private set; }

        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public SolutionStructure()
            : base(SpectNetPackage.Default.ApplicationObject.DTE.Solution,
                  Package.GetGlobalService(typeof(SVsSolution)) as IVsHierarchy)
        {
            _solutionService = Package.GetGlobalService(typeof(SVsSolution)) as IVsSolution;
            _solutionEvents = SpectNetPackage.Default.ApplicationObject.Events.SolutionEvents;
            _solutionEvents.Opened += OnSolutionOpened;
            Projects = new ReadOnlyCollection<SpectrumProject>(HierarchyItems);
            _isCollecting = false;
            Start();
        }

        /// <summary>
        /// Starts watching breakpoint changes
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
        /// Stops watching breakpoint changes
        /// </summary>
        /// <returns></returns>
        private async Task StopAsync()
        {
            _cancellationSource.Cancel();
            await _changeWatcherTask;
            _cancellationSource = null;
            _changeWatcherTask = null;
        }

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
            CollectProjects();
        }

        /// <summary>
        /// The current ZX Spectrum project
        /// </summary>
        // TODO: Use the settings that store the startup project
        public SpectrumProject CurrentProject => Projects.FirstOrDefault();

        /// <summary>
        /// The project item that represents the current TZX file
        /// </summary>
        public TzxProjectItem CurrentTzxItem =>
            CurrentProject?.TzxProjectItems.FirstOrDefault();

        /// <summary>
        /// The project item that represents the current TAP file
        /// </summary>
        public TapProjectItem CurrentTapItem =>
            CurrentProject?.TapProjectItems.FirstOrDefault();

        /// <summary>
        /// Scans the solution for Spectrum Code Discovery projects
        /// </summary>
        public void CollectProjects()
        {
            if (_isCollecting) return;
            _isCollecting = true;
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
                HierarchyItems = collectedItems;
                Projects = new ReadOnlyCollection<SpectrumProject>(HierarchyItems);
                //LoadSolutionSettings();

            }
            finally
            {
                _isCollecting = false;
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
            var romItem = CurrentProject.RomProjectItems.FirstOrDefault(ri =>
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
            var annItem = CurrentProject.AnnotationProjectItems.FirstOrDefault(ri =>
                string.Compare(Path.GetFileNameWithoutExtension(ri.Filename), fullRomName,
                    StringComparison.InvariantCultureIgnoreCase) == 0);
            return annItem?.Filename;
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
            var annItem = CurrentProject.AnnotationProjectItems.FirstOrDefault(ri =>
                string.Compare(Path.GetFileNameWithoutExtension(ri.Filename), fullRomName,
                StringComparison.InvariantCultureIgnoreCase) == 0);
            return annItem != null ? File.ReadAllText(annItem.Filename) : null;
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
            var romItem = CurrentProject.RomProjectItems.FirstOrDefault(ri =>
                string.Compare(Path.GetFileNameWithoutExtension(ri.Filename), fullRomName,
                    StringComparison.InvariantCultureIgnoreCase) == 0);
            if (romItem == null) return null;

            // --- Get the content of the ROM file
            using (var stream = new StreamReader(romItem.Filename).BaseStream)
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
    }
}

#pragma warning restore VSTHRD010 // Invoke single-threaded types on Main thread
