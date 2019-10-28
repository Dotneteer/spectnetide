using EnvDTE;

namespace Spect.Net.VsPackage.SolutionItems
{
    /// <summary>
    /// This class represents a virtual machine state file item
    /// </summary>
    public class VmStateProjectItem: SpectrumProjectItemBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        public VmStateProjectItem(ProjectItem dteProjectItem) : base(dteProjectItem)
        {
        }
    }
}