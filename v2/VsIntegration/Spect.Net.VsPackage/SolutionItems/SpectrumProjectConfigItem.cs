using EnvDTE;

namespace Spect.Net.VsPackage.SolutionItems
{
    /// <summary>
    /// This class represents a ZX Spectrum Code Discovery project
    /// configuration file
    /// </summary>
    public class SpectrumProjectConfigItem: SpectrumProjectItemBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        public SpectrumProjectConfigItem(ProjectItem dteProjectItem) : base(dteProjectItem)
        {
        }
    }
}