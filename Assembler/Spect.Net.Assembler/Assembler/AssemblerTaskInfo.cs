using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spect.Net.Assembler.Assembler
{
    /// <summary>
    /// This class represents a task extracted from comments in the source
    /// </summary>
    public class AssemblerTaskInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AssemblerTaskInfo"/> class.
        /// </summary>
        /// <param name="description">The description of the task.</param>
        /// <param name="filename">The filename the task is from.</param>
        /// <param name="line">The line the task is defined on.</param>
        public AssemblerTaskInfo(string description, string filename, int line)
        {
            Description = description;
            Filename = filename;
            Line = line;
        }

        /// <summary>
        /// Gets the description to display for the task
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Gets the filename (complete with path) of the file the task is defined in
        /// </summary>
        public string Filename { get; }

        /// <summary>
        /// Gets  the line number where the task is defined (Within the file)
        /// </summary>
        public int Line { get; }
    }
}