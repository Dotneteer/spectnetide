using System.IO;
using System.Linq;

namespace Spect.Net.VsPackage.ProjectStructure
{
    /// <summary>
    /// This class provides information about the current workspace
    /// in which a Spectrum Code Discovery project runs
    /// </summary>
    public class WorkspaceInfo
    {
        /// <summary>
        /// The current Spectrum Code Discovery project to run
        /// </summary>
        public DiscoveryProject CurrentProject { get; set; }

        /// <summary>
        /// The Rom to use when running the Spectrum virtual machine
        /// </summary>
        public RomProjectItem RomItem { get; set; }

        /// <summary>
        /// The Tzx file to play when loading
        /// </summary>
        public TzxProjectItem TzxItem { get; set; }

        /// <summary>
        /// The annotation to use with the current project
        /// </summary>
        public AnnotationProjectItem AnnotationItem { get; set; }

        /// <summary>
        /// Indicates if the virtual machine should start with a specific
        /// .vmstate file (VmState property)
        /// </summary>
        public bool StartWithVmState { get; set; }

        /// <summary>
        /// The virtual machine state file to use when starting the machine
        /// </summary>
        public VmStateProjectItem VmState { get; set; }

        /// <summary>
        /// Creates workspace info from the specified solution
        /// </summary>
        /// <param name="solution">Solution to build the workspace from</param>
        /// <returns>Workspace info</returns>
        public static WorkspaceInfo CreateFromSolution(SolutionStructure solution)
        {
            var result = new WorkspaceInfo();
            var currentProject = solution.Projects.FirstOrDefault();
            if ((result.CurrentProject = currentProject) == null)
            {
                return null;
            }

            result.RomItem = currentProject.RomProjectItems.FirstOrDefault();
            result.TzxItem = currentProject.TzxProjectItems.FirstOrDefault();
            result.AnnotationItem = currentProject.AnnotationProjectItems
                .FirstOrDefault(i => Path.GetFileName(i.Filename)?.ToLower() == "annotations.disann");
            result.VmState = currentProject.VmStateProjectItems.FirstOrDefault();
            result.StartWithVmState = false;
            return result;
        }
    }
}