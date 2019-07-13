using EnvDTE;

namespace Spect.Net.VsPackage.SolutionItems
{
    /// <summary>
    /// This class represents a ZX Spectrum TAP file item
    /// </summary>
    public class TapProjectItem: TapeProjectItemBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        public TapProjectItem(ProjectItem dteProjectItem) : base(dteProjectItem)
        {
        }
    }
}