using EnvDTE;

namespace Spect.Net.VsPackage.ProjectStructure
{
    /// <summary>
    /// This class represents a virtual machine state file item
    /// </summary>
    public class VmStateProjectItem: DiscoveryProjectItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        public VmStateProjectItem(ProjectItem dteProjectItem) : base(dteProjectItem)
        {
        }
    }
}