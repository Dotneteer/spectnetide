using EnvDTE;

namespace Spect.Net.VsPackage.ProjectStructure
{
    /// <summary>
    /// This class represents a Spectrum tape file item
    /// </summary>
    public abstract class TapeProjectItemBase: DiscoveryProjectItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        protected TapeProjectItemBase(ProjectItem dteProjectItem) : base(dteProjectItem)
        {
        }
    }
}