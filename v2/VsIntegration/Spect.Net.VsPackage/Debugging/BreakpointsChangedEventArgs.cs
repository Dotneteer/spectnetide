using System;
using System.Collections.Generic;
using EnvDTE;

namespace Spect.Net.VsPackage.Debugging
{
    /// <summary>
    /// This class represents the argument of the event when breakpoints change
    /// </summary>
    public class BreakpointsChangedEventArgs: EventArgs
    {
        public BreakpointsChangedEventArgs(List<Breakpoint> added, List<Breakpoint> modified, List<Breakpoint> deleted)
        {
            Added = added;
            Modified = modified;
            Deleted = deleted;
        }

        /// <summary>
        /// Collection of newly added breakpoints
        /// </summary>
        public List<Breakpoint> Added { get; }

        /// <summary>
        /// Collection of modified breakpoints
        /// </summary>
        public List<Breakpoint> Modified { get; }

        /// <summary>
        /// Collection of deleted breakpoints
        /// </summary>
        public List<Breakpoint> Deleted { get; }
    }
}