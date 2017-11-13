using GalaSoft.MvvmLight.Messaging;

namespace Spect.Net.VsPackage.ProjectStructure
{
    /// <summary>
    /// This event signs that a project item has been renamed.
    /// </summary>
    public class ProjectItemRenamedMessage: MessageBase
    {
        /// <summary>
        /// The old item name
        /// </summary>
        public string OldName { get; }

        /// <summary>
        /// The new name
        /// </summary>
        public string NewName { get; }

        /// <summary>Initializes a new instance of the MessageBase class.</summary>
        public ProjectItemRenamedMessage(string oldName, string newName)
        {
            OldName = oldName;
            NewName = newName;
        }
    }
}