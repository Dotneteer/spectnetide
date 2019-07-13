using EnvDTE;

namespace Spect.Net.VsPackage.SolutionItems
{
    /// <summary>
    /// This class represents a ZX Spectrum TZX file item
    /// </summary>
    public class TzxProjectItem: TapeProjectItemBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        public TzxProjectItem(ProjectItem dteProjectItem) : base(dteProjectItem)
        {
        }
    }
}