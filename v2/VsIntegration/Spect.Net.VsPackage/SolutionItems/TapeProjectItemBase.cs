using EnvDTE;

namespace Spect.Net.VsPackage.SolutionItems
{
    /// <summary>
    /// This class represents a base class for tape project item types
    /// </summary>
    public abstract class TapeProjectItemBase: SpectrumProjectItemBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        protected TapeProjectItemBase(ProjectItem dteProjectItem) : base(dteProjectItem)
        {
        }
    }
}