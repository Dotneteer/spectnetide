using EnvDTE;

namespace Spect.Net.VsPackage.SolutionItems
{
    /// <summary>
    /// This class represents an annotation file item.
    /// </summary>
    public class AnnotationProjectItem: SpectrumProjectItemBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        public AnnotationProjectItem(ProjectItem dteProjectItem) : base(dteProjectItem)
        {
        }
    }
}