using EnvDTE;

namespace Spect.Net.VsPackage.SolutionItems
{
    /// <summary>
    /// This class represents a ZX Spectrum program file item
    /// </summary>
    public abstract class ProgramItemBase: SpectrumProjectItemBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        protected ProgramItemBase(ProjectItem dteProjectItem) : base(dteProjectItem)
        {
        }
    }
}