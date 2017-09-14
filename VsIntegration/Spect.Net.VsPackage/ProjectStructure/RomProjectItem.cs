using EnvDTE;

namespace Spect.Net.VsPackage.ProjectStructure
{
    /// <summary>
    /// This class represents a ROM file item
    /// </summary>
    public class RomProjectItem: DiscoveryProjectItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        public RomProjectItem(ProjectItem dteProjectItem) : base(dteProjectItem)
        {
        }
    }
}