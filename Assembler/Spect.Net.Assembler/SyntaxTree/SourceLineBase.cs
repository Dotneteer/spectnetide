using System;
using System.Collections.Generic;

namespace Spect.Net.Assembler.SyntaxTree
{
    /// <summary>
    /// This class represents a single line of Z80 assembly in the syntax tree
    /// </summary>
    public abstract class SourceLineBase
    {
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


    }
}