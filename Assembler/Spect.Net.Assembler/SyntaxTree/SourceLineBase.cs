using System;
using System.Collections.Generic;

namespace Spect.Net.Assembler.SyntaxTree
{
    /// <summary>
    /// This class represents a single line of Z80 assembly in the syntax tree
    /// </summary>
    public abstract class SourceLineBase
    {
        private const string TASK_INDICATOR = "todo";

        /// <summary>
        /// The index of the source file this line belongs to
        /// </summary>
        public int FileIndex { get; set; }

        /// <summary>
        /// The source line number
        /// </summary>
        public int SourceLine { get; set; }

        /// <summary>
        /// The first position within a source code line
        /// </summary>
        public int Position { get; set; }

        /// <summary>
        /// Exception detected by the parser
        /// </summary>
        public Exception ParserException { get; set; }

        /// <summary>
        /// The optional label
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// Label span information
        /// </summary>
        public TextSpan LabelSpan { get; set; }

        /// <summary>
        /// Assembly keyword/pragma/directive span information
        /// </summary>
        public TextSpan KeywordSpan { get; set; }

        /// <summary>
        /// Number spans
        /// </summary>
        public List<TextSpan> Numbers { get; set; }

        /// <summary>
        /// Identifier spans
        /// </summary>
        public List<TextSpan> Identifiers { get; set; }

        /// <summary>
        /// Comment information
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// Comment span information
        /// </summary>
        public TextSpan CommentSpan { get; set; }

        /// <summary>
        /// The Z80 assembly instruction span
        /// </summary>
        public TextSpan InstructionSpan { get; set; }

        /// <summary>
        /// Indicates whether this line has an error
        /// </summary>
        public bool HasError { get; set; }

        /// <summary>
        /// Gets a value indicating whether this line creates a task.
        /// </summary>
        public bool DefinesTask => Comment != null 
            && Comment.IndexOf(TASK_INDICATOR, StringComparison.InvariantCultureIgnoreCase) >= 0;

        /// <summary>
        /// Gets the Description for a line that defines a task, or null if no tas is defined
        /// </summary>
        public string TaskDescription => !DefinesTask 
            ? null 
            : Comment.Substring(Comment.IndexOf(TASK_INDICATOR, StringComparison.InvariantCultureIgnoreCase));
    }
}