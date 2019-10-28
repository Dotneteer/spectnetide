using EnvDTE;

#pragma warning disable VSTHRD010 // Invoke single-threaded types on Main thread

namespace Spect.Net.VsPackage.SolutionItems
{
    /// <summary>
    /// This class is intended to be the base class of all project items within a
    /// ZX Spectrum Code Discovery project
    /// </summary>
    public abstract class SpectrumProjectItemBase
    {
        /// <summary>
        /// The DTE project item associated with this item.
        /// </summary>
        public ProjectItem DteProjectItem { get; }

        /// <summary>
        /// Project item identity.
        /// </summary>
        public string Identity { get; }

        /// <summary>
        /// The project item's file name.
        /// </summary>
        public string Filename { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        protected SpectrumProjectItemBase(ProjectItem dteProjectItem)
        {
            DteProjectItem = dteProjectItem;
            Filename = dteProjectItem.FileNames[0];
            Identity = dteProjectItem.Properties.Item("Identity").Value.ToString();
        }

    }
}

#pragma warning restore VSTHRD010 // Invoke single-threaded types on Main thread
