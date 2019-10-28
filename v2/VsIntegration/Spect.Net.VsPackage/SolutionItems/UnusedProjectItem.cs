using EnvDTE;

namespace Spect.Net.VsPackage.SolutionItems
{
    /// <summary>
    /// This class represents a project item without any specific purpose within
    /// tha ZX Spectrum Code Discovery project
    /// </summary>
    public class UnusedProjectItem: SpectrumProjectItemBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        public UnusedProjectItem(ProjectItem dteProjectItem) : base(dteProjectItem)
        {
        }
    }
}