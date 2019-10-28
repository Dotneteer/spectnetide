using System;

namespace Spect.Net.VsPackage.SolutionItems
{
    /// <summary>
    /// This class holds the arguments of an active project changed event
    /// </summary>
    public class ActiveProjectChangedEventArgs: EventArgs
    {
        public ActiveProjectChangedEventArgs(SpectrumProject oldProject, SpectrumProject newProject)
        {
            OldProject = oldProject;
            NewProject = newProject;
        }

        /// <summary>
        /// Old project information
        /// </summary>
        public SpectrumProject OldProject { get; }

        /// <summary>
        /// New Project information
        /// </summary>
        public SpectrumProject NewProject { get; }
    }
}