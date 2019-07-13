using EnvDTE;

namespace Spect.Net.VsPackage.SolutionItems
{
    /// <summary>
    /// This class represents a ZX Spectrum ROM project item
    /// </summary>
    public class SpectrumRomProjectItem: SpectrumProjectItemBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        public SpectrumRomProjectItem(ProjectItem dteProjectItem) : base(dteProjectItem)
        {
        }
    }
}