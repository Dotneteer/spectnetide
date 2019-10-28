using EnvDTE;

namespace Spect.Net.VsPackage.SolutionItems
{
    /// <summary>
    /// This class represents a Z80 code file item
    /// </summary>
    public class Z80CodeProjectItem: ProgramItemBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        public Z80CodeProjectItem(ProjectItem dteProjectItem) : base(dteProjectItem)
        {
        }
    }
}