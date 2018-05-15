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
        public int FileIndex;

        /// <summary>
        /// The source line number
        /// </summary>
        public int SourceLine;

        /// <summary>
        /// The first column within a source code line
        /// </summary>
        public int FirstColumn;

        /// <summary>
        /// The first position within a source code
        /// </summary>
        public int FirstPosition;

        /// <summary>
        /// The last position within a source code
        /// </summary>
        public int LastPosition;

        /// <summary>
        /// Exception detected by the parser
        /// </summary>
        public Exception ParserException;

        /// <summary>
        /// The optional label
        /// </summary>
        public string Label;

        /// <summary>
        /// Label span information
        /// </summary>
        public TextSpan LabelSpan;

        /// <summary>
        /// Assembly keyword/pragma/directive span information
        /// </summary>
        public TextSpan KeywordSpan;

        /// <summary>
        /// Number spans
        /// </summary>
        public List<TextSpan> Numbers;

        /// <summary>
        /// String spans
        /// </summary>
        public List<TextSpan> Strings;

        /// <summary>
        /// Identifier spans
        /// </summary>
        public List<TextSpan> Identifiers;

        /// <summary>
        /// Function spans
        /// </summary>
        public List<TextSpan> Functions;

        /// <summary>
        /// Macro parameters in the source line
        /// </summary>
        public List<TextSpan> MacroParams;

        /// <summary>
        /// Macro parameter names in the source line
        /// </summary>
        public List<string> MacroParamNames;

        /// <summary>
        /// Semi-variables spans
        /// </summary>
        public List<TextSpan> SemiVars;

        /// <summary>
        /// Statement spans
        /// </summary>
        public List<TextSpan> Statements;

        /// <summary>
        /// Operand spans
        /// </summary>
        public List<TextSpan> Operands;

        /// <summary>
        /// Mnemonic spans
        /// </summary>
        public List<TextSpan> Mnemonics;

        /// <summary>
        /// Comment information
        /// </summary>
        public string Comment;

        /// <summary>
        /// Comment span information
        /// </summary>
        public TextSpan CommentSpan;

        /// <summary>
        /// The Z80 assembly instruction span
        /// </summary>
        public TextSpan InstructionSpan;

        /// <summary>
        /// The source text of the line
        /// </summary>
        /// <remarks>
        /// Contains value only when the line has a macro parameter
        /// </remarks>
        public string SourceText;

        /// <summary>
        /// Set this value to an error code to emit that issue
        /// </summary>
        public string EmitIssue;

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