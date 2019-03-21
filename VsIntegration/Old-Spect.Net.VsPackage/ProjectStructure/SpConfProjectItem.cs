using EnvDTE;

namespace Spect.Net.VsPackage.ProjectStructure
{
    /// <summary>
    /// This class represents a Spectrum configuration file item
    /// </summary>
    public class SpConfProjectItem: DiscoveryProjectItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        public SpConfProjectItem(ProjectItem dteProjectItem) : base(dteProjectItem)
        {
        }
    }
}