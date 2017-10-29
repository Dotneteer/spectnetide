using System.Collections.Generic;
using System.Collections.ObjectModel;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Spect.Net.VsPackage.Utility;
using Spect.Net.VsPackage.Vsx;

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
            : base(VsxPackage.GetPackage<SpectNetPackage>().ApplicationObject.DTE.Solution,
                  Package.GetGlobalService(typeof(SVsSolution)) as IVsHierarchy)
        {
            _solutionService = Package.GetGlobalService(typeof(SVsSolution)) as IVsSolution;
            Projects = new ReadOnlyCollection<DiscoveryProject>(HierarchyItems);
        }

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
    }
}