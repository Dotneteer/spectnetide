using System;

namespace Spect.Net.VsPackage.ProjectStructure
{
    /// <summary>
    /// This event signs that a project item has been renamed.
    /// </summary>
    public class ProjectItemRenamedEventArgs: EventArgs
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
        public ProjectItemRenamedEventArgs(string oldName, string newName)
        {
            OldName = oldName;
            NewName = newName;
        }
    }
}