using System.Collections.Generic;
using System.Collections.ObjectModel;
using EnvDTE;
using Spect.Net.VsPackage.Utility;

namespace Spect.Net.VsPackage.ProjectStructure
{
    /// <summary>
    /// This class represents the structure of the solution
    /// </summary>
    public class SolutionStructure
    {
        private readonly List<DiscoveryProject> _projects = new List<DiscoveryProject>();

        /// <summary>
        /// Spectrum code discovery projects within the solution
        /// </summary>
        public IReadOnlyList<DiscoveryProject> Projects { get; }

        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public SolutionStructure()
        {
            Projects = new ReadOnlyCollection<DiscoveryProject>(_projects);
        }

        /// <summary>
        /// Scans the solution for Spectrum Code Discovery projects
        /// </summary>
        /// <param name="solution"></param>
        public void CollectProjects(Solution solution)
        {
            _projects.Clear();
            foreach (Project project in solution.Projects)
            {
                if (project.Kind == VsHierarchyTypes.SolutionFolder) Traverse(project);
                if (project.Kind == VsHierarchyTypes.CodeDiscoveryProject)
                {
                    _projects.Add(new DiscoveryProject(project));
                }
            }
        }

        /// <summary>
        /// Removes all projects from the solution structure
        /// </summary>
        public void Clear()
        {
            _projects.Clear();
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

                if (project.Kind == VsHierarchyTypes.SolutionFolder) Traverse(project);
                if (project.Kind == VsHierarchyTypes.CodeDiscoveryProject)
                {
                    _projects.Add(new DiscoveryProject(project));
                }
            }
        }
    }
}