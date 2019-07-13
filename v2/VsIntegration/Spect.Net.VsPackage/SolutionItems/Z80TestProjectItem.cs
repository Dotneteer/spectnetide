using EnvDTE;

namespace Spect.Net.VsPackage.SolutionItems
{
    /// <summary>
    /// This class represents a Z80 unit test file item
    /// </summary>
    public class Z80TestProjectItem: SpectrumProjectItemBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        public Z80TestProjectItem(ProjectItem dteProjectItem) : base(dteProjectItem)
        {
        }
    }
}