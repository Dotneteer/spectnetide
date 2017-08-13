using EnvDTE;

namespace Spect.Net.VsPackage.ProjectStructure
{
    /// <summary>
    /// This class represents a Spectrum TZX file item
    /// </summary>
    public class TzxProjectItem: DiscoveryProjectItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        public TzxProjectItem(ProjectItem dteProjectItem) : base(dteProjectItem)
        {
        }
    }
}