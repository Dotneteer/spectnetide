using EnvDTE;

namespace Spect.Net.VsPackage.ProjectStructure
{
    /// <summary>
    /// This class represents a Z80 test file item
    /// </summary>
    public class Z80TestProjectItem: DiscoveryProjectItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        public Z80TestProjectItem(ProjectItem dteProjectItem) : base(dteProjectItem)
        {
        }
    }
}