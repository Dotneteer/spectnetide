using System;
using System.Collections.Generic;

namespace Spect.Net.Assembler.SyntaxTree.Statements
{
    /// <summary>
    /// This class represents a MACRO statement
    /// </summary>
    public sealed class MacroStatement: BlockStatementBase, ILabelSetter
    {
        /// <summary>
        /// Macro argument names
        /// </summary>
        public List<string> Arguments { get; }

        public MacroStatement(List<string> arguments)
        {
            Arguments = arguments;
        }

        /// <summary>
        /// Type of end statement
        /// </summary>
        public override Type EndType => typeof(MacroEndStatement);

        /// <summary>
        /// Gets the name of the end statement
        /// </summary>
        public override string EndStatementName => "MEND";
    }
}