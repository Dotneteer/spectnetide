using System;

namespace Spect.Net.VsPackage.Tools
{
    /// <summary>
    /// This EventArgs class is used when the user runs a command line
    /// </summary>
    public class CommandLineEventArgs: EventArgs
    {
        /// <summary>
        /// Command line text
        /// </summary>
        public string CommandLine { get; }

        /// <summary>
        /// Set true, if the command line has been handled
        /// </summary>
        public bool Handled { get; set; }

        /// <summary>
        /// The command line is invalid
        /// </summary>
        public bool Invalid { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.EventArgs" /> class.
        /// </summary>
        public CommandLineEventArgs(string commandLine, bool handled = false, bool invalid = false)
        {
            CommandLine = commandLine;
            Handled = handled;
            Invalid = invalid;
        }
    }
}