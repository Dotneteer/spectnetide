using EnvDTE;

namespace Spect.Net.VsPackage.ProjectStructure
{
    /// <summary>
    /// This class represents project item that has no specific role within
    /// a Spectrum Code Discovery project
    /// </summary>
    public class UnusedProjectItem: DiscoveryProjectItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        public UnusedProjectItem(ProjectItem dteProjectItem) : base(dteProjectItem)
        {
        }
    }
}