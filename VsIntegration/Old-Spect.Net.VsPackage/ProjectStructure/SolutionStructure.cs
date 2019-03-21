using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Spect.Net.VsPackage.Utility;

#pragma warning disable VSTHRD010 // Invoke single-threaded types on Main thread

namespace Spect.Net.VsPackage.ProjectStructure
{
    /// <summary>
    /// This class represents the structure of the solution
    /// </summary>
    public class SolutionStructure: Z80HierarchyBase<Solution, DiscoveryProject>
    {
        private readonly IVsSolution _solutionService;

        /// <summary>
        /// Spectrum code discovery projects within the solution
        /// </summary>
        public IReadOnlyList<DiscoveryProject> Projects { get; }

        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public SolutionStructure()
            : base(SpectNetPackage.Default.ApplicationObject.DTE.Solution,
                  Package.GetGlobalService(typeof(SVsSolution)) as IVsHierarchy)
        {
            _solutionService = Package.GetGlobalService(typeof(SVsSolution)) as IVsSolution;
            Projects = new ReadOnlyCollection<DiscoveryProject>(HierarchyItems);
        }

        /// <summary>
        /// The current Z80 project
        /// </summary>
        public DiscoveryProject CurrentProject => Projects.FirstOrDefault();

        /// <summary>
        /// The project item that represents the current ROM
        /// </summary>
        public RomProjectItem CurrentRomItem => 
            CurrentProject?.RomProjectItems.FirstOrDefault();

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
        /// The project item that represents the current annotation file
        /// </summary>
        public AnnotationProjectItem CurrentAnnotationItem =>
            CurrentProject?.AnnotationProjectItems.FirstOrDefault();

        /// <summary>
        /// Scans the solution for Spectrum Code Discovery projects
        /// </summary>
        public void CollectProjects()
        {
            HierarchyItems.Clear();
            foreach (Project project in Root.Projects)
            {
                if (project.Kind == VsHierarchyTypes.SolutionFolder)
                {
                    Traverse(project);
                }
                else if (project.Kind == VsHierarchyTypes.CodeDiscoveryProject)
                {
                    _solutionService.GetProjectOfUniqueName(project.UniqueName, out var projectHierarchy);
                    HierarchyItems.Add(new DiscoveryProject(project, projectHierarchy));
                }
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
        private void Traverse(Project projectToTraverse)
        {
            foreach (ProjectItem item in projectToTraverse.ProjectItems)
            {
                var project = item.SubProject;
                if (project == null) continue;

                if (project.Kind == VsHierarchyTypes.SolutionFolder)
                {
                    Traverse(project);
                }
                else if (project.Kind == VsHierarchyTypes.CodeDiscoveryProject)
                {
                    _solutionService.GetProjectOfUniqueName(project.UniqueName, out var projectHierarchy);
                    HierarchyItems.Add(new DiscoveryProject(project, projectHierarchy));
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
    }
}

#pragma warning restore VSTHRD010 // Invoke single-threaded types on Main thread
