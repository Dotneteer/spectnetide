using System;

namespace Spect.Net.VsPackage.SolutionItems
{
    /// <summary>
    /// Arguments of the event which signs that a project item has been renamed.
    /// </summary>
    public class ProjectItemRenamedEventArgs : EventArgs
    {
        /// <summary>
        /// The old item name.
        /// </summary>
        public string OldName { get; }

        /// <summary>
        /// The new item name.
        /// </summary>
        public string NewName { get; }

        public ProjectItemRenamedEventArgs(string oldName, string newName)
        {
            OldName = oldName;
            NewName = newName;
        }
    }
}