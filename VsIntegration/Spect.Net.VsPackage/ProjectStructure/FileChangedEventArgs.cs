using System;

namespace Spect.Net.VsPackage.ProjectStructure
{
    /// <summary>
    /// Event arguments that sign file change
    /// </summary>
    public class FileChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Name of the changed file
        /// </summary>
        public string Filename { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.EventArgs" /> class.
        /// </summary>
        public FileChangedEventArgs(string filename)
        {
            Filename = filename;
        }
    }
}