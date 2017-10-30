using EnvDTE;

namespace Spect.Net.VsPackage.ProjectStructure
{
    /// <summary>
    /// This class represents a project item that is specific for the 
    /// SpectrumCodeDiscovery project
    /// </summary>
    public abstract class DiscoveryProjectItem
    {
        /// <summary>
        /// The DTE project item associated with this item
        /// </summary>
        public ProjectItem DteProjectItem { get; }

        /// <summary>
        /// Project item identity
        /// </summary>
        public string Identity { get; }

        /// <summary>
        /// The project item's file name
        /// </summary>
        public string Filename { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        protected DiscoveryProjectItem(ProjectItem dteProjectItem)
        {
            DteProjectItem = dteProjectItem;
            Filename = dteProjectItem.FileNames[0];
            Identity = dteProjectItem.Properties.Item("Identity").Value.ToString();
        }
    }
}